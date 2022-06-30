using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        //private readonly RestaurantDbContext _dbContext;
        //private readonly IMapper _mapper;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        //public RestaurantController(RestaurantDbContext dbContext, IMapper mapper)
        //{
        //    _dbContext = dbContext;
        //    _mapper = mapper;
        //}
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var isDeleted = _restaurantService.Delete(id);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound();
        }
        [HttpPost]
        public ActionResult CreateRestaurant([FromBody]CreateRestaurantDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var id =_restaurantService.Create(dto);
            //var restaurant = _mapper.Map<Restaurant>(dto);
            //_dbContext.Restaurants.Add(restaurant);
            //_dbContext.SaveChanges();
            return Created($"/api/restaurant/{id/*restaurant.Id*/}",null);
        }
        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            //var restaurants = _dbContext.Restaurants.Include(r=>r.Address).Include(r=>r.Dishes).ToList();
            //var restaurantsDto = restaurants.Select(x => new RestaurantDto()
            //{
            //    Name = x.Name,
            //    Category = x.Category,
            //    City = x.Address.City,
            //});
            //var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);
            var restaurantsDto = _restaurantService.GetAll();
            return Ok(restaurantsDto);
        }
        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            //var restaurant = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).FirstOrDefault(r => r.Id == id);

            //if (restaurant == null)
            //{
            //    return NotFound();
            //}
            //var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            var restaurantDto = _restaurantService.GetById(id);
            return Ok(restaurantDto);
        }
    }

}
