using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public interface IObservablePropertyBinding<T>
    {
        event EventHandler PropertyChanged;
        T PropertyValue { get; set; }
    }
}
