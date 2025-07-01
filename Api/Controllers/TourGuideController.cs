using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase

    // Finir tour des méthodes en async et check resultats

    
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    [HttpGet("getLocation")]
   
    public async Task<ActionResult<VisitedLocation>> GetLocation([FromQuery] string userName)
    {
        var user =  await _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }
        var location = await _tourGuideService.GetUserLocation(user);
        return Ok(location);
    }

    [HttpGet("getNearbyAttractions")]
    // TODO: Change this method to no longer return a List of Attractions.
    // Instead: Get the closest five tourist attractions to the user - no matter how far away they are.
    // Return a new JSON object that contains:

    // Name of Tourist attraction, 
    // Tourist attractions lat/long, 
    // The user's location lat/long, 
    // The distance in miles between the user's location and each of the attractions.
    // The reward points for visiting each Attraction.



    //    Note: Attraction reward points can be gathered from RewardsCentral
    public async Task<ActionResult<List<NearByAttraction>>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = await _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }
        var visitedLocation = await _tourGuideService.GetUserLocation(user);
        var attractions = await _tourGuideService.GetNearByAttractions(visitedLocation, user);
        return Ok(attractions);
    }

    [HttpGet("getRewards")]
    public async Task<ActionResult<IEnumerable<UserReward>>> GetRewards([FromQuery] string userName)
    {
        var user = await _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }        
        var rewards = await _tourGuideService.GetUserRewards(user);
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public async Task<ActionResult<IEnumerable<Provider>>> GetTripDeals([FromQuery] string userName)
    {        
        var user = await _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var deals = await _tourGuideService.GetTripDeals(user);

        return Ok(deals);
    }

    private async Task<User> GetUser(string userName)
    {       
        var result= await _tourGuideService.GetUser(userName);
        return result;
    }
  
}
