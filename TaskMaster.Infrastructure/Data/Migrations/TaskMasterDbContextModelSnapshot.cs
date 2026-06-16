using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskMaster.Infrastructure.Data;

#nullable disable

namespace TaskMaster.Infrastructure.Data.Migrations;

[DbContext(typeof(TaskMasterDbContext))]
partial class TaskMasterDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.0");

        modelBuilder.Entity("TaskMaster.Infrastructure.Entities.TaskEntity", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnType("TEXT");

            b.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("TEXT");

            b.Property<string>("Description")
                .HasMaxLength(1000)
                .HasColumnType("TEXT");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            b.Property<DateTime?>("DueDate")
                .HasColumnType("TEXT");

            b.Property<int>("Status")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.ToTable("Tasks");
        });
    }
}
