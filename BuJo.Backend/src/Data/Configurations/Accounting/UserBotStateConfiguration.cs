using BuJo.Domain.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuJo.Data.Configurations.Accounting;

internal sealed class UserBotStateConfiguration : IEntityTypeConfiguration<UserBotState>
{
    public void Configure(EntityTypeBuilder<UserBotState> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<UserBotState>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
