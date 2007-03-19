using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public static class EnumValueUtils
    {
        public static List<string> GetDisplayValues(IEnumerable<EnumValueInfo> enumValueSet)
        {
            return CollectionUtils.Map<EnumValueInfo, string, List<string>>(enumValueSet,
                delegate(EnumValueInfo e) { return e.Value; });
        }

        public static EnumValueInfo MapDisplayValue(IEnumerable<EnumValueInfo> enumValueSet, string displayValue)
        {
            return CollectionUtils.SelectFirst<EnumValueInfo>(enumValueSet,
                delegate(EnumValueInfo e) { return e.Value == displayValue; });
        }
    }
}
