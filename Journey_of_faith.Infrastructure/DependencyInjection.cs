using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.context;
using Journey_of_faith.Infrastructure.identity;
using Journey_of_faith.Infrastructure.identity.services;
using Journey_of_faith.Infrastructure.persistence.queries;
using Journey_of_faith.Infrastructure.repositories;
using Journey_of_faith.Infrastructure.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using System.Text;
using Google.Apis.Auth.OAuth2;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Journey_of_faith.Application.common.interfaces.caching;

namespace Journey_of_faith.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddSingleton<LoggingSaveChangesInterceptor>();
            service.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Connection"), sqlServerOptionsAction: sqloption =>
                {
                    sqloption.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(20),
                        errorNumbersToAdd: null

                    );
                });
                options.AddInterceptors(serviceProvider.GetRequiredService<LoggingSaveChangesInterceptor>());
            });

             

            service.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            service.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;

                // sign in 
                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetValue<string>("Token:Issuer"),
                        ValidAudience = configuration.GetValue<string>("Token:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Token:Key") ?? string.Empty))
                    };
                });

            return service;
        }
    }


    public static class RegisterService
    {
        public static IServiceCollection AddRegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<TokenService>();
            services.AddLoggedScoped<IIdentityService, IdentityService>();
            services.AddLoggedScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.Configure<TableSchemaName>(
                configuration.GetSection("Db")
            );
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            services.AddLoggedScoped<IQuestionRepository, QuestionRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.questions.IQuestionQueries, QuestionQueries>();
            services.AddLoggedScoped<IEventRepository, EventRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.events.IEventQueries, EventQueries>();

            services.AddScoped<IUnitOfWork, Journey_of_faith.Infrastructure.context.UnitOfWork>();
            services.AddLoggedScoped<IFileStorageService, FileStorageQuestion>();
            services.AddLoggedScoped<IExamRepository, ExamRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.quizs.IExamQueries, ExamQueries>();
            services.AddLoggedScoped<IChurchRepository, ChurchRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.churchs.IChurchQueries, ChurchQueries>();

            services.AddLoggedScoped<IUserRepository, UserRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.users.IUserQueries, UserQueries>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.notifications.IUserDeviceQueries, UserDeviceQueries>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.dashboard.IDashboardQueries, DashboardQueries>();
            services.AddLoggedScoped<ISongRepository, SongRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.songs.ISongQueries, SongQueries>();
            services.AddLoggedScoped<IArenaRepository, Journey_of_faith.Infrastructure.persistence.repositories.ArenaRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.arena.IArenaQueries, Journey_of_faith.Infrastructure.persistence.queries.ArenaQueries>();
            services.AddScoped(typeof(IGetOneToOneData<,>), typeof(GetDataRepository<,>));
            services.AddScoped(typeof(IGetOneToManyData<,>), typeof(GetDataRepository<,>));
            services.AddLoggedScoped<IRoleRepository, RoleRepository>();
            services.AddLoggedScoped<Journey_of_faith.Application.usecases.auth.IRoleQueries, RoleQueries>();
            services.AddLoggedScoped<IEmailService, EmailService>();
            services.AddLoggedScoped<IFirebaseAuthService, FirebaseAuthService>();
            services.AddLoggedScoped<IFirebaseNotification, FirebaseNotification>();
            return services;
        }
    }



    public static class RegisterAutoMapper
    {
        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<Journey_of_faith.Infrastructure.identity.ApplicationUser, Journey_of_faith.Domain.entities.User>().ReverseMap();
            });

            return services;
        }
    }


    public static class RegisterFirebase
    {
        public static IServiceCollection AddFirebaseService(this IServiceCollection services, IConfiguration configuration)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                var firebaseSection = configuration.GetSection("Firebase");

                var firebaseConfigJson = JsonSerializer.Serialize(
                    new
                    {
                        type = firebaseSection["type"],
                        project_id = firebaseSection["project_id"],
                        private_key_id = firebaseSection["private_key_id"],
                        private_key = firebaseSection["private_key"]?.Replace("\\n", "\n"),
                        client_email = firebaseSection["client_email"],
                        client_id = firebaseSection["client_id"],
                        auth_uri = firebaseSection["auth_uri"],
                        token_uri = firebaseSection["token_uri"],
                        auth_provider_x509_cert_url = firebaseSection["auth_provider_x509_cert_url"],
                        client_x509_cert_url = firebaseSection["client_x509_cert_url"]
                    }
                );

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(firebaseConfigJson),
                    HttpClientFactory = new TimeoutHttpClientFactory(TimeSpan.FromSeconds(5))
                });
            }
            return services;
        }
    }
}

