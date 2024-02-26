using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Core.Domain.Enums;
using Core.Domain.Entities;

using static Core.Domain.Common.Constants;

namespace Core.Domain.Infrastructure.EntityConfigurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards", nameof(Schemas.task));
        builder.HasKey(x => x.CardId);

        builder.Property(x => x.CardId).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(MaxCardName).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(MaxCardHexColor);
        builder.Property(x => x.Description).HasMaxLength(MaxDescription);
    }
}