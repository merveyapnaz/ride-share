using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdessoRideShare.infrastructure.Services.Interfaces;
using AdessoRideShare.model.entities;
using AdessoRideShare.model.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdessoRideShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelPlanController : Controller
    {
        private readonly ITravelPlanService _travelPlanService;

        public TravelPlanController(
            ITravelPlanService travelPlanService)
        {
            _travelPlanService = travelPlanService;
        }
        
        [HttpPost, Route("post")]
        public async Task<IActionResult> Post([FromBody] TravelPlan model)
        {
            var response = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    response = await _travelPlanService.Add(model);
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }
            else
            {
                response = "Model is not valid";
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost, Route("publish/{id}")]
        public async Task<IActionResult> Publish(int id)
        {
            var response = string.Empty;

            try
            {
                response = await _travelPlanService.Publish(id);
            }
            catch (Exception ex)
            {
                response = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost, Route("unpublish/{id}")]
        public async Task<IActionResult> UnPublish(int id)
        {
            var response = string.Empty;

            try
            {
                response = await _travelPlanService.UnPublish(id);
            }
            catch (Exception ex)
            {
                response = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }
        
        [HttpPost, Route("filterplans")]
        public async Task<IActionResult> FilterPlans([FromBody] TravelPlanVM model)
        {
            var response = new List<TravelPlan>();

            if (ModelState.IsValid)
            {
                try
                {
                    response = await _travelPlanService.FilterPlans(model);
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost, Route("registertheplan/{id}")]
        public async Task<IActionResult> RegisterThePlan(int id)
        {
            var response = string.Empty;

            try
            {
                response = await _travelPlanService.RegisterThePlan(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}