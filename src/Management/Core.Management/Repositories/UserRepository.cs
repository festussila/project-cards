using System.Net;
using System.Text;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

using Core.Domain.Enums;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Management.Interfaces;

using static Core.Domain.Common.Constants;
using static Core.Management.Repositories.HelperRepository;

namespace Core.Management.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration configuration;
    private readonly UserManager<User> userManager;
    private readonly RoleManager<Role> roleManager;
    private readonly SignInManager<User> signInManager;

    public UserRepository(
        IConfiguration configuration,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        SignInManager<User> signInManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
    }

    #region User

    public async Task<(string token, long expires, User user)> SignIn(string email, string password)
    {
        ValidatedParameter("Password", password, out password, throwException: true);
        ValidatedParameter(nameof(User.Email), email, out email, throwException: true);
        email = email.ToUpper();

        User user = await userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.NormalizedEmail == email).ConfigureAwait(false) ?? throw new GenericException("Invalid username or password", "CA001", HttpStatusCode.Unauthorized);

        SignInResult signInResult = await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

        if (!signInResult.Succeeded) throw new GenericException("Invalid username or password", "CA001", HttpStatusCode.Unauthorized);

        (string token, long expires) = GenerateUserJwt(user);

        return (token, expires, user);
    }

    #endregion

    #region Seeding Defaults - Role, Users and UserRoles

    public async Task SeedDefaultRoles()
    {
        if (roleManager.Roles.Any()) return;

        Role[] defaultRoles = [new Role { Id = (long)Roles.Member, Name = nameof(Roles.Member) }, new Role { Id = (long)Roles.Admin, Name = nameof(Roles.Admin) }];

        foreach (Role role in defaultRoles)
        {
            await roleManager.CreateAsync(role).ConfigureAwait(false);
        }
    }

    public async Task SeedDefaultUsers()
    {
        if (userManager.Users.Any()) return;

        DateTime seedDate = DateTime.UtcNow;

        User memberUser = new()
        {
            Id = seedDate.Ticks,
            FirstName = "Ava",
            LastName = "Maya",
            UserName = "member@cards.com",
            Email = "member@cards.com"
        };

        User adminUser = new()
        {
            Id = seedDate.AddMinutes(5).Ticks,
            FirstName = "Amani",
            LastName = "Jones",
            UserName = "admin@cards.com",
            Email = "admin@cards.com"
        };

        IdentityResult memberResult = await userManager.CreateAsync(memberUser).ConfigureAwait(false);

        if (memberResult.Succeeded)
        {
            await userManager.AddPasswordAsync(memberUser, "letmein@45").ConfigureAwait(false);
            await userManager.AddToRoleAsync(memberUser, nameof(Roles.Member));
        }

        IdentityResult adminResult = await userManager.CreateAsync(adminUser).ConfigureAwait(false);

        if (adminResult.Succeeded)
        {
            await userManager.AddPasswordAsync(adminUser, "letmein@45").ConfigureAwait(false);
            await userManager.AddToRoleAsync(adminUser, nameof(Roles.Admin));
        }
    }


    #endregion

    #region Helpers

    private (string token, long expires) GenerateUserJwt(User user)
    {
        List<Claim> claims = [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
        
        IEnumerable<Claim> roleClaims = user.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.Role.Name!));
        claims.AddRange(roleClaims);

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration[JwtKey]!));
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256Signature);

        DateTime baseDate = DateTime.UtcNow;
        DateTime expiryDate = baseDate.AddMinutes(Convert.ToDouble(configuration[JwtLifetime]!));

        JwtSecurityToken token = new(
            issuer: configuration[JwtIssuer],
            audience: configuration[JwtAudience],
            claims: claims,
            notBefore: baseDate,
            expires: expiryDate,
            signingCredentials: signingCredentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiryDate.ToEpoch());
    }

    #endregion

}