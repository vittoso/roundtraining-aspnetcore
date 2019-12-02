using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("/api/cities")]
    public class PointsOfInterestController : Controller
    {
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Citites.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Citites.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var poi = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (poi == null)
                return NotFound();

            return Ok(poi);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody]PointOfInterestForCreationDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Description == dto.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var city = CitiesDataStore.Current.Citites.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();

            var maxPointOfInterestId = CitiesDataStore.Current.Citites.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = dto.Name,
                Description = dto.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody]PointOfInterestForUpdateDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Description == dto.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var city = CitiesDataStore.Current.Citites.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();


            var poi = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (poi == null)
                return NotFound();


            poi.Name = dto.Name;
            poi.Description = dto.Description;


            return NoContent(); // or return OK(poi);
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody]JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var city = CitiesDataStore.Current.Citites.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();


            var poi = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (poi == null)
                return NotFound();


            var poiToPatch = new PointOfInterestForUpdateDto()
            {
                Name = poi.Name,
                Description = poi.Description,
            };

            patchDoc.ApplyTo(poiToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (poiToPatch.Description == poiToPatch.Name)
                ModelState.AddModelError("Description", "The provided description should be different from the name.");

            TryValidateModel(poiToPatch);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            poi.Name = poiToPatch.Name;
            poi.Description = poiToPatch.Description;


            return NoContent(); // or return OK(poi);
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Citites.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
                return NotFound();


            var poi = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (poi == null)
                return NotFound();

            city.PointsOfInterest.Remove(poi);

            return NoContent(); // or return OK(poi);
        }
    }
}
