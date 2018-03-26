using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.DTO;
using Pozadavky.Services;
using DotVVM.Framework.Hosting;

namespace ViewModels.PozadavkyViewModels
{
    public class DodavatelViewModel : AppViewModel
    {

        public DodavateleS21 DodavatelData { get; set; } = new DodavateleS21();

        public int DodavatelID
        {
            get { return Convert.ToInt32(Context.Parameters["Id"]); }

        }

        public bool NewDodavatel { get; set; } = false;

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                if (DodavatelID == 0) // novy Dodavatel
                {
                    NewDodavatel = true;
                }
                else
                {
                    DodavatelData = DodavatelService.GetS21DodavatelById(DodavatelID);                    
                }

            }

            return base.PreRender();
        }

        public void Save()
        {
            if (NewDodavatel)
            {
                //DodavatelService.DodavatelInsert(DodavatelData);
            }
            else
            {
                //DodavatelService.DodavatelSave(DodavatelData);
            }


            Context.RedirectToRoute("DodavatelePrehled");

        }
    }
}

