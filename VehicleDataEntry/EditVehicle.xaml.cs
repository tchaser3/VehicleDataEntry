/* Title:           Edit Vehicle
 * Date:            5-25-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to edit a vehicle*/

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

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for EditVehicle.xaml
    /// </summary>
    public partial class EditVehicle : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();

        //setting up global variables
        bool gblnActive;
        bool gblnAvailable;
        string gstrAssignedOffice;
        int gintBJCNumber;
        int gintWarehouseID;

        public EditVehicle()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void cboAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strAvailable;

            strAvailable = cboAvailable.SelectedItem.ToString();
                       
            if (strAvailable == "YES")
                gblnAvailable = true;
           else if (strAvailable == "NO")
                gblnAvailable = false;
           
        }

        private void cboActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strActive;

            strActive = cboActive.SelectedItem.ToString();

            if (strActive == "YES")
                gblnActive = true;
            else if (strActive == "NO")
                gblnActive = false;
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex;

            intSelectedIndex = cboWarehouse.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                gstrAssignedOffice = cboWarehouse.SelectedItem.ToString();

                intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(gstrAssignedOffice == MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName)
                    {
                        gintWarehouseID = MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID;
                    }
                }
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this set up the form for editing
            int intCounter;
            int intNumberOfRecords;

            LoadAvailableComboBox();
            LoadActiveComboBox();

            cboWarehouse.Items.Add("Select Warehouse");

            intNumberOfRecords = MainWindow.TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboWarehouse.Items.Add(MainWindow.TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboWarehouse.SelectedIndex = 0;
            SetControlsReadOnly(true);
            SetComboBoxEnabled(false);
            btnEdit.IsEnabled = false;
            cboAvailable.IsEnabled = false;
        }
        private void LoadAvailableComboBox()
        {
            cboAvailable.Items.Add("Select Available");
            cboAvailable.Items.Add("Yes");
            cboAvailable.Items.Add("No");
            cboAvailable.SelectedIndex = 0;
        }

        private void LoadActiveComboBox()
        {
            //load active combo box
            cboActive.Items.Add("Select Active");
            cboActive.Items.Add("Yes");
            cboActive.Items.Add("No");
            cboActive.SelectedIndex = 0;
        }

        private void txtEnterBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intLength;
            int intBJCNumber;
            bool blnFatalError = false;
            int intRecordsReturned;
            string strAssignedOffice;
            bool blnAvailable;
            

            strValueForValidation = txtEnterBJCNumber.Text;

            //getting the length
            intLength = strValueForValidation.Length;

            if (intLength == 4)
            {
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("BJC Number Is Not An Integer");
                    return;
                }

                intBJCNumber = Convert.ToInt32(strValueForValidation);

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    TheMessagesClass.InformationMessage("No Vehicles Were Found");
                    return;
                }

                //getting the assigned office
                strAssignedOffice = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].AssignedOffice;

                //setting combo boxes
                SetWarehouseComboBox(strAssignedOffice);
                cboActive.SelectedIndex = 1;
                blnAvailable = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].Available;
                if (blnAvailable == true)
                    cboAvailable.SelectedIndex = 1;
                else if (blnAvailable == false)
                    cboAvailable.SelectedIndex = 2;

                //loading the controls
                txtBJCNumber.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].BJCNumber);
                txtEmployeeName.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].FirstName + " " + TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].LastName;
                txtLicensePlate.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].LicensePlate;
                txtNotes.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].Notes;
                txtOilChangeDate.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].OilChangeDate);
                txtOilChangeOdometer.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].OilChangeOdometer);
                txtVehicleID.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID);
                txtVehicleMake.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleMake;
                txtVehicleModel.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleModel;
                txtVehicleYear.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleYear);
                txtVINNumber.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VINNumber;
                btnEdit.IsEnabled = true;
            }
        }
        private void SetWarehouseComboBox(string strAssignOffice)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                intNumberOfRecords = cboWarehouse.Items.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWarehouse.SelectedIndex = intCounter;

                    if (strAssignOffice == cboWarehouse.SelectedItem.ToString()) 
                    {
                        intSelectedIndex = intCounter;
                    }
                }

                cboWarehouse.SelectedIndex = intSelectedIndex;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Edit Vehicle // Set Warehouse Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtLicensePlate.IsReadOnly = blnValueBoolean;
            txtNotes.IsReadOnly = blnValueBoolean;
            txtOilChangeDate.IsReadOnly = blnValueBoolean;
            txtOilChangeOdometer.IsReadOnly = blnValueBoolean;
            txtVINNumber.IsReadOnly = blnValueBoolean;
        }
        private void SetComboBoxEnabled(bool blnValueBoolean)
        {
            cboActive.IsEnabled = blnValueBoolean;
            cboWarehouse.IsEnabled = blnValueBoolean;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidataion;
            int intVehicleID;
            string strLicensePlate;
            DateTime datOilChangeDate = DateTime.Now;
            int intOilChangeOdometer = 0;
            string strVINNumber;
            string strNotes;
            bool blnActive = true;
            string strAssignedOffice;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            if(btnEdit.Content.ToString() == "Edit")
            {
                SetControlsReadOnly(false);
                SetComboBoxEnabled(true);
                btnEdit.Content = "Save";
            }
            else
            {
                try
                {
                    //beginning data validation
                    intVehicleID = Convert.ToInt32(txtVehicleID.Text);
                    strValueForValidataion = txtOilChangeDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidataion);
                    if(blnThereIsAProblem == true)
                    {
                        strErrorMessage += "The Date is in the Correct Format\n";
                        blnFatalError = true;
                    }
                    else
                    {
                        datOilChangeDate = Convert.ToDateTime(strValueForValidataion);
                    }
                    strValueForValidataion = txtOilChangeOdometer.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidataion);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Oil Change Odometer Entry is not an Integer\n";
                    }
                    else
                    {
                        intOilChangeOdometer = Convert.ToInt32(strValueForValidataion);
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
                    strNotes = txtNotes.Text;
                    if(strNotes == "")
                    {
                        strNotes = "NO NOTES ENTERED";
                    }
                    if(cboActive.SelectedItem.ToString() == "Select Active")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Active Was Not Selected\n";
                    }
                    else if(cboActive.SelectedItem.ToString() == "YES")
                    {
                        blnActive = true;
                    }
                    else if (cboActive.SelectedItem.ToString() == "NO")
                    {
                        blnActive = false;
                    }
                    strAssignedOffice = cboWarehouse.SelectedItem.ToString();
                    if(strAssignedOffice == "Select Warehouse")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Warehouse Was Not Selected\n";
                    }

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    blnFatalError = TheVehicleClass.UpdateVehicleEdit(intVehicleID, strLicensePlate, intOilChangeOdometer, datOilChangeDate, strVINNumber, strNotes, blnActive, strAssignedOffice);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Was a Problem, Contact IT");
                        return;
                    }
                    else
                    {
                        ResetControls();
                        SetControlsReadOnly(true);
                        SetComboBoxEnabled(false);
                        TheMessagesClass.InformationMessage("The Transaction Has Been Saved");
                    }
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Edit Vehicle // Edit Save Button " + Ex.Message);

                    TheMessagesClass.ErrorMessage(Ex.ToString());
                }
            }
        }
        private void ResetControls()
        {
            txtBJCNumber.Text = "";
            txtEmployeeName.Text = "";
            txtEnterBJCNumber.Text = "";
            txtLicensePlate.Text = "";
            txtNotes.Text = "";
            txtOilChangeDate.Text = "";
            txtOilChangeOdometer.Text = "";
            txtVehicleID.Text = "";
            txtVehicleMake.Text = "";
            txtVehicleModel.Text = "";
            txtVehicleYear.Text = "";
            txtVINNumber.Text = "";
            cboActive.SelectedIndex = 0;
            cboAvailable.SelectedIndex = 0;
            cboWarehouse.SelectedIndex = 0;
            btnEdit.Content = "Edit";
        }
    }
}
