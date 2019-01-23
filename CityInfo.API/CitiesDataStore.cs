using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {                
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with that big park",
                    PointOfInterests = new List<PointOfInterestsDto>()
                    {
                        new PointOfInterestsDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park in the US."
                        },
                        new PointOfInterestsDto()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A 102-sotry building located in Midtown Manhattan"
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with cathedral that was never really finished",
                    PointOfInterests = new List<PointOfInterestsDto>()
                    {
                        new PointOfInterestsDto()
                        {
                            Id = 1,
                            Name = "Cathedral of Our Lady",
                            Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans"
                        },
                        new PointOfInterestsDto()
                        {
                            Id = 2,
                            Name = "Antwerp Central Station",
                            Description = "The finest example of railwau atchitecture in Belguim."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with that big tower",
                    PointOfInterests = new List<PointOfInterestsDto>()
                    {
                        new PointOfInterestsDto()
                        {
                            Id = 1,
                            Name = "Eiffel Tower",
                            Description = "A wrought iron lattice tower on the Cham de Mars, named after engineer Gustave Eiffel."
                        },
                        new PointOfInterestsDto()
                        {
                            Id = 2,
                            Name = "The Louvre",
                            Description = "The world's largest museum."
                        }
                    }
                }

            };
        }
    }
}
