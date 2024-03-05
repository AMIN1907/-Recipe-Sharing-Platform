
using RecipeSharing.models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RecipeSharing.models;
using RecipeSharing.Repository.Repository;
using RecipeSharing.models;
using Microsoft.Extensions.Hosting;
using WebApplication1.models.dto;
using RecipeSharing.models.Dto;
using Microsoft.EntityFrameworkCore;
using RecipeSharing.Models;



namespace RecipeSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class profileController : ControllerBase
    {


        private readonly ImangeprofileRepository _ProfileRepository;

        public profileController(ImangeprofileRepository ProfileRepository)
        {

            _ProfileRepository = ProfileRepository;


        }
        

        private async Task<ProfileDto> MapApplicationUserToProfileDto(ApplicationUser user)
        {
            var following = await _ProfileRepository.GetUserFolloweing(user);
            
            var followers = await _ProfileRepository.GetUserFollower(user);
            var profile = new ProfileDto
            {   

                 fname = user.FirstName,
                lname = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                barthday = user.Birthdate,
                PhoneNumber = user.PhoneNumber,
                ginder = user.Gender,
                BIO = user.BIO,
                ImageUrl = user.ImageUrl,
                following = following != null ? following.Count() : 0,
                followers = followers != null ? followers.Count() : 0
            };
            return profile;
        }

        private async Task<string> WriteFile(IFormFile file)
        {
            string filename = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = DateTime.Now.Ticks.ToString() + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
            }
            return filename;
        }



        [HttpPost("follow/string:id ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Follow(string uid2)
        {
            // Retrieve the current user's ID from the claims
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var uid1 = userIdClaim?.Value;

            // Retrieve user1 and user2 from the repository
            ApplicationUser user1 = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == uid1);
            ApplicationUser user2 = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == uid2);

            // Check if either user1 or user2 is null
            if (user1 == null || user2 == null)
                return BadRequest("One or both users not found.");

            // Retrieve the list of users that user1 is following and users following user2
            var userFollowing = await _ProfileRepository.GetUserFolloweing(user1);
            var userFollower = await _ProfileRepository.GetUserFollower(user2);

            // Check if user2 is already being followed by user1 or user1 is already following user2
            if (userFollowing.Contains(user2.Id) || userFollower.Contains(user1.Id))
                return BadRequest("User already followed.");

            // Check if user1 is trying to follow themselves
            if (user1.Id == user2.Id)
                return BadRequest("Cannot follow yourself.");

            // Add the follow relationship between user1 and user2
            await _ProfileRepository.AddFollow(user1, user2);

            return Ok("User followed successfully.");
        }




        [HttpDelete("unfollow/string:id ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> unfollow(string uid2)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var uid1 = userIdClaim?.Value;
            ApplicationUser user1 = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == uid1);
            ApplicationUser user2 = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == uid2);

            if (user1 == null || user2 == null) return BadRequest();
            if(await _ProfileRepository.GetUserFollower(user2) ==null || await _ProfileRepository.GetUserFolloweing(user1) == null) return BadRequest();
            var r = await _ProfileRepository.UnFollow(user1, user2);
            if (r == null) return BadRequest();
            return Ok();
        }

        [HttpGet("GetUserFollower")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserFollower()
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim?.Value;
            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);

            var filteredProfileDtoResponse = new List<ProfileDto>();
            foreach (var UserFolloweings in await _ProfileRepository.GetUserFollower(User))
            {
                var UserFolloweing = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == UserFolloweings);
                var dto = MapApplicationUserToProfileDto(UserFolloweing);
                filteredProfileDtoResponse.Add(await dto);
            }
            return Ok(filteredProfileDtoResponse);

           
        }
        [HttpGet("GetUserFolloweing")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserFolloweing()
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim?.Value;
            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);

            var filteredProfileDtoResponse = new List<ProfileDto>();
            foreach (var UserFolloweings in await  _ProfileRepository.GetUserFolloweing(User))
            {
                var UserFolloweing = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == UserFolloweings);
                var dto = MapApplicationUserToProfileDto(UserFolloweing);
                filteredProfileDtoResponse.Add(await dto);
            }
            return Ok(filteredProfileDtoResponse);
            
        }


        [HttpGet("GetUserFollowerbyid/string:id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserFollowerbyid(string userId)
        {
            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);

            var filteredProfileDtoResponse = new List<ProfileDto>();
            foreach (var UserFolloweings in await _ProfileRepository.GetUserFollower(User))
            {
                var UserFolloweing = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == UserFolloweings);
                var dto = MapApplicationUserToProfileDto(UserFolloweing);
                filteredProfileDtoResponse.Add(await dto);
            }
            return Ok(filteredProfileDtoResponse);


        }
        [HttpGet("GetUserFolloweingbyid/string:id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserFolloweingbyid(string userId)
        {
           
            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);

            var filteredProfileDtoResponse = new List<ProfileDto>();
            foreach (var UserFolloweings in await _ProfileRepository.GetUserFolloweing(User))
            {
                var UserFolloweing = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == UserFolloweings);
                var dto = MapApplicationUserToProfileDto(UserFolloweing);
                filteredProfileDtoResponse.Add(await dto);
            }
            return Ok(filteredProfileDtoResponse);

        }




        [HttpGet("getById / string id ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfileDto>> Get(string userId)

        {

            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);
            return Ok(MapApplicationUserToProfileDto(User));

        }
        
        
        
        [HttpGet("getUserLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<ProfileDto>> Getuserlogin()

        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;

            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);



                    return Ok(MapApplicationUserToProfileDto(User));
            
            

        }





        [HttpDelete("Deleteuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Recipe>> Deleteuserbyid()
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var id = userIdClaim.Value;

            var user = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == id);

            if (user == null)
                return BadRequest();

            await _ProfileRepository.Remove<ApplicationUser>(user);


            return Ok();

        }

        [HttpPut("updatedProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfileDto>> UpdateUserProfile(UpdateProfileDto updatedProfile)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var id = userIdClaim.Value;
            var existingUser = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == id);

            if (existingUser == null)
            {
                return NotFound();
            }

            var existingUsername = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.UserName == updatedProfile.UserName);
            if (existingUsername != null && existingUsername.Id != existingUser.Id)
            {

                return BadRequest("Username is already taken");
            }

            var existingEmail = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Email == updatedProfile.Email);
            if (existingEmail != null && existingEmail.Id != existingUser.Id)
            {

                return BadRequest("Email is already taken");
            }
            // Check if any field is updated
            if (existingUser.FirstName == updatedProfile.fname &&
                 existingUser.LastName == updatedProfile.lname &&
                 existingUser.UserName == updatedProfile.UserName &&
                 existingUser.Email == updatedProfile.Email &&
                 existingUser.BIO == updatedProfile.BIO &&
                 existingUser.PhoneNumber == updatedProfile.phone &&
                 existingUser.Birthdate == updatedProfile.Birthdate &&
                 existingUser.Birthdate == updatedProfile.Birthdate &&
                 existingUser.Gender == updatedProfile.ginder)
            {
                return BadRequest("No changes detected. Please provide updated information.");
            }

            // Update the user profile
            existingUser.FirstName = updatedProfile.fname ?? existingUser.FirstName;
            existingUser.LastName = updatedProfile.lname ?? existingUser.LastName;
            existingUser.UserName = updatedProfile.UserName ?? existingUser.UserName;
            existingUser.Email = updatedProfile.Email ?? existingUser.Email;
            existingUser.NormalizedUserName = (updatedProfile.UserName ?? existingUser.UserName).ToUpper();
            existingUser.NormalizedEmail = (updatedProfile.Email ?? existingUser.Email).ToUpper();

            existingUser.BIO = updatedProfile.BIO;
            existingUser.PhoneNumber = updatedProfile.phone;
            existingUser.Birthdate = updatedProfile.Birthdate != DateTime.MinValue ? updatedProfile.Birthdate : existingUser.Birthdate;
            existingUser.Gender = updatedProfile.ginder;
            await _ProfileRepository.updatat(existingUser);

            return Ok("The data has been updated successfully");
        }



       



        [HttpPut("profile/changepassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;
            var user = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);

            if (user == null)
            { return NotFound("User not found"); }

            var errors = new List<string>();
            // Check if any required fields are null
            if (changePassword.NewPassword == null || changePassword.ConfirmPassword == null || changePassword.CurrentPassword == null)
            {
                errors.Add("New Password, Confirm Password, and Current Password are required");
            }

            // Check if New Password matches Confirm Password
            if (changePassword.NewPassword != changePassword.ConfirmPassword)
            {
                errors.Add("New Password and Confirm Password do not match");
            }

            // Check if New Password is the same as Current Password
            if (changePassword.NewPassword == changePassword.CurrentPassword)
            {
                errors.Add("New Password and Current Password are the same");
            }
            // If there are any errors, return them
            if (errors.Any())
            {
                return BadRequest(errors);
            }
            var result = await _ProfileRepository.ChangePassword(user, changePassword);


            if (result is not ApplicationUser)
                return BadRequest(result);
            return Ok(result);
        }
        
        
        [HttpPost("uploadProfilePicture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfileDto>> uploadProfilePicture(IFormFile file)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var id = userIdClaim.Value;
            var existingUser = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == id);

            if (existingUser == null)
            {
                return NotFound();
            }
            var result = await WriteFile(file);
            existingUser.ImageUrl = result;


            await _ProfileRepository.updatat(existingUser);

            return Ok(existingUser);
        }
    }
}
//[HttpPut("AcceptRequest/id:string ")]
//[ProducesResponseType(StatusCodes.Status200OK)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//[ProducesResponseType(StatusCodes.Status404NotFound)]
//public async Task<ActionResult> AcceptRequest(string SenderID)
//{
//    var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
//    var receiverID = userIdClaim.Value;



