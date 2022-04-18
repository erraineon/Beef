﻿// <auto-generated />
using System;
using Beef.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Beef.Core.Migrations
{
    [DbContext(typeof(BeefDbContext))]
    [Migration("20220418003858_Initial")]
    partial class _20220418003858_Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("Beef.Core.Data.Trigger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChatType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandToRun")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildPermissionsRawValue")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Triggers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Trigger");
                });

            modelBuilder.Entity("Beef.Core.Data.CronTrigger", b =>
                {
                    b.HasBaseType("Beef.Core.Data.Trigger");

                    b.Property<string>("CronSchedule")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("TriggerAtUtc")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("CronTrigger");
                });

            modelBuilder.Entity("Beef.Core.Data.OneTimeTrigger", b =>
                {
                    b.HasBaseType("Beef.Core.Data.Trigger");

                    b.Property<DateTime?>("TriggerAtUtc")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("OneTimeTrigger");
                });

            modelBuilder.Entity("Beef.Core.Data.RecurringTrigger", b =>
                {
                    b.HasBaseType("Beef.Core.Data.Trigger");

                    b.Property<TimeSpan>("Interval")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("TriggerAtUtc")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("RecurringTrigger");
                });
#pragma warning restore 612, 618
        }
    }
}
