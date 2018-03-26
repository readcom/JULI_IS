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
    public static class CookiesServices
    {
        public static void SetCookie(string name, string hodnota)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = hodnota;
            cookie.Expires = DateTime.Now.AddYears(1);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        public static string GetCookieValue(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                return HttpContext.Current.Request.Cookies[name].Value;
            }
            else
            {
                return "";
            }
            
        }

        public static void DeleteCookie(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);                
            }   
        }
    }
}
