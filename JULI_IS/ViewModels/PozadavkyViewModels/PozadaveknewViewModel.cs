using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotVVM.Framework.ViewModel;
using Pozadavky.DTO;
using System.Threading.Tasks;
using Pozadavky.Services;
using DotVVM.Framework.Hosting;

namespace ViewModels.PozadavkyViewModels
{
	public class PozadaveknewViewModel : AppViewModel
	{

        public string Mena { get; set; }
        public List<string> MenaList { get; set; } = new List<string> { "CZK", "EUR", "USD" };

        public void Prepocitat()
        {
           // PozadavekData.CelkovaCena = PozadavekData.CenaZaJednotku * PozadavekData.Mnozstvi;
        }

        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();

        public List<DodavateleS21> Dodavatel { get; set; }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
               // PozadavekData.ObjednavkaId = ItemId;
                Dodavatel = DodavatelService.GetS21DodavateleOrderedByName();
            }

            return base.PreRender();
        }

        public void SaveNewPozadavek()
        {
            PozadavkyService.PozadavekInsert(PozadavekData);

            Context.RedirectToRoute("PozadavkyList");

        }

    }
}

