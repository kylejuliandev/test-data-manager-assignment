using Manager.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Web.Data.Configuration;

public sealed class SchemeDataConfiguration : IEntityTypeConfiguration<SchemeData>
{
    public void Configure(EntityTypeBuilder<SchemeData> builder)
    {
        // Set One to Many relationship for Scheme -> SchemeData
        builder.HasOne(p => p.Scheme).WithMany(b => b.Data);

        // Ensure the Key is indexed and is unique
        builder.HasIndex(s => s.Key).IsUnique();

        // Set One to Many relationship for Scheme -> User
        builder.HasOne(s => s.CreatedBy).WithMany().HasForeignKey((SchemeData p) => p.CreatedById);

        // Set One to Many relationship for Scheme -> User
        builder.HasOne(s => s.ModifiedBy).WithMany().HasForeignKey((SchemeData p) => p.ModifiedById);
    }
}
