#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using DicomAuditedInstances = ClearCanvas.Dicom.Audit.AuditedInstances;

namespace ClearCanvas.ImageViewer.Common.Auditing
{
	/// <summary>
	/// Represents a collection of DICOM instances and, optionally, the associated file paths that are the subject of an audit event.
	/// </summary>
	public sealed class AuditedInstances
	{
		private readonly DicomAuditedInstances _instances;

		/// <summary>
		/// Constructs a new, empty collection.
		/// </summary>
		public AuditedInstances()
			: this(new DicomAuditedInstances()) {}

		/// <summary>
		/// Constructs a new collection and adds files on the indicated paths, automatically parsing for patient and study instance information.
		/// </summary>
		/// <param name="recursive">True if the paths should be processed recursively; False otherwise.</param>
		/// <param name="paths">The file paths on which to search for files.</param>
		public AuditedInstances(bool recursive, params string[] paths)
			: this(new DicomAuditedInstances(recursive, paths)) {}

		private AuditedInstances(DicomAuditedInstances instances)
		{
			_instances = instances;
		}

		/// <summary>
		/// Recursively searches the given path for DICOM object files, automatically parsing for patient and study instance information.
		/// </summary>
		/// <param name="path">The file path on which to search for files.</param>
		public void AddPath(string path)
		{
			_instances.AddPath(path);
		}

		/// <summary>
		/// Searches the given path for DICOM object files, automatically parsing for patient and study instance information.
		/// </summary>
		/// <param name="path">The file path on which to search for files.</param>
		/// <param name="recursive">True if the paths should be processed recursively; False otherwise.</param>
		public void AddPath(string path, bool recursive)
		{
			_instances.AddPath(path, recursive);
		}

		/// <summary>
		/// Adds a given instance to the collection without any patient information.
		/// </summary>
		/// <remarks>
		/// This overload should be used only when patient information is not available, or is not required by
		/// the type of auditable event. In most cases, the <see cref="AddInstance(string,string,string)"/>
		/// overload is preferred over this one. Furthermore, if the type of auditable event requires some knowledge
		/// of a source or destination media, then the <see cref="AddInstance(string,string,string,string)"/>
		/// overload should be used instead.
		/// </remarks>
		/// <param name="studyInstanceUid">The study instance UID of the instance.</param>
		public void AddInstance(string studyInstanceUid)
		{
			_instances.AddInstance(studyInstanceUid);
		}

		/// <summary>
		/// Adds a given instance to the collection.
		/// </summary>
		/// <remarks>
		/// If the type of auditable event requires some knowledge
		/// of a source or destination media, then the <see cref="AddInstance(string,string,string,string)"/>
		/// overload should be used instead.
		/// </remarks>
		/// <param name="patientId">The patient ID of the instance.</param>
		/// <param name="patientName">The patient's name of the instance.</param>
		/// <param name="studyInstanceUid">The study instance UID of the instance.</param>
		public void AddInstance(string patientId, string patientName, string studyInstanceUid)
		{
			_instances.AddInstance(patientId, patientName, studyInstanceUid);
		}

		/// <summary>
		/// Adds a given instance to the collection.
		/// </summary>
		/// <remarks>
		/// This overload is used when the type of audtable event requires knowledge of a source or
		/// destination media, such as for Data Import and Data Export events. Otherwise, the
		/// <see cref="AddInstance(string,string,string)"/> overload is a perfectly acceptable
		/// substitute, especially when such source path information is not available.
		/// </remarks>
		/// <param name="patientId">The patient ID of the instance.</param>
		/// <param name="patientName">The patient's name of the instance.</param>
		/// <param name="studyInstanceUid">The study instance UID of the instance.</param>
		/// <param name="filename">The filename or path associated with the specified instance.</param>
		public void AddInstance(string patientId, string patientName, string studyInstanceUid, string filename)
		{
			_instances.AddInstance(patientId, patientName, studyInstanceUid, filename);
		}

		public static implicit operator DicomAuditedInstances(AuditedInstances instances)
		{
			return instances._instances;
		}

		public static implicit operator AuditedInstances(DicomAuditedInstances instances)
		{
			return new AuditedInstances(instances);
		}
	}
}