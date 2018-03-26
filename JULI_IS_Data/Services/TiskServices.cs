using System;
using System.Globalization;
using System.IO;
//using System.Web.UI;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Pozadavky.Services;
using System.Threading.Tasks;
using PdfSharp.Drawing.Layout;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.Diagnostics;
using Pozadavky.DTO;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Text;
using Pozadavky.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;

namespace Pozadavky.Services
{

    public static class TiskServices
    {
        const bool unicode = true;
        const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }


        // readonly static Color TableBorder = new Color(81, 125, 192);
        // readonly static Color TableBlue = new Color(235, 240, 249);
        //  readonly static Color TableGray = new Color(242, 242, 242);

        private static List<ItemsDTO> items { get; set; } = new List<ItemsDTO>();

        // vytvori MemoryStream s PDF daty pro konkretni pozadavek ID
        public static MemoryStream CreatePdfPozadavkovyListByPozadavekId(int pozadavekId = 0)
        {            
            PozadavekDTO pozadavek = PozadavkyService.GetPozadavekById(pozadavekId);
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);

            Document document = CreateDocumentByPozadavek(pozadavek);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            MemoryStream stream = new MemoryStream();
            pdfRenderer.PdfDocument.Save(stream, false);

            return stream;
        }
        
        // vytvori MemoryStream s PDF daty pro konkretni obj ID
        public static MemoryStream CreatePdfObjednavkovyListByObjId(int id)
        {
            ObjednavkaDTO obj = ObjednavkyService.GetObjById(id);

            Document document = CreateObjednavkaByObj(obj);

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(unicode, embedding);

            renderer.Document = document;
            renderer.RenderDocument();
            MemoryStream stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);



            return stream;
        }

        public static MemoryStream CreatePdfObjednavkovyListByObjIdTest(int id)
        {
            ObjednavkaDTO obj = ObjednavkyService.GetObjById(id);


            //PdfDocument document = new PdfDocument();
            //CreateDocumentByObjTest(document, obj);
            //MemoryStream stream = new MemoryStream();
            //document.Save(stream, false);

            //PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            //Document document = CreateDocumentByObj(obj);

            //pdfRenderer.Document = document;
            //pdfRenderer.RenderDocument();

            //MemoryStream stream = new MemoryStream();
            //pdfRenderer.PdfDocument.Save(stream, false);

            Document document = CreateDocumentByObjTest(obj);
       



            //string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(unicode, embedding);
    
            renderer.Document = document;
            renderer.RenderDocument();
            MemoryStream stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);
    
            return stream;
        }

        public static MemoryStream CreatePdfPotvrzeníObjednavkyByObjId(int id)
        {
            ObjednavkaDTO obj = ObjednavkyService.GetObjById(id);
            Document document = CreatePotvrzeniByObj(obj);
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(unicode, embedding);

            renderer.Document = document;
            renderer.RenderDocument();
            MemoryStream stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);

            return stream;

        }

        private static Document CreateDocumentByPozadavek(PozadavekDTO pozadavek)
        {
            Document document = new Document();
            document.DefaultPageSetup.Clone();
            document.DefaultPageSetup.BottomMargin = "4cm";
            document.DefaultPageSetup.DifferentFirstPageHeaderFooter = true;
            document.DefaultPageSetup.LeftMargin = "2cm";

            List<Sumarizace> sumarizace;
            sumarizace = SumarizaceInit(pozadavek.ID);

            DefineStyles(document);
   
            Section section = document.AddSection();            
            string TypInvestice; String Cislo = "";

            if (pozadavek.InvesticePlanovana) { TypInvestice = "Plánovaná"; Cislo = pozadavek.CisloInvestice == null ? "" : pozadavek.CisloInvestice; }
            else if (pozadavek.InvesticeNeplanovana) TypInvestice = "Neplánovaná investice";
            else {TypInvestice = "Ostatní nákupy"; Cislo = pozadavek.CisloKonta == null ? "" : pozadavek.CisloKonta; }



            Paragraph p = section.Headers.Primary.AddParagraph();
            p.Format.Font.Size = 24;
            p.AddFormattedText($"Požadavkový list", TextFormat.Bold);
            p.Format.Alignment = ParagraphAlignment.Center;
           
            p = section.AddParagraph();
            p.Format.Font.Size = 16;
            p.AddFormattedText($"\nPožadavek č.: { pozadavek.FullPozadavekID ?? ""}", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 12;
            // p.Format.SpaceBefore = "1cm";
            //p.Style = "Reference";
            //p.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            p.Format.SpaceBefore = "0.2cm";
            p.AddFormattedText($"Popis: {pozadavek.Popis ?? ""}", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 12;
            p.Format.SpaceAfter = "0.5cm";
            //p.Style = "Reference";            
            if (pozadavek.DodavatelID != null && pozadavek.DodavatelID > 0)
            p.AddFormattedText($"Dodavatel: {DodavatelService.GetDodavatelByIdAsS21(pozadavek.DodavatelID).SUPN05}, {DodavatelService.GetDodavatelByIdAsS21(pozadavek.DodavatelID).SNAM05}", TextFormat.Bold);

    
            //------------------------------------------------------------------------------------

            Table tbl = section.AddTable();
            tbl.Format.Alignment = ParagraphAlignment.Left;
            tbl.Format.Font.Size = 10;
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;
            tbl.Rows.VerticalAlignment = VerticalAlignment.Center;

            // Before you can add a row, you must define the columns
            Column column = tbl.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = tbl.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = tbl.AddColumn("1.2cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = tbl.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = tbl.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = tbl.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;


            // Create the header of the table
            Row row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("Druh nákupu");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph(TypInvestice == "Plánovaná" ? "Č. investice" : "Konto" );
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("KST");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[3].AddParagraph("Měna");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[4].AddParagraph("Žadatel");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[5].AddParagraph("Dne");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Left;



            // ------ FILL table -------
            //------------------------------------------------------------------------------------


            row = tbl.AddRow();

            row.Cells[0].AddParagraph(TypInvestice);
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph(Cislo ?? "");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            //if (pozadavek.CisloInvestice != null)
            //{
            //    row.Cells[1].AddParagraph(pozadavek.CisloInvestice ?? "");
            //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            //}
            //else
            //{
            //    row.Cells[1].AddParagraph(TypInvestice == "Plánovaná investice" ? $"{pozadavek.CisloInvestice.ToString() ?? ""}" : $"{pozadavek.CisloKonta.ToString() ?? ""}"); 
            //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            //}

            string zalozil = pozadavek.Zalozil;
            if (pozadavek.Zastoupeno != null)
            {
                zalozil += (" (" + pozadavek.Zastoupeno + ")");
            } 


            row.Cells[2].AddParagraph(pozadavek.KST ?? "");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[3].AddParagraph(pozadavek.Mena.ToString() ?? "");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[4].AddParagraph(zalozil);
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[5].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Datum));
            row.Cells[5].Format.Alignment = ParagraphAlignment.Left;

            //------------------------------------------------------------------------------------

            p = section.AddParagraph();
            p.Format.SpaceBefore = "0.3cm";
            //p.Style = "Reference";
            p.AddFormattedText($"Poznámka: {pozadavek.Poznamka ?? ""}", TextFormat.Bold);

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "1cm";
            //p.Style = "Reference";
            p.AddFormattedText($"Počet položek v požadavku: {pozadavek.PocetPolozek}", TextFormat.Bold);
            p.Format.SpaceAfter = "0.5cm";

            if (pozadavek.PocetPolozek > 0)
            {
                // ------- ITEMS z pozadavku ---------

                tbl = section.AddTable();
                tbl.Style = "Table";
                tbl.Format.Alignment = ParagraphAlignment.Left;
                tbl.Borders.Width = 0.25;
                tbl.Borders.Left.Width = 0.5;
                tbl.Borders.Right.Width = 0.5;
                tbl.Rows.LeftIndent = 0;
                tbl.Rows.Height = Unit.FromMillimeter(4);
                tbl.Rows.VerticalAlignment = VerticalAlignment.Center;

                tbl.Format.Font.Size = 10;

                // Before you can add a row, you must define the columns

                // ID
                column = tbl.AddColumn("1cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // POPIS
                column = tbl.AddColumn("6cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                //column = tbl.AddColumn("1.5cm");
                //column.Format.Alignment = ParagraphAlignment.Right;

                // Termin
                column = tbl.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                // mnozstvi
                column = tbl.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                //jedn.cena
                column = tbl.AddColumn("2.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // Celkova cena
                column = tbl.AddColumn("2.5cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                // KST
                column = tbl.AddColumn("2.5cm");
                column.Format.Alignment = ParagraphAlignment.Right;

                //column = tbl.AddColumn("1cm");
                //column.Format.Alignment = ParagraphAlignment.Right;

                //column = tbl.AddColumn("1cm");
                //column.Format.Alignment = ParagraphAlignment.Right;

                // Create the header of the table
                row = tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Format.Font.Size = 10;


                row.Cells[0].AddParagraph("Č.");

                row.Cells[1].AddParagraph("Popis");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                //row.Cells[2].AddParagraph("Jednotka");
                //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[3].AddParagraph("Množství");
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[4].AddParagraph("Jedn. cena");
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[5].AddParagraph("Celková cena");
                row.Cells[5].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[6].AddParagraph((TypInvestice == "Plánovaná" ? "INVESTICE" : "KONTO") +  " / KST");
                row.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[2].AddParagraph("Termín");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                //row.Cells[7].AddParagraph("Termín");
                //row.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                //row.Cells[8].AddParagraph("Termín");
                //row.Cells[8].Format.Alignment = ParagraphAlignment.Left;

                // ------ FILL ITEMS table -------

                items = ItemsService.GetItemsByPozadavekId(pozadavek.ID);

                int count = 1;

                foreach (var item in items)
                {
                    row = tbl.AddRow();

                    row.Cells[0].AddParagraph(count.ToString());
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                    row.Cells[1].AddParagraph(item.Popis ?? "");
                    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                    //row.Cells[2].AddParagraph(item.Jednotka ?? "");
                    //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                    row.Cells[3].AddParagraph(item.Mnozstvi.ToString() + " " + (item.Jednotka ?? ""));
                    row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

                    row.Cells[4].AddParagraph(item.CenaZaJednotku.ToString("n2"));
                    row.Cells[4].Format.Alignment = ParagraphAlignment.Right;

                    row.Cells[5].AddParagraph(item.CelkovaCena.ToString("n2"));
                    row.Cells[5].Format.Alignment = ParagraphAlignment.Right;

                    row.Cells[6].AddParagraph($"{item.CisloKonta ?? item.CisloInvestice ?? ""} / {item.KST ?? ""}");
                    row.Cells[6].Format.Alignment = ParagraphAlignment.Left;

                    row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", item.TerminDodani));
                    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                    //row.Cells[7].AddParagraph(item.KST ?? "");
                    //row.Cells[7].Format.Alignment = ParagraphAlignment.Left;

                    //row.Cells[8].AddParagraph(item.CisloKonta ?? "");
                    //row.Cells[8].Format.Alignment = ParagraphAlignment.Left;

                    count++;
                }

            }

            p = section.AddParagraph();
            p.Style = "VetsiFont";
            p.Format.SpaceBefore = "0.5cm";
            p.Format.SpaceAfter = "1.0cm";
            p.Format.Alignment = ParagraphAlignment.Right;
            p.AddFormattedText($"Celková cena bez DPH: {pozadavek.CelkovaCena.ToString("n2")} {pozadavek.Mena ?? ""}\n", TextFormat.Bold);


            //------------------------------------------------------------------------------------
            //  SUMARIZACE

            tbl = section.AddTable();             
            tbl.Format.Alignment = ParagraphAlignment.Left;
            tbl.Format.Font.Size = 10;
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;
            tbl.Rows.VerticalAlignment = VerticalAlignment.Center;

            // Before you can add a row, you must define the columns
            column = tbl.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = tbl.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Right;



            // Create the header of the table
            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph(TypInvestice == "Plánovaná" ? "Číslo investice" : "Číslo konta");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("KST");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Sumarizace požadavku");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            // ------ FILL table -------
            //------------------------------------------------------------------------------------

            foreach (var item in sumarizace)
            {
                row = tbl.AddRow();

                row.Cells[0].AddParagraph(item.CisloKonta ?? item.CisloInvestice ?? "");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[1].AddParagraph(item.KST ?? "");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[2].AddParagraph(item.suma.ToString("n2") ?? "");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            }




            //------------------------------------------------------------------------------------





            // -----------------------------  FOOTERS  FIRST PAGE ---------------------------

            tbl = section.Footers.FirstPage.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5cm");
            column = tbl.AddColumn("5cm");
            column = tbl.AddColumn("5cm");


            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Height = "0.5cm";

            row.Cells[0].AddParagraph("Odesláno na podpis");
            row.Cells[1].AddParagraph("Podepsáno vedoucím");
            row.Cells[2].AddParagraph("Podepsáno ředitel / controling");

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Height = "1.0cm";
            row.VerticalAlignment = VerticalAlignment.Center;

            if (pozadavek.Level1Odeslano == true)
            {
                row.Cells[0].AddParagraph("Odesláno na schválení dne\n");
                row.Cells[0].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Level1OdeslanoDne));
            }
            else row.Cells[0].AddParagraph("");

            if ((pozadavek.Level1SchvalovatelID != null) && (pozadavek.Level1SchvalovatelID != 0))
            {
                row.Cells[1].AddParagraph($"{UserServices.GetUserById(pozadavek.Level1SchvalovatelID).Jmeno} dne");
                row.Cells[1].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Level1SchvalenoDne));
            }
            else row.Cells[1].AddParagraph("");


            if ((pozadavek.Level2SchvalovatelID != null) && (pozadavek.Level2SchvalovatelID != 0))
            {
                row.Cells[2].AddParagraph($"{UserServices.GetUserById(pozadavek.Level2SchvalovatelID).Jmeno} dne");
                row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Level2SchvalenoDne));
            }
            else row.Cells[2].AddParagraph("");
            
            Paragraph par2 = new Paragraph();
            par2.AddText("Strana / Page: ");
            par2.AddPageField();
            par2.AddText(" / ");
            par2.AddNumPagesField();
            par2.AddText("\t\t\t\t\t\t\t");
            par2.AddFormattedText("Datum tisku: ");
            par2.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par2);

            // -----------------------------  FOOTERS  ---------------------------

            tbl = section.Footers.Primary.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5cm");            
            column = tbl.AddColumn("5cm");
            column = tbl.AddColumn("5cm");


            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Height = "0.5cm";

            row.Cells[0].AddParagraph("Odesláno na podpis");
            row.Cells[1].AddParagraph("Podepsáno vedoucím");
            row.Cells[2].AddParagraph("Podepsáno ředitel / controling");

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Height = "1.0cm";
            row.VerticalAlignment = VerticalAlignment.Center;

            if (pozadavek.Level1Odeslano == true) 
            {
                row.Cells[0].AddParagraph("Odesláno na schválení dne\n");
                row.Cells[0].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Level1OdeslanoDne));
            }
            else row.Cells[0].AddParagraph("");

            if ((pozadavek.Level1SchvalovatelID != null) && (pozadavek.Level1SchvalovatelID != 0))
            {
                row.Cells[1].AddParagraph($"{UserServices.GetUserById(pozadavek.Level1SchvalovatelID).Jmeno} dne");
                row.Cells[1].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Level1SchvalenoDne));
            }
            else row.Cells[1].AddParagraph("");


            if ((pozadavek.Level2SchvalovatelID != null) && (pozadavek.Level2SchvalovatelID != 0))
            {
                row.Cells[2].AddParagraph($"{UserServices.GetUserById(pozadavek.Level2SchvalovatelID).Jmeno} dne");
                row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", pozadavek.Level2SchvalenoDne));
            }
            else row.Cells[2].AddParagraph("");


            //image = row.Cells[0].AddImage(@"\\juli-app\Pozadavky\JULI_razitko.jpg");
            //image.Height = "1cm";
            ////image.Width = "4cm"               
            //image.LockAspectRatio = true;
            ////image.RelativeVertical = RelativeVertical.Line;
            ////image.RelativeHorizontal = RelativeHorizontal.Margin;
            ////image.Top = ShapePosition.Top;
            ////image.Left = ShapePosition.Left;
            //image.WrapFormat.Style = WrapStyle.Through;


            //if (obj.Schvaleno) // podpis
            //{
            //    string schvalovatel = UserServices.GetUserById(obj.SchvalovatelID).User;
            //    switch (schvalovatel)
            //    {
            //        // Smolka
            //        case "petr.smolka":
            //            image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Petr_Smolka.jpg");
            //            break;

            //        default:
            //            row.Cells[1].AddParagraph("\n\n\n\n");
            //            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            //            break;
            //    }

            //    image.Height = "1.2cm";
            //    image.LockAspectRatio = true; ;
            //    image.WrapFormat.Style = WrapStyle.Through;
            //}
            //else
            //{
            //    row.Cells[1].AddParagraph("\n\n\n\n");
            //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            //}

            //if (obj.Objednano) // podpis
            //{
            //    switch (obj.ObjednavatelID)
            //    {
            //        // Fejfusa
            //        case 9:
            //            image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Jiri_Fejfusa.jpg");
            //            image.WrapFormat.Style = WrapStyle.Through;
            //            image.Height = "1.5cm";
            //            break;

            //        // Coupkova
            //        case 19:
            //            image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Sarka_Coupkova.jpg");
            //            image.Height = "1cm";
            //            break;

            //        default:
            //            row.Cells[2].AddParagraph("\n\n\n\n");
            //            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            //            break;
            //    }

            //    image.LockAspectRatio = true;
            //    image.WrapFormat.Style = WrapStyle.Through;
            //}
            //else
            //{
            //    row.Cells[2].AddParagraph("\n\n\n\n");
            //    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            //}

            Paragraph par3 = new Paragraph();
            par3.AddText("Strana / Page: ");
            par3.AddPageField();
            par3.AddText(" / ");
            par3.AddNumPagesField();
            par3.AddText("\t\t\t\t\t\t\t");
            par3.AddFormattedText("Datum tisku: ");
            par3.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par3);
          

            
            return document;
        }

        // vytvoří Objednávkový list
        private static Document CreateObjednavkaByObj(ObjednavkaDTO obj)
        {
            Document document = new Document();
            document.DefaultPageSetup.Clone();
            document.DefaultPageSetup.BottomMargin = "4cm";
            document.DefaultPageSetup.DifferentFirstPageHeaderFooter = true;
            document.DefaultPageSetup.LeftMargin = "2cm";




            DodavateleDTO dodavatel = DodavatelService.GetDodavatelById(obj.DodavatelID);

            DefineStyles(document);

            Section section = document.AddSection();
            Paragraph p;

            Image image = section.Headers.FirstPage.AddImage(@"\\juli-app\Pozadavky\logo.gif");
            image.Height = "1cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;

            var adresa = section.AddTextFrame();
            adresa.Left = "8.7cm";
            adresa.Top = "0.4cm";
            adresa.WrapFormat.Style = WrapStyle.None;
            adresa.Width = "8cm";
            adresa.Height = "4cm";
            adresa.LineFormat.Width = Unit.FromMillimeter(0.4);
            p = adresa.AddParagraph();
            p.Format.Font.Size = 9;
            p.AddFormattedText($"\n\t{dodavatel.SNAM05}\n");
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD105) ? "" : "\n\t" + dodavatel.SAD105);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD205) ? "" : "\n\t" + dodavatel.SAD205);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD305) ? "" : "\n\t" + dodavatel.SAD305);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD405) ? "" : "\n\t" + dodavatel.SAD405);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD505) ? "" : "\n\t" + dodavatel.SAD505);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.PSC) ? "" : "\n\t" + dodavatel.PSC);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.WURL05) ? "" : "\n\n\t" + dodavatel.WURL05);

            p = section.AddParagraph();
            p.Format.Font.Size = 10;
            //p.Style = "VetsiFont";
            p.AddFormattedText($"JULI Motorenwerk, s.r.o.\tIČO: 47909765\n");
            p.AddFormattedText($"Modřická 65 \t\t\tDIČ: CZ47909765\n");
            p.AddFormattedText($"CZ - 664 48 Moravany");
            // file:///C:/inetpub/wwwroot/pozadavky/bin/Pozadavky.DLL
            // p.AddFormattedText(System.Reflection.Assembly.GetExecutingAssembly().CodeBase); 

            p = section.AddParagraph();
            p.Format.Font.Size = 8;
            //p.Style = "Normal";
            p.AddFormattedText($"\nZapsáno u KS Brno, oddíl C, vložka 46236");
            p.AddFormattedText($"\nRegistered by Commercial Court Brno, rubric C., No. 46236");
            p.AddFormattedText($"\n\n\nDodejte a fakturujte na výše uvedenou adresu", TextFormat.Bold);
            p.AddFormattedText($"\nDeliver and invoice adress - see above:", TextFormat.Bold);
            p.AddFormattedText($"\n\nPříjem zboží:\tpondělí - pátek");
            p.AddFormattedText($"\nOpen hours:\tMonday - Friday");
            p.AddFormattedText($"\n\t\t6:00am - 3:30pm");

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.AddFormattedText($"\n\nDodací list a fakturu požadujeme ve dvojím vyhotovení.", TextFormat.Bold);
            p.AddFormattedText($"\tWe require the deliver note and invoice in double", TextFormat.Bold);
            p.AddFormattedText($"\nUvádějte následující údaje:", TextFormat.Bold);
            p.AddFormattedText($"\t\t\t\tperformances. Mention following data:", TextFormat.Bold);
            p.AddFormattedText($"\nč. objednávky, datum obj., termín dodání, int. značka, id.");
            p.AddFormattedText($"\t\tnumber of order, date of order, date of delivery, intern");
            p.AddFormattedText($"\nčíslo, způsob balení");
            p.AddFormattedText($"\t\t\t\t\t\torder reference, ID number, packing");

            p = section.AddParagraph();
            p.Format.Font.Size = 5;
            //p.Style = "Font6";
            p.AddFormattedText($"\nBEZ UVEDENÍ TĚCHTO ÚDAJŮ BUDE FAKTURA VRÁCENA !!", TextFormat.Bold);
            p.AddFormattedText($"\t\t\tTHE INVOICE WITHOUT REQUIRED DATA WILL BE GIVE BACK", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 11;
            p.AddFormattedText($"\nOBJEDNÁVKA / ORDER", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.AddFormattedText($"\nObjednáváme za podmínek uvedených na druhé straně / We order according to the agreed conditions:\n\n", TextFormat.Bold);

            //------------------------------------------------------------------------------------

            Table tbl = section.AddTable();
            tbl.Style = "Table";
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;
            tbl.Format.Font.Size = 10;
            // tbl.Format.Alignment = ParagraphAlignment.Center;

            // Before you can add a row, you must define the columns
            Column column = tbl.AddColumn("3.3cm");


            //dat.obj.
            column = tbl.AddColumn("2.5cm");

            // termin
            column = tbl.AddColumn("2.1cm");

            // vystavil
            column = tbl.AddColumn("3.5cm");


            //tel.
            column = tbl.AddColumn("5.2cm");



            // Create the header of the table
            Row row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;


            row.Cells[0].AddParagraph("Číslo objednávky\nOrder-No.");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            //row.Cells[0].Format.Font.Size = 10;

            row.Cells[1].AddParagraph("Datum obj.\nDate of order");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[2].AddParagraph("Term. dod.\nTime of delivery");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            //row.Cells[2].Format.Font.Size = 8;

            row.Cells[3].AddParagraph("Vystavil\nProcessed by");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[4].AddParagraph("tel.: " + UserServices.GetUserById(obj.ObjednavatelID).Telefon ?? "");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            // ------ FILL table -------
            //------------------------------------------------------------------------------------

            items = ItemsService.GetItemsByObjId(obj.ID);

            obj.TerminDodani = items.Min(t => t.TerminDodani);

            row = tbl.AddRow();
            row.BottomPadding = "0.1cm";
            row.TopPadding = "0.1cm";
            row.Cells[0].AddParagraph(obj.FullObjednavkaID);
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[1].AddParagraph(String.Format("{0:dd.MM.yyyy}", obj.DatumObjednani));
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", obj.TerminDodani));
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[3].AddParagraph(UserServices.GetUserById(obj.ObjednavatelID).Jmeno);
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[4].AddParagraph("email: " + UserServices.GetUserById(obj.ObjednavatelID).Email ?? "");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            // ------------------- ITEMS -------------------------------------

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "1cm";
            //p.Style = "Reference";
            p.AddFormattedText($"\nPočet položek v objednávce / Number of items in the order:{items.Count()}", TextFormat.Bold);

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "0.5cm";
            p.AddText("");


            // ------- ITEMS z obj ---------

            tbl = section.AddTable();
            tbl.Style = "Table";
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            tbl.KeepTogether = false;

            // velikost 16,6 cm

            // ID
            column = tbl.AddColumn("0.6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // POPIS
            column = tbl.AddColumn("7.3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Termin
            //column = tbl.AddColumn("1.6cm");
            //column.Format.Alignment = ParagraphAlignment.Right;

            // mnozstvi
            column = tbl.AddColumn("1.0cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //jedn
            column = tbl.AddColumn("1.4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //cena
            column = tbl.AddColumn("1.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            
            // Celkova cena
            column = tbl.AddColumn("2.4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Cislo nabidky
            column = tbl.AddColumn("2.0cm");
        

            // Create the header of the table
            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Pol.\nNo.");

            row.Cells[1].AddParagraph("Název\nDescription");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            //row.Cells[2].AddParagraph("Term. dod.\nTime of delivery");
            //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[2].AddParagraph("Mn.\nQty.");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[3].AddParagraph("Jedn.mn.\nUnit qty");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[4].AddParagraph("Jedn.cena\nUnit price");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[5].AddParagraph("Celk.cena\nTotal price");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[6].AddParagraph("Číslo nabídky\nOffer number");
            row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
       

            // ------ FILL ITEMS table -------



            int count = 1;

            foreach (var item in items)
            {
                row = tbl.AddRow();
                row.BottomPadding = "0.1cm";
                row.TopPadding = "0.1cm";

                row.Cells[0].AddParagraph(count.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[1].AddParagraph(item.Popis ?? "");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                //row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", item.TerminDodani));
                //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


                row.Cells[2].AddParagraph(item.Mnozstvi.ToString());
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[3].AddParagraph(item.Jednotka);
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[4].AddParagraph(item.CenaZaJednotku.ToString("n2"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Right;


                row.Cells[5].AddParagraph(item.CelkovaCena.ToString("n2"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Right;

                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(item.NabidkaCislo ?? ""));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Right;

                count++;

            }

            p = section.AddParagraph();
            p.Format.SpaceBefore = "0.5cm";
            p.Format.Alignment = ParagraphAlignment.Right;
            p.AddFormattedText($"{obj.TextCenaText}: {obj.CelkovaCena.ToString("n2")} {obj.Mena}", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.Format.SpaceBefore = "0.5cm";
            p.AddFormattedText("Dodací podmínky / Terms of delivery:\t" + (String.IsNullOrEmpty(obj.TextDodaciPodmText) ? "" : obj.TextDodaciPodmText), TextFormat.Bold);
            p.AddFormattedText("\nPlatební podmínky / Terms of payment:\t" + (String.IsNullOrEmpty(obj.TextPlatebniPodmText) ? "" : obj.TextPlatebniPodmText), TextFormat.Bold);

            //p = section.AddParagraph();
            //p.Format.SpaceBefore = "1cm";
            p.AddFormattedText("\n\nPlease present the TARIC code in your invoice and in the deliver note.", TextFormat.Bold);
            p.AddFormattedText("\nIn the invoice declare that the goods has preferential origin of your country or EU.", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 8;
            p.Format.SpaceBefore = "0.5cm";
            p.AddFormattedText("Potvrzení objednávky zašlete na: objednavky@juli.cz", TextFormat.Bold);
            p.AddFormattedText("\nOrder confirmation send to: objednavky@juli.cz", TextFormat.Bold);

            //p = section.AddParagraph();
            //p.Format.Font.Size = 10;
            //p.Format.SpaceBefore = "0.5cm";
            //p.AddFormattedText("UPOZORNĚNÍ", TextFormat.Bold);
            //p.AddFormattedText("\nV době od 19.12.2017 do 1.1.2018 bude příjem firmy JULI Motorenwerk, s.r.o. z důvodu inventury uzavřen. Zboží nebude možné přijmout.", TextFormat.Bold);
           




            // -----------------------------  FOOTERS FIRST PAGE  ---------------------------

            if (obj.Schvaleno)
            {
                var Podpis1 = section.Footers.FirstPage.AddTextFrame();
                //Podpis1.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis1.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis1.Top = "1cm";
                Podpis1.Left = "7.33cm";
                //Podpis1.Left = "8.7cm";
                //Podpis1.Top = "0.4cm";
                Podpis1.Width = "3cm";
                Podpis1.Height = "0.6cm";
                Podpis1.WrapFormat.Style = WrapStyle.Through;
                Podpis1.LineFormat.Visible = false;
                //Podpis1.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis1.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Schvalovatel
                if (obj.SchvalovatelID != null)
                    p.AddFormattedText(UserServices.GetUserById(obj.SchvalovatelID).Jmeno.ToUpper());
            }

            if (obj.Objednano)
            {
                var Podpis2 = section.Footers.FirstPage.AddTextFrame();
                Podpis2.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis2.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis2.Top = "0.2cm";
                Podpis2.Left = "12.7cm";
                //Podpis2.Left = "8.7cm";
                //Podpis2.Top = "0.4cm";
                Podpis2.Width = "5cm";
                Podpis2.Height = "1.0cm";
                Podpis2.WrapFormat.Style = WrapStyle.Through;
                Podpis2.LineFormat.Visible = false;
                Podpis2.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis2.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Objednavatel
                if (!string.IsNullOrEmpty(obj.ObjednavatelName))
                    p.AddFormattedText(UserServices.GetNameByUserName(obj.ObjednavatelName).ToUpper());
            }



            tbl = section.Footers.FirstPage.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Razítko\nStamp");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("Podpis:\nSignature");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Podpis:\nSignature");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("");


            image = row.Cells[0].AddImage(@"\\juli-app\Pozadavky\JULI_razitko.jpg");
            image.Height = "1cm";
            //image.Width = "4cm"               
            image.LockAspectRatio = true;
            //image.RelativeVertical = RelativeVertical.Line;
            //image.RelativeHorizontal = RelativeHorizontal.Margin;
            //image.Top = ShapePosition.Top;
            //image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            if (obj.Schvaleno) // podpis
            {
                string schvalovatel = UserServices.GetUserById(obj.SchvalovatelID).User;
                switch (schvalovatel)
                {
                    // Smolka
                    case "petr.smolka":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Petr_Smolka.jpg");
                        break;

                    // Smolka
                    case "milan.drapal":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Milan_Drapal.jpg");
                        break;

                    case "pavel.rieder":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Pavel_Rieder.jpg");
                        break;

                    case "wolfgang.gebhardt":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Wolfgang_Gebhardt.jpg");
                        break;
                    default:
                        row.Cells[1].AddParagraph("\n\n\n\n");
                        row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.Height = "1.2cm";
                image.LockAspectRatio = true; ;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[1].AddParagraph("\n\n\n\n");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            }

            if (obj.Objednano) // podpis
            {
                switch (obj.ObjednavatelID)
                {
                    // Fejfusa
                    case 9:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Jiri_Fejfusa.jpg");
                        image.WrapFormat.Style = WrapStyle.Through;
                        image.Height = "1.5cm";
                        break;

                    // Coupkova
                    case 19:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Sarka_Coupkova.jpg");
                        image.Height = "1cm";
                        break;
                    
                    // Ticha
                    case 44:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Zina_Ticha.jpg");
                        image.Height = "1cm";
                        break;
                    default:
                        row.Cells[2].AddParagraph("\n\n\n\n");
                        row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.LockAspectRatio = true;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[2].AddParagraph("\n\n\n\n");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            }

            Paragraph par = new Paragraph();
            par.AddText("Strana / Page: ");
            par.AddPageField();
            par.AddText(" / ");
            par.AddNumPagesField();
            par.AddText("\t\t\t\t\t\t\t");
            par.AddFormattedText("Datum tisku: ");
            par.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par);

            // -----------------------------  FOOTERS  ---------------------------

            if (obj.Schvaleno)
            {
                var Podpis1 = section.Footers.Primary.AddTextFrame();
                //Podpis1.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis1.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis1.Top = "1cm";
                Podpis1.Left = "7.33cm";
                //Podpis1.Left = "8.7cm";
                //Podpis1.Top = "0.4cm";
                Podpis1.Width = "3cm";
                Podpis1.Height = "0.6cm";
                Podpis1.WrapFormat.Style = WrapStyle.Through;
                Podpis1.LineFormat.Visible = false;
                //Podpis1.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis1.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Schvalovatel
                if (obj.SchvalovatelID != null)
                    p.AddFormattedText(UserServices.GetUserById(obj.SchvalovatelID).Jmeno.ToUpper());
            }

            if (obj.Objednano)
            {
                var Podpis2 = section.Footers.Primary.AddTextFrame();
                Podpis2.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis2.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis2.Top = "0.2cm";
                Podpis2.Left = "12.7cm";
                //Podpis2.Left = "8.7cm";
                //Podpis2.Top = "0.4cm";
                Podpis2.Width = "5cm";
                Podpis2.Height = "1.0cm";
                Podpis2.WrapFormat.Style = WrapStyle.Through;
                Podpis2.LineFormat.Visible = false;
                Podpis2.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis2.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Objednavatel
                if (!string.IsNullOrEmpty(obj.ObjednavatelName))
                    p.AddFormattedText(UserServices.GetNameByUserName(obj.ObjednavatelName).ToUpper());
            }



            tbl = section.Footers.Primary.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Razítko\nStamp");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("Podpis:\nSignature");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Podpis:\nSignature");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("");


            image = row.Cells[0].AddImage(@"\\juli-app\Pozadavky\JULI_razitko.jpg");
            image.Height = "1cm";
            //image.Width = "4cm"               
            image.LockAspectRatio = true;
            //image.RelativeVertical = RelativeVertical.Line;
            //image.RelativeHorizontal = RelativeHorizontal.Margin;
            //image.Top = ShapePosition.Top;
            //image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            if (obj.Schvaleno) // podpis
            {
                string schvalovatel = UserServices.GetUserById(obj.SchvalovatelID).User;
                switch (schvalovatel)
                {
                    // Smolka
                    case "petr.smolka":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Petr_Smolka.jpg");
                        break;

                    default:
                        row.Cells[1].AddParagraph("\n\n\n\n");
                        row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.Height = "1.2cm";
                image.LockAspectRatio = true; ;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[1].AddParagraph("\n\n\n\n");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            }

            if (obj.Objednano) // podpis
            {
                switch (obj.ObjednavatelID)
                {
                    // Fejfusa
                    case 9:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Jiri_Fejfusa.jpg");
                        image.WrapFormat.Style = WrapStyle.Through;
                        image.Height = "1.5cm";
                        break;

                    // Coupkova
                    case 19:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Sarka_Coupkova.jpg");
                        image.Height = "1cm";
                        break;
                    
                    // Ticha
                    case 44:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Zina_Ticha.jpg");
                        image.Height = "1cm";
                        break;

                    default:
                        row.Cells[2].AddParagraph("\n\n\n\n");
                        row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.LockAspectRatio = true;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[2].AddParagraph("\n\n\n\n");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            }

            Paragraph par2 = new Paragraph();
            par2.AddText("Strana / Page: ");
            par2.AddPageField();
            par2.AddText(" / ");
            par2.AddNumPagesField();
            par2.AddText("\t\t\t\t\t\t\t");
            par2.AddFormattedText("Datum tisku: ");
            par2.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par2);

            //   -------------------------- END --------------------------
            return document;
        }

        private static Document CreatePotvrzeniByObj(ObjednavkaDTO obj)
        {
            Document document = new Document();
            document.DefaultPageSetup.Clone();
            document.DefaultPageSetup.BottomMargin = "4cm";
            document.DefaultPageSetup.DifferentFirstPageHeaderFooter = true;
            document.DefaultPageSetup.LeftMargin = "2cm";

            DodavateleDTO dodavatel = DodavatelService.GetDodavatelById(obj.DodavatelID);

            // You always need a MigraDoc document for rendering.

            DefineStyles(document);

            Section section = document.AddSection();
            Paragraph p;

            Image image = section.Headers.FirstPage.AddImage(@"\\juli-app\Pozadavky\logo.gif");
            image.Height = "1cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;

            var adresa = section.AddTextFrame();
            adresa.Left = "8.7cm";
            adresa.Top = "0.4cm";
            adresa.WrapFormat.Style = WrapStyle.None;
            adresa.Width = "8cm";
            adresa.Height = "4cm";
            adresa.LineFormat.Width = Unit.FromMillimeter(0.4);
            p = adresa.AddParagraph();
            p.Format.Font.Size = 9;
            p.AddFormattedText($"\n\t{dodavatel.SNAM05}\n");
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD105) ? "" : "\n\t" + dodavatel.SAD105);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD205) ? "" : "\n\t" + dodavatel.SAD205);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD305) ? "" : "\n\t" + dodavatel.SAD305);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD405) ? "" : "\n\t" + dodavatel.SAD405);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD505) ? "" : "\n\t" + dodavatel.SAD505);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.PSC) ? "" : "\n\t" + dodavatel.PSC);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.WURL05) ? "" : "\n\n\t" + dodavatel.WURL05);

            p = section.AddParagraph();
            p.Format.Font.Size = 10;
            //p.Style = "VetsiFont";
            p.AddFormattedText($"JULI Motorenwerk, s.r.o.\tIČO: 47909765\n");
            p.AddFormattedText($"Modřická 65 \t\t\tDIČ: CZ47909765\n");
            p.AddFormattedText($"CZ - 664 48 Moravany");
            // file:///C:/inetpub/wwwroot/pozadavky/bin/Pozadavky.DLL
            // p.AddFormattedText(System.Reflection.Assembly.GetExecutingAssembly().CodeBase); 

            p = section.AddParagraph();
            p.Format.Font.Size = 8;
            //p.Style = "Normal";
            p.AddFormattedText($"\nZapsáno u KS Brno, oddíl C, vložka 46236");
            p.AddFormattedText($"\nRegistered by Commercial Court Brno, rubric C., No. 46236");
            p.AddFormattedText($"\n\n\nDodejte a fakturujte na výše uvedenou adresu", TextFormat.Bold);
            p.AddFormattedText($"\nDeliver and invoice adress - see above:", TextFormat.Bold);
            p.AddFormattedText($"\n\nPříjem zboží:\tpondělí - pátek");
            p.AddFormattedText($"\nOpen hours:\tMonday - Friday");
            p.AddFormattedText($"\n\t\t6:00am - 3:30pm");
            
            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.AddFormattedText($"\n\nDodací list a fakturu požadujeme ve dvojím vyhotovení.", TextFormat.Bold);
            p.AddFormattedText($"\tWe require the deliver note and invoice in double", TextFormat.Bold);
            p.AddFormattedText($"\nUvádějte následující údaje:", TextFormat.Bold);
            p.AddFormattedText($"\t\t\t\tperformances. Mention following data:", TextFormat.Bold);
            p.AddFormattedText($"\nč. objednávky, datum obj., termín dodání, int. značka, id.");
            p.AddFormattedText($"\t\tnumber of order, date of order, date of delivery, intern");
            p.AddFormattedText($"\nčíslo, způsob balení");
            p.AddFormattedText($"\t\t\t\t\t\torder reference, ID number, packing");

            p = section.AddParagraph();
            p.Format.Font.Size = 5;
            //p.Style = "Font6";
            p.AddFormattedText($"\nBEZ UVEDENÍ TĚCHTO ÚDAJŮ BUDE FAKTURA VRÁCENA !!", TextFormat.Bold);
            p.AddFormattedText($"\t\t\tTHE INVOICE WITHOUT REQUIRED DATA WILL BE GIVE BACK", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 11;
            p.AddFormattedText($"\nPOTVRZENÍ OBJEDNÁVKY / ORDER CONFIRMATION", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.AddFormattedText($"\nPřijímáme objednávku za nákupních podmínek uvedených na druhé straně:\nWe accept your order according to the agreed conditions:\n\n", TextFormat.Bold);

            //------------------------------------------------------------------------------------

            Table tbl = section.AddTable();
            tbl.Style = "Table";
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;
            tbl.Format.Font.Size = 10;
            // tbl.Format.Alignment = ParagraphAlignment.Center;

            // Before you can add a row, you must define the columns
            Column column = tbl.AddColumn("3.3cm");


            //dat.obj.
            column = tbl.AddColumn("2.5cm");

            // termin
            column = tbl.AddColumn("2.1cm");

            // vystavil
            column = tbl.AddColumn("3.5cm");


            //tel.
            column = tbl.AddColumn("5.2cm");



            // Create the header of the table
            Row row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;


            row.Cells[0].AddParagraph("Číslo objednávky\nOrder-No.");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            //row.Cells[0].Format.Font.Size = 10;

            row.Cells[1].AddParagraph("Datum obj.\nDate of order");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[2].AddParagraph("Term. dod.\nTime of delivery");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            //row.Cells[2].Format.Font.Size = 8;

            row.Cells[3].AddParagraph("Vystavil\nProcessed by");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[4].AddParagraph("tel.: " + UserServices.GetUserById(obj.ObjednavatelID).Telefon ?? "");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            // ------ FILL table -------
            //------------------------------------------------------------------------------------


            items = ItemsService.GetItemsByObjId(obj.ID);

            obj.TerminDodani = items.Min(t => t.TerminDodani);

            row = tbl.AddRow();
            row.BottomPadding = "0.1cm";
            row.TopPadding = "0.1cm";
            row.Cells[0].AddParagraph(obj.FullObjednavkaID);
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[1].AddParagraph(String.Format("{0:dd.MM.yyyy}", obj.DatumObjednani));
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", obj.TerminDodani));
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[3].AddParagraph(UserServices.GetUserById(obj.ObjednavatelID).Jmeno);
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[4].AddParagraph("email: " + UserServices.GetUserById(obj.ObjednavatelID).Email ?? "");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            // ------------------- ITEMS -------------------------------------

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "1cm";
            //p.Style = "Reference";
            p.AddFormattedText($"\nPočet položek v objednávce / Number of items in the order: {items.Count()}", TextFormat.Bold);

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "0.5cm";
            p.AddText("");


            // ------- ITEMS z obj ---------

            tbl = section.AddTable();
            tbl.Style = "Table";
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns

            // ID
            column = tbl.AddColumn("0.6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // POPIS
            column = tbl.AddColumn("7.3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Termin
            //column = tbl.AddColumn("1.6cm");
            //column.Format.Alignment = ParagraphAlignment.Right;

            // mnozstvi
            column = tbl.AddColumn("1.0cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //jedn
            column = tbl.AddColumn("1.4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //cena
            column = tbl.AddColumn("1.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Celkova cena
            column = tbl.AddColumn("2.4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Cislo nabidky
            column = tbl.AddColumn("2.0cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Pol.\nNo.");

            row.Cells[1].AddParagraph("Název\nDescription");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            //row.Cells[2].AddParagraph("Term. dod.\nTime of delivery");
            //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[2].AddParagraph("Mn.\nQty.");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[3].AddParagraph("Jedn.mn.\nUnit qty");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[4].AddParagraph("Jedn.cena\nUnit price");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[5].AddParagraph("Celk.cena\nTotal price");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[6].AddParagraph("Číslo nabídky\nOffer number");
            row.Cells[6].Format.Alignment = ParagraphAlignment.Center;


            // ------ FILL ITEMS table -------

            int count = 1;

            foreach (var item in items)
            {
                row = tbl.AddRow();
                row.BottomPadding = "0.1cm";
                row.TopPadding = "0.1cm";

                row.Cells[0].AddParagraph(count.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[1].AddParagraph(item.Popis ?? "");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                //row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", item.TerminDodani));
                //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


                row.Cells[2].AddParagraph(item.Mnozstvi.ToString());
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[3].AddParagraph(item.Jednotka);
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[4].AddParagraph(item.CenaZaJednotku.ToString("n2"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Right;


                row.Cells[5].AddParagraph(item.CelkovaCena.ToString("n2"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Right;

                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(item.NabidkaCislo ?? ""));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Right;

                count++;

            }

            p = section.AddParagraph();
            p.Format.SpaceBefore = "0.5cm";
            p.Format.Alignment = ParagraphAlignment.Right;
            p.AddFormattedText($"{obj.TextCenaText}: {obj.CelkovaCena.ToString("n2")} {obj.Mena}", TextFormat.Bold);



            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.Format.SpaceBefore = "0.5cm";
            p.AddFormattedText("Dodací podmínky / Terms of delivery:\t" + (String.IsNullOrEmpty(obj.TextDodaciPodmText) ? "" : obj.TextDodaciPodmText), TextFormat.Bold);
            p.AddFormattedText("\nPlatební podmínky / Terms of payment:\t" + (String.IsNullOrEmpty(obj.TextPlatebniPodmText) ? "" : obj.TextPlatebniPodmText), TextFormat.Bold);
            p.AddFormattedText("\n\nPlease present the TARIC code in your invoice and in the deliver note.", TextFormat.Bold);
            p.AddFormattedText("\nIn the invoice declare that the goods has preferential origin of your country or EU.", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 8;
            p.Format.SpaceBefore = "0.5cm";
            p.AddFormattedText("Potvrzení objednávky zašlete na: objednavky@juli.cz", TextFormat.Bold);
            p.AddFormattedText("\nOrder confirmation send to: objednavky@juli.cz", TextFormat.Bold);

            // -----------------------------  FOOTERS FIRST PAGE  ---------------------------

            tbl = section.Footers.FirstPage.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Razítko dodavatele\nSupplier's stamp");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("Podpis dodavatele\nSupplier's signature");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Podpis dodavatele\nSupplier's signature");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("\n\n\n\n");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("\n\n\n\n");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("\n\n\n\n");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            Paragraph par = new Paragraph();
            par.AddText("Strana / Page: ");
            par.AddPageField();
            par.AddText(" / ");
            par.AddNumPagesField();
            par.AddText("\t\t\t\t\t\t\t");
            par.AddFormattedText("Datum tisku: ");
            par.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par);

            // -----------------------------  FOOTERS PRIMARY PAGE  ---------------------------

            tbl = section.Footers.Primary.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5.53cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Razítko dodavatele\nSupplier's stamp");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("Podpis dodavatele\nSupplier's signature");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Podpis dodavatele\nSupplier's signature");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("\n\n\n\n");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("\n\n\n\n");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("\n\n\n\n");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            Paragraph par2 = new Paragraph();
            par2.AddText("Strana / Page: ");
            par2.AddPageField();
            par2.AddText(" / ");
            par2.AddNumPagesField();
            par2.AddText("\t\t\t\t\t\t\t");
            par2.AddFormattedText("Datum tisku: ");
            par2.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par2);

            return document;
        }

        private static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            style.Font.Size = 7;

            style = document.Styles.AddStyle("VetsiFont", "Normal");
            style.Font.Size = 9;

            style = document.Styles.AddStyle("Font6", "Normal");
            style.Font.Size = 5;

            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.


            //style = document.Styles[StyleNames.Header];
            //style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            //style = document.Styles[StyleNames.Footer];
            //style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 8;

            // Create a new style called Reference based on style Normal

            //style.ParagraphFormat.SpaceBefore = "5mm";
            //style.ParagraphFormat.SpaceAfter = "2mm";
            //style.ParagraphFormat.TabStops.AddTabStop("1cm", TabAlignment.Right);
        }

        public static int SaveStreamToPdf(MemoryStream stream, int pozadavekId, string popis = "")
        {           
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "P" + DateTime.Now.Year + pozadavekId.ToString();

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);
            string fileName = popis + "_" + "tisk"  + ".pdf";

            int id = FilesService.SaveFileGetId(stream, fullPath, fileName);

            return id;       
        }   

        public static int SaveDocToPdf(Document doc, int pozadavekId, string popis = "")
        {
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "P" + DateTime.Now.Year + pozadavekId.ToString();

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);
            string fileName = popis + "_" + "tisk" + ".pdf";

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always);

            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();


            pdfRenderer.PdfDocument.Save(fileName);

            // TODO: ulozit do db.Files odkaz na soubor a vratit ID

            int id = 1;
            return id;
        }

        public static void CreatePdf()
        {
            // Create new page
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            // Drawing is done with an XGraphics object:
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter xt = new XTextFormatter(gfx);

            // Then you'll create a font:
            var font = new XFont("Verdana", 20, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always));

            // And you use that font to draw a string:
            gfx.DrawString("Příliš žluťoučký kůň úpěl ďábelské ódy", font, XBrushes.Black,
            new XRect(0, 0, page.Width, 100),
            XStringFormats.Center);

            xt.DrawString("Příliš žluťoučký kůň úpěl ďábelské ódy", font, XBrushes.Black,
            new XRect(0, 100, page.Width, 100),
            XStringFormats.TopLeft);

            // Send PDF to browser
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);

            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "P" + DateTime.Now.Year + "000"; ;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            int id = FilesService.SaveFileGetId(stream, fullPath, "temp.pdf");

            //    Context.RedirectToRoute("FileDownloadPDF", new { Id = id });

        }

        public static void CreatePdf_old()
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            // Drawing is done with an XGraphics object:
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter xt = new XTextFormatter(gfx);

            // Then you'll create a font:
            // XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            var font = new XFont("Verdana", 20, XFontStyle.Bold, new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always));

            // And you use that font to draw a string:

            gfx.DrawString("Příliš žluťoučký kůň úpěl ďábelské ódy", font, XBrushes.Black,
            new XRect(0, 0, page.Width, 100),
            XStringFormats.Center);

            xt.DrawString("Příliš žluťoučký kůň úpěl ďábelské ódy", font, XBrushes.Black,
            new XRect(0, 100, page.Width, 100),
            XStringFormats.TopLeft);


            // string path = HttpContext.Server.MapPath("~/App_Data/somedata.pdf");

            //When drawing is done, write the file:            
            string filename = @"HelloWorld.pdf";
            document.Save(filename);
            //Process.Start(filename);
        }

        private static Document CreateDocumentByObjTest(ObjednavkaDTO obj)
        {
            Document document = new Document();
            document.DefaultPageSetup.Clone();
            document.DefaultPageSetup.BottomMargin = "6cm";
            document.DefaultPageSetup.DifferentFirstPageHeaderFooter = true;



            DodavateleDTO dodavatel = DodavatelService.GetDodavatelById(obj.DodavatelID);

            DefineStyles(document);

            Section section = document.AddSection();
            Paragraph p;

            Image image = section.Headers.FirstPage.AddImage(@"\\juli-app\Pozadavky\logo.gif");
            image.Height = "1cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;

            var adresa = section.AddTextFrame();
            adresa.Left = "8.7cm";
            adresa.Top = "0.4cm";
            adresa.WrapFormat.Style = WrapStyle.None;
            adresa.Width = "8cm";
            adresa.Height = "4cm";
            adresa.LineFormat.Width = Unit.FromMillimeter(0.4);
            p = adresa.AddParagraph();
            p.Format.Font.Size = 9;
            p.AddFormattedText($"\n\t{dodavatel.SNAM05}\n");
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD105) ? "" : "\n\t" + dodavatel.SAD105);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD205) ? "" : "\n\t" + dodavatel.SAD205);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD305) ? "" : "\n\t" + dodavatel.SAD305);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD405) ? "" : "\n\t" + dodavatel.SAD405);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.SAD505) ? "" : "\n\t" + dodavatel.SAD505);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.PSC) ? "" : "\n\t" + dodavatel.PSC);
            p.AddFormattedText(String.IsNullOrEmpty(dodavatel.WURL05) ? "" : "\n\n\t" + dodavatel.WURL05);

            p = section.AddParagraph();
            p.Format.Font.Size = 10;
            //p.Style = "VetsiFont";
            p.AddFormattedText($"JULI Motorenwerk, s.r.o.\tIČO: 47909765\n");
            p.AddFormattedText($"Modřická 65 \t\t\tDIČ: CZ47909765\n");
            p.AddFormattedText($"CZ - 664 48 Moravany");
            // file:///C:/inetpub/wwwroot/pozadavky/bin/Pozadavky.DLL
            // p.AddFormattedText(System.Reflection.Assembly.GetExecutingAssembly().CodeBase); 

            p = section.AddParagraph();
            p.Format.Font.Size = 8;
            //p.Style = "Normal";
            p.AddFormattedText($"\nZapsáno u KS Brno, oddíl C, vložka 46236");
            p.AddFormattedText($"\nRegistered by Commercial Court Brno, rubric C., No. 46236");
            p.AddFormattedText($"\n\n\nDodejte a fakturujte na výše uvedenou adresu", TextFormat.Bold);
            p.AddFormattedText($"\nDeliver and invoice adress - see above:", TextFormat.Bold);
            p.AddFormattedText($"\n\nPříjem zboží:\tpondělí - pátek");
            p.AddFormattedText($"\nOpen hours:\tMonday - Friday");
            p.AddFormattedText($"\n\t\t6:00am - 3:30pm");

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.AddFormattedText($"\n\nDodací list a fakturu požadujeme ve dvojím vyhotovení.", TextFormat.Bold);
            p.AddFormattedText($"\tWe require the deliver note and invoice in double", TextFormat.Bold);
            p.AddFormattedText($"\nUvádějte následující údaje:", TextFormat.Bold);
            p.AddFormattedText($"\t\t\t\tperformances. Mention following data:", TextFormat.Bold);
            p.AddFormattedText($"\nč. objednávky, datum obj., termín dodání, int. značka, id.");
            p.AddFormattedText($"\t\tnumber of order, date of order, date of delivery, intern");
            p.AddFormattedText($"\nčíslo, způsob balení");
            p.AddFormattedText($"\t\t\t\t\t\torder reference, ID number, packing");

            p = section.AddParagraph();
            p.Format.Font.Size = 5;
            //p.Style = "Font6";
            p.AddFormattedText($"\nBEZ UVEDENÍ TĚCHTO ÚDAJŮ BUDE FAKTURA VRÁCENA !!", TextFormat.Bold);
            p.AddFormattedText($"\t\t\tTHE INVOICE WITHOUT REQUIRED DATA WILL BE GIVE BACK", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 11;
            p.AddFormattedText($"\nOBJEDNÁVKA / ORDER", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.AddFormattedText($"\nObjednáváme za podmínek uvedených na druhé straně / We order according to the agreed conditions:\n\n", TextFormat.Bold);

            //------------------------------------------------------------------------------------

            Table tbl = section.AddTable();
            tbl.Style = "Table";
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;
            tbl.Format.Font.Size = 10;
            // tbl.Format.Alignment = ParagraphAlignment.Center;

            // Before you can add a row, you must define the columns
            Column column = tbl.AddColumn("3.5cm");


            //dat.obj.
            column = tbl.AddColumn("2.5cm");


            //column = tbl.AddColumn("2.0cm");
            //column.Format.Alignment = ParagraphAlignment.Right;

            // vystavil
            column = tbl.AddColumn("3.5cm");


            //tel.
            column = tbl.AddColumn("5cm");



            // Create the header of the table
            Row row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;


            row.Cells[0].AddParagraph("Číslo objednávky\nOrder-No.");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            //row.Cells[0].Format.Font.Size = 10;

            row.Cells[1].AddParagraph("Datum obj.\nDate of order");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            //row.Cells[2].AddParagraph("termín dodání\nDate of delivery");
            //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Vystavil\nProcessed by");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[3].AddParagraph("tel.: " + UserServices.GetUserById(obj.ObjednavatelID).Telefon ?? "");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;


            // ------ FILL table -------
            //------------------------------------------------------------------------------------


            row = tbl.AddRow();
            row.BottomPadding = "0.1cm";
            row.TopPadding = "0.1cm";
            row.Cells[0].AddParagraph(obj.FullObjednavkaID);
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[1].AddParagraph(String.Format("{0:dd.MM.yyyy}", obj.DatumObjednani));
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            //row.Cells[2].AddParagraph(obj.TerminDodani.ToString());
            //row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph(UserServices.GetUserById(obj.ObjednavatelID).Jmeno);
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[3].AddParagraph("email: " + UserServices.GetUserById(obj.ObjednavatelID).Email ?? "");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;


            // ------------------- ITEMS -------------------------------------

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "1cm";
            //p.Style = "Reference";
            p.AddFormattedText($"\nPočet položek v objednávce / Number of items in the order: {obj.PocetPolozek}", TextFormat.Bold);

            p = section.AddParagraph();
            //p.Format.SpaceBefore = "0.5cm";
            p.AddText("");


            // ------- ITEMS z obj ---------

            tbl = section.AddTable();
            tbl.Style = "Table";
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            tbl.KeepTogether = false;

            // Before you can add a row, you must define the columns

            // ID
            column = tbl.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // POPIS
            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Termin
            column = tbl.AddColumn("1.8cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // mnozstvi
            column = tbl.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //jedn
            column = tbl.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //cena
            column = tbl.AddColumn("2.0cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Celkova cena
            column = tbl.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Cislo nabidky
            column = tbl.AddColumn("2.0cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Pol.\nNo.");

            row.Cells[1].AddParagraph("Název\nDescription");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Term. dod.\nTime of delivery");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[3].AddParagraph("Množství\nQuantity");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[4].AddParagraph("Jedn. mn.\nUnit quantity");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[5].AddParagraph("Jedn. cena\nPrice per unit");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;


            row.Cells[6].AddParagraph("Celk. cena\nTotal price");
            row.Cells[6].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[7].AddParagraph("Číslo nabídky\nOffer number");
            row.Cells[7].Format.Alignment = ParagraphAlignment.Center;

            // ------ FILL ITEMS table -------

            items = ItemsService.GetItemsByObjId(obj.ID);

            int count = 1;

            foreach (var item in items)
            {
                row = tbl.AddRow();
                row.BottomPadding = "0.1cm";
                row.TopPadding = "0.1cm";

                row.Cells[0].AddParagraph(count.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[1].AddParagraph(item.Popis ?? "");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[2].AddParagraph(String.Format("{0:dd.MM.yyyy}", item.TerminDodani));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;


                row.Cells[3].AddParagraph(item.Mnozstvi.ToString());
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[4].AddParagraph(item.Jednotka);
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

                row.Cells[5].AddParagraph(item.CenaZaJednotku.ToString("n2"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Right;


                row.Cells[6].AddParagraph(item.CelkovaCena.ToString("n2"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Right;

                row.Cells[7].AddParagraph(item.NabidkaCislo ?? "");
                row.Cells[7].Format.Alignment = ParagraphAlignment.Right;

                count++;

            }

            p = section.AddParagraph();
            p.Format.SpaceBefore = "0.5cm";
            p.Format.Alignment = ParagraphAlignment.Right;
            p.AddFormattedText($"{obj.TextCenaText}: {obj.CelkovaCena.ToString("n2")} {obj.Mena}", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 7;
            p.Format.SpaceBefore = "0.5cm";
            p.AddFormattedText("Dodací podmínky / Terms of delivery:\t" + (String.IsNullOrEmpty(obj.TextDodaciPodmText) ? "" : obj.TextDodaciPodmText), TextFormat.Bold);
            p.AddFormattedText("\nPlatební podmínky / Terms of payment:\t" + (String.IsNullOrEmpty(obj.TextPlatebniPodmText) ? "" : obj.TextPlatebniPodmText), TextFormat.Bold);
            p.AddFormattedText("\n\nPlease present the TARIC code in your invoice and in the deliver note.", TextFormat.Bold);
            p.AddFormattedText("\nIn the invoice declare that the goods has preferential origin of your country or EU.", TextFormat.Bold);

            p = section.AddParagraph();
            p.Format.Font.Size = 9;
            p.Format.SpaceBefore = "1.0cm";
            p.AddFormattedText("Potvrzení objednávky zašlete na: objednavky@juli.cz", TextFormat.Bold);
            p.AddFormattedText("\nOrder confirmation send to: objednavky@juli.cz", TextFormat.Bold);

            // -----------------------------  FOOTERS FIRST PAGE  ---------------------------

            if (obj.Schvaleno)
            {
                var Podpis1 = section.Footers.FirstPage.AddTextFrame();
                //Podpis1.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis1.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis1.Top = "1cm";
                Podpis1.Left = "6.8cm";
                //Podpis1.Left = "8.7cm";
                //Podpis1.Top = "0.4cm";
                Podpis1.Width = "3cm";
                Podpis1.Height = "0.6cm";
                Podpis1.WrapFormat.Style = WrapStyle.Through;
                Podpis1.LineFormat.Visible = false;
                //Podpis1.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis1.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Schvalovatel
                if (obj.SchvalovatelID != null)
                    p.AddFormattedText(UserServices.GetUserById(obj.SchvalovatelID).Jmeno.ToUpper());
            }

            if (obj.Objednano)
            {
                var Podpis2 = section.Footers.FirstPage.AddTextFrame();
                Podpis2.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis2.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis2.Top = "0.2cm";
                Podpis2.Left = "11.8cm";
                //Podpis2.Left = "8.7cm";
                //Podpis2.Top = "0.4cm";
                Podpis2.Width = "5cm";
                Podpis2.Height = "1.0cm";
                Podpis2.WrapFormat.Style = WrapStyle.Through;
                Podpis2.LineFormat.Visible = false;
                Podpis2.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis2.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Objednavatel
                if (!string.IsNullOrEmpty(obj.ObjednavatelName))
                    p.AddFormattedText(UserServices.GetNameByUserName(obj.ObjednavatelName).ToUpper());
            }



            tbl = section.Footers.FirstPage.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Razítko\nStamp");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("Podpis:\nSignature");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Podpis:\nSignature");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("");


            image = row.Cells[0].AddImage(@"\\juli-app\Pozadavky\JULI_razitko.jpg");
            image.Height = "1cm";
            //image.Width = "4cm"               
            image.LockAspectRatio = true;
            //image.RelativeVertical = RelativeVertical.Line;
            //image.RelativeHorizontal = RelativeHorizontal.Margin;
            //image.Top = ShapePosition.Top;
            //image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            if (obj.Schvaleno) // podpis
            {
                string schvalovatel = UserServices.GetUserById(obj.SchvalovatelID).User;
                switch (schvalovatel)
                {
                    // Smolka
                    case "petr.smolka":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Petr_Smolka.jpg");
                        break;

                    default:
                        row.Cells[1].AddParagraph("\n\n\n\n");
                        row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.Height = "1.2cm";
                image.LockAspectRatio = true; ;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[1].AddParagraph("\n\n\n\n");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            }

            if (obj.Objednano) // podpis
            {
                switch (obj.ObjednavatelID)
                {
                    // Fejfusa
                    case 9:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Jiri_Fejfusa.jpg");
                        image.WrapFormat.Style = WrapStyle.Through;
                        image.Height = "1.5cm";
                        break;

                    // Coupkova
                    case 19:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Sarka_Coupkova.jpg");
                        image.Height = "1cm";
                        break;

                    // Ticha
                    case 44:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Zina_Ticha.jpg");
                        image.Height = "1cm";
                        break;

                    default:
                        row.Cells[2].AddParagraph("\n\n\n\n");
                        row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.LockAspectRatio = true;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[2].AddParagraph("\n\n\n\n");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            }

            Paragraph par = new Paragraph();
            par.AddText("Strana / Page: ");
            par.AddPageField();
            par.AddText(" / ");
            par.AddNumPagesField();
            par.AddText("\t\t\t\t\t\t\t");
            par.AddFormattedText("Datum tisku: ");
            par.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par);

            // -----------------------------  FOOTERS  ---------------------------

            if (obj.Schvaleno)
            {
                var Podpis1 = section.Footers.Primary.AddTextFrame();
                //Podpis1.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis1.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis1.Top = "1cm";
                Podpis1.Left = "6.8cm";
                //Podpis1.Left = "8.7cm";
                //Podpis1.Top = "0.4cm";
                Podpis1.Width = "3cm";
                Podpis1.Height = "0.6cm";
                Podpis1.WrapFormat.Style = WrapStyle.Through;
                Podpis1.LineFormat.Visible = false;
                //Podpis1.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis1.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Schvalovatel
                if (obj.SchvalovatelID != null)
                    p.AddFormattedText(UserServices.GetUserById(obj.SchvalovatelID).Jmeno.ToUpper());
            }

            if (obj.Objednano)
            {
                var Podpis2 = section.Footers.Primary.AddTextFrame();
                Podpis2.RelativeVertical = RelativeVertical.Paragraph;
                //Podpis2.RelativeHorizontal = RelativeHorizontal.Margin;
                Podpis2.Top = "0.2cm";
                Podpis2.Left = "11.8cm";
                //Podpis2.Left = "8.7cm";
                //Podpis2.Top = "0.4cm";
                Podpis2.Width = "5cm";
                Podpis2.Height = "1.0cm";
                Podpis2.WrapFormat.Style = WrapStyle.Through;
                Podpis2.LineFormat.Visible = false;
                Podpis2.LineFormat.Width = Unit.FromMillimeter(0.1);
                p = Podpis2.AddParagraph();
                p.Format.SpaceBefore = "0.1cm";
                p.Format.Font.Size = 9;
                // Objednavatel
                if (!string.IsNullOrEmpty(obj.ObjednavatelName))
                    p.AddFormattedText(UserServices.GetNameByUserName(obj.ObjednavatelName).ToUpper());
            }



            tbl = section.Footers.Primary.AddTable();
            tbl.Borders.Width = 0.25;
            tbl.Borders.Left.Width = 0.5;
            tbl.Borders.Right.Width = 0.5;
            tbl.Rows.LeftIndent = 0;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = tbl.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Razítko\nStamp");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("Podpis:\nSignature");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[2].AddParagraph("Podpis:\nSignature");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            row = tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("");


            image = row.Cells[0].AddImage(@"\\juli-app\Pozadavky\JULI_razitko.jpg");
            image.Height = "1cm";
            //image.Width = "4cm"               
            image.LockAspectRatio = true;
            //image.RelativeVertical = RelativeVertical.Line;
            //image.RelativeHorizontal = RelativeHorizontal.Margin;
            //image.Top = ShapePosition.Top;
            //image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            if (obj.Schvaleno) // podpis
            {
                string schvalovatel = UserServices.GetUserById(obj.SchvalovatelID).User;
                switch (schvalovatel)
                {
                    // Smolka
                    case "petr.smolka":
                        image = row.Cells[1].AddImage(@"\\juli-app\Pozadavky\podpis_Petr_Smolka.jpg");
                        break;

                    default:
                        row.Cells[1].AddParagraph("\n\n\n\n");
                        row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.Height = "1.2cm";
                image.LockAspectRatio = true; ;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[1].AddParagraph("\n\n\n\n");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            }

            if (obj.Objednano) // podpis
            {
                switch (obj.ObjednavatelID)
                {
                    // Fejfusa
                    case 9:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Jiri_Fejfusa.jpg");
                        image.WrapFormat.Style = WrapStyle.Through;
                        image.Height = "1.5cm";
                        break;

                    // Coupkova
                    case 19:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Sarka_Coupkova.jpg");
                        image.Height = "1cm";
                        break;

                    // Ticha
                    case 44:
                        image = row.Cells[2].AddImage(@"\\juli-app\Pozadavky\podpis_Zina_Ticha.jpg");
                        image.Height = "1cm";
                        break;

                    default:
                        row.Cells[2].AddParagraph("\n\n\n\n");
                        row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                        break;
                }

                image.LockAspectRatio = true;
                image.WrapFormat.Style = WrapStyle.Through;
            }
            else
            {
                row.Cells[2].AddParagraph("\n\n\n\n");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            }

            Paragraph par2 = new Paragraph();
            par2.AddText("Strana / Page: ");
            par2.AddPageField();
            par2.AddText(" / ");
            par2.AddNumPagesField();
            par2.AddText("\t\t\t\t\t\t\t");
            par2.AddFormattedText("Datum tisku: ");
            par2.AddDateField("dd.MM.yyyy");


            row = tbl.AddRow();
            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Left.Visible = false;
            row.Cells[0].Borders.Bottom.Visible = false;
            row.Cells[0].Borders.Right.Visible = false;
            row.Cells[2].Borders.Right.Visible = false;
            row.Cells[0].Add(par2);

            //   -------------------------- END --------------------------
            return document;
        }

        private static string AdjustIfTooWideToFitIn(string text)
        {
           
           
            var tooWideWords = text.Split(" ".ToCharArray()).Distinct().Where(s => TooWide(s));

            var adjusted = new StringBuilder(text);
            foreach (string word in tooWideWords)
            {
                var replacementWord = MakeFit(word);
                adjusted.Replace(word, replacementWord);
            }

            return adjusted.ToString();
        }

        private static bool TooWide(string word)
        {            
            return word.Length > 10;
        }

        /// <summary>
        /// Makes the supplied word fit into the available width
        /// </summary>
        /// <returns>modified version of the word with inserted Returns at appropriate points</returns>
        private static string MakeFit(string word)
        {
            var adjustedWord = new StringBuilder();
            var current = string.Empty;
            foreach (char c in word)
            {
                if (TooWide(current + c))
                {
                    adjustedWord.Append(current);
                    adjustedWord.Append(Chars.CR);
                    current = c.ToString();
                }
                else
                {
                    current += c;
                }
            }
            adjustedWord.Append(current);

            return adjustedWord.ToString();
        }

        public class Sumarizace
        {
            public string CisloKonta { get; set; }
            public string CisloInvestice { get; set; }
            public string KST { get; set; }
            public double suma { get; set; }


        }

        public static List<Sumarizace> SumarizaceInit(int id)
        {
            var list = new List<Sumarizace>();

            string sqlquery = "select CisloKonta, CisloInvestice, KST, sum(CelkovaCena) as suma from Items " +
                "Where Smazano = 0 and PozadavekID = @id " +
                "group by CisloKonta, CisloInvestice, KST";

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var parameters = new SqlParameter("@id", id);



                var query = db.Database.SqlQuery<Sumarizace>(sqlquery, parameters);

                return query.ToList();
            }
        }


        public static void PrintFile(string fileToPrint)
        {
            Process printjob = new Process();

            printjob.StartInfo.FileName = fileToPrint;  //path of your file;

            printjob.StartInfo.Verb = "Print";

            printjob.StartInfo.CreateNoWindow = true;

            printjob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            PrinterSettings setting = new PrinterSettings();

            setting.DefaultPageSettings.Landscape = true;

            printjob.Start();
        }

    }
}
