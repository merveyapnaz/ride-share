
using AdessoRideShare.infrastructure.Repository;
using AdessoRideShare.model.entities;
using AdessoRideShare.model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdessoRideShare.infrastructure.Services.Interfaces
{
	public interface ITravelPlanService : IRepository<TravelPlan>
	{
		Task<string> Add(TravelPlan entity);
		Task<string> Publish(int id);
		Task<string> UnPublish(int id);
		Task<List<TravelPlan>> FilterPlans(TravelPlanVM model);
		Task<string> RegisterThePlan(int id);
	}
}