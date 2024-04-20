/* Title:           Display Open Work Orders
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
using NewVehicleDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for DisplayOpenWorkOrders.xaml
    /// </summary>
    public partial class DisplayOpenWorkOrders : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        VehicleClass TheVehicleClass = new VehicleClass();

        FindOpenVehicleProblemsDataSet TheFindOpenVehicleProblemsDataSet = new FindOpenVehicleProblemsDataSet();
        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();
        FindVehicleProblemByProblemIDDataSet TheFindVehicleProblemByProblemIDDataSet = new FindVehicleProblemByProblemIDDataSet();

        public DisplayOpenWorkOrders()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the grid
            try
            {
                TheFindOpenVehicleProblemsDataSet = TheVehicleProblemClass.FindOpenVehicleProblems();

                dgrProblems.ItemsSource = TheFindOpenVehicleProblemsDataSet.FindOpenVehicleProblems;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Display Open Work Orders // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrProblems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variable
            int intSelectedIndex;
            int intBJCNumber;
            string strProblemID;
            string strBJCNumber;
            
            try
            {
                intSelectedIndex = dgrProblems.SelectedIndex;

                DataGrid dataGrid = dgrProblems;
                DataGridRow Row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                DataGridCell RowAndColumn = (DataGridCell)dataGrid.Columns[0].GetCellContent(Row).Parent;
                DataGridCell BJCNumber = (DataGridCell)dataGrid.Columns[1].GetCellContent(Row).Parent;
                strProblemID = ((TextBlock)RowAndColumn.Content).Text;
                strBJCNumber = ((TextBlock)BJCNumber.Content).Text;

                MainWindow.gintProblemID = Convert.ToInt32(strProblemID);

                TheFindVehicleProblemByProblemIDDataSet = TheVehicleProblemClass.FindVehicleProblemByProblemID(MainWindow.gintProblemID);

                intBJCNumber = Convert.ToInt32(strBJCNumber);

                TheFindActiveVehicleByBJCNumberDataSet = TheVehicleClass.FindActiveVehicleByBJCNumber(intBJCNumber);

                MainWindow.gintVehicleID = TheFindActiveVehicleByBJCNumberDataSet.FindActiveVehicleByBJCNumber[0].VehicleID;

                MainWindow.TheFindOPenVehiclesByVehicleIDDataSet = TheVehicleProblemClass.FindOpenVehicleProblemsbyVehicleID(MainWindow.gintVehicleID);

                UpdateVehicleProblem UpdateVehicleProblem = new UpdateVehicleProblem();
                UpdateVehicleProblem.Show();
                Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Display Open Work Orders // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
