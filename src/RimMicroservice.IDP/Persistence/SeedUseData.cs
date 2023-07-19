using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RimMicroservices.IDP.Common;
using RimMicroservices.IDP.Entities;
using System.Security.Claims;

namespace RimMicroservices.IDP.Persistence
{
    public class SeedUseData
    {
        public static void EnsureData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<RIMIdentityContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            services.AddIdentity<User, IdentityRole>(
                opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;    
                }).AddEntityFrameworkStores<RIMIdentityContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    CreateUser(scope, "Alice", "Smith", "Viet Nam",
                        Guid.NewGuid().ToString(), "rim123456",
                        "Administrator", "alicesmith@example.com");
                }
            };
        }
        
        private static void CreateUser(IServiceScope scope, string firstName, string lastName,
            string address, string id, string password, string role,string email)
        {
            var userManagement = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = userManagement.FindByNameAsync(email).Result;

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    Id = id
                };

                var result = userManagement.CreateAsync(user, password).Result;
                var addToRoleResult = userManagement.AddToRoleAsync(user, role).Result;
                CheckResult(addToRoleResult);

                result = userManagement.AddClaimsAsync(user, new Claim[]
                {
                    new(SystemConstants.Claims.Username, user.UserName),
                    new(SystemConstants.Claims.FirstName, user.FirstName),
                    new(SystemConstants.Claims.LastName, user.LastName),
                    new(SystemConstants.Claims.Roles, role),
                    new(JwtClaimTypes.Address, user.Address),
                    new(JwtClaimTypes.Email, user.Email),
                    new(ClaimTypes.NameIdentifier, user.Id)
                }).Result;
                CheckResult(result);    
            }
        }

        private static void CheckResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}
