/* Title:           Vehicle In Yard
 * Date:            6-22-17
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
using VehicleInYardDLL;
using NewVehicleDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleInYardForm.xaml
    /// </summary>
    public partial class VehicleInYardForm : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleInYardClass TheVehicleInYardClass = new VehicleInYardClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet;

        public VehicleInYardForm()
        {
            InitializeComponent();
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

        private void txtEnterBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intLength;

            
            strValueForValidation = txtEnterBJCNumber.Text;
            intLength = strValueForValidation.Length;

            if(intLength == 4)
            {
                btnSave.Focus();
            }
            
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intBJCNumber;
            int intRecordsReturned;
            int intVehicleID;
            DateTime datTransactionDate;
            bool blnFatalError = false;
            
            try
            {
                strValueForValidation = txtEnterBJCNumber.Text;
                
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The BJC Number Is Not An Integer");
                    return;
                }

                intBJCNumber = Convert.ToInt32(strValueForValidation);

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("The BJC Number Entered Does Not Exist");
                    return;
                }
                else if (intRecordsReturned == 1)
                {
                    intVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;
                    datTransactionDate = DateTime.Now;

                    blnFatalError = TheVehicleInYardClass.InsertVehicleInYard(datTransactionDate, intVehicleID);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Was a Problem, Contact IT");
                        return;
                    }
                }

                txtEnterBJCNumber.Text = "";
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle In Yard Form // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
