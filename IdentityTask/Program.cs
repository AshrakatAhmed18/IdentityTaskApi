
using autheticationpart.Data.context;
using autheticationpart.Data.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace autheticationpart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region DataBase 
            var connectionString = builder.Configuration.GetConnectionString("Company");
            builder.Services.AddDbContext<companyContext>(
                options => options.UseSqlServer(connectionString));
            #endregion

            #region Identity Manger
            builder.Services.AddIdentity<Employee, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase= false;
                options.Password.RequireLowercase= false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 5;

                options.User.RequireUniqueEmail = true; 
            })
                .AddEntityFrameworkStores<companyContext>();
            #endregion

            #region Authorization

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "Admin");
                });

                options.AddPolicy("AdminAndUser", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "Admin" , "User");
                });
            });

            #endregion

            #region authentication schema
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Cool";
                options.DefaultChallengeScheme = "Cool";
            }).AddJwtBearer("Cool",options =>
            {
                var secretKeyString = builder.Configuration.GetValue<string>("SecretKey");
                var secretKeyInByte = Encoding.ASCII.GetBytes(secretKeyString?? string.Empty);
                var secretKey = new SymmetricSecurityKey(secretKeyInByte);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = secretKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    
                };

            });

            #endregion



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}