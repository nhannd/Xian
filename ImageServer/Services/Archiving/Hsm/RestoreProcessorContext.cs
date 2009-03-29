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
            TempDirectory = InitTempDirectory();
        }
        #endregion

        #region Private Methods
        private string InitTempDirectory()
        {
            StudyStorageLocation storage = StudyStorageLocation.FindStorageLocations(StudyStorage.Load(_item.StudyStorageKey))[0];
            if (storage == null)
                return base.TempDirectory;
            else
            {
                String basePath = Path.Combine(storage.FilesystemPath, "temp");
                basePath = Path.Combine(basePath, "RestoreQueue");
                String tempDirectory = Path.Combine(basePath, _item.GetKey().ToString());

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                return tempDirectory;
            }
        }

        #endregion
    }
}