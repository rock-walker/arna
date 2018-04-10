using AP.Shared.Geo.Models;
using System.Threading.Tasks;

namespace AP.Shared.Geo.Contracts
{
    public interface IGeoLocator
    {
        Task<GooglePlacesResult> DecodeAddress(string placeId);
    }
}
