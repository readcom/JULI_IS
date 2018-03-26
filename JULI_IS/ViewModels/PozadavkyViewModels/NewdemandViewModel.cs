using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Data;
using Pozadavky.DTO;
using Pozadavky.Services;

namespace ViewModels.PozadavkyViewModels
{
	public class NewdemandViewModel : AppViewModel
    {
        public string Popis { get; set; }
        public string Jednotka { get; set; }
        public string Mena { get; set; }
        public int Mnozstvi { get; set; }
        public float CenaZaJednotku { get; set; }
        public float CelkovaCena { get; set; }        
        public DateTime DatumObjednani { get; set; } = DateTime.Now;
        public DateTime TerminDodani { get; set; } = DateTime.Now;

        public List<DodavateleS21> Dodavatel { get; set; }
        public List<string> MenaList { get; set; } = new List<string> { "CZK", "EUR", "USD" };

        public int DodavatelID { get; set; }


        public string Message { get; set; } = "";

        //public void PozadavekSave()
        //{
        //    try
        //    {
        //        PozadavkyServices.PozadavekInsert(Constants.ActiveUser, Popis, Jednotka, Mnozstvi, CenaZaJednotku,
        //             CelkovaCena, Mena, DatumObjednani, TerminDodani, DodavatelID);

        //        Message = "Pozadavek ulozen...";
        //    }
        //    catch (Exception e)
        //    {

        //        Message = "Chyba při ukládání požadavku: " + e.Message;
        //    }            
        //}

        public void Prepocitat()
        {
            CelkovaCena = CenaZaJednotku * Mnozstvi;
        }


        public override Task PreRender()
        {
            Dodavatel = DodavatelService.GetS21DodavateleOrderedByName();
            return base.PreRender();
        }


        //public void ShowDemandPopup(int? demandId)
        //{
        //    if (demandId == null)
        //    {
        //        EditedItem = new ItemInfoData() { }; //UserRole = UserRole.User };
        //    }
        //    else
        //    {
        //        // EditedItem = UserServices.GetUserInfo(demandId.Value);
        //    }
        //    Context.ResourceManager.AddStartupScript("$('#myModal').modal('show');");
        //    //Context.ResourceManager.AddStartupScript("$('div[data-id=user-detail]').modal('show');");
        //}

        //public void ShowUserPopup(int? userId)
        //{
        //    if (userId == null)
        //    {
        //        EditedItem = new ItemInfoData(); // { UserRole = UserRole.User };
        //    }
        //    else
        //    {
        //       // EditedUser = UserServices.GetUserInfo(userId.Value);
        //    }

        //    Context.ResourceManager.AddStartupScript("$('#myModal').modal('show');");
        //}


    }
}

