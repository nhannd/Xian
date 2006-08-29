using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    public class ActionPath : Path
    {
        public const string GlobalMenus = "global-menus";
        public const string GlobalToolbars = "global-toolbars";

        public ActionPath(string pathString, ResourceResolver resolver)
            :base(pathString, resolver)
        {
        }

        public string Site
        {
            get { return this.Segments.Length > 0 ? this.Segments[0].ResourceKey : null; }
        }
    }
}
