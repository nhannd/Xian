using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common.Services.Login
{
    public class LoginResult
    {
        private bool _successful;
        private SessionToken _token;
        private string[] _groups;

        public SessionToken Token
        {
            get { return _token; }
            set { _token = value; }
        }

        public string[] Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public bool Successful
        {
            get { return _successful; }
            set { _successful = value; }
        }
    }

    public interface ILoginService
    {
        LoginResult SignIn(string userName, string password);
        void SignOff(string userName, SessionToken token);
    }
}
