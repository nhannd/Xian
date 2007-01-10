using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ExtensionScanner : MarshalByRefObject
    {
        /// <summary>
        /// Uses the ExtensionPoint type to scan the plugins folder for extensions.
        /// </summary>
        /// <returns>
        /// An enumerable collection that contains information on each extension that will help the loader
        /// load them into the AppDomain. If no extensions are found, null is returned.</returns>
        public ShredStartupInfoList ScanExtensions()
        {
            Platform.Log(this.GetType().ToString() + ":" + this.GetType().Name + " in AppDomain [" + AppDomain.CurrentDomain.FriendlyName + "]");

            ShredStartupInfoList shredInfoList = new ShredStartupInfoList();
            ShredExtensionPoint xp = new ShredExtensionPoint();
            object[] shredObjects = xp.CreateExtensions();
            foreach (object shredObject in shredObjects)
            {
                if (shredObject is IShred)
                {
                    Uri assemblyPath = new Uri(shredObject.GetType().Assembly.CodeBase);
                    shredInfoList.Add(new ShredStartupInfo(assemblyPath, (shredObject as IShred).GetDisplayName(), shredObject.GetType().FullName));
                }
            }

            return shredInfoList;
        }
    }
}
