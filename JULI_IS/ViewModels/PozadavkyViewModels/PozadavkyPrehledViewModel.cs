using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using Pozadavky.Data;
using Pozadavky.DTO;
using Pozadavky.Services;
using DotVVM.Framework.Hosting;

namespace ViewModels.PozadavkyViewModels
{
	public class PozadavkyPrehledViewModel : AppViewModel
    {
        public string ItemPopis { get; set; } = "";

        public List<PozadavekDTO> SeznamPozadavku { get; set; }

        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public GridViewDataSet<PozadavekDTO> SeznamPozadavkuGV { get; set; } = new GridViewDataSet<PozadavekDTO>()
        {
            PageSize = 10,
            SortExpression = nameof(PozadavekDTO.ID),
            SortDescending = true
        };

        public bool CurrentUserOnly { get; set; } = true;
        public bool NothingFound { get; set; } = true;
        public bool EditItemName { get; set; } = false;

        public void DeletePozadavek(int id, bool odeslano = false)
        {
            
            var pozadavek = PozadavkyService.GetPozadavekById(id);
            if (!odeslano)
                PozadavkyService.DeletePozadavek(id, odeslano);
            else AlertText = "Pozadavek nelze smazat, už byl odeslán na zpracování!";
        }

        public void EditPozadavekName()
        {
            EditItemName = true;
        }

        public void NewPozadavek()
        {
            Context.RedirectToRoute("PozadavekNew");
        }


        public void ChangeCurrentUser()
        {
            if (CurrentUserOnly)
                PozadavkyService.FillGridViewPozadavkyByUser(SeznamPozadavkuGV, UserServices.GetActiveUser());
            else
                PozadavkyService.FillGridViewPozadavkyByUser(SeznamPozadavkuGV);
        }

	    public override Task PreRender()
        {
	        if (!Context.IsPostBack)
	        {
	        }

	        if (CurrentUserOnly)
	            {
	                PozadavkyService.FillGridViewPozadavkyByUser(SeznamPozadavkuGV, UserServices.GetActiveUser());
	                //SeznamPozadavku = PozadavkyServices.GetPozadavkyByName(Constants.ActiveUser);
	            }
	            else
	            {
	                PozadavkyService.FillGridViewPozadavkyByUser(SeznamPozadavkuGV);
	                //PozadavkyServices.FillGridViewPozadavkyByUserAndItemId(SeznamPozadavkuGV, ItemId);
	                //SeznamPozadavku = PozadavkyServices.GetPozadavkyByName();
	            }
          

            NothingFound = SeznamPozadavkuGV.PagingOptions.TotalItemsCount == 0;
	        

            return base.PreRender();
        }
    }
}

