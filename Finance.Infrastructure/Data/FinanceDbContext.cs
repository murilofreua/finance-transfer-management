using Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Finance.Application.Interfaces.UnitOfWork;

namespace Finance.Infrastructure.Data
{
    public class FinanceDbContext : DbContext, IUnitOfWork
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<Transfer> Transfers => Set<Transfer>();

        public DbSet<TransferLimit> TransferLimits => Set<TransferLimit>();

        public DbSet<TransferAttempt> TransferAttempts => Set<TransferAttempt>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
        }
    }
}
