using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pozadavky.Data;
using Pozadavky.DTO;
using System.Web;
using System.Threading;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;
using Microsoft.Owin;


namespace Pozadavky.Services
{
    public static class Tools
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        public static void ShowMsg(ref string odkaz, string text)
        {
            odkaz = text;
            Thread.Sleep(2000);
            odkaz = null;
        }

        public static string TestCookie(IDotvvmRequestContext context, string key)
        {

            var aa = context.HttpContext.Request.Cookies[key];

            //HttpCookie cookie1 = Request.Cookies["place"];


            return aa;

        }

        public static string GetRequestCookie(IOwinContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
            var cookie = webContext.Request.Cookies[key];
            return cookie == null ? null : cookie.Value;
        }



    }
}
