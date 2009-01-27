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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistFilterEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistFilterEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistFilterEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistFilterEditorComponentViewExtensionPoint))]
    public class WorklistFilterEditorComponent : ApplicationComponent
    {
        class DummyItem
        {
            private readonly string _displayString;

            public DummyItem(string displayString)
            {
                _displayString = displayString;
            }

            public override string ToString()
            {
                return _displayString;
            }
        }

        private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);
        private static readonly object _workingFacilityItem = new DummyItem(SR.DummyItemWorkingFacility);
        private static readonly object _portableItem = new DummyItem(SR.DummyItemPortable);
        private static readonly object _nonPortableItem = new DummyItem(SR.DummyItemNonPortable);

        private static readonly object[] _portableChoices = new object[] { _portableItem, _nonPortableItem };

        private readonly ArrayList _facilityChoices;
        private ArrayList _selectedFacilities;

        private readonly List<EnumValueInfo> _priorityChoices;
        private readonly List<EnumValueInfo> _patientClassChoices;
        private ArrayList _selectedPortabilities;

    	private ExternalPractitionerLookupHandler _orderingPractitionerLookupHandler;
    	private ExternalPractitionerSummary _selectedOrderingPractitioner;

        private readonly WorklistAdminDetail _worklistDetail;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistFilterEditorComponent(WorklistAdminDetail detail,
            List<ProcedureTypeGroupSummary> procedureTypeGroupChoices,
            List<FacilitySummary> facilityChoices,
            List<EnumValueInfo> priorityChoices,
            List<EnumValueInfo> patientClassChoices)
        {
            _worklistDetail = detail;

            _facilityChoices = new ArrayList();
            _facilityChoices.Add(_workingFacilityItem);
            _facilityChoices.AddRange(facilityChoices);
            _selectedFacilities = new ArrayList();
            _selectedPortabilities = new ArrayList();

            _priorityChoices = priorityChoices;
            _patientClassChoices = patientClassChoices;

        }

        public override void Start()
        {

            if (_worklistDetail.FilterByWorkingFacility)
                _selectedFacilities.Add(_workingFacilityItem);
            _selectedFacilities.AddRange(_worklistDetail.Facilities);

            if (_worklistDetail.Portabilities.Contains(true))
                _selectedPortabilities.Add(_portableItem);
            if (_worklistDetail.Portabilities.Contains(false))
                _selectedPortabilities.Add(_nonPortableItem);

			_orderingPractitionerLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);
			if(_worklistDetail.OrderingPractitioners != null)
			{
				// GUI only allows 1 ordering practitioner - could change this in future if needed
				_selectedOrderingPractitioner = CollectionUtils.FirstElement(_worklistDetail.OrderingPractitioners);
			}

            base.Start();
        }

        #region Presentation Model

        public string ProcedureTypeGroupClassName
        {
            get { return _worklistDetail.WorklistClass.ProcedureTypeGroupClassDisplayName; }
        }

        public object NullFilterItem
        {
            get { return _nullFilterItem; }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        public string FormatFacility(object item)
        {
            if (item is FacilitySummary)
            {
                FacilitySummary facility = (FacilitySummary)item;
                return facility.Code;
            }
            else
                return item.ToString(); // place-holder items
        }

        public IList SelectedFacilities
        {
            get { return _selectedFacilities; }
            set
            {
                if (!CollectionUtils.Equal(value, _selectedFacilities, false))
                {
                    _selectedFacilities = new ArrayList(value);
                    this.Modified = true;
                }
            }
        }

        public IList PriorityChoices
        {
            get { return _priorityChoices; }
        }

        public IList SelectedPriorities
        {
            get { return _worklistDetail.OrderPriorities; }
            set
            {
                IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
                if (!CollectionUtils.Equal(list, _worklistDetail.OrderPriorities, false))
                {
                    _worklistDetail.OrderPriorities = new List<EnumValueInfo>(list);
                    this.Modified = true;
                }
            }
        }

        public IList PatientClassChoices
        {
            get { return _patientClassChoices; }
        }

        public IList SelectedPatientClasses
        {
            get { return _worklistDetail.PatientClasses; }
            set
            {
                IList<EnumValueInfo> list = new TypeSafeListWrapper<EnumValueInfo>(value);
                if (!CollectionUtils.Equal(list, _worklistDetail.PatientClasses, false))
                {
                    _worklistDetail.PatientClasses = new List<EnumValueInfo>(list);
                    this.Modified = true;
                }
            }
        }

        public IList PortableChoices
        {
            get { return _portableChoices; }
        }

        public IList SelectedPortabilities
        {
            get { return _selectedPortabilities; }
            set
            {
                if (!CollectionUtils.Equal(value, _selectedPortabilities, false))
                {
                    _selectedPortabilities = new ArrayList(value);
                    this.Modified = true;
                }
            }
        }

    	public ILookupHandler OrderingPractitionerLookupHandler
    	{
			get { return _orderingPractitionerLookupHandler; }
    	}

    	public ExternalPractitionerSummary SelectedOrderingPractitioner
    	{
			get { return _selectedOrderingPractitioner; }
			set
			{
				if(!Equals(value, _selectedOrderingPractitioner))
				{
					_selectedOrderingPractitioner = value;
					this.Modified = true;
					NotifyPropertyChanged("SelectedOrderingPractitioner");
				}
			}
    	}

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }

        #endregion


        internal void SaveData()
        {
            _worklistDetail.Facilities = new List<FacilitySummary>();
            _worklistDetail.Facilities.AddRange(
                new TypeSafeListWrapper<FacilitySummary>(
                    CollectionUtils.Select(_selectedFacilities, delegate(object f) { return f is FacilitySummary; })));
            _worklistDetail.FilterByWorkingFacility =
                CollectionUtils.Contains(_selectedFacilities, delegate(object f) { return f == _workingFacilityItem; });

            _worklistDetail.Portabilities = CollectionUtils.Map<object, bool>(_selectedPortabilities,
                delegate(object item) { return item == _portableItem ? true : false; });

			// GUI only allows 1 ordering practitioner - could change this in future if needed
			_worklistDetail.OrderingPractitioners = new List<ExternalPractitionerSummary>();
			if(_selectedOrderingPractitioner != null)
			{
				_worklistDetail.OrderingPractitioners.Add(_selectedOrderingPractitioner);
			}
        }
    }
}
