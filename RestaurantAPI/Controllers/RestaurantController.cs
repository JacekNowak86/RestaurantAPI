using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
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
        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            /*var isUpdated =*/_restaurantService.Update(id,dto/*, User*/);
            //if (!isUpdated)
            //{
            //    return NotFound();
            //}
            return Ok();
        }
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var isDeleted = _restaurantService.Delete(id/*, User*/);
            if (isDeleted)
            {
                return NoContent();
            }
            return NotFound();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]

        //[Authorize(Roles = "Manager")]
        public ActionResult CreateRestaurant([FromBody]CreateRestaurantDto dto)
        {
            //HttpContext.User.IsInRole("Admin");
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var userId = int.Parse(User.FindFirst(c=> c.Type == ClaimTypes.NameIdentifier).Value);
            var id =_restaurantService.Create(dto/*,userId*/);
            //var restaurant = _mapper.Map<Restaurant>(dto);
            //_dbContext.Restaurants.Add(restaurant);
            //_dbContext.SaveChanges();
            return Created($"/api/restaurant/{id/*restaurant.Id*/}",null);
        }
        [HttpGet]
        //[Authorize(Policy = "HasNationality")]
        [Authorize(Policy = "Atleast20")]
        

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
        [AllowAnonymous]
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
