
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
using RecipeSharing.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace RecipeSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class adminDashboard : ControllerBase
    {


        private readonly ImangeprofileRepository _ProfileRepository;
        private readonly iRecipeRepository _RecipeRepository;
        public adminDashboard(ImangeprofileRepository ProfileRepository, iRecipeRepository RecipeRepository)
        {

            _ProfileRepository = ProfileRepository;
            _RecipeRepository = RecipeRepository;

        }


        private async Task<RecipeResponse> MapToDTO(Recipe recipe)
        {
            if (recipe == null)
            {
                return null; // or throw an exception if necessary
            }

            var totaldislike = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == recipe.RecipeId && e.typeEvent == "dislike");
            var totalcomment = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == recipe.RecipeId && e.typeEvent == "comment");
            var totalshare = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == recipe.RecipeId && e.typeEvent == "share");
            var totallike = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == recipe.RecipeId && e.typeEvent == "like");
            ApplicationUser user = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == recipe.UserId);

            if (recipe.Shared)
            {
                var OriginaRecipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == recipe.OriginaRecipeid);

                if (OriginaRecipe == null)
                {
                    return null; // or handle the null case appropriately
                }

                var OriginaRecipetotaldislike = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "dislike");
                var OriginaRecipetotalcomment = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "comment");
                var OriginaRecipetotalshare = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "share");
                var OriginaRecipetotallike = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "like");
                ApplicationUser ouser = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == recipe.UserId);


                var RecipeResponse = new RecipeResponse
                {
                    id = recipe.RecipeId,
                    Caption = recipe.Caption,
                    Totalcomment = totalcomment.Count,
                    Totaldislike = totaldislike.Count,
                    Totalshere = totalshare.Count,
                    Totallike = totallike.Count,
                    BIO = user.BIO,
                    UserName = user.UserName,
                    userImageUrl = user.ImageUrl,
                    oRecipeResponse = new RecipeResponse
                    {
                        id = OriginaRecipe.RecipeId,
                        Name = OriginaRecipe.Name,
                        categories = OriginaRecipe.categories,
                        Instructions = OriginaRecipe.Instructions,
                        CookingTime = OriginaRecipe.CookingTime ?? 0,
                        Calories = OriginaRecipe.Calories,
                        Fat = OriginaRecipe.Fat,
                        Carbohydrates = OriginaRecipe.Carbohydrates,
                        Protein = OriginaRecipe.Protein,
                        Ingredients = OriginaRecipe.Ingredients,
                        RecipeImageUrl = OriginaRecipe.ImageUrl,
                        Totalcomment = OriginaRecipetotalcomment.Count,
                        Totaldislike = OriginaRecipetotaldislike.Count,
                        Totalshere = OriginaRecipetotalshare.Count,
                        Totallike = OriginaRecipetotallike.Count,
                        BIO = ouser.BIO,
                        UserName = ouser.UserName,
                        userImageUrl = ouser.ImageUrl
                    }
                };

                return RecipeResponse;
            }
            else
            {
                var recipeResponse = new RecipeResponse
                {
                    id = recipe.RecipeId,
                    Name = recipe.Name,
                    categories = recipe.categories,
                    Instructions = recipe.Instructions,
                    CookingTime = recipe.CookingTime ?? 0,
                    Calories = recipe.Calories,
                    Fat = recipe.Fat,
                    Carbohydrates = recipe.Carbohydrates,
                    Protein = recipe.Protein,
                    Ingredients = recipe.Ingredients,
                    RecipeImageUrl = recipe.ImageUrl,
                    Totalcomment = totalcomment.Count,
                    Totaldislike = totaldislike.Count,
                    Totalshere = totalshare.Count,
                    Totallike = totallike.Count,
                    BIO = user.BIO,
                    UserName = user.UserName,
                    userImageUrl = user.ImageUrl
                };

                return recipeResponse;
            }
        }


        private ProfileDto MapApplicationUserToProfileDto(ApplicationUser user)
        {
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
                //following = user.Followings?.Count ?? 0,
                //followers = user.Followers?.Count ?? 0
            };
            return profile;
        }



        [HttpGet("getById / string id ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApplicationUser>> GetUsergetById(string userId)

        {

            if (userId == null)
                return BadRequest();
            var User = await _ProfileRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == userId);
            return Ok(MapApplicationUserToProfileDto(User));

        }


        [HttpGet("getalluser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<ApplicationUser>> getalluser()

        {
               
            var Users = await _ProfileRepository.GetAllTEntity<ApplicationUser>();


            var filtereRecipeResponse = new List<ProfileDto>();
            foreach (var User in Users)
            {
                var dto =  MapApplicationUserToProfileDto(User);
                filtereRecipeResponse.Add(dto);
            }
            return Ok(filtereRecipeResponse);
            

        }








        [HttpGet("getallRecipe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<ApplicationUser>> getallRecipe()

        {
           
            var Results = await _ProfileRepository.GetAllTEntity<Recipe>();

            var filtereRecipeResponse = new List<RecipeResponse>();
            foreach (var Result in Results)
            {
                var dto = await MapToDTO(Result);
                filtereRecipeResponse.Add(dto);
            }
            return Ok(filtereRecipeResponse);


        }

        [HttpGet("getRecipebyid/int: id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<ApplicationUser>> getRecipebyid(int id )

        {

            var Results = await _ProfileRepository.GetAllTEntity<Recipe>(e=>e.RecipeId==id);

            var filtereRecipeResponse = new List<RecipeResponse>();
            foreach (var Result in Results)
            {
                var dto = await MapToDTO(Result);
                filtereRecipeResponse.Add(dto);
            }
            return Ok(filtereRecipeResponse);


        }

        [HttpDelete("Deletecomment/id:int ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Recipe>> Deletecomment(int RecipeId, int eventid)
        {
           
            var existingcomment = await _RecipeRepository.GetSpecialEntity<evnet>(e => e.Id == eventid && e.typeEvent == "comment");
            var Recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == RecipeId);

            if (eventid == 0 || Recipe == null)
                return BadRequest();
            else
            {
                if (existingcomment == null) return NotFound();
                else
                    await _RecipeRepository.Remove<evnet>(existingcomment);
            }

            return Ok(Recipe);

        }


        [HttpDelete("Deleteuser/string:username ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Recipe>> Deleteuser(string username)
        {
           
            var user = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.UserName == username);

            if (user == null)
                return BadRequest();
           
          await _ProfileRepository.Remove<ApplicationUser>(user);
           

            return Ok();

        }
        [HttpDelete("Deleteuser/string:id ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Recipe>> Deleteuserbyid(string id)
        {

            var user = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == id);

            if (user == null)
                return BadRequest();

            await _ProfileRepository.Remove<ApplicationUser>(user);


            return Ok();

        }


        [HttpDelete("DeletRecipe/int:id ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Recipe>> DeleteRecipebyid(int id)
        {

            var Recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == id);

            if (Recipe == null)
                return BadRequest();

            await _ProfileRepository.Remove<Recipe>(Recipe);


            return Ok();

        }


        [HttpGet("gettotal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult> GetTotal()
        {
            var totalEventTask = await _RecipeRepository.GetAllTEntity<evnet>();
            var totalUserTask = await _RecipeRepository.GetAllTEntity<ApplicationUser>();
            var totalRecipeTask = await  _RecipeRepository.GetAllTEntity<Recipe>();

           

            var models = new
            {
                totalEvent =  totalEventTask != null ? totalEventTask.Count() : 0,
                totalUser = totalUserTask != null ? totalUserTask.Count() : 0,
                totalRecipe = totalRecipeTask != null ? totalRecipeTask.Count() : 0
            };

            return Ok(models);
        }


    }
}
