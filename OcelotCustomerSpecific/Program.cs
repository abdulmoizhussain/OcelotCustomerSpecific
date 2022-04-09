using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, (IdentityServerAuthenticationOptions x) =>
    {
        string authority = "http://localhost:5001";

        x.Authority = authority;
        x.RequireHttpsMetadata = authority.StartsWith("https");
        //x.ApiName

        x.JwtValidationClockSkew = TimeSpan.Zero;

        x.TokenRetriever = (HttpRequest httpContext) =>
        {
            var fromHeader = TokenRetrieval.FromAuthorizationHeader();
            var fromQuery = TokenRetrieval.FromQueryString();

            return fromHeader(httpContext) ?? fromQuery(httpContext);
        };
    });


builder.Services.AddOcelot();


var app = builder.Build();
// Configure the HTTP request pipeline.


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", context => context.Response.WriteAsync("Ocelot is running."));
});


app.UseOcelot().Wait();

app.Run();
