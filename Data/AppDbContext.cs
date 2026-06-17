using FridgeApp.Entities;
using FridgeApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace FridgeApp.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Item> Items { get; set; }
		public DbSet<ProductDefinition> ProductDefinitions { get; set; }
		public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
		public DbSet<ItemActivityLog> ItemActivityLogs { get; set; }
		public DbSet<Fridge> Fridges { get; set; }
		public DbSet<FridgeQuickAddItem> FridgeQuickAddItems { get; set; }
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<FridgeQuickAddItem>()
				.HasOne<Fridge>()
				.WithMany()
				.HasForeignKey(item => item.FridgeId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<FridgeQuickAddItem>()
				.HasOne<ProductDefinition>()
				.WithMany()
				.HasForeignKey(item => item.ProductDefinitionId)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<ProductDefinition>().HasData(
				new ProductDefinition { Id = 1, Name = "Yumurta", Category = "Kahvaltilik", DefaultUnit = "adet", QuickAmounts = "[6,10,12,15,20,30]", TrackingType = TrackingType.Countable, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 2, Name = "Sut", Category = "Sut Urunleri", DefaultUnit = "litre", QuickAmounts = "[0.5,1,2]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 3, Name = "Yogurt", Category = "Sut Urunleri", DefaultUnit = "gram", QuickAmounts = "[250,500,1000]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 4, Name = "Peynir", Category = "Kahvaltilik", DefaultUnit = "gram", QuickAmounts = "[100,250,500]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 5, Name = "Tereyagi", Category = "Kahvaltilik", DefaultUnit = "gram", QuickAmounts = "[100,250,500]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 6, Name = "Tavuk", Category = "Et ve Tavuk", DefaultUnit = "gram", QuickAmounts = "[250,500,1000]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 7, Name = "Kiyma", Category = "Et ve Tavuk", DefaultUnit = "gram", QuickAmounts = "[250,500,1000]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true },
				new ProductDefinition { Id = 8, Name = "Domates", Category = "Sebze", DefaultUnit = "adet", QuickAmounts = "[2,4,6,8]", TrackingType = TrackingType.Approximate, IsQuickAdd = true, IsActive = true });
		}
	}
}
