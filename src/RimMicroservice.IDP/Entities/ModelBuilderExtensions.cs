using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RimMicroservices.IDP.Common;

namespace RimMicroservices.IDP.Entities
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyIdentityConfigurations(this ModelBuilder builder)
        {
            ConfigureRole(builder.Entity<IdentityRole>());
            ConfigureUser(builder.Entity<User>());
            ConfigureRoleClaim(builder.Entity<IdentityRoleClaim<string>>());
            ConfigureUseClaim(builder.Entity<IdentityUserClaim<string>>());
            ConfigureUseLogin(builder.Entity<IdentityUserLogin<string>>());
            ConfigureUseToken(builder.Entity<IdentityUserToken<string>>());
        }

        private static void ConfigureRole(EntityTypeBuilder<IdentityRole> entity)
        {
            entity.ToTable("Roles", SystemConstants.IdentitySchema)
                .HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");

            entity.Property(x => x.Name)
                .IsRequired()
                .IsUnicode()
                .HasColumnType("nvarchar(150)")
                .HasMaxLength(150);
        }

        private static void ConfigureUser(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users", SystemConstants.IdentitySchema)
                .HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");

            entity.HasIndex(x => x.Email);
            entity.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .ValueGeneratedNever();

            entity.Property(x => x.NormalizedEmail)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);

            entity.Property(x => x.UserName)
                .IsRequired()
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);

            entity.Property(x => x.NormalizedUserName)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);

            entity.Property(x => x.PhoneNumber)
                .IsUnicode(false)
                .HasColumnType("varchar(20)")
                .HasMaxLength(20);


            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            entity.Property(x => x.LastName)
                .HasColumnType("varchar(150)")
                .HasMaxLength(150);
        }

        private static void ConfigureRoleClaim(EntityTypeBuilder<IdentityRoleClaim<string>> entity)
        {
            entity.ToTable("RoleClaims", SystemConstants.IdentitySchema)
                .HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }

        private static void ConfigureUseRole(EntityTypeBuilder<IdentityUserRole<string>> entity)
        {
            entity.ToTable("UseRoles", SystemConstants.IdentitySchema)
                .HasKey(x => x.UserId);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(50)");


            entity.Property(x => x.RoleId)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }

        private static void ConfigureUseClaim(EntityTypeBuilder<IdentityUserClaim<string>> entity)
        {
            entity.ToTable("UserClaims", SystemConstants.IdentitySchema)
                .HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }

        private static void ConfigureUseLogin(EntityTypeBuilder<IdentityUserLogin<string>> entity)
        {
            entity.ToTable("UserLogins", SystemConstants.IdentitySchema)
                .HasKey(x => x.UserId);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }

        private static void ConfigureUseToken(EntityTypeBuilder<IdentityUserToken<string>> entity)
        {
            entity.ToTable("UserTokens", SystemConstants.IdentitySchema)
                .HasKey(x => x.UserId);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(50)");
        }
    }
}
