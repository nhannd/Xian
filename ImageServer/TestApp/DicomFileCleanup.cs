#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;

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
            string tempString = input.Replace("�", "");
            tempString = tempString.Replace("'", "");
            tempString = tempString.Replace("(", "");
            tempString = tempString.Replace(")", "");
            tempString = tempString.Replace("�", "");
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
                            Path.Combine(destination, dicomFile.MediaStorageSopInstanceUid + ServerPlatform.DicomFileExtension);

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
			Overlay,
			PaletteColor
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
					if (!attrib.Tag.VR.Equals(DicomVr.SQvr) || attrib.IsNull) continue;
					// Recursive search
					foreach (DicomSequenceItem item in (DicomSequenceItem[]) attrib.Values)
					{
						SearchAttributeSet(item, filename, searchType);
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
					if (attrib.Tag.TagValue > 0x70000000)
						return false;
				}
			}
			else if (searchType == SearchTypes.PaletteColor)
			{
				if (set.Contains(DicomTags.PhotometricInterpretation))
				{
					if (set[DicomTags.PhotometricInterpretation].ToString().Equals("PALETTE COLOR"))
						return true;
				}
				return false;
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

					if (SearchAttributeSet(dicomFile.DataSet, file.FullName, SearchTypes.PaletteColor))
					{
						string destination = Path.Combine(DestinationDirectory, _imageCount + ServerPlatform.DicomFileExtension);

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
