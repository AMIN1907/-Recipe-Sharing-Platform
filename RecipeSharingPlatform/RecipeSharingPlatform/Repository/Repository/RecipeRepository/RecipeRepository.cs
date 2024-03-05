
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeSharing.Controllers;

using RecipeSharing.models;
using RecipeSharing.Models;

namespace RecipeSharing.Repository.Repository
{
    public class RecipeRepository : RepositoryGeneric<Recipe>, iRecipeRepository
    {
        private readonly appDbcontext1 _db;

        public RecipeRepository(appDbcontext1 db) : base(db)
        {
            _db = db;

        }
        public async Task<evnet> AddEvent(evnet evnet)
        {
            await _db.evnets.AddAsync(evnet);

            await save();
            return evnet;
        }


     

        public async Task<Recipe> updatat(Recipe entity)
        {   
            _db.Recipe.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

    }
}


