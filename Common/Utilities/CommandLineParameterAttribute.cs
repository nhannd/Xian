using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// When placed on a field/property of a class derived from <see cref="CommandLine"/>, instructs
    /// the base class to attempt to set the field/property according to the parsed command line arguments.
    /// </summary>
    /// <remarks>
    /// If the field/property is of type string, int, or enum, it is treated as a named parameter, unless
    /// the <see cref="Position"/> property of the attribute is set, in which case it is treated as a positional
    /// parameter.  If the field/property is of type boolean, it is treated as a switch.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CommandLineParameterAttribute : Attribute
    {
        private readonly int _position = -1;
        private bool _required;
        private readonly string[] _keys;
        private readonly string _displayName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="displayName"></param>
        public CommandLineParameterAttribute(int position, string displayName)
        {
            _position = position;
            _displayName = displayName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="displayName"></param>
        public CommandLineParameterAttribute(string[] keys, string displayName)
        {
            _keys = keys;
            _displayName = displayName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is a required parameter.
        /// </summary>
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        internal int Position
        {
            get { return _position; }
        }

        internal string[] Keys
        {
            get { return _keys; }
        }

        internal string DisplayName
        {
            get { return _displayName; }
        }
    }
}
