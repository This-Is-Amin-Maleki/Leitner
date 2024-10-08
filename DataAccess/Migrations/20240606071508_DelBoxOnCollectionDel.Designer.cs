﻿// <auto-generated />
using System;
using DataAccessLeit.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLeit.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240606071508_DelBoxOnCollectionDel")]
    partial class DelBoxOnCollectionDel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ModelsLeit.Entities.Box", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("CardPerDay")
                        .HasColumnType("int");

                    b.Property<long>("CollectionId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStudied")
                        .HasColumnType("datetime2");

                    b.Property<long>("LastCardId")
                        .HasColumnType("bigint");

                    b.Property<int>("LastSlot")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.ToTable("Boxes", "dbo");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Card", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ask")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CollectionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HasMp3")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.ToTable("Cards", "dbo");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Collection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Collections", "dbo");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Container", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<long>("SlotId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SlotId");

                    b.ToTable("Containers", "dbo");
                });

            modelBuilder.Entity("ModelsLeit.Entities.ContainerCard", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("CardId")
                        .HasColumnType("bigint");

                    b.Property<long>("ContainerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("ContainerId");

                    b.ToTable("ContainerCards", "dbo");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Slot", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("BoxId")
                        .HasColumnType("bigint");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BoxId");

                    b.ToTable("Slots", "dbo");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Box", b =>
                {
                    b.HasOne("ModelsLeit.Entities.Collection", "Collection")
                        .WithMany("Boxes")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Card", b =>
                {
                    b.HasOne("ModelsLeit.Entities.Collection", "Collection")
                        .WithMany("Cards")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Container", b =>
                {
                    b.HasOne("ModelsLeit.Entities.Slot", "Slot")
                        .WithMany("Containers")
                        .HasForeignKey("SlotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("ModelsLeit.Entities.ContainerCard", b =>
                {
                    b.HasOne("ModelsLeit.Entities.Card", "Card")
                        .WithMany("ContainerCards")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ModelsLeit.Entities.Container", "Container")
                        .WithMany("ContainerCards")
                        .HasForeignKey("ContainerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Container");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Slot", b =>
                {
                    b.HasOne("ModelsLeit.Entities.Box", "Box")
                        .WithMany("Slots")
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Box");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Box", b =>
                {
                    b.Navigation("Slots");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Card", b =>
                {
                    b.Navigation("ContainerCards");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Collection", b =>
                {
                    b.Navigation("Boxes");

                    b.Navigation("Cards");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Container", b =>
                {
                    b.Navigation("ContainerCards");
                });

            modelBuilder.Entity("ModelsLeit.Entities.Slot", b =>
                {
                    b.Navigation("Containers");
                });
#pragma warning restore 612, 618
        }
    }
}
