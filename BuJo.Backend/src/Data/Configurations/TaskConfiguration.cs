using BuJo.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = BuJo.Domain.Tasks.Task;

namespace BuJo.Data.Configurations;

internal sealed class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId);
    }
}