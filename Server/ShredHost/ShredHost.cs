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
            Platform.Log("AppDomain[" + AppDomain.CurrentDomain.FriendlyName + "]");

#if DEBUG
            PrintAllAssembliesInAppDomain(AppDomain.CurrentDomain);
#endif

            // the ShredList and shreds objects are proxy objects that actually exist
            // in the secondary AppDomain
            AppDomain newAppDomain = AppDomain.CreateDomain("StagingDomain");
            ExtensionLoader loader = (ExtensionLoader)newAppDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, "ClearCanvas.Server.ShredHost.ExtensionLoader");
            ShredList shreds = loader.LoadExtensions();

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
            List<Thread> threads = new List<Thread>();
            foreach (IShred shred in shreds)
            {
                if (null != shred)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(StartupShred));
                    threads.Add(t);
                    t.Start(shred);
                }

            }

            Console.WriteLine("Press <Enter> to terminate the ShredHost.");
            Console.WriteLine();
            Console.ReadLine();

            foreach (IShred shred in shreds)
            {
                if (null != shred)
                    shred.Stop();
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }
        }

        static void StartupShred(object threadData)
        {
            IShred shred = threadData as IShred;
            int servicePort;

            lock (_lockObject)
            {
                servicePort = _servicePort;
                _servicePort++;
            }

            Platform.Log(shred.GetFriendlyName() + " shred started on port " + servicePort.ToString());
            shred.Start(servicePort);
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
        #endregion

    }    
}
