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

using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an interface that must be implemented by application component containers (components
    /// that host other components).
    /// </summary>
    public interface IApplicationComponentContainer
    {
        /// <summary>
        /// Gets all contained components.
        /// </summary>
        IEnumerable<IApplicationComponent> ContainedComponents { get; }

        /// <summary>
        /// Get the contained components that are currently visible to the user.
        /// </summary>
        IEnumerable<IApplicationComponent> VisibleComponents { get; }

        /// <summary>
        /// Ensures that the specified component is made visible to the user.
        /// </summary>
        void EnsureVisible(IApplicationComponent component);

        /// <summary>
        /// Ensures that the specified component has been started. 
        /// </summary>
        /// <remarks>
        /// A container may choose not to start components until they are actually displayed for the first time.
        /// This method ensures that a component is started regardless of whether it has ever been displayed.
        /// This is necessary, for instance, if the component is to be validated as part of validating the container.
        /// </remarks>
        void EnsureStarted(IApplicationComponent component);
    }
}
