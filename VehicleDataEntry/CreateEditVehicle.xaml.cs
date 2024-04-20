/* Title:           Create Edit Vehicle
 * Date:            5-23-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the form where a vehicle is created or editted */

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
using NewVehicleDLL;
using NewEventLogDLL;
using DataValidationDLL;
using VehicleStatusDLL;
using VehicleAssignmentDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for CreateEditVehicle.xaml
    /// </summary>
    public partial class CreateEditVehicle : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();
        VehicleAssignmentClass TheVehicleAssignmentClass = new VehicleAssignmentClass();

        //setting up the data
        VehiclesDataSet TheVehiclesDataSet = new VehiclesDataSet();
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();

        //creating global variables
        bool gblnActive;
        bool gblnAvailable;
        string gstrAssignedOffice;
        int gintEmployeeID;

        public CreateEditVehicle()
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
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the form
            try
            {
                TheVehiclesDataSet = TheVehicleClass.GetVehiclesInfo();

                //loading combo boxes
                cboActive.Items.Add("Select Active");
                cboActive.Items.Add("Yes");
                cboActive.Items.Add("No");

                cboAvailable.Items.Add("Select Availability");
                cboAvailable.Items.Add("Yes");
                cboAvailable.Items.Add("No");

                cboAvailable.SelectedIndex = 0;
                cboActive.SelectedIndex = 0;
                cboActive.IsEnabled = false;
                cboAvailable.IsEnabled = false;

                LoadWarehouseComboBox();

                SetControlsReadOnly(true);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Create Edit Vehicle // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadWarehouseComboBox()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            //this will load the combo box
            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            cboWarehouse.Items.Add("Select Warehouse");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboWarehouse.SelectedIndex = 0;
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtBJCNumber.IsReadOnly = blnValueBoolean;
            txtEmployeeID.IsReadOnly = blnValueBoolean;
            txtLicensePlate.IsReadOnly = blnValueBoolean;
            txtNotes.IsReadOnly = blnValueBoolean;
            txtOilChangeDate.IsReadOnly = blnValueBoolean;
            txtOilChangeOdometer.IsReadOnly = blnValueBoolean;
            txtVehicleMake.IsReadOnly = blnValueBoolean;
            txtVehicleModel.IsReadOnly = blnValueBoolean;
            txtVehicleYear.IsReadOnly = blnValueBoolean;
            txtVINNumber.IsReadOnly = blnValueBoolean;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            int intVehicleID;
            string strButtonValue;
            int intBJCNumber = 0;
            int intVehicleYear = 0;
            string strVehicleMake;
            string strVehicleModel;
            string strLicensePlate;
            string strVINNumber;
            int intOilChangeOdometer = 0;
            DateTime datOilChangeDate = DateTime.Now;
            string strNotes;
            int intRecordsReturned;

            try
            {
                strButtonValue = btnAdd.Content.ToString();

                if(strButtonValue == "Add")
                {
                    SetControlsReadOnly(false);

                    intVehicleID = TheVehicleClass.CreateVehicleID();

                    txtVehicleID.Text = Convert.ToString(intVehicleID);

                    btnAdd.Content = "Save";

                    cboActive.SelectedIndex = 1;
                    cboAvailable.SelectedIndex = 1;
                    txtNotes.Text = "NO NOTES PROVIDED";
                }
                else
                {
                    //beginning data validation
                    if(cboWarehouse.SelectedItem.ToString() == "Select Warehouse")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Assigned Office Was Not Selected\n";
                    }
                    strValueForValidation = txtBJCNumber.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "BJC Number Was Not An Integer\n";
                    }
                    else
                    {
                        intBJCNumber = Convert.ToInt32(strValueForValidation);
                    }
                    strValueForValidation = txtVehicleYear.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vehicle Year is not an Integer\n";
                    }
                    else
                    {
                        intVehicleYear = Convert.ToInt32(strValueForValidation);
                    }
                    strVehicleMake = txtVehicleMake.Text;
                    if(strVehicleMake == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Vehicle Make Was Not Entered\n";
                    }
                    strVehicleModel = txtVehicleModel.Text;
                    if(strVehicleModel == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Vehicle Model Was Not Entered\n";
                    }
                    strLicensePlate = txtLicensePlate.Text;
                    if(strLicensePlate == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The License Plate Was Not Entered\n";
                    }
                    strVINNumber = txtVINNumber.Text;
                    if(strVINNumber == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The VIN Number Was Not Entered\n";
                    }
                    strValueForValidation = txtOilChangeOdometer.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Oil Change Odometer Is Not An Integer\n";
                    }
                    else
                    {
                        intOilChangeOdometer = Convert.ToInt32(strValueForValidation);
                    }
                    strValueForValidation = txtOilChangeDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Oil Change Date is not a Date\n";
                    }
                    else
                    {
                        datOilChangeDate = Convert.ToDateTime(strValueForValidation);
                    }
                    strNotes = txtNotes.Text;
                    if(strNotes == "")
                    {
                        strNotes = "NO NOTES PROVIDED";
                    }
                    
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                    intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                    if(intRecordsReturned != 0)
                    {
                        TheMessagesClass.ErrorMessage("There Is An Active Vehicle With The Same BJC Number");
                        return;
                    }

                    intVehicleID = Convert.ToInt32(txtVehicleID.Text);

                    blnFatalError = TheVehicleClass.InsertVehicle(intVehicleID, intBJCNumber, intVehicleID, strVehicleMake, strVehicleModel, strLicensePlate, strVINNumber, intOilChangeOdometer, datOilChangeDate, gintEmployeeID, strNotes, gstrAssignedOffice);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("Contact IT");
                        return;
                    }
                    else
                    {
                        SetControlsReadOnly(true);
                        ResetControls();
                    }

                    TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                    intVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                    blnFatalError = TheVehicleStatusClass.InsertVehicleStatus(intVehicleID, "AVAILABLE", DateTime.Now);

                    blnFatalError = TheVehicleAssignmentClass.InsertVehicleAssignment(intVehicleID, gintEmployeeID);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Create Edit Vehicles // Add Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variable
            string strAvailable;

            //getting the contents of the combo box
            strAvailable = cboAvailable.SelectedItem.ToString();

            if(strAvailable != "Select Availability")
            {
                if (strAvailable == "Yes")
                    gblnAvailable = true;
                else if (strAvailable == "No")
                    gblnAvailable = false;
            }
        }

        private void cboActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strActive;

            strActive = cboActive.SelectedItem.ToString();

            if(strActive != "Select Active")
            {
                if (strActive == "Yes")
                    gblnActive = true;
                else if (strActive == "No")
                    gblnActive = false;
            }
        }
        private void ResetControls()
        {
            cboActive.SelectedIndex = 0;
            cboAvailable.SelectedIndex = 0;
            cboWarehouse.SelectedIndex = 0;
            txtBJCNumber.Text = "";
            txtEmployeeID.Text = "";
            txtLicensePlate.Text = "";
            txtNotes.Text = "";
            txtOilChangeDate.Text = "";
            txtOilChangeOdometer.Text = "";
            txtVehicleID.Text = "";
            txtVehicleMake.Text = "";
            txtVehicleModel.Text = "";
            txtVehicleYear.Text = "";
            txtVINNumber.Text = "";
            btnAdd.Content = "Add";
        }
        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            gstrAssignedOffice = cboWarehouse.SelectedItem.ToString();

            if (gstrAssignedOffice != "Select Warehouse")
            {
                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if(gstrAssignedOffice == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        gintEmployeeID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                        txtEmployeeID.Text = Convert.ToString(gintEmployeeID);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditVehicle EditVehicle = new EditVehicle();
            EditVehicle.ShowDialog();
        }
    }
}
