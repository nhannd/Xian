using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Threading;
using System.Security;

namespace ClearCanvas.Desktop.Actions
{
    public class ActionPermissionAttribute : ActionAttribute
    {
        private string[] _roles;

        public ActionPermissionAttribute(string actionID, string role)
            : this(actionID, new string[] { role })
        {
        }

        public ActionPermissionAttribute(string actionID, string[] roles)
            :base(actionID)
        {
            _roles = roles;
        }

        public void Demand()
        {
            foreach (string role in _roles)
            {
                PrincipalPermission perm = new PrincipalPermission(null, role);
                perm.Demand();
            }
        }

        public override void Apply(IActionBuildingContext builder)
        {
        }
    }
}
