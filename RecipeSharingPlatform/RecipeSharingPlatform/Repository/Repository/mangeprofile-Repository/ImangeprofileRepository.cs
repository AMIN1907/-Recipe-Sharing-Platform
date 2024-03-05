

using RecipeSharing.models;
using RecipeSharing.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeSharing.Repository.Repository
{
    public interface ImangeprofileRepository : IRepository<ApplicationUser>
    {
        public Task<ApplicationUser> updatat(ApplicationUser entity);
        public  Task<object> ChangePassword (ApplicationUser entity , ChangePassword  changePassword);
        public  Task<HashSet<string>> GetUserFollower(ApplicationUser user);
        public  Task<HashSet<string>> GetUserFolloweing(ApplicationUser user);
        public  Task<IActionResult> UnFollow(ApplicationUser user1, ApplicationUser user2);
        public  Task AddFollow(ApplicationUser userId1, ApplicationUser userId2);
    }
}
