using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuide.Services.Interfaces
{
    public interface IRewardsService
    {
        Task CalculateRewards(User user);
        Task<double> GetDistance(Location loc1, Location loc2);
       Task<bool> IsWithinAttractionProximity(Attraction attraction, Location location);
        Task SetDefaultProximityBuffer();
        Task SetProximityBuffer(int proximityBuffer);
    }
}