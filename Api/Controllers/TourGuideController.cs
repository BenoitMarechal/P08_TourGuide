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
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var location = _tourGuideService.GetUserLocation(user);
        return Ok(location);
    }

    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearByAttraction>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }


        var visitedLocation = _tourGuideService.GetUserLocation(user);
        var attractions = _tourGuideService.GetNearByAttractions(visitedLocation, GetUser(userName));
        return Ok(attractions);
    }

    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var rewards = _tourGuideService.GetUserRewards(user);
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var user = _tourGuideService.GetUser(userName);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var deals = _tourGuideService.GetTripDeals(user);
        return Ok(deals);
    }

    private User GetUser(string userName)
    {       
        return _tourGuideService.GetUser(userName);
    }
  
}
