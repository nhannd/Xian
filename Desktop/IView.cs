using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A base interface for all classes that represent UI "views".  A view is any class that implements
    /// some GUI functionality for the purpose of displaying the state of an underlying model to the user, and
    /// allowing the user to interact with the model.  The purpose of a view class is to shield the model from
    /// the specific implementation details of the GUI, thereby allowing the model to work with different
    /// GUI toolkits.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets the toolkitID of the GUI tookit upon which this view is based.
        /// </summary>
        string GuiToolkitID
        {
            get;
        }

        /// <summary>
        /// Exposes the underlying UI component that provides this view.  The type of the object
        /// is dependent upon the GUI toolkit.  A parent view will know how to cast
        /// this object appropriately.
        /// </summary>
        object GuiElement
        {
            get;
        }
    }
}
