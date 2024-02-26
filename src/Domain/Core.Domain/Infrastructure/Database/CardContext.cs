using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;

using static Core.Domain.Common.Constants;

namespace Core.Domain.Infrastructure.Database;

public class CardContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    private readonly IDateTimeService dateTimeService;
    private readonly IHttpContextAccessor httpContextAccessor;
    public CardContext(DbContextOptions<CardContext> options, IDateTimeService dateTimeService, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        this.dateTimeService = dateTimeService;
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (IMutableForeignKey relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        //scan given assembly for all types implementing IEntityTypeConfiguration and register
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //seed data
        modelBuilder.Seed();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges()
    {   
        ChangeTrackEntries();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {   
        ChangeTrackEntries();

        return base.SaveChangesAsync(cancellationToken);
    }

    #region Helpers
    private void ChangeTrackEntries()
    {
        DateTime now = dateTimeService.Now;

        if (base.ChangeTracker.Entries<IAuditableEntity>().Any())
        {
            foreach (EntityEntry<IAuditableEntity> entry in base.ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.ModifiedAt = now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedAt = now;
                        break;
                }
            }
        }

        if (base.ChangeTracker.Entries<IAuditableEntityWithActor>().Any())
        {
            long actorId = GetActorId();

            foreach (EntityEntry<IAuditableEntityWithActor> entry in base.ChangeTracker.Entries<IAuditableEntityWithActor>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:

                        entry.Entity.CreatedAt = now;
                        entry.Entity.ModifiedAt = now;
                        entry.Entity.CreatedById = actorId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedAt = now;
                        entry.Entity.ModifiedById = actorId;
                        break;
                }
            }
        }
    }

    private long GetActorId()
    {
        string? token = httpContextAccessor.HttpContext?.GetTokenAsync(AccessToken).Result;

        if (string.IsNullOrEmpty(token)) throw new GenericException("User must be logged in to complete request", "CA002", HttpStatusCode.Unauthorized);

        JwtSecurityToken jwtToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);

        return long.Parse(jwtToken.Claims.First(claim => claim.Type == ClaimsSub).Value);
    } 
    #endregion

    // Arranged alphabetically for ease of reference
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<CardStatus> CardStatuses => Set<CardStatus>();
}