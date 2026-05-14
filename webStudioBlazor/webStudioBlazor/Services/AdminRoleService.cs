using Microsoft.AspNetCore.Identity;

using webStudioBlazor.Data;
using webStudioBlazor.ModelDTOs;

namespace webStudioBlazor.Services
{
    public class AdminRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRoleService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Отримати всіх користувачів з ролями
        public async Task<List<UserWithRolesDto>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserWithRolesDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    Roles = roles.ToList()
                });
            }

            return result;
        }

        // Створити роль, якщо її ще немає
        public async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Додати роль юзеру
        public async Task AddRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return;

            await EnsureRoleExistsAsync(roleName);

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        // Забрати роль в юзера
        public async Task RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return;

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
        }
    }
}
