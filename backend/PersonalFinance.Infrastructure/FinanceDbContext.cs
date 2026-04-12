using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PersonalFinance.Application.Interfaces;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options), IFinanceDbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ExpenseArticle> ExpenseArticles => Set<ExpenseArticle>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

            builder.Property(c => c.MonthlyBudget).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ExpenseArticle>(builder =>
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired().HasMaxLength(150);
            builder
                .HasOne(e => e.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount).HasPrecision(18, 2);

            builder.Property(t => t.Date).IsRequired();

            builder.Property(t => t.Comment).HasMaxLength(250);

            builder
                .HasOne(t => t.Article)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }
}
