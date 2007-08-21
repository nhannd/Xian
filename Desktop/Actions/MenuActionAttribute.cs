using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a button action with the specifed action identifier and path hint.
    /// </summary>
    public class MenuActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action</param>
        /// <param name="pathHint">The suggested location of this action in the menu model</param>
        public MenuActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint)
        {
        }

        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model</param>
        /// <param name="clickHandler">Name of the method that will be invoked when the button is clicked</param>
        public MenuActionAttribute(string actionID, string pathHint, string clickHandler)
            : base(actionID, pathHint, clickHandler)
        {
        }

        /// <summary>
        /// Factory method to instantiate the action.
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="path"></param>
        /// <param name="flags"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver)
        {
            return new MenuAction(actionID, path, flags, resolver);
        }
    }
}
