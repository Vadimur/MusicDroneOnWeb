using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MusicDrone.Data;
using MusicDrone.Data.Models;
using MusicDrone.Data.Services;
using MusicDrone.Data.Services.Abstraction;
using MusicDrone.Business.Services;
using MusicDrone.Business.Services.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicDrone.Data.Constants;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MusicDrone.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if !DEBUG
            var server = Configuration["DbServer"];
            var port = Configuration["DbPort"];
            var user = Configuration["DbUser"];
            var password = Configuration["DbPassword"];
            var identityDatabaseName = Configuration["IdentityDatabase"];
                    
            var identityConnectionString = $"Server={server};Database={identityDatabaseName};User={user};Password={password};";
#else
            var identityConnectionString = Configuration.GetConnectionString("MusicDroneIdentityDb");
#endif

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation(identityConnectionString);

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<MusicDroneDbContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<MusicDroneDbContext>(options =>
                options.UseSqlServer(identityConnectionString));

            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
            services.AddAuthentication(config =>
            {
                config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicDrone.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.<BR/>
                      Enter 'Bearer' [space] and then your token in the text input below.
                      <BR/>Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                   }
                });
            });

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IAccountManagement, AccountManagement>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IRoomsUsersService, RoomsUsersService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicDrone.API v1"));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
