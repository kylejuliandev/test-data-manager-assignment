using Manager.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Web.Data.Configuration;

public sealed class SchemeConfiguration : IEntityTypeConfiguration<Scheme>
{
    public void Configure(EntityTypeBuilder<Scheme> builder)
    {
        builder.HasMany(s => s.Data).WithOne().HasForeignKey(s => s.SchemeId);

        builder.HasOne(s => s.CreatedBy).WithMany().HasForeignKey((Scheme p) => p.CreatedById);

        builder.HasOne(s => s.ModifiedBy).WithMany().HasForeignKey((Scheme p) => p.ModifiedById);
    }
}
