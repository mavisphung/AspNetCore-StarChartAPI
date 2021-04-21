using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.Where(o => o.Id == id)
                                               .FirstOrDefault();
            if (obj == null)
                return NotFound();
            obj.Satellites = _context.CelestialObjects.Where(ord => ord.OrbitedObjectId == id).ToList();
            return Ok(obj);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var data = _context.CelestialObjects.Where(o => o.Name == name);
            if (!data.Any())
                return NotFound();
            foreach (var item in data)
            {
                item.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == item.Id).ToList();
            }
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var objList = _context.CelestialObjects.ToList();
            foreach (var item in objList)
            {
                item.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == item.Id).ToList();
            }
            return Ok(objList);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject obj)
        {
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = obj.Id }, obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.Find(id);
            if (existingObject == null)
                return NotFound();
            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingObject = _context.CelestialObjects.Find(id);
            if (existingObject == null)
                return NotFound();
            existingObject.Name = name;
            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);
            if (!celestialObjects.Any())
                return NotFound();
            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
