using System.Web.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Routing;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Contrib;

namespace JULI_IS
{
    public class DotvvmStartup : IDotvvmStartup
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("Default", "", "Views/PozadavkyViews/home.dothtml");
            config.RouteTable.Add("PozadavekEdit", "pozadavek/{Id}", "Views/PozadavkyViews/pozadavek.dothtml", new { id = 0 });
            config.RouteTable.Add("ItemEdit", "item/{Id}", "Views/PozadavkyViews/item.dothtml", new { id = 0 });
            config.RouteTable.Add("DodavatelEdit", "dodavatel/{Id}", "Views/PozadavkyViews/dodavatel.dothtml", new { id = 0 });
            config.RouteTable.Add("ObjednavkaEdit", "objednavkaDetail/{Id}", "Views/PozadavkyViews/objednavkaDetail.dothtml", new { id = 0 });

            config.RouteTable.Add("PozadavkyPrehled", "PozadavkyViews/PozadavkyPrehled", "Views/PozadavkyViews/PozadavkyPrehled.dothtml");
            config.RouteTable.Add("PolozkyPrehled", "PozadavkyViews/PolozkyPrehled", "Views/PozadavkyViews/PolozkyPrehled.dothtml");
            config.RouteTable.Add("DodavatelePrehled", "PozadavkyViews/DodavatelePrehled", "Views/PozadavkyViews/DodavatelePrehled.dothtml");
            config.RouteTable.Add("sign", "PozadavkyViews/sign", "Views/PozadavkyViews/sign.dothtml");
            config.RouteTable.Add("SignObjednavky", "PozadavkyViews/SignObjednavky", "Views/PozadavkyViews/SignObjednavky.dothtml");
            config.RouteTable.Add("objednavkyTvorba", "PozadavkyViews/objednavkyTvorba", "Views/PozadavkyViews/objednavkyTvorba.dothtml");
            config.RouteTable.Add("objednavkySprava", "PozadavkyViews/objednavkySprava", "Views/PozadavkyViews/objednavkySprava.dothtml");
            config.RouteTable.Add("objednavkyPrehled", "PozadavkyViews/objednavkyPrehled", "Views/PozadavkyViews/objednavkyPrehled.dothtml");
            config.RouteTable.Add("objednavkySend", "PozadavkyViews/objednavkySend", "Views/PozadavkyViews/objednavkySend.dothtml");
            config.RouteTable.Add("PrijemZbozi", "PozadavkyViews/PrijemZbozi", "Views/PozadavkyViews/PrijemZbozi.dothtml");
            config.RouteTable.Add("logout", "PozadavkyViews/logout", "Views/PozadavkyViews/logout.dothtml");

            // prezenter - pozadavek na file/id se nepreda standardne ale preda se tride FilePresenter
            config.RouteTable.Add("FileDownload", "file/{Id}", null, presenterFactory: () => new FilePresenter());
            config.RouteTable.Add("FileDownloadPDF", "filepdf/{Id}", null, presenterFactory: () => new FilePresenterPDF());

            // DOVOLENKY
            //config.RouteTable.Add("DovolenkyHome", "DovolenkyViews/DovolenkyApp", "Views/DovolenkyViews/DovolenkyApp.dotmaster");
            //config.RouteTable.Add("DovolenkyHome", "Views/DovolenkyApp", "Views/DovolenkyApp.dotmaster");
            config.RouteTable.Add("DovolenkyHome", "DovolenkyViews/Home", "Views/DovolenkyViews/Home.dothtml");

            // Uncomment the following line to auto-register all dothtml files in the Views folder
            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            // register code-only controls and markup controls
            config.AddBusinessPackConfiguration();
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom resources and adjust paths to the built-in resources

            config.Resources.Register("jquery", new ScriptResource()
            {
                Location = new UrlResourceLocation("~/Scripts/jquery-3.1.1.min.js")
            });

            config.Resources.Register("jquery-ui", new ScriptResource()
            {
                Location = new UrlResourceLocation("~/Scripts/jquery-ui.min.js"),
                Dependencies = new[] { "jquery" }
            });

            config.Resources.Register("autoHideAlert", new ScriptResource()
            {
                Location = new UrlResourceLocation("~/Scripts/autoHideAlert.js"),
                Dependencies = new[] { "jquery" }
            });

            config.Resources.Register("bootstrap-confirm", new ScriptResource()
            {
                Location = new UrlResourceLocation("~/Scripts/confirm-bootstrap.js"),
                Dependencies = new[] { "jquery" }
            });

            config.Resources.Register("bootstrap", new ScriptResource()
            {
                // Url = "~/Scripts/bootstrap.min.js",
                Location = new UrlResourceLocation("~/Scripts/bootstrap.min.js"),
                Dependencies = new[] { "jquery" }
            });

            config.Resources.Register("Main.css", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/Style/Main.css")
            });

        }
    }
}
