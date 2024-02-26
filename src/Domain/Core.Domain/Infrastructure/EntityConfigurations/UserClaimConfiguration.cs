using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Core.Domain.Enums;
using Core.Domain.Entities;

namespace Core.Domain.Infrastructure.EntityConfigurations;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims", nameof(Schemas.task));

        builder.Property(x => x.Id).ValueGeneratedNever();
    }
}
