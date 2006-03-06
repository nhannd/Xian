
namespace ClearCanvas.Dicom.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct Path
    {

        public Path(String path)
        {
            // validate the input
            if (null == path)
                throw new System.ArgumentNullException("path", "---");

            _path = path;
        }

        public override String ToString()
        {
            return _path;
        }

        public String GetPathElementAsString(UInt16 index)
        {
            string[] arrayOfSubstrings = _path.Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (index < arrayOfSubstrings.Length)
                return arrayOfSubstrings[index];
            else
                return null;
        }

        public UInt32 GetPathElementAsInt32(UInt16 index)
        {
            String subString = GetPathElementAsString(index);
            if (null != subString)
            {
                string[] arrayOfSubstrings = subString.Split(_tagSeparator, StringSplitOptions.RemoveEmptyEntries);
                UInt32 group = (UInt32) Convert.ToUInt16(arrayOfSubstrings[0], 16) << 16;
                UInt32 element = Convert.ToUInt16(arrayOfSubstrings[1], 16);
                return ( group | element );
            }
            else
                return 0;
        }

        private String _path;
        private static readonly Char[] _pathSeparator = new Char[] { '\\' };
        private static readonly Char[] _tagSeparator = new Char[] { '(', ',', ')' };
    }
}
