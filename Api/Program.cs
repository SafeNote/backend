using FluentValidation;
using SafeNote.Api;
using SafeNote.Api.Constants;
using SafeNote.Api.Exceptions;
using SafeNote.Api.Extensions;
using SafeNote.Api.Features.Note.Endpoints;
using SafeNote.Api.Options;
using SafeNote.Api.Persistence;
using SafeNote.Api.Resolvers;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection(ConfigOptions.Config));

builder.Services.AddHttpContextAccessor();

builder.Services.AddEfAndDataProtection(builder.Configuration);

// FluentValidation and Validators
ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(IApiMarker)));

// Open Api
builder.Services.AddEndpointsApiExplorer();

// Add and Configure Swagger
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(opts => opts.InferSecuritySchemes = true);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy(AppConstants.IpRateLimit, context =>
    {
        var ipAddress = context.GetUserIpAddress();

        return RateLimitPartition.GetSlidingWindowLimiter(ipAddress, key =>
            new SlidingWindowRateLimiterOptions
            {
                Window = TimeSpan.FromSeconds(10),
                PermitLimit = 25,
                SegmentsPerWindow = 5,
                AutoReplenishment = true,
                QueueLimit = 2,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });
});

builder.Services.AddCors(options =>
{
    var scheme = builder.Configuration.GetValue<string>("Config:WebUrl:Scheme") ??
                 throw new AppException("Missing web scheme.");
    var domain = builder.Configuration.GetValue<string>("Config:WebUrl:Domain") ??
                 throw new AppException("Missing web domain.");

    options.DefaultPolicyName = "Cors";
    options.AddDefaultPolicy(policyBuilder =>
        policyBuilder.WithOrigins($"{scheme}://{domain}").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

var app = builder.Build();

app.UseCors("Cors");

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseRateLimiter();
app.MapNoteEndpoints();

app.Run();

// This is to enable testing
namespace SafeNote.Api
{
    public class Program { }
}