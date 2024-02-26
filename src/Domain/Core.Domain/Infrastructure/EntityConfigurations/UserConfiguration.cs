using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Core.Domain.Enums;
using Core.Domain.Entities;

using static Core.Domain.Common.Constants;

namespace Core.Domain.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", nameof(Schemas.task));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("UserId").ValueGeneratedNever();
        builder.Property(x => x.UserName).HasColumnName("Username").HasMaxLength(MaxUserEmail).IsRequired();
        builder.Property(x => x.NormalizedUserName).HasMaxLength(MaxUserEmail);
        builder.Property(x => x.PhoneNumber).HasMaxLength(MaxMsisdn);
        builder.Property(x => x.FirstName).HasMaxLength(MaxUserName).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(MaxUserName).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(MaxUserEmail).IsRequired();
        builder.Property(x => x.NormalizedEmail).HasMaxLength(MaxUserEmail);

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        // Each User can have many UserClaims
        builder.HasMany(e => e.Claims).WithOne(e => e.User).HasForeignKey(uc => uc.UserId).IsRequired();

        // Each User can have many UserLogins
        builder.HasMany(e => e.Logins).WithOne(e => e.User).HasForeignKey(ul => ul.UserId).IsRequired();

        // Each User can have many UserTokens
        builder.HasMany(e => e.Tokens).WithOne(e => e.User).HasForeignKey(ut => ut.UserId).IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(ur => ur.UserId).IsRequired();

        // Each User can have many Cards
        builder.HasMany(e => e.Cards).WithOne(e => e.CreatedBy).HasForeignKey(ul => ul.CreatedById).IsRequired();
    }
}