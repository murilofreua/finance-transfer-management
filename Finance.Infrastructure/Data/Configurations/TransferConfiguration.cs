using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Configurations
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            builder.ToTable("Transfers");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.ScheduledAt);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.SourceAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.DestinationAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.SourceAccountId);
            builder.HasIndex(t => t.DestinationAccountId);
            builder.HasIndex(t => t.ScheduledAt);
        }
    }
}
