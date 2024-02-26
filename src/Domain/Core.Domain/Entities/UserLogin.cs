using Microsoft.AspNetCore.Identity;

using Core.Domain.Common;

namespace Core.Domain.Entities;

public class UserLogin : IdentityUserLogin<long>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public User User { get; set; } = null!;
}
