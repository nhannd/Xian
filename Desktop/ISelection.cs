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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single or multiple selection.
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Returns the set of items that are currently selected.
        /// </summary>
        object[] Items { get; }

        /// <summary>
        /// Convenience method to obtain the currently selected item in a single-select scenario.
        /// </summary>
		/// <remarks>
		/// If no rows are selected, the method returns null.  If more than one row is selected,
        /// it is undefined which item will be returned.
		/// </remarks>
		object Item { get; }

		/// <summary>
		/// Computes the union of this selection with another and returns it.
		/// </summary>
        ISelection Union(ISelection other);

		/// <summary>
		/// Computes the intersection of this selection with another and returns it.
		/// </summary>
		ISelection Intersect(ISelection other);
        
		/// <summary>
		/// Returns an <see cref="ISelection"/> that contains every item contained
		/// in this one that doesn't exist in <param name="other" />.
		/// </summary>
		ISelection Subtract(ISelection other);

		/// <summary>
		/// Determines whether this selection contains the input object.
		/// </summary>
		bool Contains(object item);
    }
}
