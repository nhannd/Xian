using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Scripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class LanguageSupportAttribute : Attribute
    {
        private string _language;

        public LanguageSupportAttribute(string language)
        {
            _language = language;
        }

        public string Language
        {
            get { return _language; }
        }

        public override bool Match(object obj)
        {
            LanguageSupportAttribute that = obj as LanguageSupportAttribute;
            return that != null && that.Language.ToLower().Equals(this.Language.ToLower());
        }
    }
}
