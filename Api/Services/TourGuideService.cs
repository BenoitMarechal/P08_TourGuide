using GpsUtil.Location;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using TourGuide.LibrairiesWrappers;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services;

public class TourGuideService : ITourGuideService
{
    private readonly ILogger _logger;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardsService _rewardsService;
    private readonly TripPricer.TripPricer _tripPricer;
    public Tracker Tracker { get; private set; }
    private readonly Dictionary<string, User> _internalUserMap = new();
    private const string TripPricerApiKey = "test-server-api-key";
    private bool _testMode = true;

    public TourGuideService(ILogger<TourGuideService> logger, IGpsUtil gpsUtil, IRewardsService rewardsService, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _tripPricer = new();
        _gpsUtil = gpsUtil;
        _rewardsService = rewardsService;

        CultureInfo.CurrentCulture = new CultureInfo("en-US");

        if (_testMode)
        {
            _logger.LogInformation("TestMode enabled");
            _logger.LogDebug("Initializing users");
            InitializeInternalUsers();
            _logger.LogDebug("Finished initializing users");
        }

        var trackerLogger = loggerFactory.CreateLogger<Tracker>();

        Tracker = new Tracker(this, trackerLogger);
        AddShutDownHook();
    }

    public async Task<List<UserReward>> GetUserRewards(User user)
    {
        return user.UserRewards;
    }

    public async Task<VisitedLocation> GetUserLocation(User user)
    {
        var lastVisited = await user.GetLastVisitedLocation();
        var trackedLocation = await TrackUserLocation(user);
        return user.VisitedLocations.Any() ? lastVisited : trackedLocation;
    }

    public async Task<User> GetUser(string userName)
    {
        return _internalUserMap.ContainsKey(userName) ? _internalUserMap[userName] : null;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return _internalUserMap.Values.ToList();
    }

    public async Task AddUser(User user)
    {
        if (!_internalUserMap.ContainsKey(user.UserName))
        {
            _internalUserMap.Add(user.UserName, user);
        }
    }



    public async Task<List<Provider>> GetTripDeals(User user)
    {
        int cumulativeRewardPoints = user.UserRewards.Sum(i => i.RewardPoints);

        List<Provider> providers = await Task.Run(() =>
            _tripPricer.GetPrice(TripPricerApiKey, user.UserId,
                user.UserPreferences.NumberOfAdults, user.UserPreferences.NumberOfChildren,
                user.UserPreferences.TripDuration, cumulativeRewardPoints));

        user.TripDeals = providers;
        return providers;
    }


    public async Task<VisitedLocation> TrackUserLocation(User user)
    {
        VisitedLocation visitedLocation = await _gpsUtil.GetUserLocation(user.UserId);
        var attractions = await _gpsUtil.GetAttractions();
        user.AddToVisitedLocations(visitedLocation);
        await _rewardsService.CalculateRewards(user, attractions);
        return visitedLocation;
    }

    public async Task<List<NearByAttraction>> GetNearByAttractions(VisitedLocation visitedLocation, User user)
    {

        List<(Attraction, double Distance)> allAttractionsWithDistance = new();

        var userLocation = await GetUserLocation(user);
        var attractions = await _gpsUtil.GetAttractions();

        foreach (var attraction in attractions)
        {
            var attractionLocation = new Location(attraction.Latitude, attraction.Longitude);

            var distance = await _rewardsService.GetDistance(attractionLocation, userLocation.Location);

           // var rewardWrapper = new RewardCentralWrapper();

           // var reward = await rewardWrapper.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);

           // var nearbyAttraction = new NearByAttraction(attraction, userLocation.Location, distance, 0);

            allAttractionsWithDistance.Add((  attraction, distance));
        }

        var shortList = allAttractionsWithDistance.OrderBy(a => a.Distance).Take(5).ToList();


        List<NearByAttraction> shortListWithRewards = new();
        foreach(var nearByAttraction in shortList)
        {
            var rewardWrapper = new RewardCentralWrapper();
            var reward = await rewardWrapper.GetAttractionRewardPoints(nearByAttraction.Item1.AttractionId, user.UserId);
            var nearbyAttraction = new NearByAttraction(nearByAttraction.Item1, userLocation.Location, nearByAttraction.Distance, reward);
            shortListWithRewards.Add(nearbyAttraction);

        }


        return shortListWithRewards;



       // return nearbyAttractions.OrderBy(a => a.Distance).Take(5).ToList();
    }

    private void AddShutDownHook()
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, e) => Tracker.StopTracking();
    }

    /**********************************************************************************
    * 
    * Methods Below: For Internal Testing
    * 
    **********************************************************************************/

    private void InitializeInternalUsers()
    {
        for (int i = 0; i < InternalTestHelper.GetInternalUserNumber(); i++)
        {
            var userName = $"internalUser{i}";
            var user = new User(Guid.NewGuid(), userName, "000", $"{userName}@tourGuide.com");
            GenerateUserLocationHistory(user);
            _internalUserMap.Add(userName, user);
        }

        _logger.LogDebug($"Created {InternalTestHelper.GetInternalUserNumber()} internal test users.");
    }

    private void GenerateUserLocationHistory(User user)
    {
        for (int i = 0; i < 3; i++)
        {
            var visitedLocation = new VisitedLocation(user.UserId, new Location(GenerateRandomLatitude(), GenerateRandomLongitude()), GetRandomTime());
            user.AddToVisitedLocations(visitedLocation);
        }
    }

    private static readonly Random random = new Random();

    private double GenerateRandomLongitude()
    {
        return new Random().NextDouble() * (180 - (-180)) + (-180);
    }

    private double GenerateRandomLatitude()
    {
        return new Random().NextDouble() * (90 - (-90)) + (-90);
    }

    private DateTime GetRandomTime()
    {
        return DateTime.UtcNow.AddDays(-new Random().Next(30));
    }

}