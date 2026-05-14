using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace webStudioBlazor.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [MaxLength(128)]
        public string FirstName { get; set; } = string.Empty;

        [PersonalData]
        [MaxLength(128)]
        public string LastName { get; set; } = string.Empty;
    }

}
