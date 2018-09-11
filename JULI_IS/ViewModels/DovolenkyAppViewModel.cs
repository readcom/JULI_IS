using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Data;
using Pozadavky.Services;
using DotVVM.Framework.Hosting;


namespace ViewModels
{

    public class DovolenkyAppViewModel : DotvvmViewModelBase
	{

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

        public void ClearAlerts()
        {
            AlertText = null;
            ConfirmText = null;
            Vysledek = null;
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

        public override Task PreRender()
        {


         

            if (!Context.IsPostBack)
            {
                if (Constants.Test == true) Test = true;

                try
                {                    
                    ActiveUser = UserServices.GetActiveUser();
                    ActiveUserStat = ActiveUser;
                    ActiveUserLevel = UserServices.GetActiveUserLevels();
                    CanSignObjednavky = ActiveUserLevel.Contains(4);
                    CanMakeObjednavky = ActiveUserLevel.Contains(3);
                    CanSignPozadavky = (ActiveUserLevel.Contains(1) || ActiveUserLevel.Contains(2));
                    Text = $"User: {ActiveUser}, Level: ";
                    ActiveUserLevel.ForEach(e => Text += e + ", ");
                    UserLever = ActiveUserLevel.Max();
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