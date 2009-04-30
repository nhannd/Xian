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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Defines a tool.
    /// </summary>
    /// <remarks>
	/// Developers are encouraged to subclass <see cref="Tool{TContextInterface}"/> 
	/// or one of its subclasses rather than implement this interface directly.
	/// </remarks>
    public interface ITool : IDisposable
    {
        /// <summary>
        /// Called by the framework to set the tool context.
        /// </summary>
        void SetContext(IToolContext context);

        /// <summary>
        /// Called by the framework to allow the tool to initialize itself.
        /// </summary>
        /// <remarks>
		/// This method will be called after <see cref="SetContext"/> has been called, 
		/// which guarantees that the tool will have access to its context when this method is called.
		/// </remarks>
        void Initialize();

        /// <summary>
        /// Gets the set of actions that act on this tool.
        /// </summary>
        /// <remarks>
		/// This property is not guaranteed to be a dynamic property - that is, you should not assume
		/// this property will always return a different set of actions depending on the internal state 
		/// of the tool.  The class that owns the tool decides when to access this property, and 
		/// whether or not the actions can be dynamic will be dependent on the implementation of that class.
		/// </remarks>
        IActionSet Actions { get; }
    }
}
