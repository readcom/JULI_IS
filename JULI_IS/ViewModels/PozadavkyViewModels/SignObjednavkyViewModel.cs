using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Services;
using Pozadavky.DTO;
using DotVVM.Framework.Controls;

namespace ViewModels.PozadavkyViewModels
{
	public class SignObjednavkyViewModel : AppViewModel
    {

        public List<ObjednavkaDTO> SeznamObjednavek { get; set; }

        public ObjednavkaDTO ObjednavkaData { get; set; } = new ObjednavkaDTO();
        public ObjednavkaDTO ZamitanaObjednavkaData { get; set; } = new ObjednavkaDTO();
	    public bool ObjednavkaZamitana { get; set; } = false;
	    public bool JinyDuvod { get; set; } = false;
	    public string Vysledek { get; set; } = "";

        public GridViewDataSet<ObjednavkaDTO> SeznamObjednavekGv { get; set; } = new GridViewDataSet<ObjednavkaDTO>()
        {
            PageSize = 10,
            SortExpression = nameof(PozadavekDTO.ID),
            SortDescending = true
        };

        public bool CurrentUserOnly { get; set; } = true;
        public bool NothingFound { get; set; } = true;

        
        public void ObjednavkaSchvalit(int id)
	    {            
            ObjednavkaData = ObjednavkyService.GetObjById(id);
            Vysledek = ObjednavkyService.ObjednavkaSchvalit(ObjednavkaData);
	    }

        public void ObjednavkaPokusOZamitnuti(int id)
        {
            ZamitanaObjednavkaData = ObjednavkyService.GetObjById(id);
            ObjednavkaZamitana = true;
        }


        public void ObjednavkuZamitnout()
        {
            //ZamitanyPozadavekData = PozadavkyService.GetPozadavekById(id);
            Vysledek = ObjednavkyService.ObjednavkaZamitnout(
                    ZamitanaObjednavkaData,
                    ZamitanaObjednavkaData.DuvodZamitnutiID == 4 ? ZamitanaObjednavkaData.DuvodZamitnutiText :
                    DuvodyZamitnuti.FirstOrDefault(x => x.ID == ZamitanaObjednavkaData.DuvodZamitnutiID).Text
                );
            ObjednavkaZamitana = false;
        }

        public void DuvodChange()
        {
            if (ZamitanaObjednavkaData.DuvodZamitnutiID == 4)                
            {
                JinyDuvod = true;
            }
            else JinyDuvod = false;
        }

        public Reasons[] DuvodyZamitnuti { get; set; } =
             {
            new Reasons() { ID = 1, Text = "Závažná chyba v objednávce"},
            new Reasons() { ID = 2, Text = "Duplicitní objednávka"},
            new Reasons() { ID = 3, Text = "Objednávka se nebude realizovat"},  
            new Reasons() { ID = 4, Text = "Jiný důvod: "}
        };

        public class Reasons
        {
            public int ID { get; set; }
            public string Text { get; set; }
        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
            }

            if (UserServices.GetActiveUserLevels().Contains(4))
            {
                ObjednavkyService.FillGridViewObjednavkyNaPodpis(SeznamObjednavekGv);
                NothingFound = SeznamObjednavekGv.PagingOptions.TotalItemsCount == 0;
            }


            return base.PreRender();
        }


    }
}

