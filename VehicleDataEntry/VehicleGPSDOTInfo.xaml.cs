/* Title:           Vehicle GPS DOT Info
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
using NewVehicleDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleGPSDOTInfo.xaml
    /// </summary>
    public partial class VehicleGPSDOTInfo : Window
    {
        //setting up the class
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        VehicleInfoClass TheVehicleInfoClass = new VehicleInfoClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindVehicleInfoByBJCNumberDataSet TheFindVehicleInfoByBJCNumberDataSet = new FindVehicleInfoByBJCNumberDataSet();
        FindGPSPlugStatusSortedDataSet TheFindGPSPlugStatusSortedDataSet = new FindGPSPlugStatusSortedDataSet();
        FindDOTStatusSortedDataSet TheFindDOTStatusSortedDataSet = new FindDOTStatusSortedDataSet();
        FindVehicleInfoByIMEIDataSet TheFindVehicleInfoByIMEIDataSet = new FindVehicleInfoByIMEIDataSet();

        //setting variables
        bool gblnCDLRequired;
        bool gblnMedicalCardRequired;
        int gintDOTStatusID;
        int gintGPSStatusID;
        int gintVehicleID;
        int gintVehicleInfoID;
        bool gblnRecordEdit;
        string gstrDOTStatus;
        string gstrGPSStatus;
        string gstrIMEI;
        int gintTamperTag;

        public VehicleGPSDOTInfo()
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
            TheMessageClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will set up the form for use
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //loading the CDL Required Combo Box
                cboCDLRequired.Items.Add("Select");
                cboCDLRequired.Items.Add("Yes");
                cboCDLRequired.Items.Add("No");
                cboCDLRequired.SelectedIndex = 0;

                cboMedicalCardRequired.Items.Add("Select");
                cboMedicalCardRequired.Items.Add("Yes");
                cboMedicalCardRequired.Items.Add("No");
                cboMedicalCardRequired.SelectedIndex = 0;

                TheFindDOTStatusSortedDataSet = TheVehicleInfoClass.FindDOTStatusSorted();
                cboDOTStatus.Items.Add("Select");

                intNumberOfRecords = TheFindDOTStatusSortedDataSet.FindDOTStatusSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboDOTStatus.Items.Add(TheFindDOTStatusSortedDataSet.FindDOTStatusSorted[intCounter].DOTStatus);
                }

                TheFindGPSPlugStatusSortedDataSet = TheVehicleInfoClass.FindGPSPlugStatusSorted();

                cboGPSPlugStatus.Items.Add("Select");

                intNumberOfRecords = TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboGPSPlugStatus.Items.Add(TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted[intCounter].GPSStatus);
                }

                cboDOTStatus.SelectedIndex = 0;
                cboGPSPlugStatus.SelectedIndex = 0;
                btnSave.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle GPS DOT Info // Window Loaded " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboCDLRequired_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboCDLRequired.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnCDLRequired = true;
            else if (intSelectedIndex == 2)
                gblnCDLRequired = false;
        }

        private void cboMedicalCardRequired_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboMedicalCardRequired.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnMedicalCardRequired = true;
            else if (intSelectedIndex == 2)
                gblnMedicalCardRequired = false;

        }

        private void cboDOTStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboDOTStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDOTStatusID = TheFindDOTStatusSortedDataSet.FindDOTStatusSorted[intSelectedIndex].DOTStatusID;

            gstrDOTStatus = cboDOTStatus.SelectedItem.ToString();
        }

        private void cboGPSPlugStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboGPSPlugStatus.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintGPSStatusID = TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted[intSelectedIndex].GPSStatusID;

            gstrGPSStatus = cboGPSPlugStatus.SelectedItem.ToString();
        }

        private void btnFindBJCNumber_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intBJCNumber;
            string strValueForValidation;
            bool blnFatalError;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            
            //data validation
            strValueForValidation = txtBJCNumber.Text;

            SetControlsToDefault();

            txtBJCNumber.Text = strValueForValidation;

            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if(blnFatalError == true)
            {
                TheMessageClass.ErrorMessage("The BJC Number Entered Is Not an Integer");
                return;
            }

            //checking to see if the vehicle number is real
            intBJCNumber = Convert.ToInt32(strValueForValidation);

            TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

            intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count; 

            if(intRecordsReturned == 0)
            {
                TheMessageClass.ErrorMessage("The BJC Number Entered in not an Active Vehicle");
                return;
            }
            else
            {
                gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;
            }

            TheFindVehicleInfoByBJCNumberDataSet = TheVehicleInfoClass.FindVehicleInfoByBJCNumber(intBJCNumber);

            intRecordsReturned = TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber.Rows.Count;

            if (intRecordsReturned > 0)
            {
                //this will load the controls
                gblnRecordEdit = true;
                txtIMEI.Text = TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].IMEI;
                txtTamperTag.Text = Convert.ToString(TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].TamperTag);
                gintTamperTag = TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].TamperTag;
                gstrIMEI = TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].IMEI;

                gintVehicleInfoID = TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].VehicleInfoID;

                //loading the combo boxes
                if (TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].CDLRequired == true)
                    cboCDLRequired.SelectedIndex = 1;
                else
                    cboCDLRequired.SelectedIndex = 2;
                if (TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].MedicalCardRequired == true)
                    cboMedicalCardRequired.SelectedIndex = 1;
                else
                    cboMedicalCardRequired.SelectedIndex = 2;

                intNumberOfRecords = TheFindDOTStatusSortedDataSet.FindDOTStatusSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindDOTStatusSortedDataSet.FindDOTStatusSorted[intCounter].DOTStatus == TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].DOTStatus)
                    {
                        cboDOTStatus.SelectedIndex = intCounter + 1;
                    }
                }

                intNumberOfRecords = TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindGPSPlugStatusSortedDataSet.FindGPSPlugStatusSorted[intCounter].GPSStatus == TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].GPSStatus)
                    {
                        cboGPSPlugStatus.SelectedIndex = intCounter + 1;
                    }
                }

            }
            else
            {
                gblnRecordEdit = false;
                txtIMEI.Text = "NONE ASSIGNED";
                txtTamperTag.Text = "0";
            }

            btnSave.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            int intTamperTag = 0;
            string strIMEI = "";
            DateTime datTransactionDate = DateTime.Now;
            bool blnStatusChange = false;
            bool blnIMEIChange = false;
            bool blnTamperTagChange = false;

            try
            {
                if(cboCDLRequired.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "CDL Required Was Not Selected\n";
                }
                if(cboMedicalCardRequired.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Medical Card Required Was Not Selected\n";
                }
                if(cboDOTStatus.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "The DOT Status Was Not Selected\n";
                }
                if (cboGPSPlugStatus.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "GPS Plug Status Was Not Selected\n";
                }
                strIMEI = txtIMEI.Text;
                if(strIMEI == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "IMEI Was Not Entered\n";
                }
                strValueForValidation = txtTamperTag.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Tamper Tag Is Not an Integer\n";
                }
                else
                {
                    intTamperTag = Convert.ToInt32(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessageClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnRecordEdit == false)
                {
                    blnFatalError = TheVehicleInfoClass.InsertVehicleInfo(gintVehicleID, gblnCDLRequired, gblnMedicalCardRequired, gintDOTStatusID, gintGPSStatusID, strIMEI, intTamperTag);

                    TheFindVehicleInfoByBJCNumberDataSet = TheVehicleInfoClass.FindVehicleInfoByBJCNumber(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].BJCNumber);

                    gintVehicleInfoID = TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].VehicleInfoID;

                    TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "All", "New Record");

                    if(blnFatalError == true)
                    {
                        TheMessageClass.ErrorMessage("There Was a Problem, Contact IT");
                        return;
                    }
                }
                else if (gblnRecordEdit == true)
                {
                    //checking to see if the status has changed
                    if (gblnCDLRequired != TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].CDLRequired)
                    {
                        blnStatusChange = true;

                        TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "CDL Required", Convert.ToString(gblnCDLRequired));
                    }                        
                    if (gblnMedicalCardRequired != TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].MedicalCardRequired)
                    {
                        blnStatusChange = true;

                        TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "Medical Card Required", Convert.ToString(gblnMedicalCardRequired));
                    }                        
                    if (gstrDOTStatus != TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].DOTStatus)
                    {
                        blnStatusChange = true;

                        TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "DOT Status", gstrDOTStatus);
                    }                        
                    if (gstrGPSStatus != TheFindVehicleInfoByBJCNumberDataSet.FindVehicleInfoByBJCNumber[0].GPSStatus)
                    {
                        blnStatusChange = true;

                        TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "GPS Status", gstrGPSStatus);
                    }
                    if (blnStatusChange == true)
                    {
                        blnFatalError = TheVehicleInfoClass.UpdateVehicleInfoStatus(gintVehicleInfoID, gblnCDLRequired, gblnMedicalCardRequired, gintDOTStatusID, gintGPSStatusID, datTransactionDate);                                             
                    }
                    if(intTamperTag != gintTamperTag)
                    {
                        TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "Tamper Tag", Convert.ToString(gintTamperTag));

                        TheVehicleInfoClass.UpdateVehicleInfoTamperTag(gintVehicleInfoID, intTamperTag);
                    }
                    if (strIMEI != gstrIMEI)
                    {
                        TheVehicleInfoClass.InsertVehicleInfoHistory(gintVehicleInfoID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "IMEI", gstrIMEI);

                        TheVehicleInfoClass.UpdateVehicleInfoIMEI(gintVehicleInfoID, strIMEI);
                    }
                }

                SetControlsToDefault();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle GPS DOT Infor // Save Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetControlsToDefault()
        {
            txtBJCNumber.Text = "";
            txtIMEI.Text = "";
            txtTamperTag.Text = "";
            cboCDLRequired.SelectedIndex = 0;
            cboDOTStatus.SelectedIndex = 0;
            cboGPSPlugStatus.SelectedIndex = 0;
            cboMedicalCardRequired.SelectedIndex = 0;
        }
    }
}
