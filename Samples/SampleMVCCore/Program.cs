using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using UW.AspNetCore.Authentication;
using UW.Shibboleth;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

//authentication with Shibboleth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = ShibbolethDefaults.AuthenticationScheme;
}).AddUWShibboleth(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var attributes = new ShibbolethAttributeValueCollection()
        {
            new ShibbolethAttributeValue("uid", "bbadger"),
            new ShibbolethAttributeValue("givenName", "Bucky"),
            new ShibbolethAttributeValue("sn", "Badger"),
            new ShibbolethAttributeValue("mail", "bucky.badger@wisc.edu"),
            new ShibbolethAttributeValue("wiscEduPVI", "UW999A999"),
            new ShibbolethAttributeValue("isMemberOf", "uw:domain:dept.wisc.edu:administrativestaff;uw:domain:dept.wisc.edu:it:sysadmin")
        };
        options.Events = new ShibbolethEvents
        {
            OnSelectingProcessor = ctx =>
            {
                ctx.Processor = new ShibbolethDevelopmentProcessor(attributes);
                return Task.CompletedTask;
            }
        };
    }
});

WebApplication app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

