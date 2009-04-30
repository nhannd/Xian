#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
