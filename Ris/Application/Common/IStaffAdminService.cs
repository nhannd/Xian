using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    public interface IStaffAdminService
    {
        /// <summary>
        /// Search for a staff by name
        /// </summary>
        /// <param name="surname">The staff surname to search for.  May not be null</param>
        /// <param name="givenName">The staff givenname to search for.  May be null</param>
        /// <returns>A list of matching staffs</returns>
        IList<Staff> FindStaffs(string surname, string givenName);

        /// <summary>
        /// Return all staff
        /// </summary>
        /// <returns>A list of all staffs</returns>
        IList<Staff> GetAllStaffs();

        /// <summary>
        /// Add a staff
        /// </summary>
        /// <param name="staff"></param>
        void AddStaff(Staff staff);

        /// <summary>
        /// Update a staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        void UpdateStaff(Staff staff);

        /// <summary>
        /// Load a staff from an entity ref, and optionally with details
        /// </summary>
        /// <param name="staffRef">A reference to the staff to load</param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        Staff LoadStaff(EntityRef staffRef, bool withDetails);
    }
}
