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
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides a convenient set of methods for instantiating views.
    /// </summary>
    public static class ViewFactory
    {
        /// <summary>
        /// Creates a view for the specified extension point and GUI toolkit.
        /// </summary>
        /// <param name="extensionPoint">The view extension point.</param>
        /// <param name="toolkitID">The desired GUI toolkit.</param>
        /// <returns>The view object that was created.</returns>
        /// <exception cref="NotSupportedException">A view extension matching the specified GUI toolkit does not exist.</exception>
        public static IView CreateView(IExtensionPoint extensionPoint, string toolkitID)
        {
            // create an attribute representing the GUI toolkitID
            GuiToolkitAttribute toolkitAttr = new GuiToolkitAttribute(toolkitID);

            // create an extension that is tagged with the same toolkit
            return (IView)extensionPoint.CreateExtension(new AttributeExtensionFilter(toolkitAttr));
        }

        /// <summary>
        /// Creates a view for the specified extension point and current GUI toolkit.
        /// </summary>
        /// <param name="extensionPoint">The view extension point.</param>
        /// <returns>The view object that was created.</returns>
        /// <exception cref="NotSupportedException">A view extension matching the GUI toolkit of the main view does not exist.</exception>
        /// <exception cref="InvalidOperationException">The main workstation view has not yet been created.</exception>
        public static IView CreateView(IExtensionPoint extensionPoint)
        {
            return CreateView(extensionPoint, Application.GuiToolkitID);
        }

        /// <summary>
        /// Creates a view based on the view extension point that is associated with the specified
        /// model type.  The model type is any class that has a <see cref="AssociateViewAttribute"/> attribute
        /// specified.
        /// </summary>
        public static IView CreateAssociatedView(Type modelType)
        {
            object[] attrs = modelType.GetCustomAttributes(typeof(AssociateViewAttribute), true);
            if (attrs.Length == 0)
				throw new ArgumentException(SR.ExceptionAssociateViewAttributeNotSpecified, "modelType");

            AssociateViewAttribute viewAttribute = (AssociateViewAttribute)attrs[0];
            IExtensionPoint viewExtPoint = (IExtensionPoint)Activator.CreateInstance(viewAttribute.ViewExtensionPointType);

            return CreateView(viewExtPoint);
        }
    }
}
