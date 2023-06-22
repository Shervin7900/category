
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;
using CategoryApi.Infrastructures.Middlewares;
using CategoryApi.Infrastructures.DbContexts.Seeders;
using FluentValidation;
using MH.DDD.Core.Mediators;
using CategoryApi.Infrastructures.CommissionOption;
using MH.DDD.Core.ImagePattern;

namespace CategoryApi
{


    public class Startup : CategoryApi.Infrastructures.HostBuilder.IStartup
    {



        private WebApplicationBuilder builder { get; }


        private IConfiguration Configuration { get; }
        private IWebHostEnvironment hostingEnvironment { get; }
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private const string IDENTITY_API_URL_SECTION = "ApiUrls:Identity-Api";




        public Startup(WebApplicationBuilder builder)
        {
            this.builder = builder;
            this.Configuration = builder.Configuration;
            this.hostingEnvironment = builder.Environment;
        }

        public WebApplicationBuilder ConfigureServices()
        {



            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddUserHelper();
            builder.Services.AddLogging();
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());





            builder.Services.Scan(selector =>
                selector.FromCallingAssembly()
                    .AddClasses(action =>
                        action
                            .InNamespaces($"{Program.Name}.Mappers", $"{Program.Name}.Services"))
                            .AsImplementedInterfaces()
                            .WithTransientLifetime());




            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        // builder.WithOrigins("http://localhost:4200")
                        builder
                        .SetIsOriginAllowed(x => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                    });
            });
            builder.Services.AddControllers(options => 
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true ;
            }); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            
            builder.Services.AddApiVersioning(setup =>
                {
                    setup.DefaultApiVersion = new ApiVersion(1, 0);
                    setup.AssumeDefaultVersionWhenUnspecified = true;
                    setup.ReportApiVersions = true;
                    setup.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader("apiVersion"));
                });


            builder.Services.AddEndpointsApiExplorer();



             builder.Services.AddDbContext<CategoryDbContext>(opt =>
            {
                var connection = string.Format(Configuration.GetConnectionString("SqlDefault"),
                    Configuration["ApiUrls:Sql-Data"]);
                opt.UseSqlServer(connection , x=>x.UseHierarchyId());
            });
            builder.Services.AddUnitOfWork<CategoryDbContext>(); 


            builder.Services.AddImageOption(builder.Configuration);
            



            builder.Services.AddSwaggerGen(c =>
            {
                var authUrl = Configuration.GetValue<string>(IDENTITY_API_URL_SECTION);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Program.Name, Version = "v1" });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{authUrl}/connect/authorize"),
                            TokenUrl = new Uri($"{authUrl}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"api.read", "Demo API - full access"}
                            }
                        },
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{authUrl}/connect/authorize"),
                            TokenUrl = new Uri($"{authUrl}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"api.read", "Demo API - full access"}
                            }
                        },
                    }
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddTransient<HttpMessageHandlerMiddleware>();
            builder.Services.AddConsul();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            var identityUrl = builder.Configuration.GetValue<string>(IDENTITY_API_URL_SECTION);
            if (string.IsNullOrEmpty(identityUrl))
                throw new NullReferenceException($"(IdentityUrl) could not be null or empty. please specify in appsetting");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "customer";
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                  ValidateAudience = false,
                  ValidateIssuer = false ,

                  ValidateLifetime = true,
                  //ValidateIssuerSigningKey = true,
                  ClockSkew = TimeSpan.Zero,
                };
            });





            var useRabbitMq = builder.Configuration.GetValue<bool>("UseRabbitMQ", true);
            if (builder.Environment.IsProduction())
                useRabbitMq = true;
            else if (builder.Environment.EnvironmentName?.Equals("Integeration") == true)
                useRabbitMq = false;

            ((useRabbitMq)
                ? builder.Services.AddRabbitMq(builder.Configuration).AddTransient<INotifyService, EventBusNotifierService>()
                : builder.Services.AddTestRabbitMq(builder.Configuration)
            ).AddPolicyAuthorization();



            builder.Services.Scan(selector =>
                selector.FromCallingAssembly()
                .AddClasses(action =>
                    action.InNamespaces($"{Program.Name}.Messages")
                    .AssignableTo(typeof(IIntegrationEventHandler))
                )
                .AsSelf().WithSingletonLifetime());


            builder.Services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            builder.Services.AddTransient<INotifyService, EventBusNotifierService>();


            builder.Services.RegisterServiceForwarder<IFileStorageService>()
                            .AddHttpMessageHandler<HttpMessageHandlerMiddleware>();
            
            new MapsterConfiguration.MapsterConfiguration().Register(TypeAdapterConfig.GlobalSettings);


            var configuration = new CommissionOption();
            Configuration.Bind("CommissionOption" , configuration);
            builder.Services.AddSingleton(configuration);
            return builder;

        }

        public WebApplication Configure(WebApplication app)
        {




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.Name} v1");
                    c.OAuthClientId("demo_api_swagger");
                    c.OAuthAppName("Demo API - Swagger");
                    c.OAuthUsePkce();
                    c.DisplayRequestDuration();
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                });
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();


            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var serviceId = app.UseConsul();
            var consulClient = app.Services.GetService<IConsulClient>();

            app.Lifetime.ApplicationStopped.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceId);
            });
            app.UseEventBus();



            return app;
        }

        public void MigrateDBContext(WebApplication app)
        {
            
             // Do Migrate And Apply Seeders
            using (var serviceScope = app.Services.CreateScope())
            {

                var unitOfWork = serviceScope.ServiceProvider.GetService<IUnitOfWork<CategoryDbContext>>();

                if(!hostingEnvironment.EnvironmentName.Equals("Integeration") && !unitOfWork.DbContext.Database.IsInMemory())
                {
                    unitOfWork.DbContext.Database.Migrate();
                }

                new DatabaseSeederFactory(serviceScope.ServiceProvider , unitOfWork)
                    .SeedRootInDatabase();

            }
        
        }
    
    }


}