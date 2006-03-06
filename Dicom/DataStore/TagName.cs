
namespace ClearCanvas.Dicom.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct TagName
    {
        public TagName(String tagName)
        {
            // validate the input
            if (null == tagName)
                throw new System.ArgumentNullException("tagName", "---");

            if (0 == tagName.Length)
                throw new System.ArgumentOutOfRangeException("tagName", "---");

            _tagName = tagName;
        }

        public override String ToString()
        {
            return _tagName;
        }

        String _tagName;
    }
}
