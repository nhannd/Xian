namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom;

    /// <summary>
    /// A single result from a C-FIND query, a collection of <see cref="DicomTag">DicomTag</see>s.
    /// Since QueryResult implements IDictionary, it's possible to use an enumerator to extract
    /// every single tag in the result, as well as provide a key to extract a particular tag.
    /// </summary>
    public class QueryResult : IDictionary<DicomTag, string>
    {
        public Uid StudyInstanceUid
        {
            get { return new Uid(_internalDictionary[DicomTag.StudyInstanceUID]); }
        }

        public PatientID PatientID
        {
            get { return new PatientID(_internalDictionary[DicomTag.PatientID]); }
        }

        public PatientsName PatientsName
        {
            get { return new PatientsName(_internalDictionary[DicomTag.PatientsName]); }
        }

        public string ModalitiesInStudy
        {
            get { return _internalDictionary[DicomTag.ModalitiesInStudy]; }
        }

        public string StudyDescription
        {
            get { return _internalDictionary[DicomTag.StudyDescription]; }
        }

        public string StudyDate
        {
            get { return _internalDictionary[DicomTag.StudyDate]; }
        }

        public string StudyTime
        {
            get { return _internalDictionary[DicomTag.StudyTime]; }
        }

        public string AccessionNumber
        {
            get { return _internalDictionary[DicomTag.AccessionNumber]; }
        }

        public ICollection<DicomTag> DicomTags
        {
            get { return (this as IDictionary<DicomTag, string>).Keys; }
        }

        public void Add(DicomTag key, string value)
        {
            (this as IDictionary<DicomTag, string>).Add(key, value);
        }

        public string this[DicomTag key]
        {
            get { return (this as IDictionary<DicomTag, string>)[key]; }
            set { (this as IDictionary<DicomTag, string>)[key] = value; }
        }

        public override string ToString()
        {
            if (null != StudyDescription)
                return StudyDescription;
            else if (null != StudyInstanceUid)
                return StudyInstanceUid.ToString();
            else
                return null;
        }

        private Dictionary<DicomTag, string> _internalDictionary = new Dictionary<DicomTag,string>();

        #region IDictionary<DicomTag,string> Members

        void IDictionary<DicomTag, string>.Add(DicomTag key, string value)
        {
            _internalDictionary.Add(key, value);
        }

        bool IDictionary<DicomTag, string>.ContainsKey(DicomTag key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        ICollection<DicomTag> IDictionary<DicomTag, string>.Keys
        {
            get { return _internalDictionary.Keys; }
        }

        bool IDictionary<DicomTag, string>.Remove(DicomTag key)
        {
            return _internalDictionary.Remove(key);
        }

        bool IDictionary<DicomTag, string>.TryGetValue(DicomTag key, out string value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        ICollection<string> IDictionary<DicomTag, string>.Values
        {
            get { return _internalDictionary.Values; }
        }

        string IDictionary<DicomTag, string>.this[DicomTag key]
        {
            get
            {
                return _internalDictionary[key];
            }
            set
            {
                _internalDictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<DicomTag,string>> Members

        void ICollection<KeyValuePair<DicomTag, string>>.Add(KeyValuePair<DicomTag, string> item)
        {
            (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).Add(item);
        }

        void ICollection<KeyValuePair<DicomTag, string>>.Clear()
        {
            (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).Clear();
        }

        bool ICollection<KeyValuePair<DicomTag, string>>.Contains(KeyValuePair<DicomTag, string> item)
        {
            return (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).Contains(item);
        }

        void ICollection<KeyValuePair<DicomTag, string>>.CopyTo(KeyValuePair<DicomTag, string>[] array, int arrayIndex)
        {
            (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<DicomTag, string>>.Count
        {
            get { return (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).Count; }
        }

        bool ICollection<KeyValuePair<DicomTag, string>>.IsReadOnly
        {
            get { return (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<DicomTag, string>>.Remove(KeyValuePair<DicomTag, string> item)
        {
            return (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<DicomTag,string>> Members

        IEnumerator<KeyValuePair<DicomTag, string>> IEnumerable<KeyValuePair<DicomTag, string>>.GetEnumerator()
        {
            return (_internalDictionary as ICollection<KeyValuePair<DicomTag, string>>).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        #endregion
    }
}
