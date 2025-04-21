using ApiM09practica.Contexts;
using ApiM09practica.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json.Serialization;
using ApiM09practica.Hubs;
using Azure.Core;

/// ‡‡<summary>_PLACEHOLDER‡‡
/// Clase principal que configura y arranca la aplicación web.
/// ‡‡</summary>_PLACEHOLDER‡‡
public class Program
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Método principal asincrónico que configura los servicios, middleware y el pipeline de la aplicación.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Configura CORS para permitir solicitudes desde el cliente Razor.
    /// Añade SignalR para comunicación en tiempo real.
    /// Configura el DbContext con conexión a SQL Server.
    /// Configura Identity con políticas de contraseña, bloqueo y login.
    /// Configura la autenticación JWT con validaciones y eventos para manejo de tokens.
    /// Añade autorización, controladores y opciones JSON para evitar ciclos de referencia.
    /// Configura OpenAPI/Swagger con seguridad JWT.
    /// Crea roles iniciales ("Admin" y "User") al iniciar la aplicación.
    /// Configura middleware para HTTPS, autenticación, autorización, rutas, CORS, Swagger y SignalR.
    /// Finalmente, ejecuta la aplicación.
    /// </remarks>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:7156"); //Adreça del client Razor
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();
            });
        });
        builder.Services.AddSignalR();

        // Add services to the container.

        //Afegim DbContext
        var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
        object value = builder.Services.AddDbContext<dbContexts>(options => options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configuració de contrasenyes
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = true;

            // Configuració del correu electrònic
            options.User.RequireUniqueEmail = true;

            // Configuració de lockout (bloqueig d’usuari)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Configuració del login
            options.SignIn.RequireConfirmedEmail = false; // true si vols que es confirmi el correu
        })
             .AddEntityFrameworkStores<dbContexts>()
            .AddDefaultTokenProviders();

        //Configuracio del Token i les seves validacions
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");

       

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validado correctamente");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Error de autenticación: {context.Exception.Message}");
                        Console.WriteLine($"Tipo de excepción: {context.Exception.GetType().Name}");

                        if (context.Exception.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {context.Exception.InnerException.Message}");
                        }

                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        Console.WriteLine("Token recibido para validación");
                        // Si la petición es para el hub SignalR
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/UsersXat"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"Desafío de autenticación: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };


                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });
        

        builder.Services.AddAuthorization();
        builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

        

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
        });


        // Antes de var app = builder.Build();
        var loggerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Token validation - Key: {KeyLength} chars", jwtSettings["Key"]?.Length);
        logger.LogInformation("Token validation - Issuer: {Issuer}", jwtSettings["Issuer"]);
        logger.LogInformation("Token validation - Audience: {Audience}", jwtSettings["Audience"]);
        //********
        var app = builder.Build();
        app.UseRouting();
        app.UseCors();

        //***** Middlewares ******//

        //App pipeline

        // Crear rols inicials: Admin i Editor
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            // Crear roles iniciales
            await ApiM09practica.Tools.RoleTools.CrearRolsInicials(services);

            // Crear datos iniciales (usuarios y juegos)
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<dbContexts>(); // o dbContexts según tu nombre real

            var initializer = new Initializer(userManager, context);
            await initializer.SeedDataAsync();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.MapHub<Chathub>("/UsersXat");

        app.Run();
    }
}