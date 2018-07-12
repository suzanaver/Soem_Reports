using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Configuration;
using System.IO;
using System.Data;
using System.Net.Mail;

namespace SoemReports
{
    public class Notification
    {
        public Notification()
        {
            Monitor monitoring = new Monitor();
            MailMessage SCmessage = new MailMessage();
            SmtpClient SCsmtp = new SmtpClient();
            SCmessage.From = new MailAddress("suzanna.vershinin@phi-networks.co.il");
            SCmessage.To.Add("suzanna.vershinin@phi-networks.co.il");

            if (General.ErrorMessge.Contains("No Files to import"))
            {
                SCmessage.To.Add("Avi.Shnaiderman@phi-networks.co.il");
                SCmessage.To.Add("Orit.Borukhov@phi-networks.co.il");
                SCmessage.To.Add("Eli.Cohen@phi-networks.co.il");
            }
            //SCmessage.To.Add("Merom.Toledano@Phi-Networks.co.il");
            SCmessage.Subject = "SOEMSRV Reports -- Erors ";
            SCmessage.Body = "   ***  " + DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss") + "  ***   ";

            foreach (var item in General.ErrorMessge)
                SCmessage.Body +=  string.Format(item);    
      
            try
            {
                SCsmtp.Credentials = new System.Net.NetworkCredential("suzanna.vershinin@phi-networks.co.il", "sv!!771982");
                SCsmtp.Host = "smtp.office365.com";
                SCsmtp.Port = 587;
                SCsmtp.EnableSsl = true;
                SCsmtp.Timeout = int.MaxValue;
                SCsmtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                SCsmtp.Send(SCmessage);          
                SCsmtp.Dispose();
                SCmessage.Dispose();
            }
            catch (Exception ex)
            {
                monitoring.WriteToFile("  **** ex.Message " + ex.Message + "  ****");
            }
        }
    }
}
