
using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistance;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

namespace ETicaretAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Logger log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt")
                .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true,
                columnOptions: new Dictionary<string, ColumnWriterBase>
                {
                    {"message", new RenderedMessageColumnWriter() },
                    {"message_template", new MessageTemplateColumnWriter() },
                    {"level", new LevelColumnWriter() },
                    {"time_stamp", new TimestampColumnWriter() },
                    {"exception", new ExceptionColumnWriter()},
                    {"log_event", new LogEventSerializedColumnWriter() },
                    {"user_name", new UsernameColumnWriter()}
                })
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .CreateLogger();
            builder.Host.UseSerilog(log);
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec-ch-ua");
                logging.ResponseHeaders.Add("MyResponseHeader");
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.CombineLogs = true;
            });
            builder.Services.AddPersistanceServices(builder.Configuration);
            builder.Services.AddStorage<AzureStorage>();
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices();
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost:5173", "https://localhost:5173").AllowAnyHeader().AllowCredentials().AllowAnyMethod()));
            builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>()).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Admin", options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true, 
                        ValidAudience = builder.Configuration["Token:Audience"], 

                        ValidateIssuer = true, 
                        ValidIssuer = builder.Configuration["Token:Issuer"], 

                        ValidateLifetime = true, 

                        ValidateIssuerSigningKey = true, 
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),

                        LifetimeValidator = ( notBefore, expires,securityToken,validationParameters) => expires != null ? expires>DateTime.UtcNow: false,

                        NameClaimType = ClaimTypes.Name

                    };
                });
             

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSerilogRequestLogging();
            app.UseHttpLogging();
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.Use(async (context, next) => {
                var username = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null;
                LogContext.PushProperty("user_name", username);
                await next();
            });
            
            app.MapControllers();
            app.Run();
        }
    }
}
