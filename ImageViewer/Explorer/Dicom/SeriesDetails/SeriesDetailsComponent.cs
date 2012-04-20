#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	//TODO (CR Sept 2010): get rid of this - the public API of an application component
	//is meant for the view to consume.  It would be better to expose methods on the context 
	//rather than the component itself and delete the explicit interface.
    public interface ISeriesDetailComponentViewModel
	{
		string PatientId { get; }
		string PatientsName { get; }
		string PatientsBirthDate { get; }
		string AccessionNumber { get; }
		string StudyDate { get; }
		string StudyDescription { get; }
		ActionModelRoot ToolbarActionModel { get; }
		ActionModelRoot ContextMenuActionModel { get; }
		ITable SeriesTable { get; }
        IList<SeriesTableItem> Series { get; }
        IList<SeriesTableItem> SelectedSeries { get; }
		event EventHandler SelectedSeriesChanged;
		void SetSeriesSelection(ISelection selection);
		void Refresh();
		void Close();
	}

	[ExtensionPoint]
	public sealed class SeriesDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof(SeriesDetailsComponentViewExtensionPoint))]
	public class SeriesDetailsComponent : ApplicationComponent, ISeriesDetailComponentViewModel
	{
		private event EventHandler _selectedSeriesChanged;

        private readonly StudyTableItem _studyItem;
        private readonly Table<SeriesTableItem> _seriesTable;
        private readonly IList<SeriesTableItem> _seriesList;

		private ToolSet _toolSet;
		private ActionModelRoot _toolbarActionModel;
		private ActionModelRoot _contextActionModel;

        private IList<SeriesTableItem> _selectedSeries;
		private ISelection _selection;

		internal SeriesDetailsComponent(StudyTableItem studyItem)
		{
			_studyItem = studyItem;
            _seriesTable = new Table<SeriesTableItem>();
            _seriesList = new ReadOnlyListWrapper<SeriesTableItem>(_seriesTable.Items);
            _selectedSeries = new ReadOnlyListWrapper<SeriesTableItem>();
		}

		string ISeriesDetailComponentViewModel.PatientId
		{
			get { return _studyItem.PatientId; }	
		}

		string ISeriesDetailComponentViewModel.PatientsName
		{
			get
			{	
				if (_studyItem.PatientsName != null)
					return new PersonName(_studyItem.PatientsName).FormattedName;
				return "";
			}
		}

		string ISeriesDetailComponentViewModel.PatientsBirthDate
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

		string ISeriesDetailComponentViewModel.AccessionNumber
		{
			get { return _studyItem.AccessionNumber; }
		}

		string ISeriesDetailComponentViewModel.StudyDate
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

		string ISeriesDetailComponentViewModel.StudyDescription
		{
			get { return _studyItem.StudyDescription; }
		}

        protected internal StudyTableItem StudyItem
		{
			get { return _studyItem; }
		}

        public IList<SeriesTableItem> Series
		{
			get { return _seriesList; }
		}

        public IList<SeriesTableItem> SelectedSeries
		{
			get { return _selectedSeries; }
			private set
			{
				if (_selectedSeries != value)
				{
					_selectedSeries = value;
					EventsHelper.Fire(_selectedSeriesChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedSeriesChanged
		{
			add { _selectedSeriesChanged += value; }
			remove { _selectedSeriesChanged -= value; }
		}

		ActionModelRoot ISeriesDetailComponentViewModel.ToolbarActionModel
		{
			get { return _toolbarActionModel; }
		}

		ActionModelRoot ISeriesDetailComponentViewModel.ContextMenuActionModel
		{
			get { return _contextActionModel; }
		}

		ITable ISeriesDetailComponentViewModel.SeriesTable
		{
			get { return _seriesTable; }
		}

		void ISeriesDetailComponentViewModel.SetSeriesSelection(ISelection selection)
		{
			if (_selection != selection)
			{
				_selection = selection;

				//TODO (CR Sept 2010): since we're creating a new wrapper, why not just use
				//ReadOnlyCollection<T> and CollectionUtils.Cast<T>?
				if (_selection != null)
                    SelectedSeries = new ReadOnlyListWrapper<SeriesTableItem>(_selection.Items);
				else
                    SelectedSeries = new ReadOnlyListWrapper<SeriesTableItem>();
			}
		}

		public override void Start()
		{
			InitializeTable();
			BlockingOperation.Run(RefreshInternal);
			_seriesTable.Sort(new TableSortParams(_seriesTable.Columns[0], false));

			_toolSet = new ToolSet(new SeriesDetailsToolExtensionPoint(), new SeriesDetailsToolContext(this));
			_toolbarActionModel = ActionModelRoot.CreateModel(GetType().FullName, SeriesDetailsTool.ToolbarActionSite, _toolSet.Actions);
			_contextActionModel = ActionModelRoot.CreateModel(GetType().FullName, SeriesDetailsTool.ContextMenuActionSite, _toolSet.Actions);

			base.Start();
		}

		public override void Stop()
		{
			_toolbarActionModel = null;
			_contextActionModel = null;
			if (_toolSet != null)
			{
				_toolSet.Dispose();
				_toolSet = null;
			}

			base.Stop();
		}

		private void InitializeTable()
		{
			ITableColumn column = new TableColumn<SeriesIdentifier, string>(
				                        SR.TitleSeriesNumber,
                                        identifier => identifier.SeriesNumber.HasValue ? identifier.SeriesNumber.ToString() : "",
										null, .2F, delegate(SeriesIdentifier series1, SeriesIdentifier series2)
										{
											int? seriesNumber1 = series1.SeriesNumber;
											int? seriesNumber2 = series2.SeriesNumber;

											if (seriesNumber1 == null)
											{
												if (seriesNumber2 == null)
													return 0;
												else
													return 1;
											}
											else if (seriesNumber2 == null)
											{
												return -1;
											}

											return -seriesNumber1.Value.CompareTo(seriesNumber2.Value);
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
				BlockingOperation.Run(RefreshInternal);
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

			try
			{
                var storeQuery = _studyItem.Server.IsSupported<IStudyStoreQuery>()
                                     ? _studyItem.Server.GetService<IStudyStoreQuery>()
                                     : new StudyRootQueryAdapter(_studyItem.Server.GetService<IStudyRootQuery>());

                using (var bridge = new StudyStoreBridge(storeQuery))
                {
                    var entries = bridge.GetSeriesEntries(_studyItem.StudyInstanceUid);
                    _seriesTable.Items.AddRange(entries.Select(e => new SeriesTableItem(e)));
                }
			}
            catch(Exception e)
            {
                ExceptionHandler.Report(e, Host.DesktopWindow);
            }
		}

		public void Close()
		{
			base.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
		}

		#region SeriesDetailsToolContext Class

		private class SeriesDetailsToolContext : ISeriesDetailsToolContext
		{
			private readonly SeriesDetailsComponent _component;

			public SeriesDetailsToolContext(SeriesDetailsComponent component)
			{
				_component = component;
			}

            public IDicomServiceNode Server
            {
                get { return Study.Server; }
            }

            public StudyTableItem Study
			{
				get { return _component.StudyItem; }
			}

            public IList<SeriesTableItem> AllSeries
			{
				get { return _component.Series; }
			}

            public IList<SeriesTableItem> SelectedSeries
			{
				get { return _component.SelectedSeries; }
			}

			public event EventHandler SelectedSeriesChanged
			{
				add { _component.SelectedSeriesChanged += value; }
				remove { _component.SelectedSeriesChanged -= value; }
			}

			public void RefreshSeriesTable()
			{
				_component.Refresh();
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}
		}

		#endregion

		#region ReadOnlyListWrapper Class

		private class ReadOnlyListWrapper<TOut> : IList<TOut>
		{
			private const string _collectionIsReadOnly = "Collection is read-only.";
			private readonly IList _list;

			public ReadOnlyListWrapper() : this(new ArrayList()) {}

			public ReadOnlyListWrapper(object[] array) : this((Array) array) {}

			public ReadOnlyListWrapper(IList list)
			{
				_list = list;
			}

			public int IndexOf(TOut item)
			{
				return _list.IndexOf(item);
			}

			public void Insert(int index, TOut item)
			{
				throw new NotSupportedException(_collectionIsReadOnly);
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException(_collectionIsReadOnly);
			}

			public TOut this[int index]
			{
				get { return (TOut) _list[index]; }
				set { throw new NotSupportedException(_collectionIsReadOnly); }
			}

			public void Add(TOut item)
			{
				throw new NotSupportedException(_collectionIsReadOnly);
			}

			public void Clear()
			{
				throw new NotSupportedException(_collectionIsReadOnly);
			}

			public bool Contains(TOut item)
			{
				return _list.Contains(item);
			}

			public void CopyTo(TOut[] array, int arrayIndex)
			{
				foreach (var item in _list)
					array[arrayIndex++] = (TOut) item;
			}

			public int Count
			{
				get { return _list.Count; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public bool Remove(TOut item)
			{
				throw new NotSupportedException(_collectionIsReadOnly);
			}

			public IEnumerator<TOut> GetEnumerator()
			{
				foreach (var item in _list)
					yield return (TOut) item;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		#endregion
	}
}
