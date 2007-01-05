using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    public class ExtensionLoader : MarshalByRefObject
    {
        public ShredList LoadExtensions()
        {
            Platform.Log("AppDomain[" + AppDomain.CurrentDomain.FriendlyName + "]: ExtensionLoader");

            ShredList shredList = new ShredList();
            ShredExtensionPoint xp = new ShredExtensionPoint();
            object[] shredObjects = xp.CreateExtensions();
            foreach (object shredObject in shredObjects)
            {
                if (shredObject is IShred)
                    shredList.Add(shredObject as IShred);
            }
            return shredList;
        }
    }
}
