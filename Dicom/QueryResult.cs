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
    public class QueryResult : IDictionary<uint, string>
    {
        /// <summary>
        /// Gets the Study Instance UID as a Uid object.
        /// </summary>
        public Uid StudyInstanceUid
        {
            get
            {
                if (this.ContainsTag(DicomTags.StudyInstanceUID))
                    return new Uid(_internalDictionary[DicomTags.StudyInstanceUID]);
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
                if (this.ContainsTag(DicomTags.PatientID))
                    return new PatientId(_internalDictionary[DicomTags.PatientID]);
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
                if (this.ContainsTag(DicomTags.PatientsName))
                    return new PersonName(_internalDictionary[DicomTags.PatientsName]);
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
                if (this.ContainsTag(DicomTags.ModalitiesinStudy))
                    return _internalDictionary[DicomTags.ModalitiesinStudy];
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
                if (this.ContainsTag(DicomTags.StudyDescription))
                    return _internalDictionary[DicomTags.StudyDescription];
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
                if (this.ContainsTag(DicomTags.StudyDate))
                    return _internalDictionary[DicomTags.StudyDate];
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
                if (this.ContainsTag(DicomTags.StudyTime))
                    return _internalDictionary[DicomTags.StudyTime];
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
                if (this.ContainsTag(DicomTags.AccessionNumber))
                    return _internalDictionary[DicomTags.AccessionNumber];
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
                if (this.ContainsTag(DicomTags.Modality))
                    return _internalDictionary[DicomTags.Modality];
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
                if (this.ContainsTag(DicomTags.SeriesInstanceUID))
                    return new Uid(_internalDictionary[DicomTags.SeriesInstanceUID]);
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
                if (this.ContainsTag(DicomTags.SeriesDescription))
                    return _internalDictionary[DicomTags.SeriesDescription];
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
                if (this.ContainsTag(DicomTags.SeriesNumber))
                    return _internalDictionary[DicomTags.SeriesNumber];
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
                if (ContainsTag(DicomTags.NumberofStudyRelatedInstances))
                {
                    String temp = _internalDictionary[DicomTags.NumberofStudyRelatedInstances];
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
                if (this.ContainsTag(DicomTags.NumberofSeriesRelatedInstances))
                {
                    String temp = _internalDictionary[DicomTags.NumberofSeriesRelatedInstances];
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
                if (this.ContainsTag(DicomTags.SpecificCharacterSet))
                    return _internalDictionary[DicomTags.SpecificCharacterSet];
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets the whole collection of DICOM tags in this query result.
        /// </summary>
        public ICollection<uint> DicomTagCollection
        {
            get { return (this as IDictionary<uint, string>).Keys; }
        }

        /// <summary>
        /// Indexer based on the DicomTag key to this collection.
        /// </summary>
        /// <param name="key">The key representing the tag the client wants to obtain.</param>
        /// <returns>The string representation of that value.</returns>
        public string this[uint key]
        {
            get 
            {
                if (this.ContainsTag(key))
                    return (this as IDictionary<uint, string>)[key];
                else
                    return "";
            }
            set { (this as IDictionary<uint, string>)[key] = value; }
        }   

        /// <summary>
        /// Add a new tag into the collection.
        /// </summary>
        /// <param name="key">The key of the tag.</param>
        /// <param name="value">The value of the tag.</param>
        public void Add(uint key, string value)
        {
            (this as IDictionary<uint, string>).Add(key, value);
        }

        /// <summary>
        /// Empties out the collection.
        /// </summary>
        public void Clear()
        {
            (this as ICollection<KeyValuePair<uint, string>>).Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a particular tag.
        /// </summary>
        /// <param name="tag">The tag to check.</param>
        /// <returns>True - tag exists; False - tag does not exist.</returns>
        public Boolean ContainsTag(uint tag)
        {
            return (this as IDictionary<uint, string>).ContainsKey(tag);
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

        private Dictionary<uint, string> _internalDictionary = new Dictionary<uint, string>();

        #region IDictionary<DicomTag,string> Members

        void IDictionary<uint, string>.Add(uint key, string value)
        {
            _internalDictionary.Add(key, value);
        }

        bool IDictionary<uint, string>.ContainsKey(uint key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        ICollection<uint> IDictionary<uint, string>.Keys
        {
            get { return _internalDictionary.Keys; }
        }

        bool IDictionary<uint, string>.Remove(uint key)
        {
            return _internalDictionary.Remove(key);
        }

        bool IDictionary<uint, string>.TryGetValue(uint key, out string value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        ICollection<string> IDictionary<uint, string>.Values
        {
            get { return _internalDictionary.Values; }
        }

        string IDictionary<uint, string>.this[uint key]
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

        #region ICollection<KeyValuePair<uint,string>> Members

        void ICollection<KeyValuePair<uint, string>>.Add(KeyValuePair<uint, string> item)
        {
            (_internalDictionary as ICollection<KeyValuePair<uint, string>>).Add(item);
        }

        void ICollection<KeyValuePair<uint, string>>.Clear()
        {
            (_internalDictionary as ICollection<KeyValuePair<uint, string>>).Clear();
        }

        bool ICollection<KeyValuePair<uint, string>>.Contains(KeyValuePair<uint, string> item)
        {
            return (_internalDictionary as ICollection<KeyValuePair<uint, string>>).Contains(item);
        }

        void ICollection<KeyValuePair<uint, string>>.CopyTo(KeyValuePair<uint, string>[] array, int arrayIndex)
        {
            (_internalDictionary as ICollection<KeyValuePair<uint, string>>).CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<uint, string>>.Count
        {
            get { return (_internalDictionary as ICollection<KeyValuePair<uint, string>>).Count; }
        }

        bool ICollection<KeyValuePair<uint, string>>.IsReadOnly
        {
            get { return (_internalDictionary as ICollection<KeyValuePair<uint, string>>).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<uint, string>>.Remove(KeyValuePair<uint, string> item)
        {
            return (_internalDictionary as ICollection<KeyValuePair<uint, string>>).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<uint,string>> Members

        IEnumerator<KeyValuePair<uint, string>> IEnumerable<KeyValuePair<uint, string>>.GetEnumerator()
        {
            return (_internalDictionary as ICollection<KeyValuePair<uint, string>>).GetEnumerator();
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
