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
            get
            {
                if (this.ContainsTag(DicomTag.StudyInstanceUID))
                    return new Uid(_internalDictionary[DicomTag.StudyInstanceUID]);
                else
                    return new Uid();
            }
        }

        /// <summary>
        /// Gets the Patient ID as a PatientId object.
        /// </summary>
        public PatientId PatientId
        {
            get
            {
                if (this.ContainsTag(DicomTag.PatientId))
                    return new PatientId(_internalDictionary[DicomTag.PatientId]);
                else
                    return new PatientId();
            }
        }

        /// <summary>
        /// Gets Patient's Name as a PatientsName object.
        /// </summary>
        public PersonName PatientsName
        {
            get
            {
                if (this.ContainsTag(DicomTag.PatientsName))
                    return new PersonName(_internalDictionary[DicomTag.PatientsName]);
                else
                    return new PersonName();
            }
        }

        /// <summary>
        /// Gets Modalities In Study as a string.
        /// </summary>
        public string ModalitiesInStudy
        {
            get
            {
                if (this.ContainsTag(DicomTag.ModalitiesInStudy))
                    return _internalDictionary[DicomTag.ModalitiesInStudy];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Study Description as a string.
        /// </summary>
        public string StudyDescription
        {
            get
            {
                if (this.ContainsTag(DicomTag.StudyDescription))
                    return _internalDictionary[DicomTag.StudyDescription];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Study Date as a string.
        /// </summary>
        public string StudyDate
        {
            get
            {
                if (this.ContainsTag(DicomTag.StudyDate))
                    return _internalDictionary[DicomTag.StudyDate];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Study Time as a string.
        /// </summary>
        public string StudyTime
        {
            get
            {
                if (this.ContainsTag(DicomTag.StudyTime))
                    return _internalDictionary[DicomTag.StudyTime];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Accession Number as a string.
        /// </summary>
        public string AccessionNumber
        {
            get
            {
                if (this.ContainsTag(DicomTag.AccessionNumber))
                    return _internalDictionary[DicomTag.AccessionNumber];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Modality as a string.
        /// </summary>
        public string Modality
        {
            get
            {
                if (this.ContainsTag(DicomTag.Modality))
                    return _internalDictionary[DicomTag.Modality];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Series Instance UID as a Uid object.
        /// </summary>
        public Uid SeriesInstanceUid
        {
            get
            {
                if (this.ContainsTag(DicomTag.SeriesInstanceUID))
                    return new Uid(_internalDictionary[DicomTag.SeriesInstanceUID]);
                else
                    return new Uid();
            }
        }

        /// <summary>
        /// Gets Series Description as a string.
        /// </summary>
        public string SeriesDescription
        {
            get
            {
                if (this.ContainsTag(DicomTag.SeriesDescription))
                    return _internalDictionary[DicomTag.SeriesDescription];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Series Number as a string.
        /// </summary>
        public string SeriesNumber
        {
            get
            {
                if (this.ContainsTag(DicomTag.SeriesNumber))
                    return _internalDictionary[DicomTag.SeriesNumber];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets Number Of Study Related Instances as a number.
        /// </summary>
        public uint NumberOfStudyRelatedInstances
        {
            get
            {
                if (ContainsTag(DicomTag.NumberOfStudyRelatedInstances))
                {
                    String temp = _internalDictionary[DicomTag.NumberOfStudyRelatedInstances];
                    uint output;
                    if (uint.TryParse(temp, out output))
                        return output;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets Number of Series Related Instances as a number.
        /// </summary>
        public uint NumberOfSeriesRelatedInstances
        {
            get
            {
                if (this.ContainsTag(DicomTag.NumberOfSeriesRelatedInstances))
                {
                    String temp = _internalDictionary[DicomTag.NumberOfSeriesRelatedInstances];
                    uint output;
                    if (uint.TryParse(temp, out output))
                        return output;
                }
                return 0;
            }
        }

        public string SpecificCharacterSet
        {
            get
            {
                if (this.ContainsTag(DicomTag.SpecificCharacterSet))
                    return _internalDictionary[DicomTag.SpecificCharacterSet];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets the whole collection of DICOM tags in this query result.
        /// </summary>
        public ICollection<DicomTag> DicomTags
        {
            get { return (this as IDictionary<DicomTag, string>).Keys; }
        }

        /// <summary>
        /// Indexer based on the DicomTag key to this collection.
        /// </summary>
        /// <param name="key">The key representing the tag the client wants to obtain.</param>
        /// <returns>The string representation of that value.</returns>
        public string this[DicomTag key]
        {
            get 
            {
                if (this.ContainsTag(key))
                    return (this as IDictionary<DicomTag, string>)[key];
                else
                    return "";
            }
            set { (this as IDictionary<DicomTag, string>)[key] = value; }
        }   

        /// <summary>
        /// Add a new tag into the collection.
        /// </summary>
        /// <param name="key">The key of the tag.</param>
        /// <param name="value">The value of the tag.</param>
        public void Add(DicomTag key, string value)
        {
            (this as IDictionary<DicomTag, string>).Add(key, value);
        }

        /// <summary>
        /// Empties out the collection.
        /// </summary>
        public void Clear()
        {
            (this as ICollection<KeyValuePair<DicomTag, string>>).Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a particular tag.
        /// </summary>
        /// <param name="tag">The tag to check.</param>
        /// <returns>True - tag exists; False - tag does not exist.</returns>
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

    /// <summary>
    /// A helper class. This class is identical in
    /// every respect to QueryResult, but the name. A new
    /// name has been given in order to have the correct
    /// semantic when a collection of tags is used to define
    /// a query or retrieve, rather than when used as a
    /// collection of query results.
    /// </summary>
    public class QueryKey : QueryResult
    {

    }
}
