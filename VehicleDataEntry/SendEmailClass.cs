/* Title:           Send Email Class
 * Date:            7-12-17
 * Author:          Terry Holmes */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using NewEventLogDLL;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using SafetyDLL;

namespace VehicleDataEntry
{
    class SendEmailClass
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        SafetyClass TheSafetyClass = new SafetyClass();

        VehicleInspectionEmailListDataSet TheVehicleInspectionEmailListDataSet;
        

        public void EmailMessage(int intBJCNumber, string strVehicleProblem)
        {
            bool blnEmailSent;
            string strVehicleFailed;
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress;

            strVehicleFailed = Convert.ToString(intBJCNumber) + " Has Failed The Vehicle Inspection and Can Not Be Driven " + strVehicleProblem;

            TheVehicleInspectionEmailListDataSet = TheSafetyClass.GetVehicleInspectionEmailListInfo();

            intNumberOfRecords = TheVehicleInspectionEmailListDataSet.vehicleinspectionemaillist.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                strEmailAddress = TheVehicleInspectionEmailListDataSet.vehicleinspectionemaillist[intCounter].EmailAddress;

                blnEmailSent = SendEmail(strEmailAddress, "Vehicle Inspection Failure - Do Not Reply", strVehicleFailed);
            }
        }
         
        public bool SendEmail(string mailTo, string subject, string message)
        {
            string mailFrom = ConfigurationManager.AppSettings.Get("tholmes@bluejaycommunications.com");
            string username = ConfigurationManager.AppSettings.Get("bjc\tholmes");
            string password = ConfigurationManager.AppSettings.Get("F0xN3ws2017!");
            int port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("25"));
            string mailServer = ConfigurationManager.AppSettings.Get("192.168.0.2");

            try
            {

                MailMessage mailMessage = new MailMessage("bluejaycommunicationsit@gmail.com", mailTo, subject, message);
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.SubjectEncoding = Encoding.UTF8;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = true;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("bluejaycommunicationsit@gmail.com", "Bluejay2017!");
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Send Email Class // Send Mail " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                return false;
            }


        }
    }
}
