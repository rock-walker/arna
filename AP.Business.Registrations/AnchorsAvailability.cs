namespace AP.Business.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using AP.Infrastructure.EventSourcing;
    using AP.Business.Model.Registration;
    using AP.Business.Registration.Events;

    /// <summary>
    /// Manages the availability of conference anchors. Currently there is one <see cref="AnchorsAvailability"/> instance per conference.
    /// </summary>
    /// <remarks>
    /// <para>For more information on the domain, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258553">Journey chapter 3</see>.</para>
    /// <para>
    /// Some of the instances of <see cref="AnchorsAvailability"/> are highly contentious, as there could be several users trying to register for the 
    /// same conference at the same time.
    /// </para>
    /// <para>
    /// Because for large conferences a single instance of <see cref="AnchorsAvailability"/> can contain a big event stream, the class implements
    /// <see cref="IMementoOriginator"/>, so that a <see cref="IMemento"/> object with the objects' internal state (a snapshot) can be cached.
    /// If the repository supports caching snapshots, then the next time an instance of <see cref="AnchorsAvailability"/> is created, it can pass
    /// the cached <see cref="IMemento"/> in the constructor overload, and avoid reading thousands of events from the event store.
    /// </para>
    /// </remarks>
    public class AnchorsAvailability : EventSourced, IMementoOriginator
    {
        private readonly Dictionary<Guid, int> remainingAnchors = new Dictionary<Guid, int>();
        private readonly Dictionary<Guid, List<AnchorQuantity>> pendingReservations = new Dictionary<Guid, List<AnchorQuantity>>();

        /// <summary>
        /// Creates a new instance of the <see cref="AnchorsAvailability"/> class.
        /// </summary>
        /// <param name="id">The identifier. Currently this correlates to the ConferenceID as specified in <see cref="Handlers.AnchorsAvailabilityHandler"/>.</param>
        public AnchorsAvailability(Guid id)
            : base(id)
        {
            base.Handles<AvailableAnchorsChanged>(this.OnAvailableAnchorsChanged);
            base.Handles<AnchorsReserved>(this.OnAnchorsReserved);
            base.Handles<AnchorsReservationCommitted>(this.OnAnchorsReservationCommitted);
            base.Handles<AnchorsReservationCancelled>(this.OnAnchorsReservationCancelled);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AnchorsAvailability"/> class, specifying the past events.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="history">The event stream of this event sourced object.</param>
        public AnchorsAvailability(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            this.LoadFrom(history);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AnchorsAvailability"/> class, specifying a snapshot, and the new events since the snapshot was taken.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="memento">A snapshot of the object created by this entity in the past.</param>
        /// <param name="history">The event stream of this event sourced object since the <paramref name="memento"/> was created.</param>
        public AnchorsAvailability(Guid id, IMemento memento, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            var state = (Memento)memento;
            Version = state.Version;
            // make a copy of the state values to avoid concurrency problems with reusing references.
            remainingAnchors.AddRange(state.RemainingAnchors);
            pendingReservations.AddRange(state.PendingReservations);
            LoadFrom(history);
        }

        public void AddAnchors(Guid anchorType, int quantity)
        {
            base.Update(new AvailableAnchorsChanged { Anchors = new[] { new AnchorQuantity(anchorType, quantity) } });
        }

        public void RemoveAnchors(Guid anchorType, int quantity)
        {
            base.Update(new AvailableAnchorsChanged { Anchors = new[] { new AnchorQuantity(anchorType, -quantity) } });
        }

        /// <summary>
        /// Requests a reservation for anchors.
        /// </summary>
        /// <param name="reservationId">A token identifying the reservation request.</param>
        /// <param name="wantedAnchors">The list of anchor requirements.</param>
        /// <remarks>The reservation id is not the id for an aggregate root, just an opaque identifier.</remarks>
        public void MakeReservation(Guid reservationId, IEnumerable<AnchorQuantity> wantedAnchors)
        {
            var wantedList = wantedAnchors.ToList();
            if (wantedList.Any(x => !this.remainingAnchors.ContainsKey(x.AnchorType)))
            {
                throw new ArgumentOutOfRangeException("wantedAnchors");
            }

            var difference = new Dictionary<Guid, AnchorDifference>();

            foreach (var w in wantedList)
            {
                var item = GetOrAdd(difference, w.AnchorType);
                item.Wanted = w.Quantity;
                item.Remaining = this.remainingAnchors[w.AnchorType];
            }

            List<AnchorQuantity> existing;
            if (this.pendingReservations.TryGetValue(reservationId, out existing))
            {
                foreach (var e in existing)
                {
                    GetOrAdd(difference, e.AnchorType).Existing = e.Quantity;
                }
            }

            var reservation = new AnchorsReserved
            {
                ReservationId = reservationId,
                ReservationDetails = difference.Select(x => new AnchorQuantity(x.Key, x.Value.Actual)).Where(x => x.Quantity != 0).ToList(),
                AvailableAnchorsChanged = difference.Select(x => new AnchorQuantity(x.Key, -x.Value.DeltaSinceLast)).Where(x => x.Quantity != 0).ToList()
            };

            base.Update(reservation);
        }

        public void CancelReservation(Guid reservationId)
        {
            List<AnchorQuantity> reservation;
            if (this.pendingReservations.TryGetValue(reservationId, out reservation))
            {
                base.Update(new AnchorsReservationCancelled
                {
                    ReservationId = reservationId,
                    AvailableAnchorsChanged = reservation.Select(x => new AnchorQuantity(x.AnchorType, x.Quantity)).ToList()
                });
            }
        }

        public void CommitReservation(Guid reservationId)
        {
            if (this.pendingReservations.ContainsKey(reservationId))
            {
                base.Update(new AnchorsReservationCommitted { ReservationId = reservationId });
            }
        }

        private class AnchorDifference
        {
            public int Wanted { get; set; }
            public int Existing { get; set; }
            public int Remaining { get; set; }
            public int Actual
            {
                get { return Math.Min(this.Wanted, Math.Max(this.Remaining, 0) + this.Existing); }
            }
            public int DeltaSinceLast
            {
                get { return this.Actual - this.Existing; }
            }
        }

        private static TValue GetOrAdd<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = new TValue();
                dictionary[key] = value;
            }

            return value;
        }

        private void OnAvailableAnchorsChanged(AvailableAnchorsChanged e)
        {
            foreach (var anchor in e.Anchors)
            {
                int newValue = anchor.Quantity;
                int remaining;
                if (this.remainingAnchors.TryGetValue(anchor.AnchorType, out remaining))
                {
                    newValue += remaining;
                }

                this.remainingAnchors[anchor.AnchorType] = newValue;
            }
        }

        private void OnAnchorsReserved(AnchorsReserved e)
        {
            var details = e.ReservationDetails.ToList();
            if (details.Count > 0)
            {
                this.pendingReservations[e.ReservationId] = details;
            }
            else
            {
                this.pendingReservations.Remove(e.ReservationId);
            }

            foreach (var anchor in e.AvailableAnchorsChanged)
            {
                this.remainingAnchors[anchor.AnchorType] = this.remainingAnchors[anchor.AnchorType] + anchor.Quantity;
            }
        }

        private void OnAnchorsReservationCommitted(AnchorsReservationCommitted e)
        {
            this.pendingReservations.Remove(e.ReservationId);
        }

        private void OnAnchorsReservationCancelled(AnchorsReservationCancelled e)
        {
            this.pendingReservations.Remove(e.ReservationId);

            foreach (var anchor in e.AvailableAnchorsChanged)
            {
                this.remainingAnchors[anchor.AnchorType] = this.remainingAnchors[anchor.AnchorType] + anchor.Quantity;
            }
        }

        /// <summary>
        /// Saves the object's state to an opaque memento object (a snapshot) that can be used to restore the state by using the constructor overload.
        /// </summary>
        /// <returns>An opaque memento object that can be used to restore the state.</returns>
        public IMemento SaveToMemento()
        {
            return new Memento
            {
                Version = this.Version,
                RemainingAnchors = this.remainingAnchors.ToArray(),
                PendingReservations = this.pendingReservations.ToArray(),
            };
        }

        internal class Memento : IMemento
        {
            public int Version { get; internal set; }
            internal KeyValuePair<Guid, int>[] RemainingAnchors { get; set; }
            internal KeyValuePair<Guid, List<AnchorQuantity>>[] PendingReservations { get; set; }
        }
    }
}
