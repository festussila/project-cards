using Core.Domain.Entities;

namespace Core.Management.Interfaces;

public interface IUserRepository
{

    #region User

    Task<(string token, long expires, User user)> SignIn(string email, string password);

    #endregion

    #region Seed - Roles, User and User Roles
    Task SeedDefaultRoles();
    Task SeedDefaultUsers();
    #endregion

}