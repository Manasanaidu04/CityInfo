using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            //return Ok(CitiesDataStore.Current.Cities);
            var cityEntities = _cityInfoRepository.GetCities();

            var results = new List<CityWithoutPOIDto>();

            foreach (var cityEntity in cityEntities)
            {
                results.Add(new CityWithoutPOIDto
                {
                    Id = cityEntity.Id,
                    Name = cityEntity.Name,
                    Description = cityEntity.Description
                });
            }

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsofInterest = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePointsofInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsofInterest)
            {
                var cityResult = new CityDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                };

                foreach (var poi in city.PointsOfInterest)
                {
                    cityResult.PointOfInterests.Add(new PointOfInterestsDto
                    {
                        Id = poi.Id,
                        Name = poi.Name,
                        Description = poi.Description
                    });
                }

                return Ok(cityResult);
            }

            var CityWithoutPOIResult = new CityWithoutPOIDto()
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description
            };

            return Ok(CityWithoutPOIResult);

            //var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}

            //return Ok(cityToReturn);
        }
    }
}
