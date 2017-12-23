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
using Java.Lang;

namespace OBDConnection
{
    /// <summary>
    /// This thread handles the continuous data request to the ELM 327 and, when stopped,
    /// save data on file.
    /// </summary>
    public class RequestCommandThread : Thread
    {
        // For bt connection handling
        OBDConnectionService connectionService;
        // Used to destroy thread
        bool active = true;

        // To storage data
        DataStorage RPMstorage_raw;
        DataStorage SpeedStorage_raw;

        /// <summary>
        /// Create the thread which, when it starts, handles periodic request and data storage.
        /// </summary>
        /// <param name="cs">ConnectionService to manage write and read to/from bt socket.</param>
        public RequestCommandThread(OBDConnectionService cs)
        {
            this.Name = "RequestCommandThread";
            connectionService = cs;
            RPMstorage_raw = new DataStorage("rpm_measure");
            SpeedStorage_raw = new DataStorage("speed_measure");
        }

        public override void Run()
        {
            // A new session of measurement starts
            RPMstorage_raw.AppendLine("Start measure");
            SpeedStorage_raw.AppendLine("Start measure");
            // Keep asking data until the thread is destroyed
            while (active)
            {
                DateTime begin = DateTime.Now;
                RequestRPM();
                RequestSpeed();
                DateTime end = DateTime.Now;
                TimeSpan elapsedSpan = end - begin;
                Thread.Sleep(-200);
            }
        }

        private void RequestRPM()
        {
            // Sending command
            SendCommand(ListOfCommands.OBD_rpmCommand);
            // Read response, add it in RAM 
            string s = connectionService.Read();
            RPMstorage_raw.AppendLine(s);
        }

        private void RequestSpeed()
        {
            // Sending command
            SendCommand(ListOfCommands.OBD_speedCommand);
            // Read response, add it in RAM
            string s = connectionService.Read();
            SpeedStorage_raw.AppendLine(s);
        }

        /// <summary>
        /// Call this function to measure RPM and store response time of each reqeust.
        /// Useful to do some analysis
        /// </summary>
        private void RequestRPMAndMeasureTime()
        {
            // Sending command
            SendCommand(ListOfCommands.OBD_rpmCommand);
            // Read response, add it in RAM and measure time
            DateTime begin = DateTime.Now;
            string s = connectionService.Read();
            RPMstorage_raw.AppendLine(s);
            DateTime end = DateTime.Now;
            TimeSpan elapsedSpan = end - begin;
            RPMstorage_raw.timeOfResponses.Add(elapsedSpan.TotalMilliseconds);
        }

        /// <summary>
        /// Call this function to measure speed and store response time of each reqeust.
        /// Useful to do some analysis
        /// </summary>
        private void RequestSpeedAndMeasureTime()
        {
            // Sending command
            SendCommand(ListOfCommands.OBD_speedCommand);
            // Read response, add it in RAM and measure time
            DateTime begin = DateTime.Now;
            string s = connectionService.Read();
            SpeedStorage_raw.AppendLine(s);
            DateTime end = DateTime.Now;
            TimeSpan elapsedSpan = end - begin;
            SpeedStorage_raw.timeOfResponses.Add(elapsedSpan.TotalMilliseconds);
        }

        public void Cancel()
        {
            // Storage data
            RPMstorage_raw.Save();
            SpeedStorage_raw.Save();

            // Destroy this thread
            active = false;
        }

        /// <summary>
        /// Sends a command.
        /// </summary>
        /// <param name="cmd">
        /// The command to send
        /// </param>
        public void SendCommand(string cmd)
        {
            // adds the carriage return character
            var message = new Java.Lang.String(cmd + "\r");
            SendMessage(message);
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name='message'>
        /// A string of text to send.
        /// </param>
        private void SendMessage(Java.Lang.String message)
        {
            // Check that we're actually connected before trying anything
            if (connectionService.GetState() != OBDConnectionService.STATE_CONNECTED)
            {
                Toast.MakeText(Application.Context, Resource.String.not_connected, ToastLength.Short).Show();
                return;
            }

            // Check that there's actually something to send
            if (message.Length() > 0)
            {
                // Get the message bytes and tell the BluetoothConnectionService to write
                byte[] send = message.GetBytes();
                connectionService.Write(send);
            }
        }
    }
}
