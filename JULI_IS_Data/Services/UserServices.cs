using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pozadavky.Data;
using Pozadavky.DTO;
using System.Web;


namespace Pozadavky.Services
{
    public static class UserServices
    {

        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        /// <summary>
        /// vraci user name ve formatu jmeno.prijmeni
        /// </summary>
        /// <returns></returns>
        public static string GetActiveUser()
        {
            try
            {
                string name;

                //vraci DOMAIN//jmeno
                name = RemoveDomain(HttpContext.Current.Request.LogonUserIdentity.Name);
                
                //vraci jen jmeno
                if ((name == null) || (name == ""))
                    name = RemoveDomain(System.Environment.UserName);
                
                //vraci DOMAIN//jmeno
                if ((name == null) || (name==""))
                 name = RemoveDomain(System.Security.Principal.WindowsIdentity.GetCurrent().Name);


                //nevraci nic
                if ((name == null) || (name == ""))
                    name = RemoveDomain(System.Web.HttpContext.Current.User.Identity.Name);
                
                return name;
            }
            catch (Exception e)
            {
                return "Nemohu zjistit aktualniho uzivatele: " + e.Message;
            }
           
        }

        public static string GetActualDomain()
        {
            string name;

            //vraci DOMAIN//jmeno
            name = HttpContext.Current.Request.LogonUserIdentity.Name;

            name = !string.IsNullOrEmpty(name) ? ReturnDomain(name) : "";

            return name;
        }

        public static List<int> GetActiveUserLevels()
        {
            
            var user = GetActiveUser();
            List<int> seznam = new List<int>();

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var usr = (from sch in db.Users
                                select sch).Where(s => s.User == user);


                if (usr.Any())
                {
                    foreach (var item in usr)
                    {
                        seznam.Add(item.Uroven);
                    }
                }
                else
                {
                    seznam.Add(0);
                }
                
                return seznam;
            }
            
             

        }

        public static List<UsersDTO> GetUserByLevel(int level)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                return db.Users
                    .Select(s => new UsersDTO()
                    {
                        ID = s.ID,
                        JULINumber = s.JULINumber,
                        Jmeno = s.Jmeno,
                        User = s.User,
                        Email = s.Email,
                        Uroven = s.Uroven,
                        OdesilatMaily = s.OdesilatMaily,
                        NacitatDodavatele = s.NacitatDodavatele
                    })
                    .Where(w => w.Uroven == level)
                    .OrderBy(a => a.Jmeno)
                    .ToList();

            }
        }

        public static UsersDTO GetUserById(int? id)
        {
            if (id == null)
            {
                var sch = new UsersDTO()
                {
                    User = "neznámý",
                    Jmeno = "neznámý",
                    Email = "",
                    Uroven = 1,
                    OdesilatMaily = false
                };

                return sch;
            }
            else
            {
                using (var db = new PozadavkyContext(DtbConxString))
                {
                    var schv = (from sch in db.Users
                                select new UsersDTO()
                                {
                                    ID = sch.ID,
                                    JULINumber = sch.JULINumber,
                                    User = sch.User,
                                    Jmeno = sch.Jmeno,
                                    Email = sch.Email,
                                    Uroven = sch.Uroven,
                                    Telefon = sch.Telefon,
                                    OdesilatMaily = sch.OdesilatMaily,
                                    NacitatDodavatele = sch.NacitatDodavatele
                                }).SingleOrDefault(a => a.ID == id);

                    return schv;
                }
            }
        }

        /// <summary>
        /// vrati User podle uzivatele (jmeno.prijmeni)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<UsersDTO> GetUsersByUserName(string name)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var schv = (from sch in db.Users
                            where sch.User == name
                            select new UsersDTO()
                            {
                                ID = sch.ID,
                                JULINumber = sch.JULINumber,
                                User = sch.User,
                                Jmeno = sch.Jmeno,
                                Email = sch.Email,
                                Uroven = sch.Uroven,
                                OdesilatMaily = sch.OdesilatMaily,
                                NacitatDodavatele = sch.NacitatDodavatele,
                                NeverejnyPristup =  sch.NeverejnyPristup
                            }).ToList();

                return schv;
            }
        }

        public static string GetNameByUserName(string username)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var schv = (from sch in db.Users
                            select sch).FirstOrDefault(a => a.User == username);

                return schv.Jmeno;
            }
        }

        private static string RemoveDomain(string user)
        {
            if (user.Contains("\\"))
                return (user.Substring(user.LastIndexOf('\\') + 1)) ?? "";
            else return user;
        }

        private static string ReturnDomain(string user)
        {
            if (user.Contains("\\"))
                return (user.Substring(0, user.LastIndexOf('\\'))) ?? "";
            else return user;
        }

    }
}
