using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DotVVM.Framework.ViewModel;
using Pozadavky.Data;
using Pozadavky.Services;
using DotVVM.Framework.Hosting;
using JULI_IS;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;

namespace ViewModels
{

    public class AppViewModel : DotvvmViewModelBase
    {
        public string GlobalLanguage = "CS-cz";
        public string DtbConxString = "SQLConnection";

        /// <summary>
        /// Connection string pro ceske prostredi
        /// </summary>
        public string DtbConxStringCZ = "SQLConnection";

        /// <summary>
        /// Connection string pro cinske prostredi
        /// </summary>
        public string DtbConxStringEN = "SQLConnectionCN";

        public List<int> ActiveUserLevel { get; set; } = new List<int>();
        /// <summary>
        /// aktivni uzivatel ve formatu jmeno.prijmeni
        /// </summary>
        public string ActiveUser { get; set; }
        public static string ActiveUserStat { get; set; }

        public bool CanSignPozadavky { get; set; } = false;
        public bool CanSignObjednavky { get; set; } = false;
        public bool CanMakeObjednavky { get; set; } = false;

        public void Temp() { }

        //public ItemInfoData EditedItem { get; set; } = new ItemInfoData();

        public string DemandAlertText { get; set; }

        public virtual string ActivePage => Context.Route.RouteName;

        public string AlertText { get; set; }
        public string ConfirmText { get; set; }
        public string Vysledek { get; set; }

        public string TestModeText { get; set; } = "Provoz v testovacím režimu!";
        public bool Test { get; set; } = false;
        public int UserLever { get; set; } = 0;

	    public bool IsSettingTextDoObjednavky { get; set; }

	    public string SettingTextDoObjednavky { get; set; } =
	         @"UPOZORNĚNÍ/nV době od 19.12 do 1.1.2018 bude příjem firmy JULI Motorenwerk, s.r.o.z důvodu inventury uzavřen.Zboží nebude možné přijmout.";


        public bool PathChanged { get; set; } = false;
        public string FilePath { get; set; } 

        //public void ShowDemandPopup(int? demandId)
        //{
        //    if (demandId == null)
        //    {
        //        EditedItem = new ItemInfoData() { }; //UserRole = UserRole.User };
        //    }
        //    else
        //    {
        //       // EditedItem = UserServices.GetUserInfo(demandId.Value);
        //    }
        //    Context.ResourceManager.AddStartupScript("$('#myModal').modal();"); 
        //    //Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('show');");
        //}


        public void ChangeLanguage(string lang)
        {
            if (lang == "CZ")
            {
                CookiesServices.SetCookie("jazyk", "cs");
                CookiesServices.SetCookie("DTB", DtbConxStringCZ);
                DtbConxString = DtbConxStringCZ;
            }
            else
            {
                DtbConxString = DtbConxStringEN;
                CookiesServices.SetCookie("jazyk", "en");
                CookiesServices.SetCookie("DTB", DtbConxStringEN);
            }
            Context.RedirectToRoute("Default");
        }

        public void ClearAlerts()
        {
            AlertText = null;
            ConfirmText = null;
            Vysledek = null;
        }


        public void SaveSetting()
        {
            //SettingTextDoObjednavky
        }

        

        public void CloseModal(string dataid)
        {
            ClearAlerts();
            string closescript = "$('div[data-id=" + dataid + "]').modal('hide');";
            Context.ResourceManager.AddStartupScript(closescript);
        }

        public void SaveDemand()
        {
            try
            {
                //UserServices.CreateOrUpdateUserInfo(EditedItem);
                Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('hide');");
            }
            catch (Exception ex)
            {
                DemandAlertText = ex.Message;
            }
        }

        public string Text { get; set; }

        public void Redirect()
        {
            Context.RedirectToRoute("PozadavkyList");

            //try
            //{
            //    Context.RedirectToRoute("PozadavkyList");
            //}
            //catch (DotvvmInterruptRequestExecutionException e)
            //{

            //    throw;
            //}

        }

        public override Task Init()
        {
            if (!string.IsNullOrEmpty(CookiesServices.GetCookieValue("jazyk")))
            {
                Context.ChangeCurrentCulture(CookiesServices.GetCookieValue("jazyk"));
            }

            return base.Init();
        }

        public override Task PreRender()
        {

            if (!Context.IsPostBack)
            {

                // Nastaveni jazyka a prostredi podle Coookies
                if ((CookiesServices.GetCookieValue("jazyk") == "") ||
                    (CookiesServices.GetCookieValue("DTB") == ""))
                    ChangeLanguage("CZ");

                if (Constants.Test == true) Test = true;

                try
                {
                    GlobalLanguage = CookiesServices.GetCookieValue("jazyk");
                    DtbConxString = CookiesServices.GetCookieValue("DTB");

                    ActiveUser = UserServices.GetActiveUser();
                    ActiveUserStat = ActiveUser;
                    ActiveUserLevel = UserServices.GetActiveUserLevels();
                    CanSignObjednavky = ActiveUserLevel.Contains(4);
                    CanMakeObjednavky = ActiveUserLevel.Contains(3);
                    CanSignPozadavky = (ActiveUserLevel.Contains(1) || ActiveUserLevel.Contains(2));
                    Text = $"User: {ActiveUser}, Level: ";              
                    ActiveUserLevel.ForEach(e => Text += e + ", ");
                    UserLever = ActiveUserLevel.Max();

                    FilePath = SettingsService.GetSetting("FilePath");

                    if (string.IsNullOrEmpty(GlobalLanguage))
                    {
                        if (UserServices.GetActualDomain() == "JULIDOMAIN")
                        {
                            // jsme v CZ
                            CookiesServices.SetCookie("jazyk", "cs");
                            CookiesServices.SetCookie("DTB", DtbConxStringCZ);
                            DtbConxString = DtbConxStringCZ;
                        }
                        else
                        {
                            DtbConxString = DtbConxStringEN;
                            CookiesServices.SetCookie("jazyk", "en");
                            CookiesServices.SetCookie("DTB", DtbConxStringEN);
                        }
                    }

                    Text += $"Connected: {DBService.DatabaseInfo()}";

                }
                catch (Exception ex)
                {

                    Text = $"Chyba: {ex.Message} \n";
                    Text += (ex.InnerException != null)
                         ? ex.InnerException.Message
                         : "";


                }

            }
            return base.PreRender();
        }

        //public string ExtendedToString(this List<int> list)
        //{
        //    string text = "";

        //    //foreach (var item in list)
        //    //{
        //    //    text += item;
        //    //}

        //    list.ForEach(e => text += e);

        //    return text;
        //}

    }
}




//// vytvoření nového produktu
//var product = new Product();
//product.Title = "Můj nový produkt";
//product.Price = 1699;
//entities.AddToProducts(product);
//entities.SaveChanges();
 
//// úprava existujícího
//var product = entities.Products.Single(p => p.ProductID == 1); // vytáhnout si libovolným způsobem produkt nebo více produktů z databáze
//product.Title = "Jiný název";
//entities.SaveChanges();
 
//// smazání
//var product = entities.Products.Single(p => p.ProductID == 1); // vytáhnout si libovolným způsobem produkt nebo více produktů z databáze
//entities.DeleteObject(product);
//entities.SaveChanges();

    	
//.Where(p => p.CreatedDate<DateTime.Now).OrderBy(p => p.PayedDate)