using System;
using System.IO;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    public class TempFileManager
    {
        private static TempFileManager _instance;
        private readonly Dictionary<EntityRef, string> _tempFileDictionary = new Dictionary<EntityRef, string>();

        public static TempFileManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TempFileManager();

                return _instance;
            }
        }

        ~TempFileManager()
        {
            try
            {
                // Clean up all temporary files
                foreach (EntityRef key in _tempFileDictionary.Keys)
                {
                    if (File.Exists(_tempFileDictionary[key]))
                        File.Delete(_tempFileDictionary[key]);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, SR.ExceptioinFailedToDeleteTemporaryFiles);
            }
        }

        public string GetTempFile(EntityRef key)
        {
            if (_tempFileDictionary.ContainsKey(key) && File.Exists(_tempFileDictionary[key]))
                return _tempFileDictionary[key];

            return null;
        }

        public string CreateTemporaryFile(EntityRef key, string fileExtension, byte[] data)
        {
            string tempFileName = String.Format("{0}.{1}", System.IO.Path.GetTempFileName(), fileExtension);
            FileStream fs = new FileStream(tempFileName, FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();

            _tempFileDictionary[key] = tempFileName;

            return tempFileName;
        }
    }
}
