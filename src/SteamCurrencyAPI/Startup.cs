using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.AspNetCore;
using SteamCurrencyAPI.DataWrapper;
using SteamCurrencyAPI.Interfaces;
using SteamCurrencyAPI.Jobs;
using SteamCurrencyAPI.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SteamCurrencyAPI;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Steam API", Version = "v1" });
            c.AddSecurityDefinition(
                "ApiKey",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "API Key needed to access the endpoints. API Key must be in the 'Authorization' header. For public endpoints you can skip using an API Key or just type '0'.",
                    Name = "Authorization"
                }
            );
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                } }
            );
        });

        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddSingleton<ICurrencyDbContext, CurrencyDbContext>();

        services.AddScoped<ICurrencyGetValueService, CurrencyGetValueService>();
        services.AddScoped<ICurrencyService, CurrencyService>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.WithMethods("POST", "GET", "PATCH", "PUT")
                    .AllowAnyOrigin()
                    .AllowAnyHeader();
            });
        });

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("CurrencyUpdaterJob");
            q.AddJob<CurrencyUpdaterJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("CurrencyUpdaterJob-trigger")
                .WithCronSchedule("0 10 0 * * ?", x => x
                    .InTimeZone(TimeZoneInfo.Utc))
            );
        });

        services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("AllowAll");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute().AllowAnonymous();
            endpoints.MapSwagger();
            endpoints.MapControllers().AllowAnonymous();
        });
    }
}