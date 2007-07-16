using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides a convenient set of methods for instantiating view extensions.
    /// </summary>
    public static class ViewFactory
    {
        /// <summary>
        /// Creates a view extension that matches the specified GUI toolkit.
        /// </summary>
        /// <param name="extensionPoint">The view extension point.</param>
        /// <param name="toolkitID">The desired GUI toolkit</param>
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
        /// Creates a view extension that matches GUI toolkit currently in use.
        /// </summary>
        /// <param name="extensionPoint">The view extension point.</param>
        /// <returns>The view object that was created.</returns>
        /// <exception cref="NotSupportedException">A view extension matching the GUI toolkit of the main view not exist.</exception>
        /// <exception cref="InvalidOperationException">The main workstation view has not yet been created.</exception>
        public static IView CreateView(IExtensionPoint extensionPoint)
        {
            return CreateView(extensionPoint, Application.GuiToolkitID);
        }

        /// <summary>
        /// Creates a view extension based on the view extension point that is associated with the specified
        /// model type.  The model type is any class that has a <see cref="AssociateViewAttribute"/> attribute
        /// specified.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
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
