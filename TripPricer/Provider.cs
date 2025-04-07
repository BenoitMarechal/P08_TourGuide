using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPricer;

public class Provider
{
    public string Name { get; set; }
    public double Price { get; set; }
    public Guid TripId { get; set; }

    public Provider(Guid tripId, string name, double price)
    {
        Name = name;
        TripId = tripId;
        Price = price;
    }

    // Optional parameterless constructor if needed for serialization
    public Provider() { }
}
