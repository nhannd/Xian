using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ClearCanvas.Server.ShredHost
{
    /// <summary>
    /// Class created only to allow WCF serialization and usage in service operations
    /// </summary>
    [Serializable]
    public class WcfDataShredCollection : ObservableCollection<WcfDataShred>
    {

    }
}
