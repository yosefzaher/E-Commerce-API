using E_Commerce.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Configurations;
public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder
            .HasKey(w => w.WishlistId);

        builder
            .HasOne(w => w.User)
            .WithOne(u => u.Wishlist)
            .HasForeignKey<Wishlist>(w => w.UserId);

        builder
            .HasMany(w => w.WishlistItems)
            .WithOne(wi => wi.Wishlist)
            .HasForeignKey(wi => wi.WishlistId);

    }
}
