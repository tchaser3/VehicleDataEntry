/* Title:           Add DOT Status
 * Date:            8-29-17
 * Author:          Terry Holmes */

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
    /// Interaction logic for AddDOTStatus.xaml
    /// </summary>
    public partial class AddDOTStatus : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindDOTStatusByStatusDataSet TheFindDOTStatusByStatusDataSet = new FindDOTStatusByStatusDataSet();

        public AddDOTStatus()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDOTStatus;
            bool blnFatalError;
            int intRecordsReturned;

            try
            {
                strDOTStatus = txtDOTStatus.Text;
                if(strDOTStatus == "")
                {
                    TheMessagesClass.ErrorMessage("The DOT Status Was Not Entered");
                    return;
                }

                TheFindDOTStatusByStatusDataSet = TheVehicleInfoClass.FindDOTStatusByStatus(strDOTStatus);

                intRecordsReturned = TheFindDOTStatusByStatusDataSet.FindDOTStatusByStatus.Rows.Count;

                if(intRecordsReturned > 0 )
                {
                    TheMessagesClass.InformationMessage("DOT Status Already Exists");
                    return;
                }

                blnFatalError = TheVehicleInfoClass.InsertDOTStatus(strDOTStatus);

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a problem, Contact ID");
                    return;
                }

                txtDOTStatus.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Add DOT Status // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
