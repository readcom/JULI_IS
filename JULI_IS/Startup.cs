using System.Web.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using DotVVM.Framework.Storage;
using System;
using System.Web;
using System.Web.SessionState;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Infrastructure;

[assembly: OwinStartup(typeof(JULI_IS.Startup))]
namespace JULI_IS
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;

            // use DotVVM
            var tempDirectory = Path.Combine(applicationPhysicalPath, "App_Data\\temp");

            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(applicationPhysicalPath, options: options =>
            {
                options.Services.AddSingleton<IUploadedFileStorage>(serviceProvider => new FileSystemUploadedFileStorage(tempDirectory, TimeSpan.FromHours(1)));
               // options.Services.AddSingleton<ICookieManager, SystemWebCookieManager>();
                //options.AddDefaultTempStorages("temp");
                
            });
#if !DEBUG
            dotvvmConfiguration.Debug = false;
#endif

            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });


        }
    }
}
