using PlayTogether.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        public DbSet<ApplicationUserDetails> ApplicationUserDetails { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Gender> Genders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasKey(detail => detail.ApplicationUserId)
                .HasName("PrimaryKey_ApplicationUserId");

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasOne(detail => detail.ApplicationUser)
                .WithOne(user => user.ApplicationUserDetails)
                .IsRequired()
                .HasForeignKey<ApplicationUserDetails>(detail => detail.ApplicationUserId)
                .HasConstraintName("ForeignKey_User_UserDetails");

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasOne(detail => detail.CountryOfResidence)
                .WithOne()
                .IsRequired()
                .HasForeignKey<ApplicationUserDetails>(detail => detail.CountryOfResidenceId)
                .HasConstraintName("ForeignKey_User_Country")
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ApplicationUserDetails>()
                .HasOne(detail => detail.Gender)
                .WithOne()
                .IsRequired()
                .HasForeignKey<ApplicationUserDetails>(detail => detail.GenderId)
                .HasConstraintName("ForeignKey_User_Gender")
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
