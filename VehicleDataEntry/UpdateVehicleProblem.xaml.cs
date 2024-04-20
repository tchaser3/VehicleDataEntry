/* Title:           Update Vehicle Problem
 * Date:            7-7-17
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
using VehicleProblemsDLL;
using VehiclesInShopDLL;
using NewVehicleDLL;
using VehicleStatusDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for UpdateVehicleProblem.xaml
    /// </summary>
    public partial class UpdateVehicleProblem : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();

        //setting up the data
        FindVehicleProblemByProblemIDDataSet TheFindVehicleProblemByProblemIDDataSet = new FindVehicleProblemByProblemIDDataSet();
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindVehiclesInShopByVehicleIDDataSet TheFindVehiclesInShopByVehicleIDDataSet = new FindVehiclesInShopByVehicleIDDataSet();
        FindOpenVehicleProblemsByVehicleIDDataSet TheFindOpenVehicleProblemsByVehicleIDDataSet = new FindOpenVehicleProblemsByVehicleIDDataSet();

        bool gblnInShop;
        bool gblnWorkOrderComplete = false;
        bool gblnFormLoad;
        int gintTransactionID;

        public UpdateVehicleProblem()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnDisplayOpenWorkOrders_Click(object sender, RoutedEventArgs e)
        {
            DisplayOpenWorkOrders DisplayOpenWorkOrders = new DisplayOpenWorkOrders();
            DisplayOpenWorkOrders.Show();
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
            //setting local variables
            int intRecordsReturned;

            try
            {
                gblnFormLoad = true;

                gintTransactionID = 0;

                //this will load up the controls
                TheFindVehicleProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleProblemByProblemID(MainWindow.gintProblemID);

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(TheFindVehicleProblemByProblemIDDataSet.FindVehicleProblemByProblemID[0].BJCNumber);

                MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                TheFindVehiclesInShopByVehicleIDDataSet = TheVehiclesInShopClass.FindVehiclesInShopByVehicleID(MainWindow.gintVehicleID);

                intRecordsReturned = TheFindVehiclesInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    gblnInShop = true;
                    rdoYes.IsChecked = true;
                    gintTransactionID = TheFindVehiclesInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID[0].TransactionID;
                }
                else
                {
                    gblnInShop = false;
                    rdoNo.IsChecked = true;
                }

                txtBJCNumber.Text = Convert.ToString(TheFindVehicleProblemByProblemIDDataSet.FindVehicleProblemByProblemID[0].BJCNumber);
                txtProblemID.Text = Convert.ToString(TheFindVehicleProblemByProblemIDDataSet.FindVehicleProblemByProblemID[0].ProblemID);
                txtProblem.Text = TheFindVehicleProblemByProblemIDDataSet.FindVehicleProblemByProblemID[0].Problem;
                txtTransactionDate.Text = Convert.ToString(TheFindVehicleProblemByProblemIDDataSet.FindVehicleProblemByProblemID[0].TransactionDAte);

                cboWorkOrderComplete.Items.Add("Select Status");
                cboWorkOrderComplete.Items.Add("YES");
                cboWorkOrderComplete.Items.Add("No");
                cboWorkOrderComplete.SelectedIndex = 0;

                txtInvoiceTotal.IsReadOnly = true;
                
                gblnWorkOrderComplete = false;

                gblnFormLoad = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Update Vehicle Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboWorkOrderComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboWorkOrderComplete.SelectedIndex;

            if (intSelectedIndex == 1)
            {
                txtInvoiceTotal.IsReadOnly = false;
                gblnWorkOrderComplete = true;
            }               
            else
            {
                txtInvoiceTotal.IsReadOnly = true;
                gblnWorkOrderComplete = false;
            }
                
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //datavalidation
            int intSelectedIndex;
            string strValueForValidation;
            string strProblemUpdate;
            DateTime datTransactionDate = DateTime.Now;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            float fltInvoiceTotal = 0;
            int intRecordsReturned;
            int intTransactionID;
            int intUpdateLength;

            try
            {
                intSelectedIndex = cboWorkOrderComplete.SelectedIndex;
                if (intSelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Work Order Complete Was Not Selected\n";
                }
                if (intSelectedIndex == 1)
                {
                    strValueForValidation = txtInvoiceTotal.Text;
                    blnThereIsAProblem = float.TryParse(strValueForValidation, out fltInvoiceTotal);
                    if (blnThereIsAProblem == false)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Invoice Total is not Numeric\n";
                    }
                }
                strProblemUpdate = txtUpdateNotes.Text;
                if (strProblemUpdate == "")
                {
                    blnFatalError = true;
                    strProblemUpdate += "Update Notes Were Not Entered\n";
                }
                else
                {
                    intUpdateLength = strProblemUpdate.Length;

                    if(intUpdateLength < 30)
                    {
                        blnFatalError = true;
                        strErrorMessage += "There is not Enough Information For The Update\n";
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnWorkOrderComplete == true)
                {
                    blnFatalError = TheVehicleProblemClass.UpdateVehiclePRoblemCost(MainWindow.gintProblemID, fltInvoiceTotal);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemSolved(MainWindow.gintProblemID, true);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindOpenVehicleProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleProblemsbyVehicleID(MainWindow.gintVehicleID);

                    intRecordsReturned = TheFindOpenVehicleProblemsByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID.Rows.Count;

                    if(gintTransactionID > 0)
                    {
                        blnFatalError = TheVehiclesInShopClass.RemoveVehicleInShop(gintTransactionID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    
                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "NO PROBLEM", datTransactionDate);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                    else if (intRecordsReturned > 0)
                    {
                        blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "NEEDS WORK", datTransactionDate);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
                else if(gblnWorkOrderComplete == false)
                {
                    if(gblnInShop == true)
                    {
                        if(gintTransactionID == 0)
                        {
                            blnFatalError = TheVehiclesInShopClass.InsertVehicleInShop(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gintVendorID, MainWindow.gstrVehicleProblem);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "DOWN", datTransactionDate);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        else
                        {
                            blnFatalError = TheVehicleProblemClass.UpdateVehicleProblemVendorID(MainWindow.gintProblemID, MainWindow.gintVendorID);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                    else if (gblnInShop == false)
                    {
                        blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "NEEDS WORK", datTransactionDate);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, strProblemUpdate, datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                txtBJCNumber.Text = "";
                txtInvoiceTotal.Text = "";
                txtProblem.Text = "";
                txtProblemID.Text = "";
                txtTransactionDate.Text = "";
                txtUpdateNotes.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Update Vehicle Problem // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            if(gblnFormLoad == false)
            {
                gblnInShop = true;
                SelectVendor SelectVendor = new SelectVendor();
                SelectVendor.ShowDialog();
            }
        }

        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnInShop = false;
        }
    }
}
