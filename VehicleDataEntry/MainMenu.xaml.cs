/* Title:           Main Menu
 * Date:            4-18-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is the Main Menu */

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

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        //setting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
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

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About About = new About();
            About.ShowDialog();
        }

        private void btnCreateNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            /*CreateEditVehicle CreateEditVehicle = new CreateEditVehicle();
            CreateEditVehicle.Show();
            Close();*/
        }

        private void btnDailyInspection_Click(object sender, RoutedEventArgs e)
        {
            DailyVehicleInspection DailyVehicleInspection = new DailyVehicleInspection();
            DailyVehicleInspection.Show();
            Close();
        }

        private void btnVehiclesInYard_Click(object sender, RoutedEventArgs e)
        {
            VehicleInYardForm VehicleInYardForm = new VehicleInYardForm();
            VehicleInYardForm.Show();
            Close();
        }

        private void btnVehicleAssignment_Click(object sender, RoutedEventArgs e)
        {
            /*VehicleAssignment VehicleAssignment = new VehicleAssignment();
            VehicleAssignment.Show();
            Close();*/
        }

        private void btnWeeklyInspection_Click(object sender, RoutedEventArgs e)
        {
            /*EnterWeeklyInspection EnterWeeklyInspection = new EnterWeeklyInspection();
            EnterWeeklyInspection.Show();
            Close();*/
        }

        private void btnVehicleToolAssignment_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.UnderDevelopment();
        }

        private void btnProblemMenu_Click(object sender, RoutedEventArgs e)
        {
            ProblemMenu ProblemMenu = new ProblemMenu();
            ProblemMenu.Show();
            Close();
        }

        private void btnVehicleGPSDotInfo_Click(object sender, RoutedEventArgs e)
        {
            VehicleGPSDOTInfo VehicleGPSDOTInfo = new VehicleGPSDOTInfo();
            VehicleGPSDOTInfo.Show();
            Close();
        }

        private void btnAddDOTStatus_Click(object sender, RoutedEventArgs e)
        {
            AddDOTStatus AddDOTStatus = new AddDOTStatus();
            AddDOTStatus.Show();
            Close();
        }

        private void btnAddGPSStatus_Click(object sender, RoutedEventArgs e)
        {
            AddGPSStatus AddGPSStatus = new AddGPSStatus();
            AddGPSStatus.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnCreateNewVehicle.IsEnabled = false;
            btnVehicleAssignment.IsEnabled = false;
            btnWeeklyInspection.IsEnabled = false;
            btnDailyInspection.IsEnabled = false;
            btnVehiclesInYard.IsEnabled = false;
        }
    }
}
