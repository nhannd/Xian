#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="StaffStaffGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffStaffGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffStaffGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(StaffStaffGroupEditorComponentViewExtensionPoint))]
    public abstract class StaffStaffGroupEditorComponent : ApplicationComponent
    {
        class StaffGroupTable : Table<StaffGroupSummary>
        {
            public StaffGroupTable()
            {
                this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name",
                    delegate(StaffGroupSummary item) { return item.Name; }));
            }
        }

    	private readonly bool _readOnly;
        private readonly StaffGroupTable _availableGroups;
        private readonly StaffGroupTable _selectedGroups;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected StaffStaffGroupEditorComponent(bool readOnly)
			: this(new StaffGroupSummary[0], new StaffGroupSummary[0], readOnly)
		{
		}

        /// <summary>
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        public StaffStaffGroupEditorComponent(IList<StaffGroupSummary> groups, IList<StaffGroupSummary> groupChoices, bool readOnly)
        {
			_readOnly = readOnly;
            _selectedGroups = new StaffGroupTable();
			_availableGroups = new StaffGroupTable();

			Initialize(groups, groupChoices);
        }

        public IList<StaffGroupSummary> SelectedItems
        {
            get { return _selectedGroups.Items; }
        }

        #region Presentation Model

    	public bool ReadOnly
    	{
			get { return _readOnly; }
    	}

        public ITable AvailableGroupsTable
        {
            get { return _availableGroups; }
        }

        public ITable SelectedGroupsTable
        {
            get { return _selectedGroups; }
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        #endregion

		/// <summary>
		/// Protected method to re-initialize the component.
		/// </summary>
		/// <param name="groups"></param>
		/// <param name="groupChoices"></param>
		protected void Initialize(IList<StaffGroupSummary> groups, IList<StaffGroupSummary> groupChoices)
		{
			_selectedGroups.Items.Clear();
			_selectedGroups.Items.AddRange(groups);

			_availableGroups.Items.Clear();
			_availableGroups.Items.AddRange(CollectionUtils.Reject(groupChoices,
				delegate(StaffGroupSummary x)
				{
					return CollectionUtils.Contains(_selectedGroups.Items,
						delegate(StaffGroupSummary y) { return x.StaffGroupRef.Equals(y.StaffGroupRef, true); });
				}));
		}
	}

	/// <summary>
	/// Staff Group Editor that shows only non-elective staff groups.
	/// </summary>
	public class StaffNonElectiveStaffGroupEditorComponent : StaffStaffGroupEditorComponent
	{
		public StaffNonElectiveStaffGroupEditorComponent(IList<StaffGroupSummary> groups, IList<StaffGroupSummary> groupChoices, bool readOnly)
			: base(groups, groupChoices, readOnly)
        {
			Initialize(
				CollectionUtils.Reject(groups, delegate(StaffGroupSummary g) { return g.IsElective; }),
				CollectionUtils.Reject(groupChoices, delegate(StaffGroupSummary g) { return g.IsElective; }));
        }
	}

	/// <summary>
	/// Staff Group Editor that shows only elective staff groups.
	/// </summary>
	public class StaffElectiveStaffGroupEditorComponent : StaffStaffGroupEditorComponent
	{
		public StaffElectiveStaffGroupEditorComponent(IList<StaffGroupSummary> groups, IList<StaffGroupSummary> groupChoices, bool readOnly)
			: base(groups, groupChoices, readOnly)
        {
			Initialize(
				CollectionUtils.Select(groups, delegate(StaffGroupSummary g) { return g.IsElective; }),
				CollectionUtils.Select(groupChoices, delegate(StaffGroupSummary g) { return g.IsElective; }));
        }
	}

}
