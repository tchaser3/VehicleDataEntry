/* Title:           Problem Menu
 * Date:            6-30-17
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

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for ProblemMenu.xaml
    /// </summary>
    public partial class ProblemMenu : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehicleClass TheVehicleClass = new VehicleClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleProblemClass TheVehicleProblemClass = new VehicleProblemClass();

        FindActiveVehicleByBJCNumberDataSet TheFindActiveVehicleByBJCNumberDataSet = new FindActiveVehicleByBJCNumberDataSet();

        public ProblemMenu()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnPreventativeMainetance_Click(object sender, RoutedEventArgs e)
        {
            PreventativeMaintenance PreventativeMaintenance = new PreventativeMaintenance();
            PreventativeMaintenance.Show();
            Close();
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

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About About = new About();
            About.ShowDialog();
        }

        private void btnUpdateVehicleProblem_Click(object sender, RoutedEventArgs e)
        {
            DisplayOpenWorkOrders DisplayOpenWorkOrders = new DisplayOpenWorkOrders();
            DisplayOpenWorkOrders.Show();
            Close();
        }

        private void btnNewVehicleProblem_Click(object sender, RoutedEventArgs e)
        {
            NewVehicleProblem NewVehicleProblem = new NewVehicleProblem();
            NewVehicleProblem.Show();
            Close();
        }

        private void btnSendVehicleToShop_Click(object sender, RoutedEventArgs e)
        {
            SendVehicleToShop SendVehicleToShop = new SendVehicleToShop();
            SendVehicleToShop.Show();
            Close();
        }

        private void btnViewVehiclesInShop_Click(object sender, RoutedEventArgs e)
        {
            FindVehiclesInShopForm FindVehiclesInShopForm = new FindVehiclesInShopForm();
            FindVehiclesInShopForm.Show();
            Close();
        }

        private void btnVehiclesNeedingService_Click(object sender, RoutedEventArgs e)
        {
            VehicleNeedingService VehicleNeedingService = new VehicleNeedingService();
            VehicleNeedingService.Show();
            Close();
        }
    }
}
