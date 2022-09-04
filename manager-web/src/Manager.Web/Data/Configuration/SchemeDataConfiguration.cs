using Manager.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Web.Data.Configuration;

public sealed class SchemeDataConfiguration : IEntityTypeConfiguration<SchemeData>
{
    public void Configure(EntityTypeBuilder<SchemeData> builder)
    {
        builder.HasOne(p => p.Scheme).WithMany(b => b.Data);

        builder.HasOne(s => s.CreatedBy).WithMany().HasForeignKey((SchemeData p) => p.CreatedById);

        builder.HasOne(s => s.ModifiedBy).WithMany().HasForeignKey((SchemeData p) => p.ModifiedById);
    }
}
