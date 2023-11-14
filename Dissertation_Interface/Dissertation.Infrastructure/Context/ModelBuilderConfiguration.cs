using System.Reflection;
using Dissertation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Infrastructure.Context;

public class ModelBuilderConfiguration
{
    public static void Configure(ModelBuilder modelBuilder) => ApplyConfigurationsFromAssembly(modelBuilder);

    private static void ApplyConfigurationsFromAssembly(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

        modelBuilder.Entity<AcademicYear>()
            .Property(e => e.EndDate)
            .HasColumnType("date");

        modelBuilder.Entity<AcademicYear>()
            .Property(e => e.StartDate)
            .HasColumnType("date");

        modelBuilder.Entity<DissertationCohort>()
            .Property(e => e.StartDate)
            .HasColumnType("date");

        modelBuilder.Entity<DissertationCohort>()
            .Property(e => e.EndDate)
            .HasColumnType("date");

    }

}