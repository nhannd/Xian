using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public static class IconFactory
    {
        /// <summary>
        /// Attempts to create an icon using the specified image resource and resource resolver.
        /// </summary>
        /// <param name="resource">The name of the image resource</param>
        /// <param name="resolver">A resource resolver</param>
        /// <returns>a bitmap constructed from the specified image resource</returns>
        public static Bitmap CreateIcon(string resource, IResourceResolver resolver)
        {
            return new Bitmap(resolver.OpenResource(resource));
        }
    }
}
