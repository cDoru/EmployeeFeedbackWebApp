using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmployeeFeedbackWebApp.Startup))]
namespace EmployeeFeedbackWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
