using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Models a user-interface action, such as a menu or toolbar item, in a GUI-toolkit independent way.
    /// </summary>
    /// <remarks>
    /// Provides the base interface for a set of types that model user-interface actions
    /// independent of any particular GUI-toolkit.
    /// </remarks>
    public interface IAction
    {
        /// <summary>
        /// Fired when the <see cref="Enabled"/> property of this action changes.
        /// </summary>
        event EventHandler EnabledChanged;

        /// <summary>
        /// The fully-qualified logical identifier for this action.
        /// </summary>
        string ActionID { get; }

        /// <summary>
        /// The target object to which this action is bound, typically an object
        /// of type <see cref="ClearCanvas.Workstation.Model.Tools.ITool"/>.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// The menu or toolbar path for this action.
        /// </summary>
        ActionPath Path { get; set; }

        /// <summary>
        /// The label that the action should present in the UI.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// The tooltip that the action should present in the UI.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// The icon that the action should present in the UI.
        /// </summary>
        IconSet IconSet { get; }

        /// <summary>
        /// The enablement state that the action should present in the UI.
        /// </summary>
        bool Enabled { get; }
   }
}
