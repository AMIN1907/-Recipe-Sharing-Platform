
using RecipeSharing.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RecipeSharing.models;
using RecipeSharing.Models;
using RecipeSharing.models;
using WebApplication1.models;


namespace RecipeSharing.Controllers
{
    public class appDbcontext1 : IdentityDbContext<ApplicationUser>
    {
        public appDbcontext1(DbContextOptions<appDbcontext1> options) : base(options)
        { }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<evnet> evnets { get; set; }
        public DbSet<Following> Followings { get; set; }
        public DbSet<Followers> Followers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>()
                .HasOne(e => e.ApplicationUser)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
           
            modelBuilder.Entity<evnet>()
                .HasOne(e => e.Recipe)
                .WithMany()
                .HasForeignKey(e => e.Rid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Following>()
               .HasKey(r => new { r.ApplicationUser1Id, r.ApplicationUser2Id });
            modelBuilder.Entity<Following>()
                .HasOne(f => f.ApplicationUser)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.ApplicationUser1Id)
                .IsRequired();

            modelBuilder.Entity<Followers>()
               .HasKey(r => new { r.ApplicationUser1Id, r.ApplicationUser2Id });
            modelBuilder.Entity<Followers>()
                .HasOne(f => f.ApplicationUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.ApplicationUser1Id)
                .IsRequired();


        }


    }
}
   

