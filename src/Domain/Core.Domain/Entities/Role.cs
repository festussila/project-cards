using Microsoft.AspNetCore.Identity;

using Core.Domain.Common;

namespace Core.Domain.Entities;

public class Role : IdentityRole<long>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

}
