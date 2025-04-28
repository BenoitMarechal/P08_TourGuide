using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers;

public class GpsUtilWrapper : IGpsUtil
{
    private readonly GpsUtil.GpsUtil _gpsUtil;

    public GpsUtilWrapper()
    {
        _gpsUtil = new();
    }

    public async Task<VisitedLocation> GetUserLocation(Guid userId)
    {
        var result = await _gpsUtil.GetUserLocation(userId);
        return result;
    }

    public async Task<List<Attraction>> GetAttractions()
    {   
        
        var result= await _gpsUtil.GetAttractions();
        return result;
    }
}
