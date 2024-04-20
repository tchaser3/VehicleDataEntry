/* Title:           Select Vendor
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
using VendorsDLL;

namespace VehicleDataEntry
{
    /// <summary>
    /// Interaction logic for SelectVendor.xaml
    /// </summary>
    public partial class SelectVendor : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VendorsClass TheVendorsClass = new VendorsClass();

        FindVendorsSortedByVendorNameDataSet TheFindVendorsSortedByVendorNameDataSet = new FindVendorsSortedByVendorNameDataSet();

        public SelectVendor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the grid
            TheFindVendorsSortedByVendorNameDataSet = TheVendorsClass.FindVendorsSortedByVendorName();

            dgrVendors.ItemsSource = TheFindVendorsSortedByVendorNameDataSet.FindVendorsSortedByVendorName;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void dgrVendors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            DataGrid dgrVendorsDisplayed;
            DataGridRow VendorRow;
            DataGridCell VendorID;
            string strVendorID;

            try
            {
                dgrVendorsDisplayed = dgrVendors;
                VendorRow = (DataGridRow)dgrVendorsDisplayed.ItemContainerGenerator.ContainerFromIndex(dgrVendorsDisplayed.SelectedIndex);
                VendorID = (DataGridCell)dgrVendorsDisplayed.Columns[0].GetCellContent(VendorRow).Parent;
                strVendorID = ((TextBlock)VendorID.Content).Text;

                MainWindow.gintVendorID = Convert.ToInt32(strVendorID);

                Close();
               
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Select Vendor // Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
