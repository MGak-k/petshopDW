using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using System.Web;

[assembly: OwinStartupAttribute(typeof(PetShopDW.Startup))]
namespace PetShopDW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseHangfireDashboard("/job-dashboard", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            app.UseHangfireServer();

            /// Setup all recurring jobs

            RecurringJob.AddOrUpdate(() => PetShopDW.HangfireJobs.EmailJob.Execute(), Cron.Minutely());
        }

        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }



    }
}
