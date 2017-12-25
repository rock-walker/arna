// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://go.microsoft.com/fwlink/p/?LinkID=258575
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

namespace AP.Business.Registration.ReadModel.Implementation
{
    using AP.EntityModel.ReadModel;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Decorator that wraps <see cref="WorkshopDao"/> and caches this data in memory, as it can be accessed several times.
    /// This embraces eventual consistency, as we acknowledge the fact that the read model is stale even without caching.
    /// </summary>
    /// <remarks>
    /// For more information on the optimizations we did for V3
    /// see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see>.
    /// </remarks>
    public class CachingWorkshopDao : IWorkshopDao
    {
        private readonly IWorkshopDao decoratedDao;
        private readonly IMemoryCache cache;

        public CachingWorkshopDao(IWorkshopDao decoratedDao, IMemoryCache cache)
        {
            this.decoratedDao = decoratedDao;
            this.cache = cache;
        }

        public WorkshopDetails GetWorkshopDetails(string workshopCode)
        {
            var key = "WorkshopDao_Details_" + workshopCode;
            var conference = this.cache.Get(key) as WorkshopDetails;
            if (conference == null)
            {
                conference = this.decoratedDao.GetWorkshopDetails(workshopCode);
                if (conference != null)
                {
                    this.cache.Set(key, conference, DateTimeOffset.UtcNow.AddMinutes(10));
                }
            }

            return conference;
        }

        public WorkshopAlias GetWorkshopAlias(string workshopCode)
        {
            var key = "WorkshopDao_Alias_" + workshopCode;
            var conference = this.cache.Get(key) as WorkshopAlias;
            if (conference == null)
            {
                conference = this.decoratedDao.GetWorkshopAlias(workshopCode);
                if (conference != null)
                {
                    this.cache.Set(key, conference, DateTimeOffset.UtcNow.AddMinutes(20));
                }
            }

            return conference;
        }

        public IList<WorkshopAlias> GetPublishedWorkshops()
        {
            var key = "WorkshopDao_PublishedWorkshops";
            var cached = this.cache.Get(key) as IList<WorkshopAlias>;
            if (cached == null)
            {
                cached = this.decoratedDao.GetPublishedWorkshops();
                if (cached != null)
                {
                    this.cache.Set(key, cached, DateTimeOffset.UtcNow.AddSeconds(10));
                }
            }

            return cached;
        }

        /// <summary>
        /// Gets ifnromation about the seat types.
        /// </summary>
        /// <remarks>
        /// Because the seat type contains the number of available seats, and this information can change often, notice
        /// how we manage the risks associated with displaying data that is very stale by adjusting caching duration 
        /// or not even caching at all if only a few seats remain.
        /// For more information on the optimizations we did for V3
        /// see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see>.
        /// </remarks>
        public IList<AnchorType> GetPublishedAnchorTypes(Guid workshopId)
        {
            var key = "ConferenceDao_PublishedSeatTypes_" + workshopId;
            var anchorTypes = this.cache.Get(key) as IList<AnchorType>;
            if (anchorTypes == null)
            {
                anchorTypes = this.decoratedDao.GetPublishedAnchorTypes(workshopId);
                if (anchorTypes != null)
                {
                    // determine how long to cache depending on criticality of using stale data.
                    TimeSpan timeToCache;
                    if (anchorTypes.All(x => x.AvailableQuantity > 200 || x.AvailableQuantity <= 0))
                    {
                        timeToCache = TimeSpan.FromMinutes(5);
                    }
                    else if (anchorTypes.Any(x => x.AvailableQuantity < 30 && x.AvailableQuantity > 0))
                    {
                        // there are just a few seats remaining. Do not cache.
                        timeToCache = TimeSpan.Zero;
                    }
                    else if (anchorTypes.Any(x => x.AvailableQuantity < 100 && x.AvailableQuantity > 0))
                    {
                        timeToCache = TimeSpan.FromSeconds(20);
                    }
                    else
                    {
                        timeToCache = TimeSpan.FromMinutes(1);
                    }

                    if (timeToCache > TimeSpan.Zero)
                    {
                        this.cache.Set(key, anchorTypes, DateTimeOffset.UtcNow.Add(timeToCache));
                    }
                }
            }

            return anchorTypes;
        }

        public IList<AnchorTypeName> GetAnchorTypeNames(IEnumerable<Guid> seatTypes)
        {
            return this.decoratedDao.GetAnchorTypeNames(seatTypes);
        }
    }
}
