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

namespace ViewModels.PozadavkyViewModels
{
    public class PrijemZboziViewModel : AppViewModel
    {
        public string ItemPopis { get; set; } = "";

        public List<ObjednavkaDTO> SeznamObjednavek { get; set; }

        public ObjednavkaDTO ObjkData { get; set; } = new ObjednavkaDTO();

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public GridViewDataSet<ObjednavkaDTO> SeznamObjednavekGV { get; set; } = new GridViewDataSet<ObjednavkaDTO>();
        public GridViewDataSet<ObjItemsDTO> SeznamItemsGv { get; set; } = new GridViewDataSet<ObjItemsDTO>();

        public bool ObjednavkyOnly { get; set; } = true;
        public bool NothingFound { get; set; } = true;
        public bool EditItemName { get; set; } = false;


        public void ChangeView()
        {
            if (ObjednavkyOnly)
                ObjednavkyService.FillGridViewObjednavky(SeznamObjednavekGV);
            else
            {
                ItemsService.FillGridViewObjednavkyWithItemsFiltered(SeznamItemsGv, ColumnFilter, WhereFilter);
                ListObjItems.Where(w => w.Stav != null).ToList().ForEach(p => ListKST.Add(p.Stav));
                ListKST = ListKST.Distinct().OrderBy(o => o).ToList();
                ItemsService.GridViewSetSortByID(SeznamItemsGv);
            }
        }

        /// <summary>
        /// odesle avizo o dodavce na zakladatele pozadavku
        /// dale podbarvi radek na oliv.zelenou
        /// </summary>
        /// <param name="id"></param>
        public void Avizo(int id)
        {
            ClearAlerts();
            try
            {
                var objData = ObjednavkyService.GetObjById(id);
                objData.AvizoDoruceni = true;
                objData.DatumDodani = DateTime.Now;
                ObjednavkyService.SaveObj(objData);

                var adresyLst = SeznamObjednavekGV.Items.Where(w => w.ID == id).Select(s => s.PozadavekZalozil + "@juli.cz;").Distinct().ToList();
                string adresy = "";
                adresy = String.Join(String.Empty, adresyLst);

                ConfirmText += $"Odesílám avízo na adresy: {adresy}. ";

                string vysledek = MailServices.SendMail(adresy, $"Avízo o doručení zboží z objednávky č. {objData.FullObjednavkaID} ",
                  "Dobrý den,<br><br>" +
                   $"zboží z Vaší objednávky č. {objData.FullObjednavkaID} bylo dodáno na oddělení - <b>Příjem zboží</b> JULI Motorenwerk,s.r.o.<br>" +
                   "Tímto e-mailem je potvrzeno uvolnění zboží k Vašemu vyzvednutí na tomto oddělení.<br><br>" +
                   $"Dodací list naleznete v aplikaci Požadavky / Objednávky (http://juli-app/objednavkaDetail/{objData.ID}).<br><br><br>" +
                   "Internal Logistics<br>" +
                   "+420 547 124 589<br>"
                   , null, "objednavky"); //"objednavky")                

                if (vysledek == "Email byl odeslán. ") ConfirmText += vysledek;
                else AlertText += vysledek;
            }
            catch (Exception e)
            {

                AlertText += " Chyba při odeslání avíza o doručení zboží, pravděpodobě chyba v emailové adrese";
            }

        }

        public void Dodano(int id)
        {
            ClearAlerts();
            try
            {
                var ItemData = ItemsService.GetObjItemById(id);
                if (ItemData.Dodano) ItemData.Dodano = false;
                else ItemData.Dodano = true;
                ItemsService.ObjItemSave(ItemData);
            }
            catch (Exception e)
            {
                AlertText += " Chyba: " + e.Message;
            }

        }


        // ----------------------  FILTRY -----------------------------------------------------
        public List<ObjItemsDTO> ListObjItems { get; set; } = new List<ObjItemsDTO>();
        public List<ObjednavkaDTO> ListObj { get; set; } = new List<ObjednavkaDTO>();
        public List<ObjList> ListObjFullId { get; set; } = new List<ObjList>();
        public List<ObjList> ListPozadavekFullId { get; set; } = new List<ObjList>();
        public List<string> ListPozadavekFullId2 { get; set; } = new List<string>();
        public List<string> ListObjFullId2 { get; set; } = new List<string>();
        public List<string> ListKST { get; set; } = new List<string>();
        public List<string> ListDodavatele { get; set; } = new List<string>();
        public List<string> ListZalozil { get; set; } = new List<string>();
        public List<string> ListHlavniRada { get; set; } = new List<string>();

