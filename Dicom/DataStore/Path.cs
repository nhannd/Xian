using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public class Path
    {
        #region NHibernate-specific members
        /// <summary>
        /// Mandatory constructor for NHibernate.
        /// </summary>
        public Path()
        {
        }


        /// <summary>
        /// Property for NHibernate.
        /// </summary>
        protected virtual string InternalPath
        {
            get { return _path; }
            set { _path = value; }
        }
        #endregion

        public Path(string path)
        {
            // validate the input
            if (null == path)
				throw new System.ArgumentNullException("path", SR.ExceptionPathCannotBeNull);

            _path = path;
        }

        public override string ToString()
        {
            return _path;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator string(Path path)
        {
            return path.ToString();
        }

        public string GetLastPathElementAsString()
        {
            int beforeLastElementIndex = _path.LastIndexOfAny(_pathSeparator);
            if (beforeLastElementIndex > 0)
                return _path.Substring(beforeLastElementIndex + 1);
            else
                return _path;
        }

        public uint GetLastPathElementAsInt32()
        {
            string subString = GetLastPathElementAsString();
            if (null != subString)
            {
                string[] arrayOfSubstrings = subString.Split(_tagSeparator, StringSplitOptions.RemoveEmptyEntries);
                uint group = (uint)Convert.ToUInt16(arrayOfSubstrings[0], 16) << 16;
                uint element = Convert.ToUInt16(arrayOfSubstrings[1], 16);
                return (group | element);
            }
            else
                return 0;
        } 

        public string GetPathElementAsString(ushort index)
        {
            string[] arrayOfSubstrings = _path.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (index < arrayOfSubstrings.Length)
                return arrayOfSubstrings[index];
            else
                return null;
        }

        public uint GetPathElementAsInt32(ushort index)
        {
            string subString = GetPathElementAsString(index);
            if (null != subString)
            {
                string[] arrayOfSubstrings = subString.Split(_tagSeparator, StringSplitOptions.RemoveEmptyEntries);
                uint group = (uint) Convert.ToUInt16(arrayOfSubstrings[0], 16) << 16;
                uint element = Convert.ToUInt16(arrayOfSubstrings[1], 16);
                return ( group | element );
            }
            else
                return 0;
        }

        private string _path;
        private static readonly char[] _pathSeparator = new char[] { '\\' };
        private static readonly char[] _tagSeparator = new char[] { '(', ',', ')' };
    }
}
