using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
    /// <summary>
    /// Represents the execution context of a <cref="RestoreQueue"/> item.
    /// </summary>
    public class RestoreProcessorContext : ExecutionContext
    {
        #region Private Fields
        private readonly Model.RestoreQueue _item;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="RestoreProcessorContext"/>
        /// </summary>
        /// <param name="item"></param>
        public RestoreProcessorContext(RestoreQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            _item = item;
        }
        #endregion

        #region Protected Methods
        protected override string GetTemporaryDirectory()
        {
            StudyStorageLocation storage = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(_item.StudyStorageKey))[0];
            if (storage == null)
                return base.TempDirectory;
            else
            {
                String basePath = Path.Combine(storage.FilesystemPath, "temp");
                String tempDirectory = Path.Combine(basePath, String.Format("RestoreQueue-{0}", _item.GetKey()));

                for (int i = 2; i < 1000; i++)
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        break;
                    }

                    tempDirectory = Path.Combine(basePath, String.Format("RestoreQueue-{0}({1})", _item.GetKey(), i));
                }

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                return tempDirectory;
            }
        }

        #endregion
    }
}