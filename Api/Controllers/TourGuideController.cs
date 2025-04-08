using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
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
    public async Task<ActionResult<List<NearByAttraction>>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = await _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }
        var visitedLocation = await _tourGuideService.GetUserLocation(user);
        var attractions = _tourGuideService.GetNearByAttractions(visitedLocation, await GetUser(userName));
        return Ok(attractions);
    }

    [HttpGet("getRewards")]
    public async Task<ActionResult<List<UserReward>>> GetRewards([FromQuery] string userName)
    {
        var user = await _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }
        var rewards = _tourGuideService.GetUserRewards(user);
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public async Task<ActionResult<List<Provider>>> GetTripDeals([FromQuery] string userName)
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
