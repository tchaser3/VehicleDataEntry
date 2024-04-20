/* Title:           Vehicle Needing Service
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
using VehicleProblemsDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for VehicleNeedingService.xaml
    /// </summary>
    public partial class VehicleNeedingService : Window
    {
        //Setting up the class
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        FindOpenVehicleProblemsNeedingServiceDataSet TheFindOpenVehicleProblemsNeedingServiceDataSet = new FindOpenVehicleProblemsNeedingServiceDataSet();

        public VehicleNeedingService()
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
            TheFindOpenVehicleProblemsNeedingServiceDataSet = TheVehicleProblemClass.FindOpenVehicleProblemsNeedingService();

            dgrVehicles.ItemsSource = TheFindOpenVehicleProblemsNeedingServiceDataSet.FindOpenVehicleProblemsNeedingService;
        }
    }
}
