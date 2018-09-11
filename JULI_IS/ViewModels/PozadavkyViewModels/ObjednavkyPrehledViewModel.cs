using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Pozadavky.Services;
using Pozadavky.DTO;
using DotVVM.Framework.Controls;
//using AutoMapper;
//using Pozadavky.Data;

namespace ViewModels.PozadavkyViewModels
{
	public class ObjednavkyPrehledViewModel : AppViewModel
    {
        public GridViewDataSet<ObjednavkaDTO> SeznamObjednavekGv { get; set; } = new GridViewDataSet<ObjednavkaDTO>();

        public GridViewDataSet<ObjItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ObjItemsDTO>();
  

        public List<PozadavekDTO> SeznamPozadavku { get; set; } = new List<PozadavekDTO>();

        public List<ObjednavkaDTO> SeznamObjednavek { get; set; } = new List<ObjednavkaDTO>();

        public int SelectedObjId { get; set; }

        public List<ObjItemsDTO> SeznamCheckedItems { get; set; } = new List<ObjItemsDTO>();

        public PozadavekDTO PozadavekData { get; set; } = new PozadavekDTO();

        public bool AddToObj { get; set; } = false;
        public string info { get; set; }


        public bool NothingFound { get; set; } = false;

        public bool JenPozadavky { get; set; } = true;

      

        //public void ChangeObjStyle()
        //{
        //    if (JenPozadavky)
        //        PozadavkyService.FillGridViewPozadavkyByPodpisLevel(SeznamPozadavkuGv, ActiveUserLevel);
        //    else
        //        ItemsService.FillGridViewItemsNaObjednani(SeznamObjednavekGv);
        //}

	   

        public void DeleteObj(int id)
        {
            try
            {
                ObjednavkyService.DeleteObj(id); ;
                Vysledek = $"Objednávka smazána...";
            }
            catch (Exception e)
            {
                Vysledek = e.Message;
            }

        }

        // ----------------------  FILTRY -----------------------------------------------------
        public List<ObjItemsDTO> ListObjItems { get; set; } = new List<ObjItemsDTO>();
        public List<ObjList> ListObjFullId { get; set; } = new List<ObjList>();
        public List<string> ListObjFullId2 { get; set; } = new List<string>();
        public List<string> ListKST { get; set; } = new List<string>();
        public List<string> ListDodavatele { get; set; } = new List<string>();
        public List<string> ListZalozil { get; set; } = new List<string>();
        public List<string> ListHlavniRada { get; set; } = new List<string>();

        public string ColumnFilter { get; set; } = "";
        public string WhereFilter { get; set; } = "";

        public string WhereFilterObj { get; set; } = "";
        public string WhereFilterDodavatel { get; set; } = "";
        public string WhereFilterZalozil { get; set; } = "";
        public string WhereFilterKST { get; set; } = "";
        public string WhereFilterHlavniRada { get; set; } = "";

        public string WhereFilterPolozkaLike { get; set; } = "";

        public DateTime DatumOd { get; set; } = DateTime.Today.AddYears(-1);
        public DateTime DatumDo { get; set; } = DateTime.Today;
        public bool SetDatum { get; set; } = false;

        public class ObjList
        {
            public int id { get; set; }
            public string objid { get; set; }
        }

        public void SetFiltr(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                ColumnFilter = filter;
                switch (filter)
                {
                    case "ObjednavkaFullID":
                        //int PomlckaIndex = WhereFilterObj.LastIndexOf('-');
                        //WhereFilterObj = WhereFilterObj.Substring(PomlckaIndex + 1, 4) + "-" + WhereFilterObj.Substring(0, PomlckaIndex);
                        WhereFilter = WhereFilterObj;
                        break;
                    case "FullDodavatelName":
                        WhereFilter = WhereFilterDodavatel;
                        break;
                    case "Zalozil":
                        WhereFilter = WhereFilterZalozil;
                        break;
                    case "Stav":
                        WhereFilter = WhereFilterKST;
                        break;
                    case "Polozka":
                        WhereFilter = WhereFilterPolozkaLike;
                        break;
                    case "HlavniRada":
                        WhereFilter = WhereFilterHlavniRada;
                        break;
                }

                if (filter == "Datum") SetDatum = true;
                else SetDatum = false;
            }
        }

        public override Task PreRender()
        {
            if (!Context.IsPostBack)
            {
                ActiveUserLevel = UserServices.GetActiveUserLevels();
                ActiveUser = UserServices.GetActiveUser();
       
                ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, ColumnFilter, WhereFilter, null, null, ActiveUser, ActiveUserLevel);
                ListObjItems = SeznamItemsGv.Items.ToList();
                ItemsService.GridViewSetSortByID(SeznamItemsGv);

            //  ListObjItems.Where(w => (w.ObjednavkaFullID != null)).ToList().ForEach(p => ListObjFullId.Add(new ObjList { id = p.ID, objid = (p.ObjednavkaFullID.Substring(5) + "-" + p.ObjednavkaFullID.Substring(0,4))}));
                ListObjItems.Where(w => (w.ObjednavkaFullID != null)).ToList().ForEach(p => ListObjFullId.Add(new ObjList { id = p.ID, objid = (p.ObjednavkaFullID) }));

                ListObjFullId2 = ListObjFullId.OrderByDescending(o => o.objid).Select(s=>s.objid).Distinct().ToList();
                
                ListObjItems.Where(w => (w.FullDodavatelName != null)).ToList().ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                ListDodavatele = ListDodavatele.Distinct().OrderBy(o => o).ToList();
                //ListDodavatele.RemoveAll(item => item == null);

                ListObjItems.Where(w => (w.Zalozil != null)).ToList().ForEach(p => ListZalozil.Add(p.Zalozil));
                ListZalozil = ListZalozil.Distinct().OrderBy(o => o).ToList();
                //ListZalozil.RemoveAll(item => item == null);

                ListObjItems.Where(w => (w.Stav != null)).ToList().ForEach(p => ListKST.Add(p.Stav));
                ListKST = ListKST.Distinct().OrderBy(o => o).ToList();
                //ListKST.RemoveAll(item => item == null);

                ListObjItems.Where(w => (w.HlavniRada != null)).ToList().ForEach(p => ListHlavniRada.Add(p.HlavniRada));
                ListHlavniRada = ListHlavniRada.Distinct().OrderBy(o => o).ToList();
                //ListHlavniRada.RemoveAll(item => item == null);
            }
            else
            {                
               // NE, nenatahne se strankovani a zobrazi to vsechny zaznamy               
            }

            if (String.IsNullOrEmpty(WhereFilter)) ColumnFilter = "";

            if (SetDatum)
                ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, "", "", DatumOd, DatumDo, ActiveUser, ActiveUserLevel);
            else
                ItemsService.FillGridViewObjednavkyWithItemsFilteredSchvalene(SeznamItemsGv, ColumnFilter, WhereFilter, null, null, ActiveUser, ActiveUserLevel);


            //info = ObjednavkyService.InfoText;
            //PozadavekData.CelkovaCena = ItemsService.GetCelkovaCenaByPozadavekId(PozadavekId.Value);
            NothingFound = SeznamItemsGv.PagingOptions.TotalItemsCount == 0;            
            
            return base.PreRender();
        }


    }
}

