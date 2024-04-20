/* Title:           Add GPS Status
 * Date:            8-29-17
 * Author:          Terry Holmes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VehicleInfoDLL;
using NewEventLogDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for AddGPSStatus.xaml
    /// </summary>
    public partial class AddGPSStatus : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindGPSStatusByStatusDataSet TheFindGPSStatusByStatusDataSet = new FindGPSStatusByStatusDataSet();

        public AddGPSStatus()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //this will save the status
            string strGPSStatus;
            bool blnFatalError = false;
            int intRecordsReturned = 0;

            try
            {
                strGPSStatus = txtGPSStatus.Text;
                if(strGPSStatus == "")
                {
                    TheMessagesClass.ErrorMessage("The GPS Status Was Not Entered");
                    return;
                }

                TheFindGPSStatusByStatusDataSet = TheVehicleInfoClass.FindGPSStatusByStatus(strGPSStatus);

                intRecordsReturned = TheFindGPSStatusByStatusDataSet.FindGPSStatusByStatus.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.InformationMessage("The GPS Status Already Exists");
                    return;
                }

                blnFatalError = TheVehicleInfoClass.InsertGPSPlugStatus(strGPSStatus);

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact IT");
                    return;
                }

                txtGPSStatus.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Add GPS Status // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
