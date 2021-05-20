using Autofac;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ServiceStack.Redis;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TradeMap.API.Helpers;
using TradeMap.Data.Extensions;
using TradeMap.Data.Repository;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.CronJob;
using TradeMap.Service.Helpers;
using TradeMap.Service.ImplService;
using TradeMap.Service.InterfaceService;
using TradeMap.Service.Servies.ImplService;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
#pragma warning disable CA1041 // Provide ObsoleteAttribute message

        [Obsolete]
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddControllers(x => x.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()))).AddNewtonsoftJson(options =>
              {
                  options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                  options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                  options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                  options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                  foreach (var converter in GeoJsonSerializer.Create(new GeometryFactory(new PrecisionModel(), 4326)).Converters)
                  {
                      options.SerializerSettings.Converters.Add(converter);
                  }
              }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Trade Zone Map API",
                    Version = "v1"
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema
                    );
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                        securitySchema,
                    new string[] { "Bearer" }
                    }
                }

                );
            });
            services.ConnectToConnectionString(Configuration);

            #region JWT

            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetValue<string>("AppSettings:Issuer"),
                    ValidateAudience = true,
                    ValidAudience = Configuration.GetValue<string>("AppSettings:Issuer"),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };
            });

            #endregion JWT

            //===============================================
            //firebase
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "loginkhanhnd-firebase-adminsdk-q13rl-0583fba703.json");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });
            services.AddCors(options =>
            {
                options.AddPolicy(name: "policy1", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddCronJob<MyCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                c.CronExpression = @"0 * * * *";
                //  c.CronExpression = @"0 0 * * *"; every day
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterType<SystemZoneService>().As<ISystemZoneService>();
            builder.RegisterType<TradeZoneVersionServices>().As<ITradeZoneVersionServices>();
            builder.RegisterType<BuildingService>().As<IBuildingService>();
            builder.RegisterType<AccountsService>().As<IAccountsService>();
            builder.RegisterType<StoresService>().As<IStoresService>();
            builder.RegisterType<WardsService>().As<IWardsService>();
            builder.RegisterType<StreetSegmentsService>().As<IStreetSegmentsService>();
            builder.RegisterType<SegmentService>().As<ISegmentService>();
            builder.RegisterType<MapService>().As<IMapService>();
            builder.RegisterType<BrandsService>().As<IBrandsService>();
            builder.RegisterType<CampusService>().As<ICampusService>();
            builder.RegisterType<TradeZoneService>().As<ITradeZoneService>();
            builder.RegisterType<HistoryService>().As<IHistoryService>();
            builder.RegisterType<AssetsService>().As<IAssetsService>();
            builder.RegisterType<GroupZoneServices>().As<IGroupZoneServices>();
            builder.RegisterType<ViolationLogService>().As<IViolationLogService>();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.Register<IRedisClientsManager>(c =>
     new RedisManagerPool(Configuration.GetConnectionString("RedisConnectionString")));
            builder.Register(c => c.Resolve<IRedisClientsManager>().GetCacheClient());
            builder.RegisterGeneric(typeof(GenericRepository<>))
            .As(typeof(IGenericRepository<>))
            .InstancePerLifetimeScope();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");
            app.UseCors("policy1");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TRADE ZONE Api V1");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });
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