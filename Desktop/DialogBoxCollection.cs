using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="DialogBox"/> objects owned by a desktop window.
    /// </summary>
    internal class DialogBoxCollection : DesktopObjectCollection<DialogBox>
    {
        private DesktopWindow _owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"></param>
        internal DialogBoxCollection(DesktopWindow owner)
		{
            _owner = owner;
		}

        /// <summary>
        /// Creates a new dialog box with the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal DialogBox AddNew(DialogBoxCreationArgs args)
        {
            DialogBox dialog = CreateDialogBox(args);
            Open(dialog);
            return dialog;
        }

        /// <summary>
        /// Creates a new <see cref="DialogBox"/>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private DialogBox CreateDialogBox(DialogBoxCreationArgs args)
        {
            IDialogBoxFactory factory = CollectionUtils.FirstElement<IDialogBoxFactory>(
                (new DialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDialogBoxFactory();

            return factory.CreateDialogBox(args, _owner);
        }

    }
}
