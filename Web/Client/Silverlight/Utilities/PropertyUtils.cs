using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace ClearCanvas.Web.Client.Silverlight.Utilities
{
    public static class PropertyUtils
    {
        private static void SetProperties(PropertyInfo[] fromFields,
                                       object fromRecord,
                                       object toRecord)
        {
            PropertyInfo fromField = null;

            try
            {
                if (fromFields == null)
                {
                    return;
                }

                for (int f = 0; f < fromFields.Length; f++)
                {
                    fromField = (PropertyInfo)fromFields[f];
                    if (fromField.Name == "EntityConflict")
                        continue;  // Entity objects have this field and it throws an exception when copying

                    if (fromField.CanRead && fromField.CanWrite)
                    {
                        fromField.SetValue(toRecord,
                                           fromField.GetValue(fromRecord, null),
                                           null);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Copy(object source, object destination)
        {
            PropertyInfo[] fromFields = null;

            fromFields = source.GetType().GetProperties();

            SetProperties(fromFields, source, destination);
        }
    }    
}
