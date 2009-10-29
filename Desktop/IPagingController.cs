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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines an interface to control pagination through a list of items.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
    public interface IPagingController<TItem>
    {
        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        int PageSize {get; set;}

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        /// <returns></returns>
        bool HasNext { get; }

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        /// <returns></returns>
        bool HasPrevious { get; }

        /// <summary>
        /// Gets the next page of items.
        /// </summary>
        /// <returns></returns>
        IList<TItem> GetNext();

        /// <summary>
        /// Gets the previous page of items.
        /// </summary>
        /// <returns></returns>
        IList<TItem> GetPrevious();

        /// <summary>
        /// Resets this instance to the first page of items.
        /// </summary>
        /// <returns></returns>
        IList<TItem> GetFirst();

        /// <summary>
        /// Occurs when the current page changes (by calling any of <see cref="GetFirst"/>, <see cref="GetNext"/> or <see cref="GetPrevious"/>.
        /// </summary>
        event EventHandler PageChanged;
    }
}
