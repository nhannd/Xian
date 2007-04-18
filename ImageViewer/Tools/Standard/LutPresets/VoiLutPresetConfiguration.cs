using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public sealed class VoiLutPresetConfigurationKeySorter : IComparer<VoiLutPresetConfigurationKey>
	{
		public VoiLutPresetConfigurationKeySorter()
		{ 
		}

		#region IComparer<VoiLutPresetConfigurationKey> Members

		public int Compare(VoiLutPresetConfigurationKey x, VoiLutPresetConfigurationKey y)
		{
			if (String.IsNullOrEmpty(x.ModalityFilter))
			{
				if (!String.IsNullOrEmpty(y.ModalityFilter))
					return 1;
			}
			else if (String.IsNullOrEmpty(y.ModalityFilter))
			{
				return -1;
			}
			else if (x.ModalityFilter != y.ModalityFilter)
			{
				return Math.Sign(x.ModalityFilter.CompareTo(y.ModalityFilter));
			}

			if (x.KeyStroke == y.KeyStroke)
			{
				if (x.KeyStroke != XKeys.None)
					return 0; //keystrokes are the same and are not 'none', they are 'equal'.

				//the keystrokes are equal, use the name.
				int compare = x.Name.CompareTo(y.Name);
				return compare == 0 ? 0 : Math.Sign(compare);
			}
			else
			{
				if (x.KeyStroke == XKeys.None)
				{
					if (y.KeyStroke != XKeys.None)
						return 1;
				}
				else if (y.KeyStroke == XKeys.None)
				{
					return -1;
				}

				//the keystrokes are not equal and are not none.
				return Math.Sign(x.KeyStroke - y.KeyStroke);
			}
		}

		#endregion
	}

	public sealed class VoiLutPresetConfigurationKey
	{
		private string _modalityFilter;
		private XKeys _keyStroke;
		private string _name;
		private string _text;

		public VoiLutPresetConfigurationKey(string modalityFilter, XKeys keyStroke, string name)
		{
			Platform.CheckForEmptyString(name, "name");

			if (modalityFilter == null)
				modalityFilter = "";
			
			_modalityFilter = modalityFilter;
			_keyStroke = keyStroke;
			_name = name;
			_text = String.Format("Modality: {0}, Key: {1}, Name: {2}", _modalityFilter, _keyStroke.ToString(), _name);
		}

		private VoiLutPresetConfigurationKey()
		{ 
		}

		public string ModalityFilter
		{
			get { return _modalityFilter; }
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
		}

		public string Name
		{
			get { return _name; }
		}

		public void CheckConflict(VoiLutPresetConfigurationKey other, out bool nameConflict, out bool keyStrokeConflict)
		{
			nameConflict = false;
			keyStrokeConflict = false;	

			if (this._modalityFilter != other._modalityFilter)
				return;

			//equal keystrokes means they are equal keys, unless the keystroke is none,
			//in which case the name becomes the identifier.
			if (this._keyStroke == other._keyStroke)
			{
				if (this._keyStroke != XKeys.None)
					keyStrokeConflict = true;
			}

			if (String.Compare(_name, other._name, true) == 0)
				nameConflict = true;
		}

		public bool IsMatchingKey(VoiLutPresetConfigurationKey other)
		{
			bool nameConflict, keyStrokeConflict;
			CheckConflict(other, out nameConflict, out keyStrokeConflict);
			return (nameConflict || keyStrokeConflict);
		}

		public override int GetHashCode()
		{
			int returnValue = _modalityFilter.GetHashCode();
			returnValue += 3 * _keyStroke.GetHashCode();
			returnValue += 5 * _name.GetHashCode();
			return returnValue;
		}

		public override bool Equals(object obj)
		{
			VoiLutPresetConfigurationKey other = obj as VoiLutPresetConfigurationKey;
			if (other == null)
				return false;

			if (_modalityFilter != other._modalityFilter)
				return false;

			if (_keyStroke != other._keyStroke)
				return false;

			if (String.Compare(_name, other._name, true) != 0)
				return false;

			return true;
		}
		
		public override string ToString()
		{
			return _text; //what the Watch window will show in debug.
		}
	}

	public sealed class VoiLutPresetConfiguration
	{
		private VoiLutPresetConfigurationKey _key;
		private VoiLutPresetApplicatorConfiguration _applicatorConfiguration;

		public VoiLutPresetConfiguration(string modalityFilter, XKeys keyStroke, string name, VoiLutPresetApplicatorConfiguration applicatorConfiguration)
			: this(new VoiLutPresetConfigurationKey(modalityFilter, keyStroke, name), applicatorConfiguration)
		{ 
		}

		public VoiLutPresetConfiguration(VoiLutPresetConfigurationKey key, VoiLutPresetApplicatorConfiguration applicatorConfiguration)
		{
			_key = key;
			Platform.CheckForNullReference(applicatorConfiguration, "applicatorConfiguration");
			_applicatorConfiguration = applicatorConfiguration;
		}

		//for internal use only when we want to compare two of these objects by key.
		internal VoiLutPresetConfiguration(VoiLutPresetConfigurationKey key)
		{
			_key = key;
		}

		private VoiLutPresetConfiguration()
		{
		}

		public VoiLutPresetConfigurationKey Key
		{
			get { return _key; }
		}

		public string ModalityFilter
		{
			get { return _key.ModalityFilter; }
		}

		public XKeys KeyStroke
		{
			get { return _key.KeyStroke; }
		}

		public string Name
		{
			get { return _key.Name; }
		}

		public VoiLutPresetApplicatorConfiguration VoiLutPresetApplicatorConfiguration
		{
			get { return _applicatorConfiguration; }		
		}

		public override int GetHashCode()
		{
			return _key.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			VoiLutPresetConfiguration other = obj as VoiLutPresetConfiguration;
			if (other == null)
				return false;

			return Key.Equals(other.Key);
		}

		public override string ToString()
		{
			return Key.ToString();
		}
	}
}
