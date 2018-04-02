using System.Threading.Tasks;

namespace AP.Infrastructure.BlobStorage
{
    public interface IBlobStorage
    {
        Task<byte[]> Find(string id);
        //T Find<T>(string id) where T : class;
        void Save(string id, string contentType, byte[] blob);
        //void Save<T>(string id, T instance) where T : class;
        void Delete(string id);
    }
}