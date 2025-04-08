using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuide.Services.Interfaces
{
    public interface IRewardsService
    {
        Task CalculateRewards(User user);
        double GetDistance(Location loc1, Location loc2);
        Task<bool> IsWithinAttractionProximity(Attraction attraction, Location location);
        void SetDefaultProximityBuffer();
        void SetProximityBuffer(int proximityBuffer);
    }
}