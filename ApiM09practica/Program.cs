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

/// ��<summary>_PLACEHOLDER��
/// Clase principal que configura y arranca la aplicaci�n web.
/// ��</summary>_PLACEHOLDER��
public class Program
{
    /// ��<summary>_PLACEHOLDER��
    /// M�todo principal asincr�nico que configura los servicios, middleware y el pipeline de la aplicaci�n.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Configura CORS para permitir solicitudes desde el cliente Razor.
    /// A�ade SignalR para comunicaci�n en tiempo real.
    /// Configura el DbContext con conexi�n a SQL Server.
    /// Configura Identity con pol�ticas de contrase�a, bloqueo y login.
    /// Configura la autenticaci�n JWT con validaciones y eventos para manejo de tokens.
    /// A�ade autorizaci�n, controladores y opciones JSON para evitar ciclos de referencia.
    /// Configura OpenAPI/Swagger con seguridad JWT.
    /// Crea roles iniciales ("Admin" y "User") al iniciar la aplicaci�n.
    /// Configura middleware para HTTPS, autenticaci�n, autorizaci�n, rutas, CORS, Swagger y SignalR.
    /// Finalmente, ejecuta la aplicaci�n.
    /// </remarks>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:7156"); //Adre�a del client Razor
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
            // Configuraci� de contrasenyes
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = true;

            // Configuraci� del correu electr�nic
            options.User.RequireUniqueEmail = true;

            // Configuraci� de lockout (bloqueig d�usuari)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Configuraci� del login
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
                        Console.WriteLine($"Error de autenticaci�n: {context.Exception.Message}");
                        Console.WriteLine($"Tipo de excepci�n: {context.Exception.GetType().Name}");

                        if (context.Exception.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {context.Exception.InnerException.Message}");
                        }

                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        Console.WriteLine("Token recibido para validaci�n");
                        // Si la petici�n es para el hub SignalR
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
                        Console.WriteLine($"Desaf�o de autenticaci�n: {context.Error}, {context.ErrorDescription}");
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
            var context = services.GetRequiredService<dbContexts>(); // o dbContexts seg�n tu nombre real

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