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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.StudyLocator;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[ExtensionPoint]
	public sealed class SeriesDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(SeriesDetailsComponentViewExtensionPoint))]
	public class SeriesDetailsComponent : ApplicationComponent
	{
		private readonly StudyItem _studyItem;
		private readonly Table<SeriesIdentifier> _seriesTable;
		private readonly IServerTreeNode _server;

		internal SeriesDetailsComponent(StudyItem studyItem, IServerTreeNode server)
		{
			_studyItem = studyItem;
			_seriesTable = new Table<SeriesIdentifier>();
			_server = server;
		}

		public string PatientId
		{
			get { return _studyItem.PatientId; }	
		}

		public string PatientsName
		{
			get
			{	
				if (_studyItem.PatientsName != null)
					return _studyItem.PatientsName.FormattedName;
				return "";
			}
		}

		public string PatientsBirthDate
		{
			get
			{
				if (!string.IsNullOrEmpty(_studyItem.PatientsBirthDate))
				{
					DateTime? date = DateParser.Parse(_studyItem.PatientsBirthDate);
					if (date.HasValue)
						return Format.Date(date);
				}

				return "";
			}	
		}

		public string AccessionNumber
		{
			get { return _studyItem.AccessionNumber; }
		}

		public string StudyDate
		{
			get
			{
				if (!string.IsNullOrEmpty(_studyItem.StudyDate))
				{
					DateTime? date = DateParser.Parse(_studyItem.StudyDate);
					if (date.HasValue)
						return Format.Date(date);
				}

				return "";
			}
		}

		public string StudyDescription
		{
			get { return _studyItem.StudyDescription; }	
		}

		public ITable SeriesTable
		{
			get { return _seriesTable; }	
		}

		public override void Start()
		{
			InitializeTable();
			RefreshInternal();
			_seriesTable.Sort(new TableSortParams(_seriesTable.Columns[0], false));

			base.Start();
		}

		private void InitializeTable()
		{
			ITableColumn column = new TableColumn<SeriesIdentifier, string>(
				SR.TitleSeriesNumber, delegate(SeriesIdentifier identifier)
				                      	{
				                      		return identifier.SeriesNumber;
										}, 
										null, .2F,
										delegate(SeriesIdentifier series1, SeriesIdentifier series2)
				                      	         	{
				                      	         		string seriesNumber1 = series1.SeriesNumber;
														string seriesNumber2 = series2.SeriesNumber;

														if (String.IsNullOrEmpty(seriesNumber1))
				                      	         		{
															if (String.IsNullOrEmpty(seriesNumber2))
																return 0;
															else
																return 1;
				                      	         		}
														else if (string.IsNullOrEmpty(seriesNumber2))
				                      	         		{
				                      	         			return -1;
				                      	         		}

				                      	         		int n1, n2;
														if (!int.TryParse(seriesNumber1, out n1))
				                      	         			n1 = 0;

														if (!int.TryParse(seriesNumber2, out n2))
															n2 = 0;

				                      	         		return -n1.CompareTo(n2);
													});
			
			_seriesTable.Columns.Add(column);

			column = new TableColumn<SeriesIdentifier, string>(
			SR.TitleModality, delegate(SeriesIdentifier identifier)
									{
										return identifier.Modality;
									}, .2F);

			_seriesTable.Columns.Add(column);

			column = new TableColumn<SeriesIdentifier, string>(
					SR.TitleSeriesDescription, delegate(SeriesIdentifier identifier)
											{
												return identifier.SeriesDescription;
											}, 0.4F);

			_seriesTable.Columns.Add(column);

			column = new TableColumn<SeriesIdentifier, string>(
		SR.TitleNumberOfSeriesRelatedInstances, delegate(SeriesIdentifier identifier)
								{
									if (identifier.NumberOfSeriesRelatedInstances.HasValue)
										return identifier.NumberOfSeriesRelatedInstances.Value.ToString();
									else
										return "";
								},null , 0.2F, delegate(SeriesIdentifier series1, SeriesIdentifier series2)
				                      	         	{
				                      	         		int? instances1 = series1.NumberOfSeriesRelatedInstances;
														int? instances2 = series2.NumberOfSeriesRelatedInstances;

														if (instances1 == null)
				                      	         		{
															if (instances2 == null)
																return 0;
															else
																return 1;
				                      	         		}
														else if (instances2 == null)
				                      	         		{
				                      	         			return -1;
				                      	         		}

				                      	         		return -instances1.Value.CompareTo(instances2.Value);
													});

			_seriesTable.Columns.Add(column);
		}

		public void Refresh()
		{
			try
			{
				RefreshInternal();
				_seriesTable.Sort();
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, base.Host.DesktopWindow);
			}
		}

		internal void RefreshInternal()
		{
			_seriesTable.Items.Clear();

			//NOTE: this whole thing is a total hack and should not be publicly distributed or used right now.
			IStudyRootQuery query;
			if (_server.IsLocalDataStore)
			{
				query = (IStudyRootQuery)new LocalStudyRootQueryExtensionPoint().CreateExtension();
			}
			else
			{
				Server server = (Server)_server;
				query = new DicomStudyRootQuery(
					ServerTree.GetClientAETitle(), server.AETitle, server.Host, server.Port);
			}

			try
			{
				SeriesIdentifier identifier = new SeriesIdentifier();
				identifier.StudyInstanceUid = _studyItem.StudyInstanceUID;
				IList<SeriesIdentifier> results = query.SeriesQuery(identifier);
				_seriesTable.Items.AddRange(results);
			}
			finally
			{
				if (query is IDisposable)
					((IDisposable)query).Dispose();
			}
		}

		public void Close()
		{
			base.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
		}
	}
}
