using AdessoRideShare.infrastructure.Repository;
using AdessoRideShare.infrastructure.Services.Interfaces;
using AdessoRideShare.infrastructure.UnitOfWork;
using AdessoRideShare.model.context;
using AdessoRideShare.model.entities;
using AdessoRideShare.model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AdessoRideShare.infrastructure.Services
{
    public class TravelPlanService : Repository<TravelPlan>, ITravelPlanService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TravelPlan> _repository;
        public TravelPlanService(IUnitOfWork uow,
            IRepository<TravelPlan> repository,
            AdessoRideShareContext context) : base(context)
        {
            _uow = uow;
            _repository = repository;
        }

        public async Task<string> Add(TravelPlan entity)
        {
            var response = "Travel plan successfully saved.";

            _repository.Add(entity);

            await _uow.SaveChangesAsync();

            return response;
        }

        public async Task<string> Publish(int id)
        {
            var response = "Travel plan successfully published.";
            var plan = _repository.FindById(id);

            plan.IsOnAir = true;

            _repository.Update(plan);
            await _uow.SaveChangesAsync();

            return response;
        }

        public async Task<string> UnPublish(int id)
        {
            var response = "The itinerary has been successfully unpublished.";
            var plan = _repository.FindById(id);

            plan.IsOnAir = false;

            _repository.Update(plan);
            await _uow.SaveChangesAsync();

            return response;
        }

        public async Task<List<TravelPlan>> FilterPlans(TravelPlanVM model)
        {
            var plans = await _repository
                .Query(x => x.From == model.From && x.To == model.To && x.IsOnAir)
                .ToListAsync();

            return plans;
        }

        public async Task<string> RegisterThePlan(int id)
        {
            var response = "Successfully registered travel plan.";
            var plan = _repository.FindById(id);

            if (plan != null)
            {
                var state = true;

                if (!plan.IsOnAir)
                {
                    state = false;
                    response = "It must be published in order to participate in the travel plan";
                }
                else if (plan.NumberOfSeats <= 0)
                {
                    state = false;
                    response = "There are no vacancies for the relevant travel plan.";
                }

                if (state)
                {
                    plan.NumberOfSeats -= 1;

                    _repository.Update(plan);
                    await _uow.SaveChangesAsync();
                }
            }
            else
            {
                response = "Related travel plan not found";
            }

            return response;
        }
    }
}
