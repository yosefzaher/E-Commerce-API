using E_Commerce.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Configurations;
public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder
            .HasKey(wi => wi.WishlistItemId);

        builder
            .HasOne(wi => wi.Wishlist)
            .WithMany(w => w.WishlistItems)
            .HasForeignKey(wi => wi.WishlistId);

        builder
            .HasOne(wi => wi.Product)
            .WithMany(p => p.WishlistItems)
            .HasForeignKey(wi => wi.ProductId);
    }
}
