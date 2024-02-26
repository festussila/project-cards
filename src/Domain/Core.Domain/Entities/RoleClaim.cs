using Microsoft.AspNetCore.Identity;

using Core.Domain.Common;

namespace Core.Domain.Entities;

public class RoleClaim : IdentityRoleClaim<long>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public Role Role { get; set; } = null!;
}
