using ApiM09practica.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace ApiM09practica.Contexts
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase encargada de crear el contexto para acceder a la base de datos.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Solo hay la creacion de contexto user y su relacion con la tabla games es todo lo que necesitamos para el funcionamiento.
    /// </remarks>
    public class dbContexts : IdentityDbContext<ApplicationUser>
    {
        public dbContexts(DbContextOptions<dbContexts> options ) : base(options) { }

        public DbSet<Games> games { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(d => d.videoGames)
                .WithOne(f => f.user)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
