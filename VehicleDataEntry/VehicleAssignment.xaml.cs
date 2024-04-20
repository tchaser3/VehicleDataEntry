/* Title:           Vehicle Assignment
 * Date:            6-23-17
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
using VehicleAssignmentDLL;
using DataValidationDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleAssignment.xaml
    /// </summary>
    public partial class VehicleAssignment : Window
    {
        //setting up the clases
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindCurrentAssignedVehicleByVehicleIDDataSet TheFindCurrentAssignedVehicleByVehicleIDDataSet = new FindCurrentAssignedVehicleByVehicleIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public VehicleAssignment()
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

        private void txtBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting variables
            string strValueForValidation;
            bool blnFatalError = false;
            int intBJCNumber;
            int intRecordsReturned;
            int intLength;

            strValueForValidation = txtBJCNumber.Text;

            intLength = strValueForValidation.Length;

            if(intLength == 4)
            {
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The BJC Number Entered Is not Integer");
                    return;
                }
                else
                {
                    intBJCNumber = Convert.ToInt32(strValueForValidation);
                }

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Was Not Found");
                    return;
                }
                else
                {
                    MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                    TheFindCurrentAssignedVehicleByVehicleIDDataSet = TheVehicleAssignmentClass.FindCurrentAssignedVehicleByVehicleID(MainWindow.gintVehicleID);

                    intRecordsReturned = TheFindCurrentAssignedVehicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        txtCurrentFirstName.Text = "Not Assigned";
                        txtCurrentLastName.Text = "Not Assigned";
                    }
                    else
                    {
                        txtCurrentFirstName.Text = TheFindCurrentAssignedVehicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID[0].FirstName;
                        txtCurrentLastName.Text = TheFindCurrentAssignedVehicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID[0].LastName;
                    }

                    cboSelectEmployee.IsEnabled = true;
                }
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will load the combo box
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intLength;
            string strLastName;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                if(intLength >= 3)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords > -1)
                    {
                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                        }
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                    }
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle Assignment // Enter Last Name Text Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strFullName;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex;

                if(intSelectedIndex > 0)
                {
                    strFullName = cboSelectEmployee.SelectedItem.ToString();

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(strFullName == TheComboEmployeeDataSet.employees[intCounter].FullName)
                        {
                            MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intCounter].EmployeeID;
                            break;
                        }
                    }

                    btnProcess.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle Assignment // cbo Select Employee Selection Changed Event " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboSelectEmployee.IsEnabled = true;
            btnProcess.IsEnabled = true;
        }
        private void ResetControls()
        {
            txtBJCNumber.Text = "";
            txtCurrentFirstName.Text = "";
            txtCurrentLastName.Text = "";
            txtEnterLastName.Text = "";
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.IsEnabled = false;
            btnProcess.IsEnabled = false;
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //data validation
            string strValueForValidation;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intLength;
            int intRecordsReturned;
            int intTransactionID;

            try
            {
                strValueForValidation = txtBJCNumber.Text;
                intLength = strValueForValidation.Length;
                if(intLength != 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The BJC Number Is Not Correct\n";
                }
                strValueForValidation = cboSelectEmployee.SelectedItem.ToString();
                if(strValueForValidation == "Select Employee")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheVehicleClass.UpdateVehicleEmployeeID(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(MainWindow.gintVehicleID, MainWindow.gintEmployeeID);

                if (blnFatalError == true)
                    throw new Exception();


                intRecordsReturned = TheFindCurrentAssignedVehicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID.Rows.Count;

                if(intRecordsReturned == 1)
                {
                    intTransactionID = TheFindCurrentAssignedVehicleByVehicleIDDataSet.FindCurrentAssignedVehicleByVehicleID[0].TransactionID;

                    blnFatalError = TheVehicleAssignmentClass.UpdateCurrentVehicleAssignment(intTransactionID, false);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Is A Problem, Contact IT");
                    }
                }

                TheMessagesClass.InformationMessage("The Record Has Been Saved");
                ResetControls();
                txtBJCNumber.Focus();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Date Entry // Vehicle Assignment // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
