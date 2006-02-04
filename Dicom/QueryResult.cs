namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom;

    public class QueryResult : IDictionary<DicomTag, string>
    {

        public Uid StudyInstanceUid
        {
            get
            {
                return new Uid("1.2.840.1.2.311432.43242.266");
            }
            private set
            {
            }
        }

        public ICollection<DicomTag> DicomTags
        {
            get
            {
                return null;
            }
        }

        #region IDictionary<DicomTag,string> Members

        void IDictionary<DicomTag, string>.Add(DicomTag key, string value)
        {

            throw new Exception("The method or operation is not implemented.");
        }

        bool IDictionary<DicomTag, string>.ContainsKey(DicomTag key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        ICollection<DicomTag> IDictionary<DicomTag, string>.Keys
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IDictionary<DicomTag, string>.Remove(DicomTag key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IDictionary<DicomTag, string>.TryGetValue(DicomTag key, out string value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        ICollection<string> IDictionary<DicomTag, string>.Values
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        string IDictionary<DicomTag, string>.this[DicomTag key]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<KeyValuePair<DicomTag,string>> Members

        void ICollection<KeyValuePair<DicomTag, string>>.Add(KeyValuePair<DicomTag, string> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<KeyValuePair<DicomTag, string>>.Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool ICollection<KeyValuePair<DicomTag, string>>.Contains(KeyValuePair<DicomTag, string> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<KeyValuePair<DicomTag, string>>.CopyTo(KeyValuePair<DicomTag, string>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int ICollection<KeyValuePair<DicomTag, string>>.Count
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool ICollection<KeyValuePair<DicomTag, string>>.IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool ICollection<KeyValuePair<DicomTag, string>>.Remove(KeyValuePair<DicomTag, string> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<KeyValuePair<DicomTag,string>> Members

        IEnumerator<KeyValuePair<DicomTag, string>> IEnumerable<KeyValuePair<DicomTag, string>>.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
