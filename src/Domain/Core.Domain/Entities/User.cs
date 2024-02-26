using Core.Domain.Common;

using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public class User : IdentityUser<long>, IAuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public ICollection<Card> Cards { get; set; } = new List<Card>();
    public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    public ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}