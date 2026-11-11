using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Configurations
{
    public class TransferLimitConfiguration : IEntityTypeConfiguration<TransferLimit>
    {
        public void Configure(EntityTypeBuilder<TransferLimit> builder)
        {
            builder.ToTable("TransferLimits");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Period)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.MaxAmountPerHour)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.MaxAttemptsPerHour)
                .IsRequired();

            builder.HasIndex(t => t.Period)
                .IsUnique();
        }
    }
}
