using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    internal class DialogBoxCollection : DesktopObjectCollection<DialogBox>
    {
        private DesktopWindow _owner;

        internal DialogBoxCollection(DesktopWindow owner)
		{
            _owner = owner;
		}

        internal DialogBox AddNew(DialogBoxCreationArgs args)
        {
            DialogBox dialog = CreateDialogBox(args);
            Open(dialog);
            return dialog;
        }

        protected virtual DialogBox CreateDialogBox(DialogBoxCreationArgs args)
        {
            IDialogBoxFactory factory = CollectionUtils.FirstElement<IDialogBoxFactory>(
                (new DialogBoxFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultDialogBoxFactory();

            return factory.CreateDialogBox(args, _owner);
        }

    }
}
