/* Title:           Preventative Maintenance
 * Date:            7-3-17
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
using NewVehicleDLL;
using VehicleProblemsDLL;
using DataValidationDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for PreventativeMaintenance.xaml
    /// </summary>
    public partial class PreventativeMaintenance : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting the data
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();
        
        //setting global variables
        bool gblnOilChangeComplete;
        DateTime gdatOilChangeDate;
        int gintOldOdometerReading;

        public PreventativeMaintenance()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnProblemMenu_Click(object sender, RoutedEventArgs e)
        {
            ProblemMenu ProblemMenu = new ProblemMenu();
            ProblemMenu.Show();
            Close();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboSelectCompletion.Items.Add("Select Completion");
            cboSelectCompletion.Items.Add("YES");
            cboSelectCompletion.Items.Add("NO");
            cboSelectCompletion.SelectedIndex = 0;

            SetControlsReadOnly(true);

            btnProcess.IsEnabled = false;

            MainWindow.gblnWorkOrderSelected = false;

            txtInvoiceTotal.Visibility = Visibility.Hidden;
        }

        private void txtEnterBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            bool blnFatalError = false;
            int intBJCNumber;
            int intRecordsReturned;
            int intLength;

            strValueForValidation = txtEnterBJCNumber.Text;
            intLength = strValueForValidation.Length;
            btnProcess.IsEnabled = false;
            cboSelectCompletion.SelectedIndex = 0;

            if(intLength == 4)
            {
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The BJC Number is not an Integer");
                    return;
                }
                else
                {
                    intBJCNumber = Convert.ToInt32(strValueForValidation);

                    TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                    intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("BJC Number Was Not Found");
                        return;
                    }
                    else
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                        MainWindow.TheFindOPenVehiclesByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleProblemsbyVehicleID(MainWindow.gintVehicleID);

                        intRecordsReturned = MainWindow.TheFindOPenVehiclesByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            SelectExistingWorkOrder SelectExistingWorkOrder = new SelectExistingWorkOrder();
                            SelectExistingWorkOrder.ShowDialog();
                        }

                        txtOilChangeDate.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].OilChangeDate);
                        txtOilChangeOdometer.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].OilChangeOdometer);
                        txtVehicleMake.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleMake;
                        txtVehicleModel.Text = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleModel;
                        txtVehicleYear.Text = Convert.ToString(TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleYear);
                    }
                }
            }
        }
        private void SetControlsReadOnly(bool blnValueBoolean)
        {
            txtOilChangeOdometer.IsReadOnly = blnValueBoolean;
        }

        private void cboSelectCompletion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strOilChangeComplete;
            string strValueForValidation;
            bool blnFatalError;
            int intLength;

            intSelectedIndex = cboSelectCompletion.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                strValueForValidation = txtEnterBJCNumber.Text;
                intLength = strValueForValidation.Length;
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The BJC Number Entered is not An Integer");
                    return;
                }
                else
                {
                    if (intLength != 4)
                    {
                        TheMessagesClass.ErrorMessage("There Are Either To Few or To Many Integers");
                        return;
                    }
                }

                strOilChangeComplete = cboSelectCompletion.SelectedItem.ToString();

                btnProcess.IsEnabled = true;

                if(strOilChangeComplete == "YES")
                {
                    gblnOilChangeComplete = true;
                    gdatOilChangeDate = DateTime.Now;
                    txtOilChangeDate.Text = Convert.ToString(gdatOilChangeDate);
                    SetControlsReadOnly(false);
                    gintOldOdometerReading = Convert.ToInt32(txtOilChangeOdometer.Text);
                    txtInvoiceTotal.Visibility = Visibility.Visible;

                    SelectVendor SelectVendor = new SelectVendor();
                    SelectVendor.ShowDialog();
                }
                else
                {
                    strValueForValidation = txtOilChangeDate.Text;
                    txtInvoiceTotal.Visibility = Visibility.Hidden;
                    blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);

                    if(blnFatalError == false)
                    {
                        gdatOilChangeDate = Convert.ToDateTime(txtOilChangeDate.Text);
                        gblnOilChangeComplete = false;
                        SetControlsReadOnly(true);
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("The Date Is Not Present");
                        return;
                    }
                }
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variable
            string strValueForValidation;
            bool blnFatalError = false;
            string strProblem;
            float fltTotal = 0;
            int intTransactionID;

            try
            {
                MainWindow.gdatTransactionDate = DateTime.Now;
                strProblem = "OIL CHANGE NEEDED";

                //performing data validation
                if (gblnOilChangeComplete == true)
                {
                    strValueForValidation = txtOilChangeOdometer.Text;
                    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Oil Change Odometer Reading is not an Integer");
                        return;
                    }
                    else
                    {
                        MainWindow.gintOdometerReading = Convert.ToInt32(strValueForValidation);

                        if (MainWindow.gintOdometerReading <= gintOldOdometerReading)
                        {
                            TheMessagesClass.ErrorMessage("The Odometer Reading is Less Than or Equal To The Old Reading");
                            return;
                        }
                    }

                    blnFatalError = float.TryParse(txtInvoiceTotal.Text, out fltTotal);

                    if(blnFatalError == false)
                    {
                        TheMessagesClass.ErrorMessage("The Total is not Numeric");
                        return;
                    }
                }

                if(MainWindow.gblnWorkOrderSelected == false)
                {

                    MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;
                    

                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, MainWindow.gdatTransactionDate, strProblem);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Please Contact IT");
                        return;
                    }

                    TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(MainWindow.gdatTransactionDate);

                    MainWindow.gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;
                }

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, strProblem, MainWindow.gdatTransactionDate);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("There Has Been A Problem, Please Contact IT");
                    return;
                }

                if(gblnOilChangeComplete == true)
                {
                    strProblem = " OIL CHANGE COMPLETED";

                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, strProblem, MainWindow.gdatTransactionDate);
                    if(blnFatalError == false)
                        blnFatalError = TheVehicleClass.UpdateOilChangeInformation(MainWindow.gintVehicleID, MainWindow.gintOdometerReading, MainWindow.gdatTransactionDate);
                    if(blnFatalError == false)
                        blnFatalError = TheVehicleProblemClass.UpdateVehiclePRoblemCost(MainWindow.gintProblemID, fltTotal);
                    if (blnFatalError == false)
                        blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemSolved(MainWindow.gintProblemID, gblnOilChangeComplete);
                    if (blnFatalError == false)
                        blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemVendorID(MainWindow.gintProblemID, MainWindow.gintVendorID);



                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Please Contact IT");
                        return;
                    }
                }

                ResetControls();
                SetControlsReadOnly(true);

                TheMessagesClass.InformationMessage("Problem Updated");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Preventative Maintenance // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.Message);
            }
            
        }
        private void ResetControls()
        {
            txtEnterBJCNumber.Text = "";
            txtInvoiceTotal.Text = "";
            txtInvoiceTotal.Visibility = Visibility.Hidden;
            txtOilChangeDate.Text = "";
            txtOilChangeOdometer.Text = "";
            txtVehicleMake.Text = "";
            txtVehicleModel.Text = "";
            txtVehicleYear.Text = "";
            btnProcess.IsEnabled = false;
            cboSelectCompletion.SelectedIndex = 0;
        }
    }
}
