using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public sealed class VoiLutPresetConfigurationCollection : IEnumerable<VoiLutPresetConfiguration>
	{
		private SortedList<VoiLutPresetConfigurationKey, VoiLutPresetConfiguration> _configurations;

		public VoiLutPresetConfigurationCollection()
		{
			_configurations = new SortedList<VoiLutPresetConfigurationKey, VoiLutPresetConfiguration>(new VoiLutPresetConfigurationKeySorter());
		}

		public int Count
		{
			get { return _configurations.Count; }
		}

		private void CheckConflictException(VoiLutPresetConfiguration newConfiguration, VoiLutPresetConfiguration[] matchingConfigurations, string operationName)
		{
			//at most there can be two other conflicts, but we'll compute the combination.
			bool combinedNameConflict = false, combinedKeyStrokeConflict = false;
			foreach (VoiLutPresetConfiguration configuration in matchingConfigurations)
			{
				bool nameConflict, keyStrokeConflict;
				newConfiguration.Key.CheckConflict(configuration.Key, out nameConflict, out keyStrokeConflict);
				combinedNameConflict |= nameConflict;
				combinedKeyStrokeConflict |= keyStrokeConflict;
			}

			if (combinedNameConflict && combinedKeyStrokeConflict)
				throw new InvalidOperationException(String.Format(SR.ExceptionWindowLevelPresetDualConflict, operationName));
			else if (combinedNameConflict)
				throw new InvalidOperationException(String.Format(SR.ExceptionWindowLevelPresetSingleConflict, operationName, SR.Name));
			else// if (combinedKeyStrokeConflict)
				throw new InvalidOperationException(String.Format(SR.ExceptionWindowLevelPresetSingleConflict, operationName, SR.KeyStroke));
		}

		private VoiLutPresetConfiguration[] FindMatchingConfigurations(VoiLutPresetConfigurationKey key, VoiLutPresetConfigurationKey excludeExactKey)
		{
			List<VoiLutPresetConfiguration> matches = new List<VoiLutPresetConfiguration>();
			foreach (VoiLutPresetConfiguration configuration in _configurations.Values)
			{
				if (configuration.Key.IsMatchingKey(key))
				{
					if (excludeExactKey == null || !excludeExactKey.Equals(configuration.Key))
						matches.Add(configuration);
				}
			}

			return matches.ToArray();
		}

		public void Add(VoiLutPresetConfiguration newConfiguration)
		{
			Platform.CheckForNullReference(newConfiguration, "newConfiguration");

			//in the case of an add, the same item key counts as a conflict.
			VoiLutPresetConfiguration[] matches = FindMatchingConfigurations(newConfiguration.Key, null);
			if (matches.Length == 0)
			{
				this.UnsafeUpdate(newConfiguration);
			}
			else
			{
				CheckConflictException(newConfiguration, matches, SR.Added);
			}
		}

		public void Replace(VoiLutPresetConfiguration existingConfiguration, VoiLutPresetConfiguration newConfiguration)
		{
			Platform.CheckForNullReference(existingConfiguration, "existingConfiguration");
			Platform.CheckForNullReference(newConfiguration, "newConfiguration");

			int existingIndex = _configurations.IndexOfValue(existingConfiguration);
			if (existingIndex < 0)
			{
				throw new ArgumentException(SR.ExceptionPresetConfigurationDoesNotExist);
			}

			VoiLutPresetConfiguration[] matches = FindMatchingConfigurations(newConfiguration.Key, existingConfiguration.Key);
			if (matches.Length == 0)
			{
				this.Remove(existingConfiguration);
				this.UnsafeUpdate(newConfiguration);
			}
			else
			{
				CheckConflictException(newConfiguration, matches, SR.Edited);
			}
		}

		public void UnsafeUpdate(VoiLutPresetConfiguration updateConfiguration)
		{
			Platform.CheckForNullReference(updateConfiguration, "updateConfiguration");

			this.RemoveAll(updateConfiguration.Key);
			_configurations[updateConfiguration.Key] = updateConfiguration;
		}

		public VoiLutPresetConfiguration[] FindMatchingConfigurations(VoiLutPresetConfigurationKey key)
		{
			Platform.CheckForNullReference(key, "key");
			return FindMatchingConfigurations(key, null);
		}

		public void RemoveAll(VoiLutPresetConfigurationKey key)
		{
			Platform.CheckForNullReference(key, "key");

			VoiLutPresetConfiguration[] matches = this.FindMatchingConfigurations(key);
			foreach (VoiLutPresetConfiguration configuration in matches)
				this.Remove(configuration);
		}

		public bool Remove(VoiLutPresetConfiguration item)
		{
			Platform.CheckForNullReference(item, "item");

			VoiLutPresetConfiguration searchDummy = new VoiLutPresetConfiguration(item.Key);
			int index = _configurations.IndexOfValue(searchDummy);
			if (index >= 0)
			{
				_configurations.RemoveAt(index);
				return true;
			}

			return false;
		}

		public void Clear()
		{
			_configurations.Clear();
		}

		#region IEnumerable<VoiLutPresetConfiguration> Members

		public IEnumerator<VoiLutPresetConfiguration> GetEnumerator()
		{
			return _configurations.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _configurations.Values.GetEnumerator();
		}

		#endregion
	}
}
