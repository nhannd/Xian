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
    /// Represents the collection of <see cref="Shelf"/> objects for a desktop window.
    /// </summary>
    public sealed class ShelfCollection : DesktopObjectCollection<Shelf>
    {
        private DesktopWindow _owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ShelfCollection(DesktopWindow owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Opens a new shelf.
        /// </summary>
        /// <param name="component">The <see cref="IApplicationComponent"/> that is to be hosted in the returned <see cref="Shelf"/>.</param>
        /// <param name="title">The title of the <see cref="Shelf"/>.</param>
        /// <param name="displayHint">A hint for how the <see cref="Shelf"/> should be initially displayed.</param>
        public Shelf AddNew(IApplicationComponent component, string title, ShelfDisplayHint displayHint)
        {
            return AddNew(component, title, null, displayHint);
        }

		/// <summary>
		/// Opens a new shelf.
		/// </summary>
		/// <param name="component">The <see cref="IApplicationComponent"/> that is to be hosted in the returned <see cref="Shelf"/>.</param>
		/// <param name="title">The title of the <see cref="Shelf"/>.</param>
		/// <param name="name">A name/identifier for the <see cref="Shelf"/>.</param>
		/// <param name="displayHint">A hint for how the <see cref="Shelf"/> should be initially displayed.</param>
		public Shelf AddNew(IApplicationComponent component, string title, string name, ShelfDisplayHint displayHint)
        {
            return AddNew(new ShelfCreationArgs(component, title, name, displayHint));
        }

        /// <summary>
        /// Opens a new shelf given the input <see cref="ShelfCreationArgs"/>.
        /// </summary>
        public Shelf AddNew(ShelfCreationArgs args)
        {
            Shelf shelf = CreateShelf(args);
            Open(shelf);
            return shelf;
        }

        /// <summary>
        /// Creates a new shelf.
        /// </summary>
        private Shelf CreateShelf(ShelfCreationArgs args)
        {
            IShelfFactory factory = CollectionUtils.FirstElement<IShelfFactory>(
                (new ShelfFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultShelfFactory();

            return factory.CreateShelf(args, _owner);
        }
    }
}
