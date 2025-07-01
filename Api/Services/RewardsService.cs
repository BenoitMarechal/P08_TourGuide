using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = int.MaxValue;
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

    public async Task SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public async Task SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    //public async Task CalculateRewards(User user, List<Attraction> attractions)
    //{
    //    count++;

    //    // Create a snapshot of the visited locations to avoid modification issues
    //    List<VisitedLocation> userLocationsSnapshot = user.VisitedLocations.ToList();

    //    // List<Attraction> attractions = await _gpsUtil.GetAttractions();

    //    foreach (var visitedLocation in userLocationsSnapshot)
    //    {
    //        foreach (var attraction in attractions)
    //        {
    //            if (!user.UserRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
    //            {
    //                var nearAttraction = await NearAttraction(visitedLocation, attraction);
    //                if (nearAttraction)
    //                {
    //                    user.AddUserReward(new UserReward(visitedLocation, attraction, await GetRewardPoints(attraction, user)));
    //                }
    //            }
    //        }
    //    }
    //}

    public async Task CalculateRewards(User user, List<Attraction> attractions)
    {
        count++;
        var userLocationsSnapshot = user.VisitedLocations.ToList();

        // Copy the existing rewards once to reduce repeated calls to .Any
        var existingRewardNames = new HashSet<string>(
            user.UserRewards.Select(r => r.Attraction.AttractionName)
        );

        var rewardTasks = new List<Task>();

        foreach (var location in userLocationsSnapshot)
        {
            foreach (var attraction in attractions)
            {
                if (existingRewardNames.Contains(attraction.AttractionName))
                    continue;

                rewardTasks.Add(ProcessReward(location, attraction, user));
            }
        }

        await Task.WhenAll(rewardTasks);
    }

    private async Task ProcessReward(VisitedLocation location, Attraction attraction, User user)
    {
        if (await NearAttraction(location, attraction))
        {
            var points = await GetRewardPoints(attraction, user);
            user.AddUserReward(new UserReward(location, attraction, points));
        }
    }



    public async Task<bool> IsWithinAttractionProximity(Attraction attraction, Location location)
    {

        var distance = await GetDistance(attraction, location);
        return distance <= _attractionProximityRange;
    }

    private async Task<bool> NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        var result = await GetDistance(attraction, visitedLocation.Location);

        return result <= _proximityBuffer;
    }

    public async Task<int> GetRewardPoints(Attraction attraction, User user)
    {
        var result= await _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
        return result;
    }

    public async Task <double> GetDistance(Location loc1, Location loc2)
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
