using BuJo.Domain.Habits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuJo.Data.Configurations;

internal sealed class HabitLogConfiguration : IEntityTypeConfiguration<HabitLog>
{
    public void Configure(EntityTypeBuilder<HabitLog> builder)
    {
        builder.HasKey(hl => hl.Id);

        builder.HasIndex(hl => new { hl.HabitId, hl.Date });

        builder.HasIndex(hl => hl.Date);

        builder.Property(hl => hl.IsCompleted)
            .HasDefaultValue(false);

        builder.HasOne(hl => hl.Habit)
            .WithMany(h => h.Logs)
            .HasForeignKey(hl => hl.HabitId);
    }
}