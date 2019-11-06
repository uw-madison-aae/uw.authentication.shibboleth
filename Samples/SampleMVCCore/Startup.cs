using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Security.Claims;
using UW.AspNetCore.Authentication;
using UW.Shibboleth;

namespace SampleMVCCore
{
    public class Startup
    {

        private IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            Configuration = config;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc();

            if (!Environment.IsDevelopment())
            {
                //authentication with Shibboleth
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
                }).AddUWShibbolethForIISWithVariables();
            }
            else
            {
                // authenticate with local devauth
                // use the shibboleth processor to process these fake header/variables
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
                }).AddDevAuthentication(new Dictionary<string, string>() {
                    {"uid", "bbadger" },
                    {"givenName", "Bucky" },
                    {"sn", "Badger"},
                    {"mail", "bucky.badger@wisc.edu" },
                    {"wiscEduPVI", "UW999A999" },
                    {"isMemberOf", "uw:domain:dept.wisc.edu:administrativestaff;uw:domain:dept.wisc.edu:it:sysadmin" }
                });

                //// authenticate with local devauth
                //// put in the specific claims you want 
                //services.AddAuthentication(options =>
                //{
                //    options.DefaultScheme = DevAuthenticationDefaults.AuthenticationScheme;
                //}).AddDevAuthentication(new List<Claim>() {
                //    new Claim(StandardClaimTypes.Name, "bbadger"),
                //    new Claim(StandardClaimTypes.GivenName, "Bucky"),
                //    new Claim(StandardClaimTypes.Surname, "Badger"),
                //    new Claim(StandardClaimTypes.PPID, "UW999A999"),
                //    new Claim(StandardClaimTypes.Email, "bucky.badger@wisc.edu"),
                //    new Claim(StandardClaimTypes.Group, "uw:domain:dept.wisc.edu:administrativestaff"),
                //    new Claim(StandardClaimTypes.Group, "uw:domain:dept.wisc.edu:it:sysadmin"),
                //});
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
