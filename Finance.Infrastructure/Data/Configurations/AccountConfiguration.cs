using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.PersonName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2);

            builder.Property(a => a.OverdraftLimit)
                .HasPrecision(18, 2);

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
