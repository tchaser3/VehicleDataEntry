/* Title:           Main Window - Vehicle Data Entry
 * Date:            4-18-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the main window for Vehicle Data Entry */

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataValidationDLL;
using NewEmployeeDLL;
using VehicleProblemsDLL;
using NewEventLogDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        AutoSignInClass TheAutoSignInClass = new AutoSignInClass();

        //setting up data sets
        public static VerifyLogonDataSet TheVerifyLoginDataSet = new VerifyLogonDataSet();
        public static FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        public static FindOpenVehicleProblemsByVehicleIDDataSet TheFindOPenVehiclesByVehicleIDDataSet = new FindOpenVehicleProblemsByVehicleIDDataSet();
        
        public static int gintVehicleID;
        public static int gintEmployeeID;
        public static string gstrInspectionType;
        public static DateTime gdatTransactionDate;
        public static int gintOdometerReading;
        public static int gintInspectionID;
        public static bool gblnServicable;
        public static string gstrInspectionStatus;
        public static int gintProblemID;
        public static int gintVendorID;
        public static bool gblnWorkOrderSelected;
        public static string gstrVehicleProblem;

        //setting up global variables
        int gintNoOfMisses;

        public MainWindow()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting variables
            bool blnFatalError = false;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            gintNoOfMisses = 0;

            TheAutoSignInClass.AutoSignInVehicles();

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("There Has Been a Problem, Contact IT");
            }

            PleaseWait.Hide();
            
            pbxEmployeeID.Focus();
        }
       
        private void LogonFailed()
        {
            gintNoOfMisses++;

            if(gintNoOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attempts to Login Into Vehicle Data Entry");

                TheMessagesClass.ErrorMessage("There Have Been Three Attempts To Sign In\nThe Application Will Now Close");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("You Have Failed the Sign In Process");
            }
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intRecordsReturned;

            try
            {
                //data validation
                strValueForValidation = pbxEmployeeID.Password.ToUpper();
                strLastName = txtLastName.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee ID is not an Integer\n";
                }
                else
                {
                    intEmployeeID = Convert.ToInt32(strValueForValidation);
                }
                if(strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Last Name Was Not Entered\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //getting the employee
                TheVerifyLoginDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

                intRecordsReturned = TheVerifyLoginDataSet.VerifyLogon.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    LogonFailed();
                }
                else
                {
                   if(TheVerifyLoginDataSet.VerifyLogon[0].EmployeeGroup == "USERS")
                   {
                       LogonFailed();
                   }
                    else
                    {
                        MainMenu MainMenu = new MainMenu();
                        MainMenu.Show();
                        Hide();
                    }
                    
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Main Window // Sign In Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        
    }
}
