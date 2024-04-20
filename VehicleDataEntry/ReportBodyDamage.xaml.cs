/* Title:           Report Body Damage
 * Date:            7-12-17
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
using NewEventLogDLL;
using VehicleBodyDamageDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for ReportBodyDamage.xaml
    /// </summary>
    public partial class ReportBodyDamage : Window
    {
        //setting the classes
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleBodyDamageClass TheVehicleBodyDamageClass = new VehicleBodyDamageClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data
        FindVehicleBodyDamageByVehicleIDDataSet TheFindVehicleBodyDamageByVehicleIDDataSet = new FindVehicleBodyDamageByVehicleIDDataSet();
        
        public ReportBodyDamage()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the data grid
            //setting local variables
            int intRecordsReturned;

            TheFindVehicleBodyDamageByVehicleIDDataSet = TheVehicleBodyDamageClass.FindVehicleBodyDamageByVehicleID(MainWindow.gintVehicleID);

            intRecordsReturned = TheFindVehicleBodyDamageByVehicleIDDataSet.FindVehicleBodyDamageByVehicleID.Rows.Count;

            if(intRecordsReturned == 0)
            {
                TheMessagesClass.InformationMessage("This Will Be The First Reported Body Damage");
            }

            dgrBodyDamage.ItemsSource = TheFindVehicleBodyDamageByVehicleIDDataSet.FindVehicleBodyDamageByVehicleID;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //this will save the transaction
            string strDamageReported;
            bool blnFatalError = false;

            try
            {
                //data validation
                strDamageReported = txtEnterBodyDamage.Text;
                if(strDamageReported == "")
                {
                    TheMessagesClass.ErrorMessage("There Was No Body Damage Entered");
                    return;
                }

                blnFatalError = TheVehicleBodyDamageClass.InsertNewVehicleBodyDamage(MainWindow.gintVehicleID, strDamageReported, DateTime.Now, false);

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was A Problem, Contact IT");
                    return;
                }

                TheMessagesClass.InformationMessage("Damage Entered");

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Report Body Damage // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
