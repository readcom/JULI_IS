using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Pozadavky.Services
{
    public static class MailServices
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        public static string SendMail(string prijemci, string subject, string text, List<string> prilohyPath = null, string odkud = "")
        {
            string sMessage;
            try
            {
                SmtpClient smtpClient = new SmtpClient();

                //you can provide invalid from address. but to address Should be valid
                using (var message = new MailMessage())
                {

                    MailAddress fromAddress = null;

                    if (odkud ==  "objednavky")
                    {
                        fromAddress = new MailAddress("objednavky@juli.cz", "JULI Objednávky");
                        smtpClient.Host = "JULI-MAIL.julidomain.local";
                    }
                    else
                    {
                        fromAddress = new MailAddress("juli.robot@juli.cz", "JULI Robot");
                        smtpClient.Host = "JULI-MAIL.julidomain.local";
                    }
                    
                    smtpClient.Port = 465;
                    //smtpClient.Port = 587;


                    smtpClient.UseDefaultCredentials = true;

                    message.From = fromAddress;
                    message.Subject = subject;
                    message.Body = text;
                    message.IsBodyHtml = true;

                    if (Constants.TestEmaily)
                    {
                        if (prijemci.Trim().LastOrDefault() != ';')
                            prijemci += ";";
                        prijemci += " marek.novak@juli.cz;";
                    }
                    foreach (var email in prijemci.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(email);
                    }

                    if (prilohyPath != null && prilohyPath.Count > 0)
                    {
                        foreach (var priloha in prilohyPath)
                        {
                            message.Attachments.Add(new Attachment(priloha));
                        }                        
                    }
                    

                    //smtpClient.EnableSsl = true; 

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtpClient.Send(message);

                    sMessage = "Email byl odeslán. ";
                }
            }
            catch (Exception ex)
            {
                sMessage = "Email nelze odeslat!\n ";
                sMessage += ex.Message;

                if (ex.InnerException != null)
                     sMessage += ex.InnerException.Message;
            }

            return sMessage;
        }


    }
}
