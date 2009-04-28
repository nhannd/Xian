using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    class XmlRoleProvider:RoleProvider
    {
        private string _path;
        private Dictionary<string, List<string>> _roles = new Dictionary<string, List<string>>();

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            // Initialize _XmlFileName and make sure the path
            // is app-relative
            _path = config["file"];

            if (String.IsNullOrEmpty(_path))
                _path = "~/Data/Roles.xml";

            _path = HostingEnvironment.MapPath(_path);
            Platform.CheckTrue(File.Exists(_path), String.Format("File {0} doesn't exist", _path));

        }
        private void ReadData()
        {
            XmlDocument doc = new XmlDocument();
            using (Stream stream = File.OpenRead(_path))
            {
                doc.Load(stream);

                _roles = new Dictionary<string, List<string>>();
                foreach (XmlNode node in doc.SelectNodes("//Users/User"))
                {
                    string username = node.Attributes["UserName"].Value;
                    List<string> tokens = new List<string>();
                    foreach (XmlNode authorityNode in node.SelectNodes("AuthorityToken"))
                    {
                        tokens.Add(authorityNode.InnerText);
                    }
                    _roles.Add(username, tokens);
                }
            }

        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
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

        public override void CreateRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetAllRoles()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string[] GetRolesForUser(string username)
        {
            ReadData();
            if (!_roles.ContainsKey(username))
                throw new SecurityException("No authority specified for this user");

            return _roles[username].ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool RoleExists(string roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}