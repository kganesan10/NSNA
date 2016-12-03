using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PN2016.Startup))]
namespace PN2016
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
