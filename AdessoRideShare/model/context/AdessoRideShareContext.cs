using AdessoRideShare.model.entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AdessoRideShare.model.context
{
	public partial class AdessoRideShareContext : DbContext
	{
		public AdessoRideShareContext()
		{
		}

		public AdessoRideShareContext(DbContextOptions<AdessoRideShareContext> options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys());

			foreach (var relationShip in foreignKeys)
			{
				relationShip.DeleteBehavior = DeleteBehavior.Restrict;
			}

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

		#region List of Entities

		public virtual DbSet<TravelPlan> TravelPlan { get; set; }

		#endregion List of Entities
	}
}