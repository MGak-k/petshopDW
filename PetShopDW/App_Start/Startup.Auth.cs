using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using PetShopDW.Models;

namespace PetShopDW
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // This uses cookie to store information for the signed in user
            var authOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = TimeSpan.FromDays(7),
            };
            app.UseCookieAuthentication(authOptions);
        }
    }
}