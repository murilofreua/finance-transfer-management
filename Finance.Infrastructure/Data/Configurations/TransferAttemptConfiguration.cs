using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Configurations
{
    public class TransferAttemptConfiguration : IEntityTypeConfiguration<TransferAttempt>
    {
        public void Configure(EntityTypeBuilder<TransferAttempt> builder)
        {
            builder.ToTable("TransferAttempts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.WasSuccessful)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.AccountId, x.CreatedAt });
        }
    }
}
