using AdessoRideShare.infrastructure.Repository;
using AdessoRideShare.model.context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AdessoRideShare.infrastructure.UnitOfWork
{
    public class UnitOfWork : ControllerBase, IUnitOfWork
    {
        private bool disposed = false;
        protected readonly AdessoRideShareContext _context;
        public List<EntityEntry> newEntries = new List<EntityEntry>();

        public UnitOfWork(AdessoRideShareContext context,
            IConfiguration configuration)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        public void ChangeTracking(bool changeTracking = false)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = changeTracking;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                var result = -1;

                result = await _context.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _context.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}