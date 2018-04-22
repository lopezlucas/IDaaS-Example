using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace WebAppOkta
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
            services.Configure<OpenIdConnectOptions>(Configuration.GetSection("Authentication:Okta"));

            var serviceProvider = services.BuildServiceProvider();
            var authOptions = serviceProvider.GetService<IOptions<OpenIdConnectOptions>>();

            services.AddMvc()
                .AddRazorPagesOptions(options => {
                     options.Conventions.AuthorizePage("/Index");
                 });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.ClientId = authOptions.Value.ClientId;
                options.ClientSecret = authOptions.Value.ClientSecret;
                options.Authority = authOptions.Value.Authority;
                options.CallbackPath = authOptions.Value.CallbackPath;
                options.ResponseType = authOptions.Value.ResponseType;
                options.SaveTokens = authOptions.Value.SaveTokens;
                options.UseTokenLifetime = authOptions.Value.UseTokenLifetime;
                options.GetClaimsFromUserInfoEndpoint = authOptions.Value.GetClaimsFromUserInfoEndpoint;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    NameClaimType = authOptions.Value.TokenValidationParameters.NameClaimType
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
