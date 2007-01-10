using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using ClearCanvas.Common;


namespace ClearCanvas.Server.ShredHost
{
    [ExtensionPoint()]
    public class ShredExtensionPoint : ExtensionPoint<IShred>
    {
    }

    public static class ShredHost
    {
        public static void Start()
        {
            Platform.Log("Starting up in AppDomain [" + AppDomain.CurrentDomain.FriendlyName + "]");

#if DEBUG
            PrintAllAssembliesInAppDomain(AppDomain.CurrentDomain);
#endif

            // the ShredList and shreds objects are proxy objects that actually exist
            // in the secondary AppDomain
            AppDomain stagingDomain = AppDomain.CreateDomain("StagingDomain");
            ExtensionScanner loader = (ExtensionScanner)stagingDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, "ClearCanvas.Server.ShredHost.ExtensionScanner");
            ShredStartupInfoList shredInfoList = loader.ScanExtensions();

#if DEBUG
            PrintAllAssembliesInAppDomain(AppDomain.CurrentDomain);
#endif
#if false
            // create the list of IShredCommunication proxy objects in the Default AppDomain
            List<IShredCommunication> proxies = new List<IShredCommunication>();
            foreach (object extension in extensions)
            {
                IShred shred = extension as IShred;
                if (null != shred)
                {
                    ProxyList proxyList = shred.GetCommunicationProxyList();
                    IShredCommunication proxy = proxyList.Proxies[0] as IShredCommunication;
                    if (null != proxy)
                    {
                        proxies.Add(proxy);
                    }
                }
            }
#endif

            // create the individual threads for all the shreds
            foreach (ShredStartupInfo shredStartupInfo in shredInfoList)
            {
                if (null != shredStartupInfo)
                {
                    // create the shred's own AppDomain for it to run in
                    AppDomain newShredDomain = AppDomain.CreateDomain(shredStartupInfo.ShredName);
                    IShred shred = (IShred)newShredDomain.CreateInstanceFromAndUnwrap(shredStartupInfo.AssemblyPath.LocalPath, shredStartupInfo.ShredTypeName);

                    // start up the thread that will actually run the shred
                    Thread t = new Thread(new ParameterizedThreadStart(StartupShred));
                    ShredInfo newShredInfo = new ShredInfo(t, newShredDomain, shred);
                    _shredInfoList.Add(newShredInfo);
                    t.Start(newShredInfo);
                }

            }

            // all the shreds have been started, so we can dismantle the secondary domain that was used 
            // for scanning for all Extensions that are shreds
            AppDomain.Unload(stagingDomain);

        }

        public static void Stop()
        {
            Platform.Log("Shred Host stop request received");

            foreach (ShredInfo shredInfo in _shredInfoList)
            {
                if (null != shredInfo)
                {
                    Platform.Log(shredInfo.Shred.GetFriendlyName() + ": Signally stop");
                    shredInfo.Shred.Stop();
                }
            }

            foreach (ShredInfo shredInfo in _shredInfoList)
            {
                Platform.Log(shredInfo.Shred.GetFriendlyName() + ": Waiting to join thread");
                shredInfo.ShredThreadObject.Join();
                Platform.Log(shredInfo.Shred.GetFriendlyName() + ": Thread joined");
                AppDomain.Unload(shredInfo.ShredDomain);
            }

            Platform.Log("All threads joined; completing Shred Host stop");
        }

        static void StartupShred(object threadData)
        {
            ShredInfo shredInfo = threadData as ShredInfo;
            int servicePort;

            lock (_lockObject)
            {
                servicePort = _servicePort;
                _servicePort++;
            }

            Platform.Log(shredInfo.Shred.GetFriendlyName() + " shred about to be started on port " + servicePort.ToString());
            shredInfo.Shred.Start(servicePort);
        }

        #region print asms in AD helper f(x)
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
        private static int _servicePort = 21000;
        private static object _lockObject = new object();
        private static ShredInfoList _shredInfoList = new ShredInfoList();

        class ShredDescription
        {
            public ShredDescription(string friendlyName)
            {
            }

        }

        static private Dictionary<string, ShredDescription> _shredDescriptions;

        #endregion

    }    
}
