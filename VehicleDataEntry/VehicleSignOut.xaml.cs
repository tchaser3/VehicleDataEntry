/* Title:           Vehicle Sign Out
 * Date:            4-18-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used for signing a vehicle in or out */

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
    /// Interaction logic for VehicleSignOut.xaml
    /// </summary>
    public partial class VehicleSignOut : Window
    {
        public VehicleSignOut()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
