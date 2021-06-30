using FinalProject.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinalProject.Data
{
	public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
	{
		public DbSet<Story> Stories { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Fragment> Fragments { get; set; }

		public DbSet<Tag> Tags { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Story>().Property(m => m.Title).IsRequired();
			modelBuilder.Entity<Story>().Property(m => m.Description).IsRequired();
			modelBuilder.Entity<Story>().Property(m => m.Genre).IsRequired();

			modelBuilder.Entity<Comment>().Property(c => c.Text).IsRequired();
			modelBuilder.Entity<Comment>().Property(c => c.StoryId).IsRequired();

			modelBuilder.Entity<Fragment>().Property(f => f.Text).IsRequired();
			modelBuilder.Entity<Fragment>().Property(f => f.StoryId).IsRequired();

			modelBuilder.Entity<Tag>().Property(t => t.Name).IsRequired();
			modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();

			modelBuilder
				.Entity<Tag>()
				.HasMany(t => t.Stories)
				.WithMany(t => t.Tags)
				.UsingEntity(j => j.ToTable("StoryTag"));
		}
	}
}
