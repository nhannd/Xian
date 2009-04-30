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
using System.IO;
using System.Security;
using System.Web.Hosting;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    [XmlRoot("User")]
    public class XmlMembershipUser : MembershipUser
    {
        private string _password;
        private string _userName;

        [XmlAttribute]
        public new string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public bool PasswordExpired
        {
            get{
                return String.IsNullOrEmpty(Password); 
            }
        }

        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            if (!String.IsNullOrEmpty(Password) && oldPassword!=Password)
            {
                throw new SecurityException("Invalid password");
            }
            _password = newPassword;
            return true;
        }
    }

    class XmlMembershipProvider: MembershipProvider
    {
        private object _syncRoot = new object();
        private string _path;

        private Dictionary<string, XmlMembershipUser> _users;

        private void ReadData()
        {
            if (_users==null)
            {
                lock(_syncRoot)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(_path);

                    _users = new Dictionary<string, XmlMembershipUser>();
                    foreach (XmlNode node in doc.SelectNodes("//Users/User"))
                    {
                        XmlMembershipUser user = XmlUtils.Deserialize<XmlMembershipUser>(node);
                        if (user.IsApproved)
                        {
                            _users.Add(user.UserName, user);
                        }
                    }
                }
                
            }
        }


        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _path = config["file"];

            if (String.IsNullOrEmpty(_path))
                _path = "~/Users.xml";

            _path = HostingEnvironment.MapPath(_path);
            Platform.CheckTrue(File.Exists(_path), String.Format("File {0} doesn't exist", _path));
        }
        
        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            MembershipUser user = GetUser(username, true);
            if (user == null)
                throw new SecurityException("No such user");

            if (user.ChangePassword(oldPassword, newPassword))
            {
                UpdateUser(user);
                return true;
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool EnablePasswordReset
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetPassword(string username, string answer)
        {
            if (!_users.ContainsKey(username))
                throw new SecurityException("Invalid username");

            XmlMembershipUser user = _users[username];
            return user.Password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (!_users.ContainsKey(username))
                throw new SecurityException("Invalid username");

            XmlMembershipUser user = _users[username];
            return user;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotSupportedException();
        }
                
        public override string GetUserNameByEmail(string email)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            XmlMembershipUser theUser = user as XmlMembershipUser;
            XmlDocument doc = new XmlDocument();
            doc.Load(_path);
            XmlNode node = doc.SelectSingleNode(String.Format("//Users/User[@UserName='{0}']", theUser.UserName));
            if (node == null)
                throw new SecurityException("No such user");

            XmlNode parent = node.ParentNode;
            parent.RemoveChild(node);
            parent.AppendChild(doc.ImportNode(XmlUtils.Serialize(theUser), true));
            doc.Save(_path);
        }

        public override bool ValidateUser(string username, string password)
        {

            ReadData();

            if (!_users.ContainsKey(username))
                return false;

            XmlMembershipUser user = _users[username];
            if (user.PasswordExpired)
                throw new PasswordExpiredException();

            if (!user.Password.Equals(password))
                return false;

            return true;
        }
    }
}