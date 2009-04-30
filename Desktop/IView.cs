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
    /// A base interface for all classes that represent UI views.
    /// </summary>
    /// <remarks>
    /// <para>
	/// A view is a class that provides a UI representation for another object (the model).
	/// </para>
	/// <para>
    /// The purpose of a view class is to separate the presentation from the application logic,
    /// allowing the application to work with different GUI toolkits.
	/// </para>
    /// </remarks>
    public interface IView
    {
        /// <summary>
        /// Gets the toolkitID of the GUI tookit in which the view is implemented.
        /// </summary>
        string GuiToolkitID
        {
            get;
        }

        /// <summary>
        /// Gets the underlying GUI component for this view.
        /// </summary>
        /// <remarks>
        /// The type of the returned object is specific to a given GUI toolkit.  For example,
        /// a view implemented in Windows Forms would return a Windows Forms Control object.
        /// </remarks>
        object GuiElement
        {
            get;
        }
    }
}
