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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// A convenience class for creating an action model with standard Add, Edit, and Delete actions.
    /// </summary>
    /// <remarks>
    /// An instance of this class can be configured to have any or all of Add, Edit, and Delete actions.
    /// Standard labels and icons will be used for these actions, however you can freely modify these values.
    /// You may also add additional custom actions to an instance of this class.
    /// </remarks>
    public class CrudActionModel : SimpleActionModel
    {
        /// <summary>
        /// Resource key for the "Add" icon.
        /// </summary>
        public const string IconAddResource = "Icons.AddToolSmall.png";

        /// <summary>
        /// Resource key for the "Edit" icon.
        /// </summary>
        public const string IconEditResource = "Icons.EditToolSmall.png";

        /// <summary>
        /// Resource key for the "Delete" icon.
        /// </summary>
        public const string IconDeleteResource = "Icons.DeleteToolSmall.png";

        private static readonly object AddKey = new object();
        private static readonly object EditKey = new object();
        private static readonly object DeleteKey = new object();


        /// <summary>
        /// Constructor that creates an instance with Add, Edit and Delete actions.
        /// </summary>
        public CrudActionModel()
            :this(true, true, true)
        {
        }

        /// <summary>
        /// Constructor that allows specifying which of Add, Edit, and Delete actions should appear.
        /// </summary>
        /// <param name="add"></param>
        /// <param name="edit"></param>
        /// <param name="delete"></param>
        public CrudActionModel(bool add, bool edit, bool delete)
            :this(add, edit, delete, new ResourceResolver(typeof(CrudActionModel).Assembly))
        {
        }

		/// <summary>
		/// Constructor that allows specifying which of Add, Edit, and Delete actions should appear.
		/// </summary>
		/// <param name="add"></param>
		/// <param name="edit"></param>
		/// <param name="delete"></param>
		/// <param name="fallBackResolver"></param>
		public CrudActionModel(bool add, bool edit, bool delete, IResourceResolver fallBackResolver)
			: base(new ResourceResolver(typeof(CrudActionModel).Assembly, fallBackResolver))
		{
			if (add)
			{
				this.AddAction(AddKey, SR.TitleAdd, IconAddResource);
			}
			if (edit)
			{
				this.AddAction(EditKey, SR.TitleEdit, IconEditResource);
			}
			if (delete)
			{
				this.AddAction(DeleteKey, SR.TitleDelete, IconDeleteResource);
			}
		}

        /// <summary>
        /// Gets the Add action.
        /// </summary>
        public ClickAction Add
        {
            get { return this[AddKey]; }
        }

        /// <summary>
        /// Gets the Edit action.
        /// </summary>
        public ClickAction Edit
        {
            get { return this[EditKey]; }
        }

        /// <summary>
        /// Gets the Delete action.
        /// </summary>
        public ClickAction Delete
        {
            get { return this[DeleteKey]; }
        }
    }
}
