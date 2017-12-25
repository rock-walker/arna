namespace AP.Infrastructure.Sql.BlobStorage
{
    using AP.Core.Database;
    using AP.Infrastructure.BlobStorage;
    using EntityFramework.DbContextScope.Interfaces;

    /// <summary>
    /// Simple local blob storage simulator for easy local debugging. 
    /// Assumes the blobs are persisted as text through an <see cref="ITextSerializer"/>.
    /// </summary>
    public class SqlBlobStorage : AmbientContext<BlobStorageDbContext>, IBlobStorage
    {
        private readonly IDbContextScopeFactory _factory;

        public SqlBlobStorage(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public byte[] Find(string id)
        {
            return DbContext.Find(id);
        }

        public void Save(string id, string contentType, byte[] blob)
        {
             DbContext.Save(id, contentType, blob);
        }

        public void Delete(string id)
        {
            DbContext.Delete(id);
        }
    }
}