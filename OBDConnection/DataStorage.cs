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
        public DataStorage(string nameFile = "OBDConnection.txt")
        {
            linesToWrite = new List<string>();
            this.nameFile = nameFile;
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
            var directoryPath = directory.AbsolutePath;
            var filename = System.IO.Path.Combine(directoryPath, nameFile);
                    
            // Write
            using (var streamWriter = new StreamWriter(filename, true))
            {
                foreach(string s in linesToWrite)
                {
                    streamWriter.WriteLine(s);
                }
            }
        }

        /// <summary>
        /// To erase the file content.
        /// </summary>
        public void ResetFile()
        {
            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            var directoryPath = directory.AbsolutePath;
            var filename = System.IO.Path.Combine(directoryPath, nameFile);

            FileStream storedFile = File.Open(filename, FileMode.Open);

            // Set the length of filestream to 0 and flush it to the physical file.
            storedFile.SetLength(0);
            storedFile.Close(); // This flushes the content, too.
        }

        /// <summary>
        /// To load the data saved, maybe useful if you want to check the current file content before 
        /// the beginning of a new drive session.
        /// </summary>
        /// <returns></returns>
        public List<string> Load()
        {
            var directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            var directoryPath = directory.AbsolutePath;
            var filename = System.IO.Path.Combine(directoryPath, nameFile);

            List<string> dataRead = new List<string>();

            // Write
            using (var streamReader = new StreamReader(filename, true))
            {
                dataRead.Add(streamReader.ReadLine());
            }

            return dataRead;
        }
    }
}