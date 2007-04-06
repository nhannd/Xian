using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Extends the <see cref="IAction"/> interface for actions that have single-click
    /// behaviour, such as menu items and toolbar buttons.
    /// </summary>
    public interface IClickAction : IAction
    {
        /// <summary>
        /// Fired when the <see cref="Checked"/> property of this action changes.
        /// </summary>
        event EventHandler CheckedChanged;
        
        /// <summary>
        /// Reports whether this action is a "check" action, that is, an action that behaves as a toggle.
        /// </summary>
        bool IsCheckAction { get; }

        /// <summary>
        /// The checked state that the action should present in the UI, if this is a "check" action.
        /// </summary>
        /// <remarks>
        /// This property has no meaning if <see cref="IsCheckAction"/> returns false.
        /// </remarks>
        bool Checked { get; }

		/// <summary>
		/// Gets a value indicating whether parent items should be checked if this
		/// <see cref="IClickAction"/> is checked.
		/// </summary>
		bool CheckParents { get; }

		/// <summary>
		/// The keystroke that the UI should attempt to intercept and invoke the action.
		/// </summary>
		XKeys KeyStroke { get; set; }

        /// <summary>
        /// Called by the UI when the user clicks on the action.
        /// </summary>
        void Click();
    }
}
