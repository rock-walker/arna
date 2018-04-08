using AP.Shared.Geo.Models;

namespace AP.Shared.Geo.Contracts
{
    public interface IGeoLocator
    {
        GooglePlacesResult DecodeAddress(string placeId);
    }
}
