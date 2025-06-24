namespace GpsUtil.Location
{
    public class NearByAttraction
    {
        public string AttractionName { get; set; }
        //  public Location AttractionLocation { get; set; }

        public Location AttractionLocation { get; set; }

        public Location UserLocation { get; set; }
        public double Distance { get; set; }
        public double Reward { get; set; }

        public NearByAttraction(Attraction attraction, Location userLocation, double distance, double reward)
        {
            AttractionName = attraction.AttractionName;
            AttractionLocation = new Location(attraction.Latitude, attraction.Longitude);
            UserLocation = userLocation;
            Distance = distance;
            Reward = reward;
        }
    }
}
