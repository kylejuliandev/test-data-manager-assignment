using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manager.Web.Data.Configuration;

public sealed class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole
        {
            Id = "9ffe1c8e-6dd6-4d1f-8e5a-93911e41cc90",
            Name = "user",
            NormalizedName = "USER",
            ConcurrencyStamp = "a123097a-63c2-4c22-be57-d08a7793c0b8"
        });

        builder.HasData(new IdentityRole
        {
            Id = "bd2b7bc6-fa36-4988-9563-7ff609cf794c",
            Name = "admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = "9e3856b0-725b-4dd5-85bf-78f5fbf9278c"
        });

        builder.HasData(new IdentityRole
        {
            Id = "02171634-f2c9-4054-966f-675702641552",
            Name = "superuser",
            NormalizedName = "SUPERUSER",
            ConcurrencyStamp = "d4f5cdf6-9477-4b30-b011-1a2ef02c29f5"
        });
    }
}
