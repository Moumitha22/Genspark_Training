using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Models;

namespace BankManagementSystem.Contexts
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .HasConstraintName("FK_Account_User")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .HasConstraintName("FK_Transaction_Account")
                .OnDelete(DeleteBehavior.Cascade);
     
        }
    }
}