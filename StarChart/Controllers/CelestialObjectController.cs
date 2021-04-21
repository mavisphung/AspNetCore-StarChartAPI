using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
