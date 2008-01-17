using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Authentication
{
    public class UserInfo
    {
        public UserInfo(string userName, string displayName, DateTime? validFrom, DateTime? validUntil)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.ValidFrom = validFrom;
            this.ValidUntil = validUntil;
        }

        public string UserName;
        public string DisplayName;
        public DateTime? ValidFrom;
        public DateTime? ValidUntil;
    }
}
