
using RecipeSharing.Controllers;

using RecipeSharing.models;
using RecipeSharing.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static RecipeSharing.Controllers.profileController;
using NuGet.Protocol.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RecipeSharing.Repository.Repository
{
    public class mangeprofilerepository : RepositoryGeneric<ApplicationUser>, ImangeprofileRepository
    {
        private readonly appDbcontext1 _db;
        private readonly UserManager<RecipeSharing.models.ApplicationUser> _userManager;
        public mangeprofilerepository(appDbcontext1 db, UserManager<ApplicationUser> userManager) : base(db)
        {
            _db = db;
            _userManager = userManager;
        }



        public async Task<HashSet<string>> GetUserFollower(ApplicationUser user)
        {
            
            var friendships = await _db.Followers
                                             .Where(f => f.ApplicationUser1Id == user.Id)
                                             .ToListAsync();

            var friendIds = friendships.Select(f =>
                                    f.ApplicationUser1Id == user.Id ? f.ApplicationUser2Id : f.ApplicationUser1Id)
                                    .ToHashSet();

            return friendIds;
        }

        public async Task<HashSet<string>> GetUserFolloweing(ApplicationUser user)
        {

            var friendships = await _db.Followings
                                             .Where(f => f.ApplicationUser1Id == user.Id )
                                             .ToListAsync();

            var friendIds = friendships.Select(f =>
                                    f.ApplicationUser1Id == user.Id ? f.ApplicationUser2Id : f.ApplicationUser1Id)
                                    .ToHashSet();

            return friendIds;
        }
        public async Task<IActionResult> UnFollow(ApplicationUser user1, ApplicationUser user2)
        {

            // Find the friendship to delete
            var followingToDelete = await _db.Followings
        .Where(f => f.ApplicationUser1Id == user1.Id && f.ApplicationUser2Id == user2.Id)
        .FirstOrDefaultAsync();

            var followerToDelete = await _db.Followers
       .Where(f => f.ApplicationUser1Id == user2.Id && f.ApplicationUser2Id == user1.Id)
       .FirstOrDefaultAsync();

            if (followerToDelete == null || followingToDelete == null)
            {
                return  null ;
            }


              _db.Followings.Remove(followingToDelete);
              _db.Followers.Remove(followerToDelete);

            await save();

            return new OkResult(); ;
        }
      

        public async Task AddFollow(ApplicationUser userId1, ApplicationUser userId2)
        {
            userId1.Following ??= new List<Following>();
            userId2.Followers ??= new List<Followers>();

            userId1.Following.Add(new Following { ApplicationUser1Id = userId1.Id, ApplicationUser2Id = userId2.Id });
            userId2.Followers.Add(new Followers { ApplicationUser1Id = userId2.Id, ApplicationUser2Id = userId1.Id });

            await save();

        }


        public async Task<object> ChangePassword(ApplicationUser entity, ChangePassword changePassword)
        {
            var errors = new List<string>();
            var result = await _userManager.ChangePasswordAsync(entity, changePassword.CurrentPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                errors.AddRange(result.Errors.Select(e => e.Description));
                return errors;
            }
            await save();
            return entity;
        }

        


        public async Task<ApplicationUser> updatat(ApplicationUser entity)
        {   
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

    }
}


