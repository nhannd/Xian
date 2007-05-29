using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Provides utilities for staff name resolution.
    /// </summary>
    public class StaffFinder
    {
        /// <summary>
        /// Attempts to resolve the specified query to a single staff.  The query may consist of part of the surname,
        /// optionally followed by a comma and then part of the given name (e.g. "sm, b" for smith, bill).  The method
        /// returns true if the query resolves to a unique staff with the system, or false otherwise.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static bool ResolveName(string query, out StaffSummary staff)
        {
            if (!string.IsNullOrEmpty(query))
            {
                ListStaffRequest request = new ListStaffRequest();
                request.PageRequest = new PageRequestDetail(0, 2);  // need at most 2 rows to know if resolution was successful

                string[] names = query.Split(',');
                if (names.Length > 0)
                    request.LastName = names[0].Trim();
                if (names.Length > 1)
                    request.FirstName = names[1].Trim();

                ListStaffResponse response = null;
                Platform.GetService<IStaffAdminService>(
                    delegate(IStaffAdminService service)
                    {
                        response = service.ListStaff(request);
                    });
                if (response.Staffs.Count == 1)
                {
                    staff = response.Staffs[0];
                    return true;
                }
            }

            // can't resolve
            staff = null;
            return false;
        }
          
        /// <summary>
        /// Attempts to resolve the specified query to a single staff.  The query may consist of part of the surname,
        /// optionally followed by a comma and then part of the given name (e.g. "sm, b" for smith, bill).  If the 
        /// query matches more than a single name, a dialog is shown allowing the user to select a staff person.
        /// The method returns true if the name is successfully resolved, or false otherwise.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="desktopWindow"></param>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static bool ResolveNameInteractive(string query, IDesktopWindow desktopWindow, out StaffSummary staff)
        {
            bool resolved = ResolveName(query, out staff);
            if (!resolved)
            {
                StaffSummaryComponent staffComponent = new StaffSummaryComponent(true);

                if (!string.IsNullOrEmpty(query))
                {
                    string[] names = query.Split(',');
                    if (names.Length > 0)
                        staffComponent.LastName = names[0].Trim();
                    if (names.Length > 1)
                        staffComponent.FirstName = names[1].Trim();
                }

                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    desktopWindow, staffComponent, SR.TitleStaff);

                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    staff = (StaffSummary)staffComponent.SelectedStaff.Item;
                    if (staff != null)
                    {
                        resolved = true;
                    }
                }
            }
            return resolved;
        }

    }
}
