/* Title:           Auto Sign In Class
 * Date:            5-23-17
 * Author:          Terry Holmes
 * 
 * Description:     This class is designed to auto log in the vehicles */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEventLogDLL;
using NewEmployeeDLL;
using DateSearchDLL;
using VehicleStatusDLL;

namespace VehicleDataEntry
{
    class AutoSignInClass
    {
        //setting up the classes
        VehicleStatusClass TheVehicleStatusClass = new VehicleStatusClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //Setting up the Vehicle DataSet
        VehicleStatusDataSet TheVehicleStatusDataSet = new VehicleStatusDataSet();

        public bool AutoSignInVehicles()
        {
            //setting local variables
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            DateTime datTransactionDate = DateTime.Now;
            DateTime datStatusDate;

            try
            {
                //getting the data
                TheVehicleStatusDataSet = TheVehicleStatusClass.GetVehicleStatusInfo();

                intNumberOfRecords = TheVehicleStatusDataSet.vehiclestatus.Rows.Count - 1;

                datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    datStatusDate = TheDateSearchClass.RemoveTime(TheVehicleStatusDataSet.vehiclestatus[intCounter].TransactionDate);

                    if(datStatusDate < datTransactionDate)
                    {
                        if(TheVehicleStatusDataSet.vehiclestatus[intCounter].VehicleStatus == "SIGNED OUT")
                        {
                            TheVehicleStatusDataSet.vehiclestatus[intCounter].VehicleStatus = "AVAILABLE";
                            TheVehicleStatusDataSet.vehiclestatus[intCounter].TransactionDate = datTransactionDate;
                            TheVehicleStatusClass.UpdateVehicleStatusDB(TheVehicleStatusDataSet);
                        }
                    }
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Vehicle Data Entry // Auto Sign in Class // Auto Sign In Vehicles " + Ex.Message);

                blnFatalError = true;
            }

            return blnFatalError;
        }
    }
}
