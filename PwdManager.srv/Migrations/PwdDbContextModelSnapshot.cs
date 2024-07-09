﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PwdManager.srv.Data;

#nullable disable

namespace PwdManager.srv.Migrations
{
    [DbContext(typeof(PwdDbContext))]
    partial class PwdDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PwdManager.Shared.Data.ApiUser", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("AzureId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Apiusers");

                    b.HasData(
                        new
                        {
                            UserId = "43c38655-9aa0-48b4-aab1-7cd175500f09",
                            AzureId = "y"
                        },
                        new
                        {
                            UserId = "5bda2409-9516-4983-90a3-08363427e744",
                            AzureId = "r"
                        });
                });

            modelBuilder.Entity("PwdManager.Shared.Data.ApiUserCoffre", b =>
                {
                    b.Property<int>("CoffreId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("Access")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CoffreId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ApiUserCoffres");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.Coffre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Coffres");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "pwd sample",
                            PasswordHash = "AQAAAAIAAYagAAAAEBJN85RhB6dNhzLF8TIsaQZOAUmG/30o9xcquDlyTuHCehNk1PHbr4O5Qe8r2aDNTQ==",
                            Salt = "ets",
                            Title = "sample"
                        },
                        new
                        {
                            Id = 2,
                            Description = "pwd sample",
                            PasswordHash = "AQAAAAIAAYagAAAAEPUq1HHrxM3kcxmCLDBW+hTk7L+kkqCJ/kaIGO1rKo/k29p/r1I8nXFNwcQzQ5mimw==",
                            Salt = "ets",
                            Title = "sample 2"
                        });
                });

            modelBuilder.Entity("PwdManager.Shared.Data.CoffreLog", b =>
                {
                    b.Property<int>("CoffreLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CoffreLogId"));

                    b.Property<int?>("CoffreId")
                        .HasColumnType("integer");

                    b.Property<string>("CoffreName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateOperation")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CoffreLogId");

                    b.HasIndex("CoffreId");

                    b.HasIndex("UserId");

                    b.ToTable("CoffreLogs");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.Entree", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CoffreId")
                        .HasColumnType("integer");

                    b.Property<string>("EncryptedLogin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EncryptedPwd")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EncryptedURL")
                        .HasColumnType("text");

                    b.Property<string>("IVLogin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IVPwd")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IVUrl")
                        .HasColumnType("text");

                    b.Property<string>("Icon")
                        .HasColumnType("text");

                    b.Property<string>("TagLogin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TagPwd")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TagUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CoffreId");

                    b.ToTable("Entrees");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.EntreeHistory", b =>
                {
                    b.Property<int>("EntreeHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EntreeHistoryId"));

                    b.Property<int?>("CoffreId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateOperation")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EncryptedLogin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EncryptedPwd")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EncryptedURL")
                        .HasColumnType("text");

                    b.Property<int?>("EntreeId")
                        .HasColumnType("integer");

                    b.Property<string>("EntreeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IVLogin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IVPwd")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IVUrl")
                        .HasColumnType("text");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TagLogin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TagPwd")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TagUrl")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EntreeHistoryId");

                    b.HasIndex("CoffreId");

                    b.HasIndex("EntreeId");

                    b.HasIndex("UserId");

                    b.ToTable("EntreeLogs");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.ApiUserCoffre", b =>
                {
                    b.HasOne("PwdManager.Shared.Data.Coffre", "Coffre")
                        .WithMany("ApiUserCoffres")
                        .HasForeignKey("CoffreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PwdManager.Shared.Data.ApiUser", "ApiUser")
                        .WithMany("ApiUserCoffres")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApiUser");

                    b.Navigation("Coffre");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.CoffreLog", b =>
                {
                    b.HasOne("PwdManager.Shared.Data.Coffre", "Coffre")
                        .WithMany("CoffreLogs")
                        .HasForeignKey("CoffreId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PwdManager.Shared.Data.ApiUser", "ApiUser")
                        .WithMany("CoffreLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApiUser");

                    b.Navigation("Coffre");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.Entree", b =>
                {
                    b.HasOne("PwdManager.Shared.Data.Coffre", "Coffre")
                        .WithMany("Entrees")
                        .HasForeignKey("CoffreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Coffre");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.EntreeHistory", b =>
                {
                    b.HasOne("PwdManager.Shared.Data.Coffre", "Coffre")
                        .WithMany("EntreeHistories")
                        .HasForeignKey("CoffreId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PwdManager.Shared.Data.Entree", "Entree")
                        .WithMany("EntreeHistories")
                        .HasForeignKey("EntreeId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("PwdManager.Shared.Data.ApiUser", "ApiUser")
                        .WithMany("EntreeHistories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApiUser");

                    b.Navigation("Coffre");

                    b.Navigation("Entree");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.ApiUser", b =>
                {
                    b.Navigation("ApiUserCoffres");

                    b.Navigation("CoffreLogs");

                    b.Navigation("EntreeHistories");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.Coffre", b =>
                {
                    b.Navigation("ApiUserCoffres");

                    b.Navigation("CoffreLogs");

                    b.Navigation("EntreeHistories");

                    b.Navigation("Entrees");
                });

            modelBuilder.Entity("PwdManager.Shared.Data.Entree", b =>
                {
                    b.Navigation("EntreeHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
