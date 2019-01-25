using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }


        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointOfInterest(int cityId)
        {
            try
            {
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
                if(!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with Id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                var poiForCityResults = new List<PointOfInterestsDto>();
                foreach (var poi in pointsOfInterestForCity)
                {
                    poiForCityResults.Add(new PointOfInterestsDto
                    {
                        Id = poi.Id,
                        Name = poi.Name,
                        Description = poi.Description
                    });
                }

                return Ok(poiForCityResults);
                //if (city == null)
                //{
                //    _logger.LogInformation($"City with Id {cityId} wasn't found when accessing points of interest");
                //    return NotFound();
                //}

                //return Ok(city.PointOfInterests);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points od interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem happened while handling your request");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if(!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            var poiResult = new PointOfInterestsDto()
            {
                Id = pointOfInterest.Id,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            return Ok(poiResult);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointOfInterest = city.PointOfInterests.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterest == null)
            //    return NotFound();

            //return Ok(pointOfInterest);
        }

        [HttpPost("{cityId}/pointofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)

        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointOfInterests).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestsDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description

            };

            city.PointOfInterests.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);

        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterests.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
                return NotFound();

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterests.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
                return NotFound();

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
                ModelState.AddModelError("Description", "Description should be different from the Name.");

            TryValidateModel(pointOfInterestToPatch);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointOfInterests.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
                return NotFound();

            city.PointOfInterests.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest deleted",
                $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted");

            return NoContent();
        }
    }
}
