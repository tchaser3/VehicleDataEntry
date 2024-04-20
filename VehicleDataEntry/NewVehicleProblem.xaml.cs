/* Title:       New Vehicle Problem
 * Date:        7-7-17
 * Author:      Terry Holmes */

using DataValidationDLL;
using NewEventLogDLL;
using NewVehicleDLL;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VehicleProblemsDLL;
using VehiclesInShopDLL;
using VehicleStatusDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for NewVehicleProblem.xaml
    /// </summary>
    public partial class NewVehicleProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        VehiclesInShopClass TheVehicleInShopClass = new VehiclesInShopClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();

        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();
        FindOpenVehicleProblemsByVehicleIDDataSet TheFindOpenVehicleProblemByVehicleIDDataSet = new FindOpenVehicleProblemsByVehicleIDDataSet();
        FindVehiclesInShopByVehicleIDDataSet TheFindVehiclesInShopByVehicleIDDataSet = new FindVehiclesInShopByVehicleIDDataSet();

        ExistingOpenProblemsDataSet TheExistingOpenOrderProblemDataSet = new ExistingOpenProblemsDataSet();

        int gintProblemID;
        bool gblnNewWorkOrder;
        bool gblnInShop = false;

        public NewVehicleProblem()
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

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnProblemMenu_Click(object sender, RoutedEventArgs e)
        {
            ProblemMenu ProblemMenu = new ProblemMenu();
            ProblemMenu.Show();
            Close();
        }

        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            //this will display the Select Vendors
            gblnInShop = true;

            SelectVendor SelectVendor = new SelectVendor();
            SelectVendor.ShowDialog();
        }

        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            gblnInShop = false;
            MainWindow.gintVehicleID = 1001;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will set the form into an initial condition
            rdoNo.IsChecked = true;

            HideTextBoxes();
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //this will save the record
            //setting local variables
            int intBJCNumber = 0;
            string strValueForValidation;
            string strProblem;
            string strProblemNotes;
            string strErrorMessage = "";
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            int intRecordsReturned;
            DateTime datTransactionDate = DateTime.Now;

            try
            {
                //data validation
                strValueForValidation = txtBJCNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The BJC Number is not an Integer\n";
                }
                else
                {
                    intBJCNumber = Convert.ToInt32(strValueForValidation);

                    TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                    intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The BJC Number Was Not Found\n";
                    }
                    else
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;
                    }
                }
                strProblem = txtProblem.Text;
                if(gblnNewWorkOrder == true)
                {
                    if (strProblem == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Problem Was Not Entered\n";
                    }
                }
               
                strProblemNotes = txtAddedNotes.Text;
                if(strProblemNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Notes Were Not Entered\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnNewWorkOrder == true)
                {
                    //setting up new problems
                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, datTransactionDate, txtProblem.Text);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(datTransactionDate);

                    MainWindow.gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;
                }

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, txtAddedNotes.Text, datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                if(gblnInShop == true)
                {
                    TheFindVehiclesInShopByVehicleIDDataSet = TheVehicleInShopClass.FindVehiclesInShopByVehicleID(MainWindow.gintVehicleID);

                    intRecordsReturned = TheFindVehiclesInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        blnFatalError = TheVehicleInShopClass.InsertVehicleInShop(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gintVendorID, txtProblem.Text);

                        if (blnFatalError == true)
                            throw new Exception();
                    }

                    blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "DOWN", datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if (gblnInShop == false)
                {
                    blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "NEEDS WORK", datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                
                txtAddedNotes.Text = "";
                txtBJCNumber.Text = "";
                txtProblem.Text = "";
                rdoYes.IsChecked = false;
                rdoNo.IsChecked = true;
                HideTextBoxes();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // New Vehicle Problem // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtBJCNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intLength;
            string strValueForValidation;
            int intBJCNumber;
            int intVehicleID;
            int intRecordsReturned;
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;

            strValueForValidation = txtBJCNumber.Text;
            intLength = strValueForValidation.Length;

            if(intLength == 4)
            {
                blnFatalError = TheDataValidationClass.VerifyIntegerRange(strValueForValidation);
                TheExistingOpenOrderProblemDataSet.openorders.Rows.Clear();
                gintProblemID = 0;

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The BJC Number is not an Integer");
                    return;
                }

                intBJCNumber = Convert.ToInt32(strValueForValidation);

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                intRecordsReturned = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.ErrorMessage("The Vehicle Does Not Exist or is Retired");
                    return;
                }

               
                ExistingOpenProblemsDataSet.openordersRow SelectRow = TheExistingOpenOrderProblemDataSet.openorders.NewopenordersRow();

                SelectRow.ProblemID = -1;
                SelectRow.Problem = "NEW WORK ORDER";

                TheExistingOpenOrderProblemDataSet.openorders.Rows.Add(SelectRow);
                

                intVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                TheFindOpenVehicleProblemByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleProblemsbyVehicleID(intVehicleID);

                intNumberOfRecords = TheFindOpenVehicleProblemByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ExistingOpenProblemsDataSet.openordersRow NewOpenOrderRow = TheExistingOpenOrderProblemDataSet.openorders.NewopenordersRow();

                        NewOpenOrderRow.ProblemID = TheFindOpenVehicleProblemByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID[intCounter].ProblemID;
                        NewOpenOrderRow.Problem = TheFindOpenVehicleProblemByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID[intCounter].Problem;

                        TheExistingOpenOrderProblemDataSet.openorders.Rows.Add(NewOpenOrderRow);
                    }
                }

                dgrOpenProblems.ItemsSource = TheExistingOpenOrderProblemDataSet.openorders;
            }
            else if(intLength > 4)
            {
                TheMessagesClass.ErrorMessage("The BJC Number is not Correct");
                return;
            }
            
        }

        private void dgrOpenProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid OpenOrderGrid;
            DataGridRow OpenOrderRow;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                intSelectedIndex = dgrOpenProblems.SelectedIndex;
                HideTextBoxes();

                if(intSelectedIndex > - 1)
                {
                    OpenOrderGrid = dgrOpenProblems;
                    OpenOrderRow = (DataGridRow)OpenOrderGrid.ItemContainerGenerator.ContainerFromIndex(OpenOrderGrid.SelectedIndex);
                    ProblemID = (DataGridCell)OpenOrderGrid.Columns[0].GetCellContent(OpenOrderRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;

                    gintProblemID = Convert.ToInt32(strProblemID);

                    if(gintProblemID == -1)
                    {
                        txtProblem.Visibility = Visibility.Visible;
                        gblnNewWorkOrder = true;
                    }
                    else if(gintProblemID > -1)
                    {
                        gblnNewWorkOrder = false;
                    }

                    txtAddedNotes.Visibility = Visibility.Visible;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // New Vehicle Problem // Open Problems Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
        private void HideTextBoxes()
        {
            txtAddedNotes.Visibility = Visibility.Hidden;
            txtProblem.Visibility = Visibility.Hidden;
        } 
    }
}
