using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class for performing a Reindex of the database.
    /// </summary>
    public class ReindexProcessor
    {
        #region Public Properties

        public int StudyFoldersToScan { get; private set; }

        public int DatabaseStudiesToScan { get; private set; }

        public string FilestoreDirectory { get; private set; }

        public List<string> DirectoryList { get;private set; } 

        #endregion

        #region Constructors

        public ReindexProcessor()
        {
            FilestoreDirectory = GetFileStoreDirectory();
            DirectoryList = new List<string>();
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            try
            {
                FileProcessor.Process(FilestoreDirectory, "*.*", delegate(string file, out bool cancel)
                                                                     {
                                                                         if (Directory.Exists(file))
                                                                             DirectoryList.Add(file);

                                                                         cancel = false;

                                                                     }, false);
            }
            catch (Exception)
            {
                
            }

            StudyFoldersToScan = DirectoryList.Count;
        }

        public void Process()
        {
            
        }

        #endregion

        #region Private Methods

        private static string GetFileStoreDirectory()
        {
            string directory = null;
            Platform.GetService<IDicomServerConfiguration>(
                s => directory = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration.FileStoreDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        #endregion
    }
}
