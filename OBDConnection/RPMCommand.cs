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
    /// <summary>
    /// Displays the current engine revolutions per minute (RPM).
    /// </summary>
    public class RPMCommand : OBDCommand
    {
        private int rpm = -1;

        /// <summary>
        /// Constructors.
        /// </summary>
        /// <param name="cs"></param>
        public RPMCommand() : base(ListOfCommands.OBD_rpmCommand)
        {
        }

        protected override void PerformCalculations()
        {
            // ignore first two bytes [41 0C] of the response((A*256)+B)/4
            rpm = (buffer[2] * 256 + buffer[3]) / 4;
        }

        public override string GetFormattedResult()
        {
            return String.Format("{0} {1}", rpm, GetResultUnit());
        }

        public override string GetCalculatedResult()
        {
            return rpm.ToString();
        }

        public override string GetResultUnit()
        {
            return "RPM";
        }

        public override string GetName()
        {
            return nameof(ListOfCommands.OBD_rpmCommand);
        }

        public int getRPM()
        {
            return rpm;
        }
    }
}