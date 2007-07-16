using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="Shelf"/> objects for a given desktop window.
    /// </summary>
    public class ShelfCollection : DesktopObjectCollection<Shelf>
    {
        private DesktopWindow _owner;

        protected internal ShelfCollection(DesktopWindow owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Opens a new shelf.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="displayHint"></param>
        /// <returns></returns>
        public Shelf AddNew(IApplicationComponent component, string title, ShelfDisplayHint displayHint)
        {
            return AddNew(component, title, null, displayHint);
        }

        /// <summary>
        /// Opens a new shelf.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <param name="displayHint"></param>
        /// <returns></returns>
        public Shelf AddNew(IApplicationComponent component, string title, string name, ShelfDisplayHint displayHint)
        {
            return AddNew(new ShelfCreationArgs(component, title, name, displayHint));
        }

        /// <summary>
        /// Opens a new shelf.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Shelf AddNew(ShelfCreationArgs args)
        {
            Shelf shelf = CreateShelf(args);
            Open(shelf);
            return shelf;
        }

        protected virtual Shelf CreateShelf(ShelfCreationArgs args)
        {
            IShelfFactory factory = CollectionUtils.FirstElement<IShelfFactory>(
                (new ShelfFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultShelfFactory();

            return factory.CreateShelf(args, _owner);
        }
    }
}
