using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.TestApp
{
    public class DicomFileCleanup
    {
        private string _sourceDirectory;
        private string _destinationDirectory;

        public string SourceDirectory
        {
            get { return _sourceDirectory; }
            set { _sourceDirectory = value; }
        }

        public string DestinationDirectory
        {
            get { return _destinationDirectory; }
            set { _destinationDirectory = value; }
        }

        public void Scan()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(SourceDirectory);

            ScanDirectories(dirInfo);
        }
        public static string FormatToPascal(string value)
        {
            if (value == null)
                return null;

            StringBuilder sb = new StringBuilder();
            bool lastCharWasSpace = false;
            foreach (char c in value)
            {
                if (c.ToString() == " ")
                    lastCharWasSpace = true;
                else if (lastCharWasSpace || sb.Length == 0)
                    sb.Append(c.ToString().ToUpperInvariant());
                else
                    sb.Append(c.ToString().ToLowerInvariant());
            }
            return sb.ToString();
        }

        public static string CreateVariableName(string input)
        {
            // Now create the variable name
            char[] charSeparators = new char[] { '(', ')', ',', ' ',':', '\'', '-', '/', '&', '[', ']', '@' };

            // just remove apostrophes so casing is correct
            string tempString = input.Replace("’", "");
            tempString = tempString.Replace("'", "");
            tempString = tempString.Replace("(", "");
            tempString = tempString.Replace(")", "");
            tempString = tempString.Replace("–", "");
            if (tempString.Contains(":"))
                tempString = tempString.Substring(0, tempString.IndexOf(":"));

            String[] nodes = tempString.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            string output = "";
            foreach (String node in nodes)
                output += FormatToPascal(node);

            return output;
        }
        private void ScanDirectories(DirectoryInfo dir)
        {

            FileInfo[] files = dir.GetFiles();

            DicomLogger.LogInfo("Scanning directory: {0}", dir.FullName);

            foreach (FileInfo file in files)
            {

                DicomFile dicomFile = new DicomFile(file.FullName);

                if (dicomFile.SopClass.Equals(SopClass.MediaStorageDirectoryStorage))
                    continue;
                try
                {
                    dicomFile.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

                    string destination = this.DestinationDirectory;


                    destination = Path.Combine(destination, CreateVariableName(dicomFile.TransferSyntax.Name));
                    destination = Path.Combine(destination, dicomFile.DataSet[DicomTags.StudyInstanceUid].ToString());
                    destination = Path.Combine(destination, dicomFile.DataSet[DicomTags.SeriesInstanceUid].ToString());

                    try
                    {
                        Directory.CreateDirectory(destination);

                        string filename =
                            Path.Combine(destination, dicomFile.MediaStorageSopInstanceUid + ".dcm");

                        if (File.Exists(filename))
                            File.Delete(file.FullName);
                        else
                            File.Move(file.FullName, filename);
                    }
                    catch (Exception)
                    { }
                   
                }
                catch (DicomException)
                {
                    // TODO:  Add some logging for failed files
                }

            }

            String[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (String subPath in subdirectories)
            {
                DirectoryInfo subDir = new DirectoryInfo(subPath);
                ScanDirectories(subDir);
                continue;
            }

        }
    }
}
