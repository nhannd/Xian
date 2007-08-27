using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public interface IMetadataBroker : IPersistenceBroker
    {
        IList<Type> ListEntityClasses();
        IList<Type> ListEnumValueClasses();
    }
}