        public string ColumnFilter { get; set; } = "";
        public string WhereFilter { get; set; } = "";

        public string WhereFilterPozadavek { get; set; } = "";
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
            ColumnFilter = filter;

            switch (filter)
            {
                case "ObjednavkaFullID":
                    WhereFilter = WhereFilterPozadavek;
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


        public override Task PreRender()
        {
            
            

            if (!Context.IsPostBack)
            {
                //ActiveUserLevel = UserServices.GetActiveUserLevels();
                //ActiveUser = UserServices.GetActiveUser();


                if (ObjednavkyOnly)
                {
                    
                    ObjednavkyService.FillGridViewObjednavky(SeznamObjednavekGV);
                    ListObj = SeznamObjednavekGV.Items.ToList();
                    ObjednavkyService.GridViewSetSortByID(SeznamObjednavekGV);

                    ListObj.Where(w => (w.FullObjednavkaID != null)).ToList().ForEach(p => ListObjFullId.Add(new ObjList { id = p.ID, objid = (p.FullObjednavkaID) }));
                    ListObjFullId2 = ListObjFullId.OrderByDescending(o => o.objid).Select(s => s.objid).Distinct().ToList();
          
                    ListObj.Where(w => (w.FullDodavatelName != null && w.FullDodavatelName.Length > 3)).ToList().ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                    ListDodavatele = ListDodavatele.Distinct().OrderBy(o => o).ToList();

                    ListObj.ForEach(p => ListZalozil.Add(p.PozadavekZalozil));
                    ListZalozil = ListZalozil.Distinct().OrderBy(o => o).ToList();
                    ListZalozil.RemoveAll(item => item == null);

                    ObjednavkyService.FillGridViewObjednavky(SeznamObjednavekGV);
                    //ListObj.ForEach(p => ListKST.Add(p.Stav));
                    //ListKST = ListKST.Distinct().OrderBy(o => o).ToList();


                    //ListObj.ForEach(p => ListHlavniRada.Add(p.HlavniRada));
                    //ListHlavniRada = ListHlavniRada.Distinct().OrderBy(o => o).ToList();
                }

                else if (!ObjednavkyOnly)

                {

                    ItemsService.FillGridViewObjednavkyWithItemsFiltered(SeznamItemsGv, ColumnFilter, WhereFilter);
                    ListObjItems = SeznamItemsGv.Items.ToList();

                    ListObjItems.ForEach(p => ListPozadavekFullId.Add(new ObjList { id = p.ID, objid = p.ObjednavkaFullID }));
                    ListObjFullId2 = ListPozadavekFullId.OrderByDescending(o => o.objid).Select(s => s.objid).Distinct().ToList();

                    ListObjItems.ForEach(p => ListDodavatele.Add(p.FullDodavatelName));
                    ListDodavatele = ListDodavatele.Distinct().OrderBy(o => o).ToList();

                    ListObjItems.ForEach(p => ListZalozil.Add(p.Zalozil));
                    ListZalozil = ListZalozil.Distinct().OrderBy(o => o).ToList();

                    ListObjItems.ForEach(p => ListKST.Add(p.Stav));
                    ListKST = ListKST.Distinct().OrderBy(o => o).ToList();

                    ListObjItems.ForEach(p => ListHlavniRada.Add(p.HlavniRada));
                    ListHlavniRada = ListHlavniRada.Distinct().OrderBy(o => o).ToList();
                }

            }
            else
            {
                if (ObjednavkyOnly)
                {
                    if (SetDatum)
                        ObjednavkyService.FillGridViewObjednavkyFiltered(SeznamObjednavekGV, "", "", DatumOd, DatumDo);
                    else
                        ObjednavkyService.FillGridViewObjednavkyFiltered(SeznamObjednavekGV, ColumnFilter, WhereFilter);

                }
                else
                {
                    if (SetDatum)
                        ItemsService.FillGridViewObjednavkyWithItemsFiltered(SeznamItemsGv, "", "", DatumOd, DatumDo);
                    else
                        ItemsService.FillGridViewObjednavkyWithItemsFiltered(SeznamItemsGv, ColumnFilter, WhereFilter);

                    NothingFound = SeznamItemsGv.PagingOptions.TotalItemsCount == 0;
                }
            }

           

            NothingFound = SeznamObjednavekGV.PagingOptions.TotalItemsCount == 0;

            return base.PreRender();
        }
    }
}

