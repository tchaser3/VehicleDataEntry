/* Title:           Select Existing Work Order
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

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for SelectExistingWorkOrder.xaml
    /// </summary>
    public partial class SelectExistingWorkOrder : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        public SelectExistingWorkOrder()
        {
            InitializeComponent();
        }

        private void dgrWorkOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProblemID;
            string strProblemID;

            try
            {
                //setting local variable
                dataGrid = dgrWorkOrders;
                selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                ProblemID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                strProblemID = ((TextBlock)ProblemID.Content).Text;

                //find the record
                MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                MainWindow.gblnWorkOrderSelected = true;

                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Select Existing Work Order // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //loading the grid
            MainWindow.gintProblemID = 0;

            dgrWorkOrders.ItemsSource = MainWindow.TheFindOPenVehiclesByVehicleIDDataSet.FindOpenVehicleProblemsByVehicleID;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.gintProblemID == 0)
            {
                TheMessagesClass.ErrorMessage("Work Order Not Selected");
            }
            else
            {
                Close();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
