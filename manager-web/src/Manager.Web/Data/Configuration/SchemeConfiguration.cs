using Manager.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Web.Data.Configuration;

public sealed class SchemeConfiguration : IEntityTypeConfiguration<Scheme>
{
    public void Configure(EntityTypeBuilder<Scheme> builder)
    {
        // Set One to Many relationship for Scheme -> SchemeData
        builder.HasMany(s => s.Data).WithOne().HasForeignKey(s => s.SchemeId);

        // Set One to Many relationship for Scheme -> User
        builder.HasOne(s => s.CreatedBy).WithMany().HasForeignKey((Scheme p) => p.CreatedById);

        // Set One to Many relationship for Scheme -> User
        builder.HasOne(s => s.ModifiedBy).WithMany().HasForeignKey((Scheme p) => p.ModifiedById);
    }
}
