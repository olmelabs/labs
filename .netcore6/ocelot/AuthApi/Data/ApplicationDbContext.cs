using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/*

    +------------------+-------------------+
    |      Table       |   Description     |
    +------------------+-------------------+
    | AspNetUsers      | The users.        |
    | AspNetRoles      | The roles.        |
    | AspNetUserRoles  | Roles of users.   |
    | AspNetUserClaims | Claims by users.  |
    | AspNetRoleClaims | Claims by roles.  |
    | AspNetUserLogins | Federation logins |
    | AspNetUserTokens | Token storage     |
    +------------------+-------------------+

    AspNetUserLogins table to hold information about 3rd party/external logins, 
    for example users who login into your site via Google, Facebook, Twitter etc. 

    The table AspNetUserTokens is for external authentication token storage and is filled 
    by SignInManager.UpdateExternalAuthenticationTokensAsync method.

 */

namespace AuthApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}