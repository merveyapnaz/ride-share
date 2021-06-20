using AdessoRideShare.infrastructure.Repository;
using System;
using System.Threading.Tasks;

namespace AdessoRideShare.infrastructure.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IRepository<T> Repository<T>() where T : class;
		void ChangeTracking(bool changeTracking = false);
		int SaveChanges();
		Task<int> SaveChangesAsync();
		public void DetachAllEntities();
	}
}
