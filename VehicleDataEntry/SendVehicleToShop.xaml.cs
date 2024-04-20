/* Title:           Send Vehicle To Shop
 * Date:            7-19-17
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
using VehiclesInShopDLL;
using VehicleProblemsDLL;
using VendorsDLL;
using NewVehicleDLL;
using VehicleStatusDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for SendVehicleToShop.xaml
    /// </summary>
    public partial class SendVehicleToShop : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        VehicleProblemClass TheVehicleProblemsClass = new VehicleProblemClass();
        VendorsClass TheVendorClass = new VendorsClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();

        FindOpenVehicleProblemsDataSet TheFindOpenVehicleProblemsDataSet = new FindOpenVehicleProblemsDataSet();
        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorNameDataSet;
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindVehiclesInShopByVehicleIDDataSet TheFindVehiclesInShopByVehicleIDDataSet = new FindVehiclesInShopByVehicleIDDataSet();

        bool gblnProblemSelected;
        bool gblnVendorSelected;

        string gstrProblem;

        public SendVehicleToShop()
        {
            InitializeComponent();
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

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the grids
            try
            {
                TheFindOpenVehicleProblemsDataSet = TheVehicleProblemsClass.FindOpenVehicleProblems();

                dgrWorkOrders.ItemsSource = TheFindOpenVehicleProblemsDataSet.FindOpenVehicleProblems;

                TheFindVendorsSortedByVendorNameDataSet = TheVendorClass.FindVendorsSortedByVendorName();

                dgrVendors.ItemsSource = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName;

                gblnProblemSelected = false;
                gblnVendorSelected = false;
                btnProcess.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Send Vehicle To Shop // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrWorkOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intBJCNumber;
            DataGrid WorkOrderGrid;
            DataGridRow WorkRow;
            DataGridCell ProblemID;
            string strProblemID;
            DataGridCell Problem;
            DataGridCell BJCNumber;
            string strBJCNumber;

            try
            {
                intSelectedIndex = dgrWorkOrders.SelectedIndex;

                if(intSelectedIndex > -1)
                {
                    WorkOrderGrid = dgrWorkOrders;
                    WorkRow = (DataGridRow)WorkOrderGrid.ItemContainerGenerator.ContainerFromIndex(WorkOrderGrid.SelectedIndex);
                    ProblemID = (DataGridCell)WorkOrderGrid.Columns[0].GetCellContent(WorkRow).Parent;
                    BJCNumber = (DataGridCell)WorkOrderGrid.Columns[1].GetCellContent(WorkRow).Parent;
                    Problem = (DataGridCell)WorkOrderGrid.Columns[3].GetCellContent(WorkRow).Parent;

                    //converting to String
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    strBJCNumber = ((TextBlock)BJCNumber.Content).Text;
                    gstrProblem = ((TextBlock)Problem.Content).Text;

                    intBJCNumber = Convert.ToInt32(strBJCNumber);
                    MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                    TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                    MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                    gblnProblemSelected = true;

                    if((gblnProblemSelected == true) && (gblnVendorSelected == true))
                    {
                        btnProcess.IsEnabled = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Send Vehicle To Shop // Work Order Selection Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrVendors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            DataGrid VendorGrid;
            DataGridRow VendorRow;
            DataGridCell VendorID;
            string strVendorID;
            
            try
            {
                VendorGrid = dgrVendors;
                VendorRow = (DataGridRow)VendorGrid.ItemContainerGenerator.ContainerFromIndex(VendorGrid.SelectedIndex);
                VendorID = (DataGridCell)VendorGrid.Columns[0].GetCellContent(VendorRow).Parent;
                strVendorID = ((TextBlock)VendorID.Content).Text;

                MainWindow.gintVendorID = Convert.ToInt32(strVendorID);

                gblnVendorSelected = true;

                if ((gblnProblemSelected == true) && (gblnVendorSelected == true))
                {
                    btnProcess.IsEnabled = true;

                    
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry //Send Vehicle To Shop // Vendors Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will add the vehicle in the shop
            bool blnFatalError = false;
            int intRecordsReturned;
            int intVendorID;
            bool blnAddRecord = false;
            int intTransactionID;
            DateTime datTransactionDate = DateTime.Now;

            //checking to see if the vehicle is in the shop already
            TheFindVehiclesInShopByVehicleIDDataSet = TheVehiclesInShopClass.FindVehiclesInShopByVehicleID(MainWindow.gintVehicleID);

            intRecordsReturned = TheFindVehiclesInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID.Rows.Count;

            if(intRecordsReturned > 0)
            {
                intVendorID = TheFindVehiclesInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID[0].VendorID;

                if(intVendorID == MainWindow.gintVendorID)
                {
                    TheMessagesClass.InformationMessage("The Vehicle is Already At This Vehicle");

                    blnAddRecord = false;
                }
                else
                {
                    intTransactionID = TheFindVehiclesInShopByVehicleIDDataSet.FindVehiclesInShopByVehicleID[0].TransactionID;

                    blnFatalError = TheVehiclesInShopClass.RemoveVehicleInShop(intTransactionID);

                    blnAddRecord = true;
                }
            }
            else if (intRecordsReturned == 0)
            {
                blnAddRecord = true;
            }

            //adding the record
            if(blnAddRecord == true)
            {
                blnFatalError = TheVehiclesInShopClass.InsertVehicleInShop(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gintVendorID, gstrProblem);

                if (blnFatalError == false)
                    blnFatalError = TheVehicleStatusClass.UpdateVehicleStatus(MainWindow.gintVehicleID, "DOWN", DateTime.Now);

                if (blnFatalError == false)
                    blnFatalError = TheVehicleProblemsClass.UpdateVehicleProblemVendorID(MainWindow.gintProblemID, MainWindow.gintVendorID);

                if (blnFatalError == false)
                    blnFatalError = TheVehicleProblemsClass.InsertVehicleProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, "VEHICLE SENT TO SHOP", DateTime.Now);

                if(blnFatalError == false)
                {
                    TheMessagesClass.InformationMessage("The Vehicle Is In The Shop");
                }
            }

            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("There Has Been A Problem, Contact ID");
            }
        }
    }
}
