﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PromoterBot.Data;

#nullable disable

namespace PromoterBot.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220920180407_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PromoterBot.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("RegionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RegionId");

                    b.ToTable("Cities");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Ташкент",
                            RegionId = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "Янгиюль",
                            RegionId = 1
                        },
                        new
                        {
                            Id = 3,
                            Name = "Назарбек",
                            RegionId = 1
                        },
                        new
                        {
                            Id = 4,
                            Name = "Келес",
                            RegionId = 1
                        },
                        new
                        {
                            Id = 5,
                            Name = "Чирчик",
                            RegionId = 1
                        },
                        new
                        {
                            Id = 6,
                            Name = "Наманган",
                            RegionId = 2
                        },
                        new
                        {
                            Id = 7,
                            Name = "Фергана",
                            RegionId = 3
                        },
                        new
                        {
                            Id = 8,
                            Name = "Самарканд",
                            RegionId = 4
                        },
                        new
                        {
                            Id = 9,
                            Name = "Бухара",
                            RegionId = 5
                        },
                        new
                        {
                            Id = 10,
                            Name = "Навои",
                            RegionId = 6
                        },
                        new
                        {
                            Id = 11,
                            Name = "Андижан",
                            RegionId = 7
                        });
                });

            modelBuilder.Entity("PromoterBot.Models.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("FavouriteBrands")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("PromoterId")
                        .HasColumnType("integer");

                    b.Property<string>("SocialNetwork")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("PromoterId");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("PromoterBot.Models.Promoter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ChatId")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Promoters");
                });

            modelBuilder.Entity("PromoterBot.Models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Regions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Ташкентская область"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Наманганская область"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Ферганская область"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Самаркандская область"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Бухарская область"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Навоийская область"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Андижанская область"
                        });
                });

            modelBuilder.Entity("PromoterBot.Models.City", b =>
                {
                    b.HasOne("PromoterBot.Models.Region", "Region")
                        .WithMany("Cities")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Region");
                });

            modelBuilder.Entity("PromoterBot.Models.Participant", b =>
                {
                    b.HasOne("PromoterBot.Models.Promoter", "Promoter")
                        .WithMany()
                        .HasForeignKey("PromoterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Promoter");
                });

            modelBuilder.Entity("PromoterBot.Models.Region", b =>
                {
                    b.Navigation("Cities");
                });
#pragma warning restore 612, 618
        }
    }
}
