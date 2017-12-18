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
using Android;

namespace OBDConnection
{
    /// <summary>
    /// An object of this class will be used to record and (at the end of sessions) store the
    /// acquired data. Each signal will have its own dataStorage and so its own file on memory.
    /// </summary>
    public class DataStorage
    {
        List<string> linesToWrite;
        string nameFile;

        public List<double> timeOfResponses;

        public List<string> LinesToWrite
        {
            get { return linesToWrite; }
        }

        public string NameFile
        {
            get { return nameFile; }
        }

        /// <summary>
        /// Constructor for the dataStorage for a particular signal.
        /// </summary>
        /// <param name="nameFile">
        /// Name of the txt file stored in Documents.</param>
        public DataStorage(string nameFile = "OBDConnection")
        {
            linesToWrite = new List<string>();
            this.nameFile = String.Format("{0}_{1:dd-MM-yy_HH:mm:ss}.txt", nameFile, DateTime.Now);
            timeOfResponses = new List<double>();
        }

        /// <summary>
        /// To add a line to the list<string> that will be saved with the Save() method.
        /// </summary>
        /// <param name="line"></param>
        public void AppendLine(string line)
        {
            linesToWrite.Add(line);
        }

        /// <summary>
        /// Putting desired data in a file stored in Documents.
        /// </summary>
        public void Save()
        {
            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            var directoryPath = directory.AbsolutePath + "/OBDConnection";
            var filename = System.IO.Path.Combine(directoryPath, nameFile);
                    
            // Write
            using (var streamWriter = new StreamWriter(filename, true))
            {
                // writing measures
                foreach(string s in linesToWrite)
                {
                    streamWriter.WriteLine(s);
                }
                // writing deltaT for each single measure
                streamWriter.WriteLine("Time of responses: ");
                double sum = 0;
                foreach(double i in timeOfResponses)
                {
                    sum += i;
                    streamWriter.WriteLine(i);
                }
                // writing mean deltaT
                streamWriter.WriteLine("Mean time: " + (sum / timeOfResponses.Count));
            }
        }
    }
}