using Data;
using Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DotNetEnv;

namespace Youtomp4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            Console.WriteLine("A API KEY É: " + Environment.GetEnvironmentVariable("API_KEY"));

            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            if (apiKey == null)
            {
                throw new ArgumentNullException("API_KEY", "A chave não está definida");
            }

            // Área do banco de dados
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Área de autenticação
            _ = builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })


            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,


                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Adicione a configuração de autorização
            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<YoutubeService>();

            builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minha API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insira o token JWT como: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API V1"));
            }

            app.Run();

        }
    }
}