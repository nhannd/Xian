using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
    /// <summary>
    /// Represents the execution context of a <see cref="ServiceLock"/> item.
    /// </summary>
    public class ServiceLockProcessorContext : ExecutionContext
    {
        #region Private fields
        private readonly Model.ServiceLock _item;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ServiceLockProcessorContext"/>
        /// </summary>
        /// <param name="item"></param>
        public ServiceLockProcessorContext(Model.ServiceLock item)
        {
            Platform.CheckForNullReference(item, "item");
            _item = item;
            TempDirectory = InitTempDirectory();
        }
        #endregion
       
        #region Private Methods
        private string InitTempDirectory()
        {
            ServerFilesystemInfo filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(_item.FilesystemKey);
            if (filesystem == null)
            {
                // not ready?
                return base.TempDirectory;
            }
            else
            {
                String basePath = Path.Combine(filesystem.Filesystem.FilesystemPath, "temp");
                basePath = Path.Combine(basePath, _item.ServiceLockTypeEnum.Lookup);
                String tempDirectory = Path.Combine(basePath, _item.GetKey().ToString());

                for (int i = 2; i < 1000; i++)
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        break;
                    }

                    tempDirectory = Path.Combine(basePath, String.Format("{0}({1})", _item.GetKey().ToString(), i));
                }

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                return tempDirectory;
            }
        }
        #endregion

    }
}