//    var Request = await _friendRepository.GetSpecialEntity<RequestsSent>((r => (r.SenderId == SenderID && r.ReceiverId == receiverID) || (r.SenderId == receiverID && r.ReceiverId == SenderID)));
//    if (Request == null) return NotFound("Request id NotFound");
//    await _friendRepository.Remove<RequestsSent>(Request);

//    var Sender = await _friendRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == SenderID);
//    var receiver = await _friendRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == receiverID);
//    if (Sender == null || receiver == null) return NotFound("Sender id NotFound");

//    await _friendRepository.Addfriend(Sender, receiver);

//    return Ok();

//}
//[HttpDelete("unfriend/id:string ")]
//[ProducesResponseType(StatusCodes.Status200OK)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//[ProducesResponseType(StatusCodes.Status404NotFound)]
//public async Task<ActionResult> unfriend(string SenderID)
//{
//    var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
//    var receiverID = userIdClaim.Value;

//    var friendshipToDelete1 = await _friendRepository.GetSpecialEntity<ApplicationUser>(e => (e.Id == SenderID));
//    var friendshipToDelete2 = await _friendRepository.GetSpecialEntity<ApplicationUser>(e => (e.Id == receiverID));


//    await _friendRepository.DeleteFriend(friendshipToDelete1, friendshipToDelete2);
//    return Ok();

//}
//[HttpGet("GetAllFriends")]
//[ProducesResponseType(StatusCodes.Status200OK)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//[ProducesResponseType(StatusCodes.Status404NotFound)]
//public async Task<ActionResult<info>> GetAllFriends()
//{
//    var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
//    var userid = userIdClaim?.Value;

//    var user = await _friendRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userid);

//    var friends = await _friendRepository.GetUserFriends(user);



//    return Ok(await GetFriendsResponse(friends, userid));


//}
//private async Task<List<info>> GetFriendsResponse(HashSet<string> friendsID, string userid)
//{
//    List<info> friendsinfo = new List<info>();
//    foreach (var uid in friendsID)
//    {
//        var friendinfo = await _friendRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == uid);

//        var mutualFriendsCount = await MutualFriends(uid, userid); // Call method properly with parameters

//        var response = new info
//        {
//            udi = friendinfo.Id, // No need to access Result as we await the task
//            fname = friendinfo.fname,
//            imgeurl = friendinfo.imgeurl,
//            lname = friendinfo.lname,
//            username = friendinfo.UserName,
//            MutualFriends = mutualFriendsCount.Count
//        };

//        friendsinfo.Add(response);
//    }
//    return friendsinfo;
//}
