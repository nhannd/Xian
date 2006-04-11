namespace ClearCanvas.Dicom
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
        /// <summary>
        /// Gets the Study Instance UID as a Uid object.
        /// </summary>
        public Uid StudyInstanceUid
        {
            get { return new Uid(_internalDictionary[DicomTag.StudyInstanceUID]); }
        }

        /// <summary>
        /// Gets the Patient ID as a PatientId object.
        /// </summary>
        public PatientId PatientId
        {
            get { return new PatientId(_internalDictionary[DicomTag.PatientId]); }
        }

        /// <summary>
        /// Gets Patient's Name as a PatientsName object.
        /// </summary>
        public PatientsName PatientsName
        {
            get { return new PatientsName(_internalDictionary[DicomTag.PatientsName]); }
        }

        /// <summary>
        /// Gets Modalities In Study as a string.
        /// </summary>
        public string ModalitiesInStudy
        {
            get { return _internalDictionary[DicomTag.ModalitiesInStudy]; }
        }

        /// <summary>
        /// Gets Study Description as a string.
        /// </summary>
        public string StudyDescription
        {
            get { return _internalDictionary[DicomTag.StudyDescription]; }
        }

        /// <summary>
        /// Gets Study Date as a string.
        /// </summary>
        public string StudyDate
        {
            get { return _internalDictionary[DicomTag.StudyDate]; }
        }

        /// <summary>
        /// Gets Study Time as a string.
        /// </summary>
        public string StudyTime
        {
            get { return _internalDictionary[DicomTag.StudyTime]; }
        }

        /// <summary>
        /// Gets Accession Number as a string.
        /// </summary>
        public string AccessionNumber
        {
            get { return _internalDictionary[DicomTag.AccessionNumber]; }
        }

        public string Modality
        {
            get { return _internalDictionary[DicomTag.Modality]; }
        }

        public Uid SeriesInstanceUid
        {
            get { return new Uid(_internalDictionary[DicomTag.SeriesInstanceUID]); }
        }

        public string SeriesDescription
        {
            get { return _internalDictionary[DicomTag.SeriesDescription]; }
        }

        public string SeriesNumber
        {
            get { return _internalDictionary[DicomTag.SeriesNumber]; }
        }

        /// <summary>
        /// Gets the whole collection of DICOM tags in this query result.
        /// </summary>
        public ICollection<DicomTag> DicomTags
        {
            get { return (this as IDictionary<DicomTag, string>).Keys; }
        }

        public string this[DicomTag key]
        {
            get { return (this as IDictionary<DicomTag, string>)[key]; }
            set { (this as IDictionary<DicomTag, string>)[key] = value; }
        }

        public void Add(DicomTag key, string value)
        {
            (this as IDictionary<DicomTag, string>).Add(key, value);
        }

        public void Clear()
        {
            (this as ICollection<KeyValuePair<DicomTag, string>>).Clear();
        }

        public Boolean ContainsTag(DicomTag tag)
        {
            return (this as IDictionary<DicomTag, string>).ContainsKey(tag);
        }

        /// <summary>
        /// Returns string representation of this query result.
        /// </summary>
        /// <returns>Study Description if that is available, otherwise the Study Instance UID.</returns>
        public override string ToString()
        {
            if (null != StudyDescription)
                return StudyDescription;
            else 
                return StudyInstanceUid.ToString();
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

    public class QueryKey : QueryResult
    {

    }
}
