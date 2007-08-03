using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Authorization
{
    /// <summary>
    /// Helper class for providing authority group definitions to be imported at deployment time.
    /// </summary>
    public class AuthorityGroupDefinition
    {
        private string _name;
        private string[] _tokens;

        public AuthorityGroupDefinition(string name, string[] tokens)
        {
            _name = name;
            _tokens = tokens;
        }

        /// <summary>
        /// Gets the name of the authority group.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the set of tokens that are assigned to the group.
        /// </summary>
        public string[] Tokens
        {
            get { return _tokens; }
        }
    }
}
