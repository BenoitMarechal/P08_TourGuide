using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral =rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }
   
    public async Task CalculateRewards(User user)
    {
        count++;

        // Create a snapshot of the visited locations to avoid modification issues
        List<VisitedLocation> userLocationsSnapshot = user.VisitedLocations.ToList();

        List<Attraction> attractions = await _gpsUtil.GetAttractions();

        foreach (var visitedLocation in userLocationsSnapshot)
        {
            foreach (var attraction in attractions)
            {
                if (!user.UserRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
                {
                    var nearAttraction = await NearAttraction(visitedLocation, attraction);
                    if (nearAttraction)
                    {
                        user.AddUserReward(new UserReward(visitedLocation, attraction, await GetRewardPoints(attraction, user)));
                    }
                }
            }
        }
    }


    public async Task<bool> IsWithinAttractionProximity(Attraction attraction, Location location)
    {
        Console.WriteLine("dist");
        Console.WriteLine(GetDistance(attraction, location));
        var distance = GetDistance(attraction, location);
        return distance <= _attractionProximityRange;
    }

    private async Task<bool> NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        var result = GetDistance(attraction, visitedLocation.Location);

        return result <= _proximityBuffer;
    }

    public async Task<int> GetRewardPoints(Attraction attraction, User user)
    {
        var result= await _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
        return result;
    }

    public double GetDistance(Location loc1, Location loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
