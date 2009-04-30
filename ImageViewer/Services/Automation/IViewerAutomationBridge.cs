#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Services.Automation
{
	/// <summary>
	/// Enum specifying how to treat the study request when the primary study is the
	/// primary study in an existing viewer.
	/// </summary>
	public enum OpenStudiesBehaviour
	{
		/// <summary>
		/// Specifies that the existing viewer should be activated (Default).
		/// </summary>
		ActivateExistingViewer = 0,
		/// <summary>
		/// Specifies that a new viewer should always be opened.
		/// </summary>
		AlwaysOpenNewViewer
	}

	/// <summary>
	/// Exception thrown when a query fails via <see cref="IStudyRootQuery"/>.
	/// </summary>
	public class QueryFailedException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public QueryFailedException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Interface for bridge to <see cref="IViewerAutomation"/>.
	/// </summary>
	/// <remarks>
	/// The bridge design pattern allows the public interface (<see cref="IViewerAutomationBridge"/>) and it's
	/// underlying implementation <see cref="IViewerAutomation"/> to vary independently.
	/// </remarks>
	public interface IViewerAutomationBridge : IDisposable
	{
		/// <summary>
		/// Specifies what the behaviour should be if the primary study is already the primary study in an existing viewer.
		/// </summary>
		OpenStudiesBehaviour OpenStudiesBehaviour { get; set; }

		/// <summary>
		/// Comparer used to sort results from <see cref="IStudyRootQuery.StudyQuery"/>.
		/// </summary>
		IComparer<StudyRootStudyIdentifier> StudyComparer { get; set; }

		/// <summary>
		/// Gets all the active <see cref="Viewer"/>s.
		/// </summary>
		/// <returns></returns>
		IList<Viewer> GetViewers();

		/// <summary>
		/// Gets all the active viewers having the given primary study instance uid.
		/// </summary>
		IList<Viewer> GetViewers(string primaryStudyInstanceUid);

		/// <summary>
		/// Gets all the active viewers where the primary study has the given accession number.
		/// </summary>
		IList<Viewer> GetViewersByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Gets all the active viewers where the primary study has the given patient id.
		/// </summary>
		IList<Viewer> GetViewersByPatientId(string patientId);

		/// <summary>
		/// Opens the given study.
		/// </summary>
		Viewer OpenStudy(string studyInstanceUid);

		/// <summary>
		/// Opens the given studies.
		/// </summary>
		Viewer OpenStudies(IEnumerable<string> studyInstanceUids);

		/// <summary>
		/// Opens the given study.
		/// </summary>
		Viewer OpenStudies(List<OpenStudyInfo> studiesToOpen);
				
		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession number.
		/// </summary>
		Viewer OpenStudiesByAccessionNumber(string accessionNumber);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> accession numbers.
		/// </summary>
		Viewer OpenStudiesByAccessionNumber(IEnumerable<string> accessionNumbers);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient id.
		/// </summary>
		Viewer OpenStudiesByPatientId(string patientId);

		/// <summary>
		/// Opens all studies matching the given <b>exact</b> patient ids.
		/// </summary>
		Viewer OpenStudiesByPatientId(IEnumerable<string> patientIds);

		/// <summary>
		/// Activates the given <see cref="Viewer"/>.
		/// </summary>
		void ActivateViewer(Viewer viewer);

		/// <summary>
		/// Closes the given <see cref="Viewer"/>.
		/// </summary>
		/// <param name="viewer"></param>
		void CloseViewer(Viewer viewer);

		/// <summary>
		/// Gets additional studies, not including the primary one, for the given <see cref="Viewer"/>.
		/// </summary>
		IList<string> GetViewerAdditionalStudies(Viewer viewer);
	}
}