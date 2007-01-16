using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using ClearCanvas.Common;


namespace ClearCanvas.Server.ShredHost
{
    public static class ShredHost
    {
        public static void Start()
        {
            Platform.Log("Starting up in AppDomain [" + AppDomain.CurrentDomain.FriendlyName + "]");

            // the ShredList and shreds objects are proxy objects that actually exist
            // in the secondary AppDomain
            AppDomain stagingDomain = AppDomain.CreateDomain("StagingDomain");
            ExtensionScanner scanner = (ExtensionScanner)stagingDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, "ClearCanvas.Server.ShredHost.ExtensionScanner");
            ShredStartupInfoList shredStartupInfoList = null;

            try
            {
                shredStartupInfoList = scanner.ScanExtensions();
            }
            catch (PluginException pluginException)
            {
                // There was a problem loading the plugins, including if there were no plugins found
                // This is an innocuous problem, and just means that there are no shreds to run
                Platform.Log(pluginException, LogLevel.Warn);
            }

            if (null != shredStartupInfoList)
            {
                // create the data structure that will hold the shreds and their thread, etc. related objects
                foreach (ShredStartupInfo shredStartupInfo in shredStartupInfoList)
                {
                    if (null != shredStartupInfo)
                    {
                        // create the shred's own AppDomain for it to run in
                        AppDomain newShredDomain = AppDomain.CreateDomain(shredStartupInfo.ShredName);
                        IShred shred = (IShred)newShredDomain.CreateInstanceFromAndUnwrap(shredStartupInfo.AssemblyPath.LocalPath, shredStartupInfo.ShredTypeName);

                        // start up the thread that will actually run the shred

                        ShredController newShredInfo = new ShredController(newShredDomain, shred, shredStartupInfo);
                        _shredInfoList.Add(newShredInfo);
                    }

                }
            }          

            // all the shreds have been created, so we can dismantle the secondary domain that was used 
            // for scanning for all Extensions that are shreds
            AppDomain.Unload(stagingDomain);

            foreach (ShredController shredInfo in _shredInfoList)
            {
                shredInfo.Start();
            }

        }

        public static void Stop()
        {
            Platform.Log("Shred Host stop request received");

            foreach (ShredController shredController in _shredInfoList)
            {
                Platform.Log(shredController.Shred.GetDisplayName() + ": Signalling stop");
                shredController.Stop();
                Platform.Log(shredController.Shred.GetDisplayName() + ": Stopped");
            }

            Platform.Log("Completing Shred Host stop");

            _shredInfoList.Clear();
        }


        #region Print asms in AD helper f(x)
        public static void PrintAllAssembliesInAppDomain(AppDomain ad)
        {
            Assembly[] loadedAssemblies = ad.GetAssemblies();
            Console.WriteLine("***** Here are the assemblies loaded in {0} *****\n",
                ad.FriendlyName);
            foreach (Assembly a in loadedAssemblies)
            {
                Console.WriteLine("-> Name: {0}", a.GetName().Name);
                Console.WriteLine("-> Version: {0}", a.GetName().Version);
            }
        }
        #endregion

        #region Private Members
        private static ShredControllerList _shredInfoList = new ShredControllerList();
        #endregion

        internal static ShredControllerList ShredControllerList
        {
            get { return _shredInfoList; }
        }
    }    
}
