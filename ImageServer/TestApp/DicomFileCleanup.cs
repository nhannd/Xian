using System;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.TestApp
{
    public class DicomFileCleanup
    {
        private string _sourceDirectory;
        private string _destinationDirectory;
    	private int _imageCount = 1;

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

            Platform.Log(LogLevel.Info, "Scanning directory: {0}", dir.FullName);

            foreach (FileInfo file in files)
            {

                DicomFile dicomFile = new DicomFile(file.FullName);

                if (dicomFile.SopClass.Equals(SopClass.MediaStorageDirectoryStorage))
                    continue;
                try
                {
                    dicomFile.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

					string destination = DestinationDirectory;


                    destination = Path.Combine(destination, CreateVariableName(dicomFile.TransferSyntax.Name));
                    destination = Path.Combine(destination, dicomFile.DataSet[DicomTags.StudyInstanceUid].ToString());
                    destination = Path.Combine(destination, dicomFile.DataSet[DicomTags.SeriesInstanceUid].ToString());

                    try
                    {
                        Directory.CreateDirectory(destination);

                        string filename =
                            Path.Combine(destination, dicomFile.MediaStorageSopInstanceUid + ".dcm");

						if (File.Exists(filename))
						{
							Platform.Log(LogLevel.Info, "File has already been stored in destination folder: {0}", file.FullName);
							File.Delete(file.FullName);
						}
						else
							File.Move(file.FullName, filename);
                    }
                    catch (Exception)
                    { }
                   
                }
                catch (Exception)
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

		private enum SearchTypes
		{
			PrivateSequence,
			Overlay
		}
		private bool SearchAttributeSet(DicomAttributeCollection set, string filename, SearchTypes searchType)
		{
			if (searchType == SearchTypes.PrivateSequence)
			{
				foreach (DicomAttribute attrib in set)
				{
					if (attrib.Tag.IsPrivate && attrib.Tag.VR.Equals(DicomVr.SQvr))
					{
						Platform.Log(LogLevel.Info, "Found file with private SQ: {0}", filename);
						return true;
					}
					else if (attrib.Tag.VR.Equals(DicomVr.SQvr) && !attrib.IsNull)
					{
						// Recursive search
						foreach (DicomSequenceItem item in (DicomSequenceItem[]) attrib.Values)
						{
							SearchAttributeSet(item, filename, searchType);
						}
					}
				}
			}
			else if (searchType == SearchTypes.Overlay)
			{
				foreach (DicomAttribute attrib in set)
				{
					if ((attrib.Tag.TagValue & 0xFF000000) == 0x60000000)
					{
						Platform.Log(LogLevel.Info, "Found embedded overlay in file: {0}", filename);
						return true;
					}
					else if (attrib.Tag.TagValue > 0x70000000)
						return false;
				}
			}

			return false;
		}

		public void SearchDirectories(DirectoryInfo dir)
		{

			FileInfo[] files = dir.GetFiles();

			Platform.Log(LogLevel.Info, "Scanning directory: {0}", dir.FullName);

			foreach (FileInfo file in files)
			{

				DicomFile dicomFile = new DicomFile(file.FullName);

				try
				{
					Platform.Log(LogLevel.Info, "Checking file: {0}", file.FullName);
					dicomFile.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

					if (SearchAttributeSet(dicomFile.DataSet, file.FullName, SearchTypes.Overlay))
					{
						string destination = Path.Combine(DestinationDirectory, _imageCount + ".dcm");

						if (File.Exists(destination))
							File.Delete(destination);

						File.Copy(file.FullName,destination);

						_imageCount++;
					}
				}
				catch (Exception)
				{
					// TODO:  Add some logging for failed files
				}
			}

			String[] subdirectories = Directory.GetDirectories(dir.FullName);
			foreach (String subPath in subdirectories)
			{
				DirectoryInfo subDir = new DirectoryInfo(subPath);
				SearchDirectories(subDir);
				continue;
			}

		}
    }
}
