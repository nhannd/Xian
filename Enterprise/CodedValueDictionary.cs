using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using System.IO;
using System.Reflection;
using Iesi.Collections;
using System.Collections;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Represents a set of coded-values
    /// </summary>
    /// <typeparam name="TCodedValue">The class of coded-value that is contained in this dictionary</typeparam>
    public class CodedValueDictionary<TCodedValue>
        where TCodedValue : CodedValue<TCodedValue>, new()
    {
        private Set _values;
        private object _syncLock = new object();

        internal CodedValueDictionary()
        {
        }

        /// <summary>
        /// Obtains the instance for the specified code, or throws an exception if the code is invalid
        /// </summary>
        /// <param name="code"></param>
        /// <exception cref="ArgumentOutOfRangeException">The code is not valid</exception>
        /// <returns></returns>
        public TCodedValue LookupByCode(string code)
        {
            TCodedValue instance = CollectionUtils.SelectFirst<TCodedValue>(this.ValueSet, delegate(TCodedValue v) { return v.Code.Equals(code); });
            if (instance == null)
                throw new ArgumentOutOfRangeException(string.Format("{0} is not a valid code", code));
            return instance;
        }

        /// <summary>
        /// Obtains the instance for the specified value, or throws an exception if the code is invalid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TCodedValue LookupByValue(string value)
        {
            TCodedValue instance = CollectionUtils.SelectFirst<TCodedValue>(this.ValueSet, delegate(TCodedValue v) { return v.Value == value; });
            if (instance == null)
                throw new ArgumentOutOfRangeException(string.Format("{0} is not a valid value", value));
            return instance;
        }
        
        /// <summary>
        /// Returns the set of values that are active
        /// </summary>
        public TCodedValue[] ActiveValues
        {
            get { return GetValues(true); } 
        }

        /// <summary>
        /// Returns the entire set of values
        /// </summary>
        public TCodedValue[] Values
        {
            get { return GetValues(false); }
        }

        /// <summary>
        /// Returns the set of display strings for values that are active
        /// </summary>
        public string[] ActiveDisplayValues
        {
            get { return GetDisplayValues(true); }
        }

        /// <summary>
        /// Returns the entire set of display values
        /// </summary>
        public string[] DisplayValues
        {
            get { return GetDisplayValues(false); }
        }

        #region Helper methods

        private string[] GetDisplayValues(bool activeOnly)
        {
            return CollectionUtils.ToArray<string>(
                CollectionUtils.Map<TCodedValue, string>(SelectValues(activeOnly),
                    delegate(TCodedValue value) { return value.Value; }));
        }

        private TCodedValue[] GetValues(bool activeOnly)
        {
            return CollectionUtils.ToArray<TCodedValue>(SelectValues(activeOnly));
        }

        private IEnumerable SelectValues(bool activeOnly)
        {
            return activeOnly ? CollectionUtils.Select(this.ValueSet, delegate(object obj) { return (obj as TCodedValue).Active; }) : this.ValueSet;
        }

        private Set ValueSet
        {
            get
            {
                // access must be thread-safe, because the initialization process must never occur more than once
                lock (_syncLock)
                {
                    if (_values == null)
                    {
                        Initialize();
                    }
                    return _values;
                }
            }
        }

        private void Initialize()
        {
            _values = new HybridSet();
            ProcessStaticValues();
            ProcessXml();
        }

        /// <summary>
        /// Uses reflection to add any static member instances of the class to the dictionary
        /// </summary>
        private void ProcessStaticValues()
        {
            Type codeValueClass = typeof(TCodedValue);
            FieldInfo[] fields = codeValueClass.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Equals(codeValueClass))
                {
                    TCodedValue cv = (TCodedValue)field.GetValue(null);
                    if (cv.Code != field.Name)
                    {
                        throw new InvalidOperationException(
                            string.Format("Member {0} of class {1} has code {2}. Code must match member name.",
                            field.Name, codeValueClass.FullName, cv.Code));
                    }
                    _values.Add(cv);
                }
            }
        }

        /// <summary>
        /// Processes associated XML document to populate the dictionary
        /// </summary>
        private void ProcessXml()
        {
            Type clazz = typeof(TCodedValue);
            string resourceName = string.Format("{0}.cv.xml", clazz.FullName);
            Version version = clazz.Assembly.GetName().Version;

            try
            {
                IConfigurationService configService = ApplicationContext.GetService<IConfigurationService>();
                string xml = configService.LoadDocument(resourceName, version, null, null);
                ReadXml(new StringReader(xml));
            }
            catch (EntityNotFoundException)
            {
                ResourceResolver rr = new ResourceResolver(typeof(TCodedValue).Assembly);
                using (Stream s = rr.OpenResource(resourceName))
                {
                    ReadXml(new StreamReader(s));
                }
            }
        }

        private void ReadXml(TextReader reader)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);
            foreach (XmlElement cvElement in xmlDoc.GetElementsByTagName("coded-value"))
            {
                string code = cvElement.GetAttribute("code");

                // does this value exist already?
                TCodedValue codedValue = CollectionUtils.SelectFirst<TCodedValue>(_values, delegate(TCodedValue v) { return v.Code == code; });
                if (codedValue == null)
                {
                    // if not, create it
                    codedValue = new TCodedValue();
                }

                // intialize or update the value from xml
                codedValue.FromXml(cvElement);
                _values.Add(codedValue);
            }
        }

        #endregion
    }
}
