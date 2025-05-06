using GpsUtil.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.Users;
using TourGuide.Utilities;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;

    public RewardServiceTest(DependencyFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void UserGetRewards()
    {
        _fixture.Initialize(0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var attattractions = await _fixture.GpsUtil.GetAttractions();
        var attraction = attattractions.First();        
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        await _fixture.TourGuideService.TrackUserLocation(user);
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.True(userRewards.Count == 1);
    }

    [Fact]
    public async void IsWithinAttractionProximity()
    {
        var attattractions = await _fixture.GpsUtil.GetAttractions();
        var attraction = attattractions.First();
        Assert.True(await _fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

    [Fact]
    public async Task NearAllAttractions()
    {
        _fixture.Initialize(1);
        await _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        // Get first user
        var allUsers = await _fixture.TourGuideService.GetAllUsers();
        var user = allUsers[0];

        // Get list of attractions
        var attractions = await _fixture.GpsUtil.GetAttractions();


        // Calcute user's rewards
        await _fixture.RewardsService.CalculateRewards(user, attractions);

        var userRewards = await _fixture.TourGuideService.GetUserRewards(user);
        // OK




        _fixture.TourGuideService.Tracker.StopTracking();

        Assert.Equal(attractions.Count, userRewards.Count());
    }


}
