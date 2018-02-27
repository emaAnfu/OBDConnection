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
        // Samples counter
        int samplesCounter;
        // Used to monitor the state of this thread from the main activity
        public bool isRunning = false;

        // To storage data
        DataStorage RPMstorage_raw;
        DataStorage SpeedStorage_raw;
        DataStorage TimeOfResponses;

        // To measure time responses
        DateTime begin;
        DateTime end;
        TimeSpan elapsedSpan;

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
            TimeOfResponses = new DataStorage("time_of_response");
        }

        public override void Run()
        {
            // A new session of measurement starts
            RPMstorage_raw.AppendLine("Start measure");
            SpeedStorage_raw.AppendLine("Start measure");
            TimeOfResponses.AppendLine("Start measure");
            samplesCounter = 0;
            isRunning = true;
            // Take 1000 samples (see documentations for details)
            while (samplesCounter < 1000)
            {
                begin = DateTime.Now;
                RequestRPM();
                RequestSpeed();
                end = DateTime.Now;
                elapsedSpan = end - begin;
                TimeOfResponses.AppendDouble(elapsedSpan.TotalMilliseconds);
                samplesCounter++;
            }
            Cancel();
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

        private void Cancel()
        {
            // Close measure session in files
            RPMstorage_raw.AppendLine("Finish measure");
            SpeedStorage_raw.AppendLine("Finish measure");
            // Storage data
            RPMstorage_raw.Save();
            SpeedStorage_raw.Save();
            // Manage time of responses file
            CloseTimeMeasuringFile();
            // Update status
            Toast.MakeText(Application.Context, "Session finish.", ToastLength.Long).Show();
            isRunning = false;
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

        /// <summary>
        /// To manage the closing of file with the samples time storing
        /// </summary>
        private void CloseTimeMeasuringFile()
        {
            // to write each single measure time
            TimeOfResponses.CopyDoubleToLines();
            TimeOfResponses.AppendLine("Mean time of response:");
            // computing mean response time
            double sum = 0;
            foreach (double i in TimeOfResponses.DoubleToWrite)
            {
                sum += i;
            }
            TimeOfResponses.AppendLine((sum / TimeOfResponses.DoubleToWrite.Count).ToString());
            TimeOfResponses.AppendLine("Finish measure.");
            TimeOfResponses.Save();
        }
    }
}
