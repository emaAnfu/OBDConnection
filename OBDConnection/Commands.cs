using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OBDConnection
{
    /* In this class you can find a list of some commands in ASCII */
    public class Commands
    {
        /* Member variables */

        List<string> myATCommands;
        List<string> myOBDCommands;

        /* List of commands */

        /* Command related to the ELM327 chip */
        public static string AT_Reset = "AT Z";
        public static string AT_Repeat = "AT \r";
        public static string AT_ECO_Off = "AT E0";
        public static string AT_ECO_On = "AT E1";
        public static string AT_Linefeeds_Off = "AT L0";
        public static string AT_Linefeeds_On = "AT L1";
        public static string AT_AutomaticProtocol = "AT SP0";
        public static string AT_ReadVoltage = "AT RV";

        /* Actual OBD commands */
        public static string OBD_rpmCommand = "01 0C";
        public static string OBD_speedCommand = "01 0D";

        /* Properties */

        public List<string> ATCommands
        {
            get
            {
                return myATCommands;
            }
            set
            {
                myATCommands = value;
            }
        }

        public List<string> OBDCommands
        {
            get
            {
                return myOBDCommands;
            }
            set
            {
                myOBDCommands = value;
            }
        }

        /* Constructor */
        public Commands()
        {
            myATCommands = new List<string>();
            myOBDCommands = new List<string>();
        }

        /* Private functions */

        /// <summary>
        /// Initialize the commands list with some known commands (known at compile time)
        /// Note: remember that all commands sent to ELM327 must be terminatedwith a carriage 
        /// return character -> hex 0D
        /// </summary>
        private void InitializeCommands()
        {
            /* Command related to the ELM327 chip */
            string AT_Reset = "AT Z";
            string AT_Repeat = "AT \r";
            string AT_ECO_Off = "AT E0";
            string AT_ECO_On = "AT E1";
            string AT_Linefeeds_Off = "AT L0";
            string AT_Linefeeds_On = "AT L1";
            string AT_AutomaticProtocol = "AT SP0";
            string AT_ReadVoltage = "AT RV";

            /* Actual OBD commands */
            string OBD_rpmCommand = "01 0C";
            string OBD_speedCommand = "01 0D";
        }
    }
}