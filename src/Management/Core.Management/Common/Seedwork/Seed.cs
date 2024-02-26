using Core.Management.Interfaces;

namespace Core.Management.Common.Seedwork;

public class Seed(IUserRepository userRepository) : ISeed
{
    public async Task SeedDefaults()
    {
        //Seed default roles - member and admin roles
        await userRepository.SeedDefaultRoles().ConfigureAwait(false);

        //Seed sample users - member and admin user
        await userRepository.SeedDefaultUsers().ConfigureAwait(false);

    }
}
