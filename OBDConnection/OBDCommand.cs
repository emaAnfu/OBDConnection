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
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.RegularExpressions;

namespace OBDConnection
{
    public abstract class OBDCommand
    {
        /* Member variables */

        protected List<int> buffer = null;
        protected String cmd = null;
        protected bool useImperialUnits = false;
        protected String rawData = null;
        protected TimeSpan responseDelayInMs = new TimeSpan();
        private int start;
        private int end;

        /* Constructors */

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="command"></param>
        public OBDCommand(String command)
        {
            this.cmd = command;
            this.buffer = new List<int>();
        }

        /* Member functions */

        /// <summary>
        /// Sends the OBD-II request and deals with the response.
        /// Note about "MethodImplOptions.Synchronized": The method can be executed by only one thread at a time. 
        /// Static methods lock on the type, whereas instance methods lock on the instance. 
        /// Only one thread can execute in any of the instance functions, and only one thread can 
        /// execute in any of a class's static functions.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Run(OBDConnectionService cs, byte[] receivedBuffer, int bufLen)
        {
            start = System.Environment.TickCount;
            SendCommand(cs);
            ReadResult(receivedBuffer, bufLen);
            end = System.Environment.TickCount;
        }

        /// <summary>
        /// Sends the OBD-II request.
        /// </summary>
        /// <param name="outputBuffer"></param>
        protected void SendCommand(OBDConnectionService connectionService)
        {
            // Check that we're actually connected before trying anything
            if (connectionService.GetState() != OBDConnectionService.STATE_CONNECTED)
            {
                Toast.MakeText(Application.Context, Resource.String.not_connected, ToastLength.Short).Show();
                return;
            }
            // adds the carriage return character
            var message = new Java.Lang.String(cmd + "\r");
            // Get the message bytes and tell the BluetoothConnectionService to write
            byte[] send = message.GetBytes();
            connectionService.Write(send);
            // Sleep Thread cause it has done its job (to check)
            if (responseDelayInMs != null && responseDelayInMs.Milliseconds > 0)
            {
                Thread.Sleep(responseDelayInMs.Milliseconds);
            }
        }

        /// <summary>
        /// Re-sends the OBD-II request.
        /// </summary>
        /// <param name="outputBuffer"></param>
        protected void ResendCommand(OBDConnectionService connectionService)
        {
            // Check that we're actually connected before trying anything
            if (connectionService.GetState() != OBDConnectionService.STATE_CONNECTED)
            {
                Toast.MakeText(Application.Context, Resource.String.not_connected, ToastLength.Short).Show();
                return;
            }
            // adds the carriage return character
            var message = new Java.Lang.String("\r");
            // Get the message bytes and tell the BluetoothConnectionService to write
            byte[] send = message.GetBytes();
            connectionService.Write(send);
            // Sleep Thread cause it has done its job (to check)
            if (responseDelayInMs != null && responseDelayInMs.Milliseconds > 0)
            {
                Thread.Sleep(responseDelayInMs.Milliseconds);
            }
        }

        /// <summary>
        /// Reads the OBD-II response
        /// </summary>
        //protected void ReadResult(Stream inputStream)
        protected void ReadResult(byte[] bufferIn, int bufLen)
        {
            ReadRawData(bufferIn, bufLen);
            //CheckForErrors();
            FillBuffer();
            PerformCalculations();
        }

        /// <summary>
        ///  This method exists so that for each command, there must be a method that is
        ///  called only once to perform calculations.
        /// </summary>
        protected abstract void PerformCalculations();

        // Patterns
        private static string WHITESPACE_PATTERN = "\\s";
        private static string BUSINIT_PATTERN = "(BUS INIT)|(BUSINIT)|(\\.)";
        private static string SEARCHING_PATTERN = "SEARCHING";
        private static string DIGITS_LETTERS_PATTERN = "([0-9A-F])+";

