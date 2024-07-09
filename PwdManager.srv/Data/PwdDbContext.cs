using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PwdManager.srv.Models;
using System.Collections.Generic;
using PwdManager.Shared;
using PwdManager.Shared.Data;
namespace PwdManager.srv.Data
{
    public class PwdDbContext : DbContext
    {

        public virtual DbSet<Coffre> Coffres { get; set; }
        public virtual DbSet<ApiUser> Apiusers { get; set; }
        public virtual DbSet<ApiUserCoffre> ApiUserCoffres { get; set; }

        public virtual DbSet<Entree> Entrees { get; set; }
        public virtual DbSet<CoffreLog> CoffreLogs { get; set; }
        public virtual DbSet<EntreeHistory> EntreeLogs { get; set; }

        public PwdDbContext()
        {
        }
        public PwdDbContext(DbContextOptions<PwdDbContext> options)
        : base(options)
        {
        }

        //// The following configures EF to create a Sqlite database file in the
        // //// special "local" folder for your platform.
        // protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite($"Data Source={DbPath}");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Coffre>()
                .HasMany(e => e.Entrees)
                .WithOne(e => e.Coffre)
                .HasForeignKey(e => e.CoffreId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Coffre>()
                .HasMany(e => e.CoffreLogs)
                .WithOne(e => e.Coffre)
                .HasForeignKey(e => e.CoffreId)              
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Coffre>()
            .HasMany(e => e.EntreeHistories)
            .WithOne(e => e.Coffre)
            .HasForeignKey(e => e.CoffreId)
            .OnDelete(DeleteBehavior.SetNull);



            modelBuilder.Entity<Entree>()
            .HasMany(e => e.EntreeHistories)
            .WithOne(e => e.Entree)
            .HasForeignKey(e => e.EntreeId)
             .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<ApiUser>()
            //    .HasMany(e => e.EntreeHistories)
            //    .WithOne(e => e.ApiUser)
            //    .HasForeignKey(e => e.EntreeHistoryId);


            //modelBuilder.Entity<ApiUser>()
            //    .HasMany(e => e.CoffreLogs)
            //    .WithOne(e => e.ApiUser)
            //    .HasForeignKey(e => e.CoffreLogId);


            modelBuilder
                .Entity<CoffreLog>()
                .Property(e => e.Operation)
                .HasConversion(
                    v => v.ToString(),
                    v => (Operation)Enum.Parse(typeof(Operation), v));
            modelBuilder
                .Entity<EntreeHistory>()
                .Property(e => e.Operation)
                .HasConversion(
                    v => v.ToString(),
                    v => (Operation)Enum.Parse(typeof(Operation), v));
            modelBuilder
                .Entity<ApiUserCoffre>()
                .Property(e => e.Access)
                .HasConversion(
                    v => v.ToString(),
                    v => (Access)Enum.Parse(typeof(Access), v));
            #region Relation N-N

            modelBuilder.Entity<ApiUserCoffre>()
                .HasKey(t => new { t.CoffreId, t.UserId });
            modelBuilder.Entity<ApiUserCoffre>()
                .HasOne(pt => pt.Coffre)
                .WithMany(p => p.ApiUserCoffres)
                .HasForeignKey(pt => pt.CoffreId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ApiUserCoffre>()
                .HasOne(pt => pt.ApiUser)
                .WithMany(t => t.ApiUserCoffres)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion Relation N-N

            #region Init User

            var hasher = new PasswordHasher<ApiUser>();
            modelBuilder.Entity<ApiUser>().HasData(
            new ApiUser
            {
                AzureId="y",
                UserId = "43c38655-9aa0-48b4-aab1-7cd175500f09"
            },
            new ApiUser
            {
                AzureId = "r",
                UserId = "5bda2409-9516-4983-90a3-08363427e744"
            });
            #endregion Init User

            #region exemplaire coffre
            modelBuilder.Entity<Coffre>().HasData(
                new Coffre
                {
                    Id = 1,
                    PasswordHash = hasher.HashPassword(new ApiUser { AzureId= "", UserId=""}, "P@ssword1"),
                    Title = "sample",
                    Description="pwd sample",
                    Salt="ets",
            

                },
                new Coffre
                {
                    Id = 2,
                    PasswordHash = hasher.HashPassword(new ApiUser { AzureId = "", UserId = "" }, "P@ssword2"),
                    Title = "sample 2",
                    Description = "pwd sample",
                    Salt = "ets",

                }
            );
            #endregion exemplaire coffre

        }
    }
}
