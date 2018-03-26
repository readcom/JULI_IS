using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using Pozadavky.DTO;
using Pozadavky.Services;

namespace ViewModels.PozadavkyViewModels
{
    public class DodavatelePrehledViewModel : AppViewModel
    {

        public GridViewDataSet<DodavateleS21> GridViewSeznamDodavatelu { get; set; } = new GridViewDataSet<DodavateleS21>()
        {
            PageSize = 15,
            SortExpression = nameof(DodavateleS21.SNAM05),
            SortDescending = false
        };

        public void DeleteDodavatel(int ID)
        {
            DodavatelService.DeleteDodavatel(ID);
        }

        public override Task PreRender()
        {
            DodavatelService.FillGridViewS21Dodavatele(GridViewSeznamDodavatelu);
            return base.PreRender();
        }
    }
}
