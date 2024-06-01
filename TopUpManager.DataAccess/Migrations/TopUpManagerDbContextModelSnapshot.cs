﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TopUpManager.DataAccess.DBContext;

#nullable disable

namespace TopUpManager.DataAccess.Migrations
{
    [DbContext(typeof(TopUpManagerDbContext))]
    partial class TopUpManagerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TopUpManager.Common.Entity.Beneficiary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Beneficiaries");
                });

            modelBuilder.Entity("TopUpManager.Common.Entity.TopUpTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("BeneficiaryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BeneficiaryId");

                    b.ToTable("TopUpTransactions");
                });

            modelBuilder.Entity("TopUpManager.Common.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TopUpManager.Common.Entity.Beneficiary", b =>
                {
                    b.HasOne("TopUpManager.Common.Entity.User", null)
                        .WithMany("Beneficiaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TopUpManager.Common.Entity.TopUpTransaction", b =>
                {
                    b.HasOne("TopUpManager.Common.Entity.Beneficiary", null)
                        .WithMany("TopUpTransactions")
                        .HasForeignKey("BeneficiaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TopUpManager.Common.Entity.Beneficiary", b =>
                {
                    b.Navigation("TopUpTransactions");
                });

            modelBuilder.Entity("TopUpManager.Common.Entity.User", b =>
                {
                    b.Navigation("Beneficiaries");
                });
#pragma warning restore 612, 618
        }
    }
}
