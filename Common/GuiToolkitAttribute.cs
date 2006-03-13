using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Attribute used to mark a class as using a specific GUI toolkit.  Typically this attribute
    /// is used on an extension class (in addition to the <see cref="ExtensionOfAttribute"/>) to allow
    /// plugin code to determine at runtime if the given extension is compatible with the GUI toolkit
    /// that is currently in use by the main window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GuiToolkitAttribute : Attribute
    {
        private GuiToolkitID _toolkitID;

        public GuiToolkitAttribute(GuiToolkitID toolkitID)
        {
            _toolkitID = toolkitID;
        }

        public GuiToolkitID ToolkitID
        {
            get { return _toolkitID; }
        }

        public static bool operator ==(GuiToolkitAttribute x, GuiToolkitAttribute y)
        {
            return x.ToolkitID == y.ToolkitID;
        }

        public static bool operator !=(GuiToolkitAttribute x, GuiToolkitAttribute y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return _toolkitID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is GuiToolkitAttribute)
            {
                return (obj as GuiToolkitAttribute) == this;
            }
            else
            {
                return false;
            }
        }
    }
}
