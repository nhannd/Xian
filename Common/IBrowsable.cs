using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public interface IBrowsable
    {
        /// <summary>
        /// Formal name of this object, typically the type name or assembly name.  Cannot be null.
        /// </summary>
        string FormalName
        {
            get;
        }

        /// <summary>
        /// Friendly name of the object, if one exists, otherwise null.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// A friendly description of this object, if one exists, otherwise null.
        /// </summary>
        string Description
        {
            get;
        }
    }
}
