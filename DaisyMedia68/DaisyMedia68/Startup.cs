using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DaisyMedia68.Startup))]
namespace DaisyMedia68
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
