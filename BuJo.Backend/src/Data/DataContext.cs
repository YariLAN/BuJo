using System.Reflection;
using BuJo.Domain.Accounting;
using BuJo.Domain.Habits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using TaskItem = BuJo.Domain.Tasks.Task;

namespace BuJo.Data;

public sealed class DataContext : DbContext
{
    private readonly IReadOnlyList<IInterceptor> _interceptors;

    public DataContext(
        DbContextOptions<DataContext> options,
        IEnumerable<IInterceptor> interceptors)
        : base(options)
    {
        _interceptors = interceptors.ToList();
        ChangeTracker.DeleteOrphansTiming = CascadeTiming.OnSaveChanges;
        ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserBotState> UserBotStates => Set<UserBotState>();

    public DbSet<Habit> Habits => Set<Habit>();

    public DbSet<HabitLog> HabitLogs => Set<HabitLog>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(_interceptors);

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
