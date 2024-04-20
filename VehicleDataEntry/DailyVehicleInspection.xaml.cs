/* Title:           Daily Vehicle Inspection
 * Date:            6-21-17
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
using NewEmployeeDLL;
using NewVehicleDLL;
using InspectionsDLL;
using VehicleHistoryDLL;
using DataValidationDLL;
using VehicleProblemsDLL;
using VehicleStatusDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for DailyVehicleInspection.xaml
    /// </summary>
    public partial class DailyVehicleInspection : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        InspectionsClass TheInspectionsClass = new InspectionsClass();
        VehicleHistoryClass TheVehicleHistoryClass = new VehicleHistoryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();

        //Setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindDailyVehicleInspectionDateMatchDataSet TheFindDailyVehicleInspectionDateMatchDataSet = new FindDailyVehicleInspectionDateMatchDataSet();

        int gintBJCNumber;
        string gstrVehicleStatus;
        
        public DailyVehicleInspection()
        {
            InitializeComponent();
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void txtEnterBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intBJCLength;
            int intRecordsReturned;
            bool blnFatalError;

            //data validation
            strValueForValidation = txtEnterBJCNumber.Text;
            intBJCLength = strValueForValidation.Length;

            if(intBJCLength == 4)
            {
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("Th BJC Number Entered is not an Integer");
                    return;
                }
                else
                {
                    gintBJCNumber = Convert.ToInt32(strValueForValidation);
                }

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(gintBJCNumber);

                intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Not Found");
                    return;
                }
                else
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;
                }
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strLastName;
            int intLength;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                cboEmployee.Items.Clear();
                cboEmployee.Items.Add("Select Employee");

                if (intLength >= 3)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }
                    }
                    else
                    {
                        TheMessagesClass.InformationMessage("No Employees Found");
                    }

                    cboEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Daily Vehicle Inspectioin // txtEnterLastName_TextChanged " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            string strFullName;

            intSelectedIndex = cboEmployee.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                strFullName = cboEmployee.SelectedItem.ToString();

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(strFullName == TheComboEmployeeDataSet.employees[intCounter].FullName)
                    {
                        MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                        btnProcess.IsEnabled = true;
                        break;
                    }
                }
            }
            else
            {
                btnProcess.IsEnabled = false;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will process the information
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            try
            {
                //data validation
                strValueForValidation = txtEnterBJCNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The BJC Number Is Not An Integer\n";
                }
                strValueForValidation = cboEmployee.SelectedItem.ToString();
                if (strValueForValidation == "Select Employee")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (rdoFailed.IsChecked == false)
                {
                    if (rdoPassed.IsChecked == false)
                    {
                        if (rdoPassedServiceRequired.IsChecked == false)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Passed, Passed Service Required, or Failure Was Not Checked\n";
                        }
                    }
                }
                strValueForValidation = txtOdometerReading.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Odometer Reading is not an Integer\n";
                }
                else
                {
                    MainWindow.gintOdometerReading = Convert.ToInt32(strValueForValidation);
                }

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                MainWindow.gdatTransactionDate = DateTime.Now;

                blnFatalError = TheInspectionsClass.InsertDailyVehicleInspection(MainWindow.gintVehicleID, MainWindow.gdatTransactionDate, MainWindow.gintEmployeeID, MainWindow.gstrInspectionStatus, MainWindow.gintOdometerReading);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact ID");
                    return;
                }

                blnFatalError = TheVehicleHistoryClass.InsertVehicleHistory(MainWindow.gintVehicleID, MainWindow.gintEmployeeID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID);

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact ID");
                    return;
                }

                blnFatalError = TheVehicleClass.UpdateVehicleEmployeeID(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Was a Problem, Contact ID");
                    return;
                }


                TheFindDailyVehicleInspectionDateMatchDataSet = TheInspectionsClass.FindDailyVehicleInspectionDateMatch(MainWindow.gdatTransactionDate);

                MainWindow.gintInspectionID = TheFindDailyVehicleInspectionDateMatchDataSet.FindDailyVehicleInspectionByDateMatch[0].TransactionID;

                TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, gstrVehicleStatus, DateTime.Now);

                if ((rdoFailed.IsChecked == true) || (rdoPassedServiceRequired.IsChecked == true))
                {

                    VehicleInspectionProblem VehicleInspectionProblem = new VehicleInspectionProblem();
                    VehicleInspectionProblem.ShowDialog();
                }

                if (rdoFailed.IsChecked == true)
                {
                    PleaseWait PleaseWait = new PleaseWait();
                    PleaseWait.Show();

                    TheSendEmailClass.EmailMessage(gintBJCNumber, MainWindow.gstrVehicleProblem);

                    PleaseWait.Close();
                }

                txtEnterBJCNumber.Text = "";
                cboEmployee.Items.Clear();
                txtEnterLastName.Text = "";
                txtOdometerReading.Text = "";
                rdoFailed.IsChecked = false;
                rdoPassed.IsChecked = false;
                rdoPassedServiceRequired.IsChecked = false;
                cboBodyDamageReported.SelectedIndex = 0;
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Daily Vehicle Inspection // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnProcess.IsEnabled = false;

            MainWindow.gstrInspectionType = "DAILY";

            cboBodyDamageReported.Items.Add("Select Answer");
            cboBodyDamageReported.Items.Add("YES");
            cboBodyDamageReported.Items.Add("NO");
            cboBodyDamageReported.SelectedIndex = 0;
        }

        private void rdoPassedServiceRequired_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnServicable = true;
            MainWindow.gstrInspectionStatus = "PASSED SERVICE REQUIRED";
            gstrVehicleStatus = "NEEDS WORK";
        }

        private void rdoFailed_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnServicable = false;
            MainWindow.gstrInspectionStatus = "FAILED";
            gstrVehicleStatus = "DOWN";
        }

        private void rdoPassed_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrInspectionStatus = "PASSED";
            gstrVehicleStatus = "NO PROBLEM";
        }

        private void cboBodyDamageReported_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboBodyDamageReported.SelectedIndex;

            if(intSelectedIndex == 1)
            {
                ReportBodyDamage ReportBodyDamage = new ReportBodyDamage();
                ReportBodyDamage.ShowDialog();
            }
        }
    }
}
