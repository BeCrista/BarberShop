using Microsoft.AspNetCore.Identity;

public class IdentitySeeder
{
    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        Console.WriteLine("⏳ Executando seeder de roles...");

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Admin", "Cliente" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                Console.WriteLine($"🔧 Criando role: {role}");
                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (result.Succeeded)
                    Console.WriteLine($"✅ Role '{role}' criada com sucesso.");
                else
                    Console.WriteLine($"❌ Erro ao criar role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else
            {
                Console.WriteLine($"✔️ Role '{role}' já existe.");
            }
        }

        var adminEmail = "admin@barbearia.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                Console.WriteLine("✅ Usuário admin criado.");
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("✅ Role 'Admin' atribuída ao usuário.");
            }
            else
            {
                Console.WriteLine($"❌ Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine("ℹ️ Usuário admin já existe.");
        }
    }
}
