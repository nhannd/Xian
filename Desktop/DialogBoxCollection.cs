#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
        /// <param name="owner">The <see cref="DesktopWindow"/> that owns the dialog box.</param>
        internal DialogBoxCollection(DesktopWindow owner)
		{
            _owner = owner;
		}

        /// <summary>
        /// Creates a new dialog box with the specified arguments.
        /// </summary>
        internal DialogBox AddNew(DialogBoxCreationArgs args)
        {
            DialogBox dialog = CreateDialogBox(args);
            Open(dialog);
            return dialog;
        }

        /// <summary>
        /// Creates a new <see cref="DialogBox"/>.
        /// </summary>
        private DialogBox CreateDialogBox(DialogBoxCreationArgs args)
        {
            IDialogBoxFactory factory = CollectionUtils.FirstElement<IDialogBoxFactory>(
                (new DialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDialogBoxFactory();

            return factory.CreateDialogBox(args, _owner);
        }

    }
}
