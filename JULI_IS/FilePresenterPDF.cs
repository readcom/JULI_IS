using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using Microsoft.Owin;
using Pozadavky.Data;

namespace JULI_IS
{
    public class FilePresenterPDF : IDotvvmPresenter
    {
        public async Task ProcessRequest(IDotvvmRequestContext context)
        {
            var id = Convert.ToInt32(context.Parameters["Id"]);

            using (var db = new PozadavkyContext())
            {
                var FileToDownload = db.Files.Find(id);

                context.GetOwinContext().Response.ContentType = "application/pdf";
                //context.OwinContext.Response.Headers["content-length"] =
                context.GetOwinContext().Response.Headers["Content-Disposition"] = "attachment; filename=" + FileToDownload.FileName;

     
                await context.GetOwinContext().Response.SendFileAsync(FileToDownload.FullPath);

                //FilesService.DeleteFile(id);

            }

            //return Task.FromResult(0);
        }
    }
}
