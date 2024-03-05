
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RecipeSharing.models;
using RecipeSharing.Repository.Repository;
using RecipeSharing.models.dto;
using Microsoft.Extensions.Hosting;

using RecipeSharing.models.Dto;
using RecipeSharing.Models;
using RecipeSharing.models;

namespace RecipeSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {


        private readonly iRecipeRepository _RecipeRepository;

        public RecipeController(iRecipeRepository RecipeRepository)
        {

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
            var totaltasty = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == recipe.RecipeId && e.typeEvent == "tasty");
            ApplicationUser user = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == recipe.UserId );

            if (recipe.Shared)
            {
                var OriginaRecipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == recipe.OriginaRecipeid);

                if (OriginaRecipe == null)
                {
                    return null; // or handle the null case appropriately
                }

                var OriginaRecipetotaldislike = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "dislike");
                var OriginaRecipetotalcomment = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "comment");
                var OriginaRecipetotalshare   = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "share");
                var OriginaRecipetotallike    = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "like");
                var OriginaRecipetotaltasty   = await _RecipeRepository.GetAllTEntity<evnet>(e => e.Rid == OriginaRecipe.RecipeId && e.typeEvent == "tasty"); 
                ApplicationUser ouser = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == recipe.UserId);


                var RecipeResponse = new RecipeResponse
                {
                    id = recipe.RecipeId,
                    Caption = recipe.Caption,
                    Totalcomment = totalcomment.Count,
                    Totaldislike = totaldislike.Count,
                    Totalshere = totalshare.Count,
                    Totallike = totallike.Count,
                    totaltasty = totaltasty.Count,
                    BIO = user.BIO,
                    UserName = user.UserName,
                    userImageUrl = user.ImageUrl,
                    oRecipeResponse = new RecipeResponse
                    {
                        id = OriginaRecipe.RecipeId,
                        Name = OriginaRecipe.Name,
                        categories = OriginaRecipe.categories,
                        CuisineOrigin = OriginaRecipe.CuisineOrigin,
                        Instructions = OriginaRecipe.Instructions,
                        CookingTime = OriginaRecipe.CookingTime ?? 0,
                        Calories = OriginaRecipe.Calories,
                        Fat = OriginaRecipe.Fat,
                        Carbohydrates = OriginaRecipe.Carbohydrates,
                        Protein = OriginaRecipe.Protein,
                        Ingredients = OriginaRecipe.Ingredients,
                        RecipeImageUrl = OriginaRecipe.ImageUrl,
                        Totalcomment = OriginaRecipetotalcomment.Count,
                        totaltasty =    OriginaRecipetotaltasty.Count,
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
                    CuisineOrigin = recipe.CuisineOrigin,
                    CookingTime = recipe.CookingTime ?? 0,
                    Calories = recipe.Calories,
                    Fat = recipe.Fat,
                    Carbohydrates = recipe.Carbohydrates,
                    Protein = recipe.Protein,
                    Ingredients = recipe.Ingredients,
                    RecipeImageUrl = recipe.ImageUrl,
                    Totalcomment = totalcomment.Count,
                    totaltasty = totaltasty.Count,
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
                // Handle exception
            }
            return Path.GetFileName(filename);
        }




        private async Task<evnet> AddEvents(int Recipeid, string userid, string TypeEvent, string taxt)
        {
            if (TypeEvent != "comment")
            {
                var existingEvent = await _RecipeRepository.GetSpecialEntity<evnet>(e => e.Rid == Recipeid && e.userid == userid && e.typeEvent == TypeEvent);

                if (existingEvent != null)

                    return null;

            }



            var evnet = new evnet
            {
                Rid = Recipeid,
                userid = userid,
                eventDate = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm tt"),
                taxt = taxt,
                typeEvent = TypeEvent
            };
            return evnet;
        }


        [HttpGet("getallRecipeSearch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecipeResponse>> GetAllRecipesSearch([FromQuery] string? name = null, [FromQuery] string? ingredients = null, [FromQuery] string? cuisine = null)
        {
            var allRecipes = await _RecipeRepository.GetAllTEntity<Recipe>();

            if (allRecipes == null || allRecipes.Count == 0)
                return NotFound();

            var filteredRecipes = allRecipes.Where(recipe =>
                (name == null || recipe.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) == true) &&
                (ingredients == null || recipe.Ingredients?.Contains(ingredients, StringComparison.OrdinalIgnoreCase) == true) &&
                (cuisine == null || recipe.CuisineOrigin?.Equals(cuisine, StringComparison.OrdinalIgnoreCase) == true)
            ).ToList();

            if (filteredRecipes.Count == 0)
                return NotFound();

            var filteredRecipeResponse = new List<RecipeResponse>();

            foreach (var recipe in filteredRecipes)
            {
                var dto = await MapToDTO(recipe);
                filteredRecipeResponse.Add(dto);
            }

            return Ok(filteredRecipeResponse);
        }


        [HttpGet("getallRecipe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecipeResponse>> getallRecipe()

        {
            var Results = await _RecipeRepository.GetAllTEntity<Recipe>();

            if (Results == null) return NotFound();
            var filtereRecipeResponse = new List<RecipeResponse>();

            foreach (var Result in Results)
            {
                var dto = await MapToDTO(Result);
                filtereRecipeResponse.Add(dto);
            }
            return Ok(filtereRecipeResponse);

        }



        [HttpGet("getRecipesByCategory/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RecipeResponse>>> GetRecipesByCategory(string category)
        {
            // Query the repository to get recipes with matching category
            var recipes = await _RecipeRepository.GetAllTEntity<Recipe>(e => e.categories == category);

            if (recipes == null || !recipes.Any())
            {
                return NotFound();
            }

            // Map the recipes to DTOs
            var filteredRecipeResponses = new List<RecipeResponse>();
            foreach (var recipe in recipes)
            {
                var dto = await MapToDTO(recipe);
                filteredRecipeResponses.Add(dto);
            }

            return Ok(filteredRecipeResponses);
        }


        [HttpGet("getRecipesByCuisineOrigin/{CuisineOrigin}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RecipeResponse>>> getRecipesByCuisineOrigin(string CuisineOrigin)
        {
            // Query the repository to get recipes with matching category
            var recipes = await _RecipeRepository.GetAllTEntity<Recipe>(e => e.CuisineOrigin == CuisineOrigin);

            if (recipes == null || !recipes.Any())
            {
                return NotFound();
            }

            // Map the recipes to DTOs
            var filteredRecipeResponses = new List<RecipeResponse>();
            foreach (var recipe in recipes)
            {
                var dto = await MapToDTO(recipe);
                filteredRecipeResponses.Add(dto);
            }

            return Ok(filteredRecipeResponses);
        }


        [HttpPut("updateRecipe/{recipeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRecipeAndImage(int recipeId, RecipeDTO updatedRecipe, IFormFile file)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim?.Value;

            var existingRecipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == recipeId);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            if (existingRecipe.UserId != userId)
            {
                return BadRequest("You are not authorized to update this recipe.");
            }

            bool anyFieldUpdated =
                existingRecipe.Name != updatedRecipe.Name ||
                existingRecipe.Instructions != updatedRecipe.Instructions ||
                existingRecipe.CookingTime != updatedRecipe.CookingTime ||
                existingRecipe.Calories != updatedRecipe.Calories ||
                existingRecipe.CuisineOrigin != updatedRecipe.CuisineOrigin ||
                existingRecipe.Fat != updatedRecipe.Fat ||
                existingRecipe.Carbohydrates != updatedRecipe.Carbohydrates ||
                existingRecipe.Protein != updatedRecipe.Protein ||
                existingRecipe.categories != updatedRecipe.categories ||
                existingRecipe.Ingredients != updatedRecipe.Ingredients;

            if (!anyFieldUpdated && file == null)
            {
                return BadRequest("No changes detected. Please provide updated information.");
            }

            existingRecipe.Name = updatedRecipe.Name ?? existingRecipe.Name;
            existingRecipe.categories = updatedRecipe.categories ?? existingRecipe.categories;
            existingRecipe.Instructions = updatedRecipe.Instructions ?? existingRecipe.Instructions;
            existingRecipe.CookingTime = updatedRecipe.CookingTime ?? existingRecipe.CookingTime;
            existingRecipe.Calories = updatedRecipe.Calories ?? existingRecipe.Calories;
            existingRecipe.Fat = updatedRecipe.Fat ?? existingRecipe.Fat;
            existingRecipe.Carbohydrates = updatedRecipe.Carbohydrates ?? existingRecipe.Carbohydrates;
            existingRecipe.Protein = updatedRecipe.Protein ?? existingRecipe.Protein;
            existingRecipe.Ingredients = updatedRecipe.Ingredients ?? existingRecipe.Ingredients;
            existingRecipe.CuisineOrigin = updatedRecipe.Ingredients ?? existingRecipe.CuisineOrigin;

            if (file != null)
            {
                existingRecipe.ImageUrl = await WriteFile(file);
            }

            await _RecipeRepository.updatat(existingRecipe);

            return Ok("The recipe details and/or image have been updated successfully");
        }

       



        [HttpGet("GetSpecificRecipe/id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecipeResponse>> GetSpecificRecipe(int id)

        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;
            if (userId == null)
                return BadRequest();
            var Result = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == id);

            if (Result == null) return NotFound();
          
            return Ok(await MapToDTO(Result));


        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Recipe>> CreateRecipe([FromForm] RecipeDTO recipeDTO, IFormFile? file)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            if (userIdClaim == null)
            {
                return BadRequest("User ID claim not found.");
            }
            var userId = userIdClaim.Value;

            // Assuming WriteFile returns a string (file path or URL)
            string imageUrl = null;
            if (file != null)
            {
                imageUrl = await WriteFile(file);
            }

            Recipe model = new Recipe
            {
                Name = recipeDTO.Name,
                Instructions = recipeDTO.Instructions,
                UserId = userId,
                ImageUrl = imageUrl,
                CookingTime = recipeDTO.CookingTime,
                categories = recipeDTO.categories,
                CuisineOrigin = recipeDTO.CuisineOrigin,
                Ingredients = recipeDTO.Ingredients,
                Calories = recipeDTO.Calories,
                Carbohydrates = recipeDTO.Carbohydrates,
                Fat = recipeDTO.Fat,
                Protein = recipeDTO.Protein,
                createdate = DateTime.UtcNow // Use UTC time
            };

            try
            {
                await _RecipeRepository.Create(model);
                return Ok(model); // Returning the created model
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while creating the recipe."); // Internal Server Error
            }
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

            await _RecipeRepository.Remove<Recipe>(Recipe);


            return Ok();

        }





        [HttpPost("event/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<evnet>> Event(int id, [FromQuery] string eventType)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;

            var recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == id);

            if (recipe == null)
                return NotFound();

            // Check if the provided event type is valid
            switch (eventType.ToLower())
            {
                case "favorites":
                case "totry":
                case "mealplans":
                case "like":
                case "dislike":
                case "tasty":
                    break; // Valid event types
                default:
                    return BadRequest($"Invalid event type: {eventType}");
            }

            var @event = await AddEvents(id, userId, eventType, null);
            if (@event == null)
                return BadRequest($"This {eventType} has been added before");

            await _RecipeRepository.AddEvent(@event);

            return Ok(@event);
        }


        [HttpDelete("event/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<evnet>> RemoveEvent(int id, [FromQuery] string eventType)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;

            var recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == id);

            if (recipe == null)
                return NotFound();

            // Check if the provided event type is valid
            switch (eventType.ToLower())
            {
                case "favorites":
                case "totry":
                case "mealplans":
                case "like":
                case "dislike":
                case "tasty":
                    break; // Valid event types
                default:
                    return BadRequest($"Invalid event type: {eventType}");
            }

            // Check if the event exists before attempting removal
            var existingEvent = await _RecipeRepository.GetSpecialEntity<evnet>(e => e.Rid == id && e.userid == userId && e.typeEvent == eventType);
            if (existingEvent == null)
                return BadRequest($"No {eventType} event found to remove.");

            // Perform the removal of the event
            await _RecipeRepository.Remove<evnet>(existingEvent);

            return Ok($"Successfully removed {eventType}.");
        }




      


        

        [HttpDelete("comment/id:int ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Recipe>> Deletecomment(int RecipeId, int eventid)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;
            var existingcomment = await _RecipeRepository.GetSpecialEntity<evnet>(e => e.Id == eventid && e.userid == userId && e.typeEvent == "comment");
            var Recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == RecipeId);

            if (eventid == 0 || userId == null || Recipe == null)
                return BadRequest();
            else
            {
                if (existingcomment == null) return NotFound();
                else
                    await _RecipeRepository.Remove<evnet>(existingcomment);
            }

            return Ok(Recipe);

        }




   



        [HttpPut("shere/id:int/post id  ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<Recipe>> repost(int id, [FromBody] string caption)

        {
            var orignalRecipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == id);
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;
          
            Recipe model = new()
            {
                UserId = userId,
                OriginaRecipeid = orignalRecipe.RecipeId,
                Shared = true,
                Caption = caption,
                createdate = DateTime.Now
            };
            await _RecipeRepository.Create(model);

            return Ok(model);
        }




        [HttpPost("comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> comment(int id, [FromBody] CommentDto CommentDto)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            var userId = userIdClaim.Value;
            var Recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == id);

            if (Recipe == null)
            {
                return NoContent(); // Recipe with the given id not found
            }

            var Event = await AddEvents(id, userId, "comment", null);
            await _RecipeRepository.AddEvent(Event);



            return Ok();

        }


        [HttpGet("events/user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<EventDto>>> GetUserEvents([FromQuery] string eventType)
        {
            var userIdClaim = (HttpContext.User.Identity as ClaimsIdentity)?.Claims.FirstOrDefault(c => c.Type == "uid");
            if (userIdClaim == null)
                return BadRequest("User ID  not found.");

            var userId = userIdClaim.Value;

            var userEvents = await _RecipeRepository.GetAllTEntity<evnet>(e => e.userid == userId &&e.typeEvent== eventType);

            if (userEvents == null || userEvents.Count == 0)
                return NotFound();

            

            // Map the recipes to DTOs
            var filteredRecipeResponses = new List<RecipeResponse>();
            foreach (var userEvent in userEvents)
            {
                var recipe = await _RecipeRepository.GetSpecialEntity<Recipe>(e => e.RecipeId == userEvent.Rid);

                if (recipe == null ) return NotFound();




                var dto = await MapToDTO(recipe);
                filteredRecipeResponses.Add(dto);
            }

            return Ok(filteredRecipeResponses);
            
        }

        [HttpGet("GetPostEvent/id:int ")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<EventDto>>> GetPostEvent(int id, [FromQuery] string eventType)

        {
            if (id == null) return BadRequest();

            var likes = await _RecipeRepository.GetAllTEntity<evnet>(e => e.typeEvent == eventType && e.Rid == id);
            if (likes == null) return NotFound();
            List<EventDto> EventDtolist = new List<EventDto>();
            foreach (var like in likes)
            {

                var respoens = await _RecipeRepository.GetSpecialEntity<ApplicationUser>(e => e.Id == like.userid);
                var model = new EventDto
                {
                    typeEvent = like.typeEvent,
                    eventDate = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm tt"),
                    fname = respoens.FirstName,
                    lname = respoens.LastName,
                    imgeurl = respoens.ImageUrl,
                    username = respoens.UserName

                };
                EventDtolist.Add(model);
            }

            return Ok(EventDtolist);


        }

    }
}
