using GpsUtil.Location;
using System.Threading.Tasks;
using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services.Interfaces
{
    public interface ITourGuideService
    {
        Tracker Tracker { get; }

        void AddUser(User user);
        List<User> GetAllUsers();
        // List<NearByAttraction> GetNearByAttractions(VisitedLocation visitedLocation);

        List<NearByAttraction> GetNearByAttractions(VisitedLocation visitedLocation, User user);
        //List<Provider> GetTripDeals(User user);
        Task<List<Provider>> GetTripDeals(User user);
        Task<User> GetUser(string userName);
        VisitedLocation GetUserLocation(User user);
        Task<List<UserReward>> GetUserRewards(User user);
        VisitedLocation TrackUserLocation(User user);
    }
}