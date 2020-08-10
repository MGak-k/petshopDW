using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PetShopDW.Startup))]
namespace PetShopDW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
