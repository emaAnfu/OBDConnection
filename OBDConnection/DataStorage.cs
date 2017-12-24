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
        // List of data to write
        List<string> linesToWrite;
        // Name of file that will be stored on memory
        string nameFile;
        // List of double if you need to do some processing before converting them to string
        // (not implemented yet)
        List<double> doubleToWrite;

        public List<string> LinesToWrite
        {
            get { return linesToWrite; }
        }

        public string NameFile
        {
            get { return nameFile; }
        }

        public List<double> DoubleToWrite
        {
            get { return doubleToWrite; }
        }

        /// <summary>
        /// Constructor for the dataStorage for a particular signal.
        /// </summary>
        /// <param name="nameFile">
        /// Name of the txt file stored in Documents.</param>
        public DataStorage(string nameFile = "OBDConnection")
        {
            linesToWrite = new List<string>();
            this.nameFile = String.Format("{0:dd-MM-yy_HH-mm-ss}_{1}.txt", DateTime.Now, nameFile);
            doubleToWrite = new List<double>();
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
        /// Call this to copy the list of double to the list of lines
        /// </summary>
        public void CopyDoubleToLines()
        {
            foreach (double d in doubleToWrite)
            {
                linesToWrite.Add(d.ToString());
            }
        }

        /// <summary>
        /// To add a double to the list<double> 
        /// </summary>
        /// <param name="d"></param>
        public void AppendDouble(double d)
        {
            doubleToWrite.Add(d);
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
                // writing lines
                foreach(string s in linesToWrite)
                {
                    streamWriter.WriteLine(s);
                }
            }
        }
    }
}