        protected String ReplaceAll(string pattern, string input, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        protected String RemoveAll(string pattern, string input)
        {            
            return Regex.Replace(input, pattern, "");
        }

        protected void FillBuffer()
        {
            rawData = RemoveAll(WHITESPACE_PATTERN, rawData);
            rawData = RemoveAll(BUSINIT_PATTERN, rawData);

            if (!Regex.IsMatch(rawData, DIGITS_LETTERS_PATTERN))
            {
                //throw new NonNumericResponseException(rawData);
            }

            // read string each two chars
            buffer.Clear();
            int begin = 0;
            int end = 2;
            while (end <= rawData.Length)
            {
                buffer.Add(Convert.ToInt32(rawData.Substring(begin, 2), 16));                
                begin = end;
                end += 2;
            }
        }

        /// <summary>
        /// Read raw data
        /// </summary>
        /// <param name="inputStream"></param>
        // protected void ReadRawData(Stream inputStream)
        protected void ReadRawData(byte[] bufferIn, int bufLen)
        {
            //byte b = 0;
            StringBuilder sb = new StringBuilder();

            // read until '>' arrives OR end of stream reached
            char c;
            //// -1 if the end of the stream is reached
            //while ((b=(sbyte)inputStream.ReadByte()) > -1)
            //foreach(byte b in bufferIn)
            for (int i=0; i<bufLen; i++)
            {
                c = (char)bufferIn[i];
                if (c == '>') // read until '>' arrives
                {
                    break;
                }
                sb.Append(c);
            }

            /* Imagine the following response 41 0c 00 0d.
             * ELM sends strings!!So, ELM puts spaces between each "byte". Pay
             * attention: 41 is actually TWO bytes (two chars) in the socket. 
             */
            rawData = RemoveAll(SEARCHING_PATTERN, sb.ToString());

            /* Data may have echo or informative text like "INIT BUS..." or similar.
             * The response ends with two carriage return characters. So we need to take
             * everything from the last carriage return before those two (trimmed above). 
             */
            //kills multiline.. rawData = rawData.Substring(rawData.LastIndexOf(13) + 1);
            rawData = RemoveAll(WHITESPACE_PATTERN, rawData); //removes all [ \t\n\x0B\f\r]
        }

        /// <summary>        
        /// </summary>
        /// <returns>
        /// Return the raw command response in string representation.
        /// </returns>
        public String GetResult()
        {
            return rawData;
        }

        /// <summary>        
        /// </summary>
        /// <returns>
        /// Return a formatted command response in string representation.
        /// </returns>
        public abstract String GetFormattedResult();

        /// <summary>        
        /// </summary>
        /// <returns>
        /// Return the command response in string representation, without formatting.
        /// </returns>
        public abstract String GetCalculatedResult();

        /// <summary>
        /// Property of the field buffer. Return a list of integers.
        /// </summary>
        protected List<int> Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// return true if imperial units are used, or false otherwise
        /// </returns>
        public bool UseImperialUnits()
        {
            return useImperialUnits;
        }

        /// <summary>
        /// The unit of the result, as used in {@link #GetFormattedResult()}
        /// </summary>
        /// <returns>
        /// return a String representing a unit or "", never null
        /// </returns>
        public virtual String GetResultUnit()
        {
            return "";//no unit by default
        }

        /// <summary>
        /// Set to 'true' if you want to use imperial units, false otherwise. 
        /// By default this value is set to 'false'.
        /// </summary>
        /// <param name="isImperial"></param>
        public void UseImperialUnits(bool isImperial)
        {
            this.useImperialUnits = isImperial;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// return the OBD command name.
        /// </returns>
        public abstract String GetName();

        /// <summary>
        /// Time the command waits before returning from #sendCommand()
        /// </summary>
        /// <returns> Delay in ms. </returns>
        public TimeSpan ResponseDelayInMs
        {
            get { return responseDelayInMs; }
            set { responseDelayInMs = value; }
        }

        //fixme resultunit (?)
        /// <summary>
        /// Property
        /// </summary>
        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// Property
        /// </summary>
        public int End
        {
            get { return end; }
            set { end = value; }
        }

        /// <summary>
        /// Response is made of two char for the mode follow by the PID
        /// @since 1.0-RC12 (?)
        /// </summary>
        /// <returns></returns>
        public String GetCommandPID()
        {
            return cmd.Substring(3);   
        }

        /// <summary>
        /// Two char for the mode
        /// </summary>
        /// <returns></returns>
        public String GetCommandMode()
        {
            if (cmd.Length >= 2)
            {
                return cmd.Substring(0, 2);
            }
            else
            {
                return cmd;
            }
        }

    }
}
 
 
 
 
 
 
 