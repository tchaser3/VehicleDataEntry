/* Title:           Vehicle Inspection Problems
 * Date:            6-22-17
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
using InspectionsDLL;
using VehicleProblemsDLL;
using NewEventLogDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleInspectionProblem.xaml
    /// </summary>
    public partial class VehicleInspectionProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InspectionsClass TheInspectionClass = new InspectionsClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        FindVehicleProblemByDateMatchDataSet TheFindVehicleProblemByDateMatchDataSet = new FindVehicleProblemByDateMatchDataSet();
        FindOpenVehicleProblemsByVehicleIDDataSet TheFindOpenVehicleProblemsByVehicleIDDataSet = new FindOpenVehicleProblemsByVehicleIDDataSet();
        ExistingOpenProblemsDataSet TheExistingOpenProblemsDataSet = new ExistingOpenProblemsDataSet();
        FindVehicleProblemByProblemIDDataSet TheFindVehicleProblemByProblemIDDataSet = new FindVehicleProblemByProblemIDDataSet();

        int gintProblemID;
        bool gblnNewWorkOrder;
        bool gblnMultipleOrders;
        int gintMultipleSelectedIndex;
        
        public VehicleInspectionProblem()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting up to load the combo box
            gintProblemID = 0;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //HideTextBoxes();

                cboMultipleProblems.Items.Add("Select");
                cboMultipleProblems.Items.Add("Yes");
                cboMultipleProblems.Items.Add("No");
                cboMultipleProblems.SelectedIndex = 0;

                ExistingOpenProblemsDataSet.openordersRow FirstRow = TheExistingOpenProblemsDataSet.openorders.NewopenordersRow();

                FirstRow.ProblemID = -1;
                FirstRow.Problem = "NEW WORK ORDER";

                TheExistingOpenProblemsDataSet.openorders.Rows.Add(FirstRow);

                TheFindOpenVehicleProblemsByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleProblemsbyVehicleID(MainWindow.gintVehicleID);

                intNumberOfRecords = TheFindOpenVehicleProblemsByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        ExistingOpenProblemsDataSet.openordersRow NewOrderRow = TheExistingOpenProblemsDataSet.openorders.NewopenordersRow();

                        NewOrderRow.Problem = TheFindOpenVehicleProblemsByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID[intCounter].Problem;
                        NewOrderRow.ProblemID = TheFindOpenVehicleProblemsByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID[intCounter].ProblemID;

                        TheExistingOpenProblemsDataSet.openorders.Rows.Add(NewOrderRow);
                    }
                }

                dgrWorkOrders.ItemsSource = TheExistingOpenProblemsDataSet.openorders;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle Inspection Problem // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //this will save the transaction
            string strInspectionNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intProblemID = 0;
            DateTime datTransactionDate = DateTime.Now;
            int intLength;

            try
            {
                MainWindow.gstrVehicleProblem = txtVehicleProblem.Text;
                if(gintMultipleSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Multiple Orders Was Not Selected\n";
                }
                if(gblnNewWorkOrder == true)
                {
                    if (MainWindow.gstrVehicleProblem == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Vehicle Problem Was Not Entered\n";
                    }
                }
                
                strInspectionNotes = txtInspectionNotes.Text;
                if (strInspectionNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Inspection Notes Were Not Entered\n";
                }
                else
                {
                    intLength = strInspectionNotes.Length;
                    if(intLength < 10)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Not Enough Information For Notes\n";
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnNewWorkOrder == true)
                {
                    //iserting into table
                    blnFatalError = TheVehicleProblemClass.InsertVehicleProblem(MainWindow.gintVehicleID, datTransactionDate, MainWindow.gstrVehicleProblem);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindVehicleProblemByDateMatchDataSet = TheVehicleProblemClass.FindVehicleProblemByDateMatch(datTransactionDate);

                    gintProblemID = TheFindVehicleProblemByDateMatchDataSet.FindVehicleProblemByDateMatch[0].ProblemID;
                }

                blnFatalError = TheVehicleProblemClass.InsertVehicleProblemUpdate(gintProblemID, MainWindow.TheVerifyLoginDataSet.VerifyLogon[0].EmployeeID, strInspectionNotes, datTransactionDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheInspectionClass.InsertVehicleInspectionProblem(MainWindow.gintVehicleID, MainWindow.gintInspectionID, MainWindow.gstrInspectionType, MainWindow.gstrVehicleProblem, MainWindow.gintOdometerReading, MainWindow.gblnServicable, strInspectionNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Vehicle Has Been Updated");

                if(gblnMultipleOrders == true)
                {
                    txtInspectionNotes.Text = "";
                    txtVehicleProblem.Text = "";
                    //HideTextBoxes();
                    cboMultipleProblems.SelectedIndex = 0;
                }
                else if(gblnMultipleOrders == false)
                {

                    Close();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle Inspection Problem // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrWorkOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid OpenOrderGrid;
            DataGridRow OpenOrderRow;
            DataGridCell ProblemID;
            string strProblemID;
            DataGridCell Problem;
            string strProblem;

            try
            {
                intSelectedIndex = dgrWorkOrders.SelectedIndex;
                //HideTextBoxes();

                if (intSelectedIndex > -1)
                {
                    OpenOrderGrid = dgrWorkOrders;
                    OpenOrderRow = (DataGridRow)OpenOrderGrid.ItemContainerGenerator.ContainerFromIndex(OpenOrderGrid.SelectedIndex);
                    ProblemID = (DataGridCell)OpenOrderGrid.Columns[0].GetCellContent(OpenOrderRow).Parent;
                    strProblemID = ((TextBlock)ProblemID.Content).Text;
                    Problem = (DataGridCell)OpenOrderGrid.Columns[1].GetCellContent(OpenOrderRow).Parent;
                    strProblem = ((TextBlock)ProblemID.Content).Text;

                    gintProblemID = Convert.ToInt32(strProblemID);

                    if(intSelectedIndex > 0)
                    {
                        TheFindVehicleProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleProblemByProblemID(gintProblemID);

                        MainWindow.gstrVehicleProblem = TheFindVehicleProblemByProblemIDDataSet.FindVehicleProblemByProblemID[0].Problem;

                        txtVehicleProblem.Text = MainWindow.gstrVehicleProblem;

                        txtVehicleProblem.IsReadOnly = true;
                    }

                    if (gintProblemID == -1)
                    {
                        txtVehicleProblem.Visibility = Visibility.Visible;
                        gblnNewWorkOrder = true;
                    }
                    else if (gintProblemID > -1)
                    {
                        gblnNewWorkOrder = false;
                    }

                    txtInspectionNotes.Visibility = Visibility.Visible;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Vehicle Inspection Problem // Open Problems Grid " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        /*private void HideTextBoxes()
        {
            txtInspectionNotes.Visibility = Visibility.Hidden;
            txtVehicleProblem.Visibility = Visibility.Hidden;
        }*/

        private void cboMultipleProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gintMultipleSelectedIndex = cboMultipleProblems.SelectedIndex;

            if (gintMultipleSelectedIndex == 1)
                gblnMultipleOrders = true;
            else if (gintMultipleSelectedIndex == 2)
                gblnMultipleOrders = false;
        }
    }
}
