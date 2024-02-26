using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Core.Domain.Enums;
using Core.Domain.Entities;

namespace Core.Domain.Infrastructure.EntityConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims", nameof(Schemas.task));

        builder.Property(x => x.Id).ValueGeneratedNever();
    }
}
