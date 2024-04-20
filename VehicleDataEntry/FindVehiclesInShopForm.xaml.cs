/* Title:           Find Vehicles In Shop
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
using VehiclesInShopDLL;
using NewEventLogDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for FindVehiclesInShopForm.xaml
    /// </summary>
    public partial class FindVehiclesInShopForm : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        VehiclesInShopClass TheVehiclesInShopClass = new VehiclesInShopClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindAllVehiclesInShopDataSet TheFindAllVehiclesInShopDataSet = new FindAllVehiclesInShopDataSet();

        public FindVehiclesInShopForm()
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
            TheFindAllVehiclesInShopDataSet = TheVehiclesInShopClass.FindAllVehiclesInShop();

            dgrVehicles.ItemsSource = TheFindAllVehiclesInShopDataSet.FindAllVehiclesInShop;
        }
    }
}
