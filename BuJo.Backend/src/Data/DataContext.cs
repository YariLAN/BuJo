using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace BuJo.Data;

public sealed class DataContext : DbContext
{


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        
        modelBuilder.UseEnumConvention();
    }
    
    /// <summary>
    /// Применить миграции
    /// </summary>
    public Task MigrateAsync()
        => Database.MigrateAsync();
}

internal static class EfCoreExtensions
{
    public static void UseEnumConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                if (!property.PropertyType.IsEnum)
                {
                    continue;
                }

                var propertyBuilder = modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasMaxLength(255);

                var nullability = new NullabilityInfoContext().Create(property);
                switch (nullability.ReadState)
                {
                    case NullabilityState.NotNull:
                        propertyBuilder
                            .HasConversion<string>()
                            .IsRequired();
                        break;

                    case NullabilityState.Nullable:
                        propertyBuilder.HasConversion<string?>();
                        break;

                    case NullabilityState.Unknown:
                        throw new NotSupportedException("Используйте NRT");

                    default:
                        throw new ArgumentOutOfRangeException(nameof(property), nullability.ReadState, $"{nameof(NullabilityInfo.ReadState)}");
                }
            }
        }
    }
}
