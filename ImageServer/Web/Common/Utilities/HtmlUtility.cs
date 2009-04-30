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
using System.Reflection;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class HtmlUtility
    {
    	///
    	/// Encode a string so that it is suitable for rendering in an Html page.
    	/// Also ensure all Xml escape characters are encoded properly.
        public static string Encode(string text)
        {
            if (text == null) return string.Empty;
            String encodedText = new SecurityElement("dummy", text).Text; //decode any escaped xml characters.
            return HttpUtility.HtmlEncode(encodedText).Replace(Environment.NewLine, "<BR/>");
            
        }

        /// <summary>
        /// Returns the <see cref="EnumInfoAttribute"/> of an enum value, if it's defined.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumInfoAttribute GetEnumInfo<TEnum>(TEnum value)
            where TEnum:struct
        {
            FieldInfo field = typeof (TEnum).GetField(value.ToString());
            if (field != null)
            {
                return AttributeUtils.GetAttribute<EnumInfoAttribute>(field);
            }
            else
                return null;
        }

        public static string GetEvalValue(object item, string itemName, string defaultValue)
        {
            string value = DataBinder.Eval(item, itemName, "");

            if (value == null || value.Equals("")) return defaultValue;
            else return value;
        }

         public static void AddCssClass(WebControl control, string cssClass)
         {
             control.CssClass += " " + cssClass;
         }
         
        public static void RemoveCssClass(WebControl control, string cssClass)
        {
            control.CssClass = control.CssClass.Replace(" " + cssClass, "");
        }

    }


}
