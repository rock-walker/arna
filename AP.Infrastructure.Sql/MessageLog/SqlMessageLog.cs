namespace AP.Infrastructure.Sql.MessageLog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.MessageLog;
    using Infrastructure.Messaging;
    using Infrastructure.Serialization;
    using EntityFramework.DbContextScope.Interfaces;
    using AP.Core.Database;

    public class SqlMessageLog : AmbientContext<MessageLogDbContext>, IEventLogReader
    {
        private IMetadataProvider metadataProvider;
        private ITextSerializer serializer;
        private readonly IDbContextScopeFactory factory;
        private readonly IAmbientDbContextLocator locator;

        public SqlMessageLog(IDbContextScopeFactory factory, 
          IAmbientDbContextLocator locator, ITextSerializer serializer, IMetadataProvider metadataProvider)
            :base(locator)
        {
            this.serializer = serializer;
            this.metadataProvider = metadataProvider;
            this.factory = factory;
            this.locator = locator;
        }

        public void Save(IEvent @event)
        {
            using (var context = factory.Create())
            {
                var metadata = this.metadataProvider.GetMetadata(@event);

                DbContext.Set<MessageLogEntity>().Add(new MessageLogEntity
                {
                    Id = Guid.NewGuid(),
                    SourceId = @event.SourceId.ToString(),
                    Kind = metadata.TryGetValue(StandardMetadata.Kind),
                    AssemblyName = metadata.TryGetValue(StandardMetadata.AssemblyName),
                    FullName = metadata.TryGetValue(StandardMetadata.FullName),
                    Namespace = metadata.TryGetValue(StandardMetadata.Namespace),
                    TypeName = metadata.TryGetValue(StandardMetadata.TypeName),
                    SourceType = metadata.TryGetValue(StandardMetadata.SourceType) as string,
                    CreationDate = DateTime.UtcNow.ToString("o"),
                    Payload = serializer.Serialize(@event),
                });

                context.SaveChanges();
            }
        }

        public void Save(ICommand command)
        {
            using (var context = factory.Create())
            {
                var metadata = metadataProvider.GetMetadata(command);

                DbContext.Set<MessageLogEntity>().Add(new MessageLogEntity
                {
                    Id = Guid.NewGuid(),
                    SourceId = command.Id.ToString(),
                    Kind = metadata.TryGetValue(StandardMetadata.Kind),
                    AssemblyName = metadata.TryGetValue(StandardMetadata.AssemblyName),
                    FullName = metadata.TryGetValue(StandardMetadata.FullName),
                    Namespace = metadata.TryGetValue(StandardMetadata.Namespace),
                    TypeName = metadata.TryGetValue(StandardMetadata.TypeName),
                    SourceType = metadata.TryGetValue(StandardMetadata.SourceType) as string,
                    CreationDate = DateTime.UtcNow.ToString("o"),
                    Payload = serializer.Serialize(command),
                });

                context.SaveChanges();
            }
        }

        public IEnumerable<IEvent> Query(QueryCriteria criteria)
        {
            return new SqlQuery(this.factory, this.locator,  this.serializer, criteria);
        }

        private class SqlQuery : AmbientContext<MessageLogDbContext>, IEnumerable<IEvent>
        {
            private IDbContextScopeFactory factory;
            private IAmbientDbContextLocator locator;
            private ITextSerializer serializer;
            private QueryCriteria criteria;

            public SqlQuery(IDbContextScopeFactory factory, IAmbientDbContextLocator locator, ITextSerializer serializer, QueryCriteria criteria)
                :base(locator)
            {
                this.factory = factory;
                this.locator = locator;
                this.serializer = serializer;
                this.criteria = criteria;
            }

            public IEnumerator<IEvent> GetEnumerator()
            {
                return new DisposingEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class DisposingEnumerator : IEnumerator<IEvent>
            {
                private SqlQuery sqlQuery;
                private MessageLogDbContext context;
                private IEnumerator<IEvent> events;

                public DisposingEnumerator(SqlQuery sqlQuery)
                {
                    this.sqlQuery = sqlQuery;
                }

                ~DisposingEnumerator()
                {
                    if (context != null) context.Dispose();
                }

                public void Dispose()
                {
                    if (context != null)
                    {
                        context.Dispose();
                        context = null;
                        GC.SuppressFinalize(this);
                    }
                    if (events != null)
                    {
                        events.Dispose();
                    }
                }

                public IEvent Current { get { return events.Current; } }
                object IEnumerator.Current { get { return this.Current; } }

                public bool MoveNext()
                {
                    if (context == null)
                    {
                        using (var context = sqlQuery.factory.Create())
                        {
                            var queryable = sqlQuery.DbContext.Set<MessageLogEntity>().AsQueryable()
                                .Where(x => x.Kind == StandardMetadata.EventKind);

                            var where = sqlQuery.criteria.ToExpression();
                            if (where != null)
                                queryable = queryable.Where(where);

                            events = queryable
                                .AsEnumerable()
                                .Select(x => this.sqlQuery.serializer.Deserialize<IEvent>(x.Payload))
                                .GetEnumerator();
                        }
                    }

                    return events.MoveNext();
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
