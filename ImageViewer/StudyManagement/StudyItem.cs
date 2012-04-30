#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    //TODO (Marmot): move to Common.

	/// <summary>
	/// A study item.
	/// </summary>
	public class StudyItem : StudyRootStudyIdentifier
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem(string studyInstanceUid, IDicomServiceNode server)
		{
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(server, "server");
			StudyInstanceUid = studyInstanceUid;
            Server = server;
        }

		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
        public StudyItem(IStudyRootData other, IDicomServiceNode server)
			: this(other, other, null as IIdentifier)
		{
            Platform.CheckForNullReference(server, "server");
            Server = server;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StudyItem"/>.
        /// </summary>
        public StudyItem(IPatientData patient, IStudyData study, IDicomServiceNode server)
            : base(patient, study, null)
        {
            Platform.CheckForNullReference(server, "server");
            Platform.CheckForNullReference(study, "study");
            Platform.CheckForEmptyString(study.StudyInstanceUid, "study.StudyInstanceUid");

            Server = server;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StudyItem"/>.
        /// </summary>
        public StudyItem(IPatientData patient, IStudyData study, IIdentifier identifier)
            : base(patient, study, identifier)
        {
            Platform.CheckForNullReference(study, "study");
            Platform.CheckForEmptyString(study.StudyInstanceUid, "study.StudyInstanceUid");
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StudyItem"/>.
        /// </summary>
        public StudyItem(IStudyRootStudyIdentifier other)
            : base(other)
        {
        }

	    /// <summary>
	    /// Gets or sets the server.
	    /// </summary>
	    public IDicomServiceNode Server
	    {
            get { return RetrieveAE as IDicomServiceNode; }
            set { RetrieveAE = value; }
	    }

        public new PersonName PatientsName
        {
            get { return new PersonName(base.PatientsName); }
            set { base.PatientsName = (value ?? ""); }
        }

        public new PersonName ReferringPhysiciansName
        {
            get { return new PersonName(base.ReferringPhysiciansName); }
            set { base.ReferringPhysiciansName = (value ?? ""); }
        }
    }

	/// <summary>
	/// A list of <see cref="StudyItem"/> objects.
	/// </summary>
	public class StudyItemList : List<StudyItem>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItemList"/>.
		/// </summary>
		public StudyItemList()
		{
		}

		public StudyItemList(IEnumerable<StudyItem> items)
			: base(items)
		{
		}
	}

	/// <summary>
	/// A map of query parameters.
	/// </summary>
	public class QueryParameters : IDictionary<string, string>
	{
	    private readonly Dictionary<string, string> _parameters;

		/// <summary>
		/// Initializes a new instance of <see cref="QueryParameters"/>.
		/// </summary>
		public QueryParameters()
		{
		    _parameters = new Dictionary<string, string>();
            this["PatientsName"] = String.Empty;
            this["ReferringPhysiciansName"] = String.Empty;
            this["PatientId"] = String.Empty;
            this["AccessionNumber"] = String.Empty;
            this["StudyDescription"] = String.Empty;
            this["ModalitiesInStudy"] = String.Empty;
            this["StudyDate"] = String.Empty;
            this["StudyInstanceUid"] = String.Empty;
        }

        /// <summary>
        /// Creates a copy of <paramref name="other"/>.
        /// </summary>
        public QueryParameters(QueryParameters other)
        {
            _parameters = new Dictionary<string, string>();
            foreach (var queryParameter in other)
                _parameters.Add(queryParameter.Key, queryParameter.Value);
        }

        #region IDictionary<string,string> Members

        public string this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                    return null;

                string value;
                return !_parameters.TryGetValue(key, out value) ? null : value;
            }
            set
            {
                if (!_parameters.ContainsKey(key))
                    _parameters.Add(key, value);
                else
                    _parameters[key] = value;
            }
        }
        
        public void Add(string key, string value)
        {
            _parameters.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _parameters.Keys; }
        }

        public bool Remove(string key)
        {
            return _parameters.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _parameters.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return _parameters.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Members

        public void Clear()
        {
            _parameters.Clear();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            ((ICollection<KeyValuePair<string, string>>)_parameters).Add(item);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string,string>>)_parameters).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, string>>)_parameters).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((ICollection<KeyValuePair<string, string>>) _parameters).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        #endregion
    }
}
