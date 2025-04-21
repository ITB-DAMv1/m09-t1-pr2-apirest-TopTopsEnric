using ApiM09practica.models;
using Microsoft.AspNetCore.Identity;

namespace ApiM09practica.Contexts
{
    public class Initializer
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly dbContexts _context;

        public Initializer(UserManager<ApplicationUser> userManager, dbContexts context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            // Crear usuarios solo si no existen
            var adminEmail = "admin@gmail.com";
            var userEmail = "user@gmail.com";

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "adminuser",
                    Email = adminEmail,
                    Name = "Admin",
                    Surname = "User"
                };
                var result = await _userManager.CreateAsync(adminUser, "AdminPassword123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var normalUser = await _userManager.FindByEmailAsync(userEmail);
            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = "normaluser",
                    Email = userEmail,
                    Name = "Normal",
                    Surname = "User"
                };
                var result = await _userManager.CreateAsync(normalUser, "UserPassword123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(normalUser, "User");
                }
            }

            // Crear 5 juegos asociados al usuario normal (o admin si prefieres)
            if (!_context.games.Any())
            {
                var juegos = new List<Games>
            {
                new Games { Name = "Rematch", Description = "Juego de futbol en tercera persona", ProfileImage = "https://kagi.com/proxy/FPI2SGKSJNH77L4S2Q5XUUUD6Y.jpg?c=RY7gPs8FwtEst727kY8lasa0i9bcyS-ucVgbYjeBbx-NCMEal4YxRQPObvTp66-0I-mwfUFhcdNpycT1jE8PEHKMubJ2Quocwm4-25rxM1DER5rtgYve4ZBJIDUXp3_opAuwVQCXEhK824ws40VOPju6WwGROQO9SZYWJncS8SXMrScs02p0Z1-O4Enct9V3mOkLvENSGAs3CqjhsY_P8vEoJWPAR7wFhBQ0_HtWInxyDyETUzn3DdN2IaAIeBWy", votos = 0, UserId = normalUser.Id },
                new Games { Name = "Dying light 2", Description = "Juego de zombies en 1 persona", ProfileImage = "https://kagi.com/proxy/3c17bf3bb4c9b5b572f69217554f0afdebade3a37959bac3.png?c=NfTI84C87mMSVtVUs80WNnuG_QLaIpa0TFJ_6Q2Le_WdcvQKhMXMePxHOG-G7T5F5g9B0nsOimeEeWsw_iUy04kWNGUMZnMoeuvIOtqaVtzsDnvO4QD0bBf0QJBfbnuuat-0WsMHO40ypQrSq9-Jy60SKKNqgmUuPK07CQIGIQM%3D", votos = 0, UserId = normalUser.Id },
                new Games { Name = "Total war shogun 2", Description = "Juego de estrategia de la saga total war", ProfileImage = "https://kagi.com/proxy/shogun2-1650241291767.jpg?c=Q2dSd-mdzq8mKMDu62o73jqu7ahtNSXnb0MlCRZ1lVAKilrLmAMOd905_J1l8f-2Fcoetcco-_0hdnu7ukivRo_VyL0z2tu5zgfu2rVe_3Q%3D", votos = 0, UserId = normalUser.Id },
                new Games { Name = "Battelfield 4", Description = "6 extrega de la saga battelfield", ProfileImage = "https://kagi.com/proxy/Battlefield_4_cover_art.jpg?c=9cn5Kxse4yD05EJkf6QML9dK4clUbdQ9Oq4d5gDoyHCoMdew-dDQ41fZbPYmh1rxnC8W_brRfcl8J3obq_pGSc7O7m3x7oN8LMgMBcpmS9_OjZw9OGufpMJbsNmSOoSI", votos = 0, UserId = normalUser.Id },
                new Games { Name = "Fallout new Vegas", Description = "El mejor puto juego", ProfileImage = "https://kagi.com/proxy/250px-Fallout_New_Vegas.jpg?c=9cn5Kxse4yD05EJkf6QML9dK4clUbdQ9Oq4d5gDoyHD2lO5eDDvTY1Nok8x6MmFw6cP1GXg1cE0XK7aJW_1FTz5h9cnD1yfVIsGAzntqRB2twbHixO1aO0OnBywxHX4j7XqxyqzI5yNJd8My8hMmZjsYUsZdxDWcsZmNM8FmDvE%3D", votos = 0, UserId = normalUser.Id }
            };

                await _context.games.AddRangeAsync(juegos);
                await _context.SaveChangesAsync();
            }
        }
    }
}
