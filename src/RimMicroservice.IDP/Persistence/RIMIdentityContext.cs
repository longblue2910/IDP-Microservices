using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RimMicroservices.IDP.Entities;
using RimMicroservices.IDP.Entities.Configuration;

namespace RimMicroservices.IDP.Persistence
{
    public class RIMIdentityContext : IdentityDbContext<User>
    {
        public RIMIdentityContext(DbContextOptions<RIMIdentityContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfigurationsFromAssembly(typeof(RIMIdentityContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}
