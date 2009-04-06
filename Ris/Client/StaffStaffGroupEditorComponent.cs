#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
    public class StaffStaffGroupEditorComponent : ApplicationComponent
    {
        class StaffGroupTable : Table<StaffGroupSummary>
        {
            public StaffGroupTable()
            {
                this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name",
                    delegate(StaffGroupSummary item) { return item.Name; }));
            }
        }

        private readonly StaffGroupTable _availableGroups;
        private readonly StaffGroupTable _selectedGroups;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected StaffStaffGroupEditorComponent()
			: this(new StaffGroupSummary[0], new StaffGroupSummary[0])
		{
		}

        /// <summary>
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        public StaffStaffGroupEditorComponent(IList<StaffGroupSummary> groups, IList<StaffGroupSummary> groupChoices)
        {
            _selectedGroups = new StaffGroupTable();
			_availableGroups = new StaffGroupTable();

			Initialize(groups, groupChoices);
        }

        public IList<StaffGroupSummary> SelectedItems
        {
            get { return _selectedGroups.Items; }
        }

        #region Presentation Model

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
}
