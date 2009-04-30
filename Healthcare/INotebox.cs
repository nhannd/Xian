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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    [ExtensionPoint]
    public class NoteboxExtensionPoint : ExtensionPoint<INotebox>
    {
    }

    /// <summary>
    /// Defines an interface to a notebox.
    /// </summary>
    public interface INotebox
    {
        /// <summary>
        /// Queries the notebox for its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        IList GetItems(INoteboxQueryContext nqc);

        /// <summary>
        /// Queries the notebox for a count of its contents.
        /// </summary>
        /// <param name="nqc"></param>
        /// <returns></returns>
        int GetItemCount(INoteboxQueryContext nqc);
    }

    /// <summary>
    /// Defines an interface that provides a <see cref="Worklist"/> with information about the context
    /// in which it is executing.
    /// </summary>
    public interface INoteboxQueryContext
    {
        /// <summary>
        /// Gets the staff on whose behalf the notebox query is executing.
        /// </summary>
        Staff Staff { get; }

		/// <summary>
		/// For group-based noteboxes, gets the group for which the notebox query is executing.
		/// </summary>
		StaffGroup StaffGroup { get; }

        /// <summary>
        /// Gets the working <see cref="Facility"/> for which the notebox query is executing, or null if the working facility is not known.
        /// </summary>
        Facility WorkingFacility { get; }

        /// <summary>
        /// Gets the <see cref="SearchResultPage"/> that specifies which page of the notebox is requested.
        /// </summary>
        SearchResultPage Page { get; }

        /// <summary>
        /// Obtains an instance of the specified broker.
        /// </summary>
        /// <typeparam name="TBrokerInterface"></typeparam>
        /// <returns></returns>
        TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker;
    }
}
