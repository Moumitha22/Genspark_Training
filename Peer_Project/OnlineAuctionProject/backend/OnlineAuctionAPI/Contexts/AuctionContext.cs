using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Contexts;

public class AuctionContext : DbContext
{
    public AuctionContext(DbContextOptions options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<AuctionItem> AuctionItems { get; set; }
    public DbSet<BidItem> BidItems { get; set; }
    public DbSet<FileData> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<VirtualWallet> VirtualWallets { get; set; }
    public DbSet<VirtualWalletHistory> VirtualWalletHistories { get; set; }
    public DbSet<EAgreement> EAgreements { get; set; }
    public DbSet<DeletedUsers> DeletedUsers { get; set; }
    public DbSet<AuctionDeleteRequest> AuctionDeleteRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Status>()
            .HasData(
                new Status { Id = 1, Name = "Active" },
                new Status { Id = 2, Name = "Deleted" }
            );

        modelBuilder.Entity<User>().HasQueryFilter(u => u.StatusId != 2);

        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        modelBuilder.Entity<User>().Property(u => u.Username).HasMaxLength(20);

        modelBuilder
            .Entity<User>()
            .HasOne(u => u.Status)
            .WithMany()
            .HasForeignKey(u => u.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AuctionItem>()
            .Property(ai => ai.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder
            .Entity<AuctionItem>()
            .HasOne(at => at.Seller)
            .WithMany(u => u.Auctions)
            .HasForeignKey(a => a.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<BidItem>()
            .HasOne(bi => bi.Bidder)
            .WithMany(u => u.Bids)
            .HasForeignKey(b => b.BidderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<BidItem>()
            .HasOne(bi => bi.AuctionItem)
            .WithMany(at => at.Bids)
            .HasForeignKey(b => b.AuctionItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<BidItem>()
            .Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder
            .Entity<AuctionItem>()
            .HasOne(at => at.WinningBid)
            .WithOne()
            .HasForeignKey<AuctionItem>(at => at.WinnerId)
            .OnDelete(DeleteBehavior.SetNull);

        // modelBuilder.Entity<FileData>()
        // .HasOne(a => a.AuctionItem)
        // .WithMany()
        // .HasForeignKey(a => a.AuctionItemId)
        // .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileData>().HasIndex(f => f.AuctionItemId).IsUnique(false);

        // modelBuilder.Entity<AuctionItem>()
        //     .Property(ai => ai.Category)
        //     .IsRequired()
        //     .HasMaxLength(100);

        modelBuilder
            .Entity<AuctionItem>()
            .HasMany(a => a.FileAttachments)
            .WithOne(f => f.AuctionItem)
            .HasForeignKey(f => f.AuctionItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<FileData>()
            .HasOne(f => f.AuctionItem)
            .WithMany(a => a.FileAttachments)
            .HasForeignKey(f => f.AuctionItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<EAgreement>()
            .HasOne(e => e.Bidding)
            .WithMany()
            .HasForeignKey(e => e.BiddingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeletedUsers>().HasIndex(d => d.UserId).IsUnique();

        modelBuilder.Entity<DeletedUsers>().HasIndex(d => d.Email).IsUnique();
    }
}
