using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto/*, int userId*/);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query/*string searchPhrase*/);
        RestaurantDto GetById(int id);
        bool Delete(int id/*, ClaimsPrincipal user*/);
        
        void /*bool*/ Update(int id, UpdateRestaurantDto dto/*, ClaimsPrincipal user*/);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        public readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).FirstOrDefault(r => r.Id == id);
            if (restaurant is null)
            {
                return null;
            }
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return restaurantDto;
        }
        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query/*string searchPhrase*/)
        {

            var baseQuery = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPhrase == null
                || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r=> r.Name },
                    {nameof(Restaurant.Description), r=> r.Description },
                    {nameof(Restaurant.Category), r=> r.Category }
                };
                var selectedColumn = columsSelector[query.SortBy];
                baseQuery = query.SortDirection == SortDirection.ASC ? baseQuery.OrderBy(selectedColumn) : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                .Skip(query.PageSize*(query.PageNumber-1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDto, totalItemsCount , query.PageSize, query.PageNumber);
            return result;
        }

        public int Create(CreateRestaurantDto dto/*, int userId*/)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
        public bool Delete(int id/*, ClaimsPrincipal user*/)
        {
            _logger.LogInformation("loguje delete");
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant is null)
            {
                return false;
            }
            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
            return true;
        }
        public void /*bool*/ Update(int id, UpdateRestaurantDto dto/*, ClaimsPrincipal user*/)
        {
            
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
                //return false;
            }
            var authorizationResult =  _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            _dbContext.SaveChanges();
            //return true;
        }
    }
}
