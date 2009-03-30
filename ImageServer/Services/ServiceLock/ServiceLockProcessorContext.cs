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
        }
        #endregion
       
        #region Private Methods
        protected override string GetTemporaryDirectory()
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
                String tempDirectory = Path.Combine(basePath, String.Format("{0}-{1}", _item.ServiceLockTypeEnum.Lookup, _item.GetKey()));
                
                for (int i = 2; i < 1000; i++)
                {
                    if (!Directory.Exists(tempDirectory))
                    {
                        break;
                    }

                    tempDirectory = Path.Combine(basePath, String.Format("{0}-{1}({2})", _item.ServiceLockTypeEnum.Lookup, _item.GetKey(), i));
                }

                if (!Directory.Exists(tempDirectory))
                    Directory.CreateDirectory(tempDirectory);

                return tempDirectory;
            }
        }
        #endregion

    }
}