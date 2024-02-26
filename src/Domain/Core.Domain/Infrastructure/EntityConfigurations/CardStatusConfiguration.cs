using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Core.Domain.Enums;
using Core.Domain.Entities;

using static Core.Domain.Common.Constants;

namespace Core.Domain.Infrastructure.EntityConfigurations;

public class CardStatusConfiguration : IEntityTypeConfiguration<CardStatus>
{
    public void Configure(EntityTypeBuilder<CardStatus> builder)
    {
        builder.ToTable("CardStatuses", nameof(Schemas.task));
        builder.HasKey(x => x.CardStatusId);

        builder.Property(x => x.CardStatusId).ValueGeneratedNever();
        builder.Property(x => x.Status).HasMaxLength(MaxCardStatus).IsRequired();

        builder.HasIndex(x => x.Status).IsUnique();
    }
}
