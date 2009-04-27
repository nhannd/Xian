using System;
using System.Web;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Web.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpContextData: IDisposable
    {
        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private const string CUSTOM_DATA_ENTRY = "CUSTOM_DATA_ENTRY";
        private IReadContext _readContext;
        private readonly object _syncRoot = new object();

        private HttpContextData()
        {
        }

        static public HttpContextData Current
        {
            get
            {
                lock( HttpContext.Current.Items.SyncRoot)
                {
                    HttpContextData instance = HttpContext.Current.Items[CUSTOM_DATA_ENTRY] as HttpContextData;
                    if (instance == null)
                    {
                        instance = new HttpContextData();
                        HttpContext.Current.Items[CUSTOM_DATA_ENTRY] = instance;
                    }
                    return instance;
                }
                
            }
        }

        public IReadContext ReadContext
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_readContext == null)
                    {
                        _readContext = _store.OpenReadContext();
                    }
                    return _readContext;
                }
                
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_readContext!=null)
            {
                _readContext.Dispose();
            }
        }

        #endregion
    }
}