

using RecipeSharing.models;
using RecipeSharing.Models;

namespace RecipeSharing.Repository.Repository
{
    public interface iRecipeRepository : IRepository<Recipe>
    {
       public Task<Recipe> updatat(Recipe entity);
       public  Task<evnet> AddEvent(evnet evnet);
       


    }
}
