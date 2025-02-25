using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location;

public class VisitedLocation
{
    public Guid UserId { get; }
    public Location Location { get; }
    public DateTime TimeVisited { get; }

    public VisitedLocation(Guid userId, Location location, DateTime timeVisited)
    {
        UserId = userId;
        Location = location;
        TimeVisited = timeVisited;
    }
}
