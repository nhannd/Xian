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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "Staff Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    class StaffImporter : CsvDataImporterBase
    {
        private const int _numFields = 9;

        private IPersistenceContext _context;

        #region CsvDataImporterBase overrides

        /// <summary>
        /// Import staff from CSV format.
        /// </summary>
        /// <param name="rows">
        /// Each string in the list must contain 25 CSV fields, as follows:
        ///     0 - UserName
        ///     1 - StaffType
        ///     2 - Id
        ///     3 - FamilyName
        ///     4 - GivenName
        ///     5 - MiddleName
        ///     6 - Prefix
        ///     7 - Suffix
        ///     8 - Degree
        /// </param>
        /// <param name="context"></param>
        public override void Import(List<string> rows, IUpdateContext context)
        {
            _context = context;

            List<Staff> importedStaff = new List<Staff>();

            foreach (string row in rows)
            {
                string[] fields = ParseCsv(row, _numFields);

                string userName = fields[0];

            	StaffTypeEnum staffType = context.GetBroker<IEnumBroker>().Find<StaffTypeEnum>(fields[1]);

                string staffId = fields[2];
                string staffFamilyName = fields[3];
                string staffGivenName = fields[4];
                string staffMiddlename = fields[5];
                string staffPrefix = fields[6];
                string staffSuffix = fields[7];
                string staffDegree = fields[8];

                Staff staff = GetStaff(staffId, importedStaff);

                if (staff == null)
                {
                    staff = new Staff();

                    staff.Id = staffId;
                    staff.Type = staffType;
                    staff.Name = new PersonName(staffFamilyName, staffGivenName, staffMiddlename, staffPrefix, staffSuffix, staffDegree);
                    staff.UserName = userName;

                    _context.Lock(staff, DirtyState.New);

                    importedStaff.Add(staff);
                }

            }
        }

        #endregion

        #region Private Methods

        private Staff GetStaff(string staffId, IList<Staff> importedStaff)
        {
            Staff staff = null;

            staff = CollectionUtils.SelectFirst<Staff>(importedStaff,
                delegate(Staff s) { return s.Id == staffId; });

            if (staff == null)
            {
                StaffSearchCriteria criteria = new StaffSearchCriteria();
                criteria.Id.EqualTo(staffId);

                IStaffBroker broker = _context.GetBroker<IStaffBroker>();
                staff = CollectionUtils.FirstElement(broker.Find(criteria));
            }

            return staff;
        }

        #endregion

    }
}
