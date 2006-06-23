using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.Desktop.View.WinForms
{
    public static class IconFactory
    {
        /// <summary>
        /// Creates a bitmap from a resource embedded in an assembly, and prepares it to be used
        /// as an icon on a toolbar or menu.
        /// </summary>
        /// <param name="assemblyDefiningType">Any type that resides in the same assembly as the embedded resource</param>
        /// <param name="resource">The name of the resource (e.g. MyIcon.bmp)</param>
        /// <returns>A Bitmap loaded with the specified resource</returns>
        /// <remarks>
        /// If the resource is not found within the specified assembly, an exception will be thrown.
        /// </remarks>
        public static Bitmap CreateIcon(Type assemblyLocatingType, string resource)
        {
            Bitmap icon = new Bitmap(assemblyLocatingType, resource);
            icon.MakeTransparent(Color.Red);
            return icon;
        }
    }
}
