using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Describes a settings group.
    /// </summary>
    public class SettingsGroupDescriptor : IEquatable<SettingsGroupDescriptor>
    {
        public static List<SettingsGroupDescriptor> ListInstalledSettingsGroups()
        {
            List<SettingsGroupDescriptor> groups = new List<SettingsGroupDescriptor>();

            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                foreach (Type t in plugin.Assembly.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(ApplicationSettingsBase)) && !t.IsAbstract)
                    {
                        SettingsGroupDescriptor group = new SettingsGroupDescriptor(
                            SettingsClassMetaDataReader.GetGroupName(t),
                            SettingsClassMetaDataReader.GetVersion(t),
                            SettingsClassMetaDataReader.GetGroupDescription(t),
                            t.AssemblyQualifiedName);

                        groups.Add(group);
                    }
                }
            }

            return groups;
        }

        private string _name;
        private Version _version;
        private string _description;
        private string _assemblyQualifiedTypeName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        /// <param name="assemblyQualifiedTypeName"></param>
        public SettingsGroupDescriptor(string name, Version version, string description, string assemblyQualifiedTypeName)
        {
            _name = name;
            _version = version;
            _description = description;
            _assemblyQualifiedTypeName = assemblyQualifiedTypeName;
        }

        public SettingsGroupDescriptor(Type settingsClass)
        {
            _name = SettingsClassMetaDataReader.GetGroupName(settingsClass);
            _version = SettingsClassMetaDataReader.GetVersion(settingsClass);
            _description = SettingsClassMetaDataReader.GetGroupDescription(settingsClass);
            _assemblyQualifiedTypeName = settingsClass.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the name of the settings group.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the version of the settings group.
        /// </summary>
        public Version Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets the description of the settings group.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the assembly-qualified type name of the class that implements the settings group.
        /// </summary>
        public string AssemblyQualifiedTypeName
        {
            get { return _assemblyQualifiedTypeName; }
        }

        /// <summary>
        /// Settings groups are considered equal if they have the same name and version
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as SettingsGroupDescriptor);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode() ^ _version.GetHashCode();
        }

        #region IEquatable<SettingsGroupDescriptor> Members

        /// <summary>
        /// Settings groups are considered equal if they have the same name and version
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SettingsGroupDescriptor other)
        {
            return other != null && this._name == other._name && this._version == other._version;
        }

        #endregion
    }
}
