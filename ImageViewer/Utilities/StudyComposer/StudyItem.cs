using System.Drawing;
using System.Text;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A <see cref="StudyComposerItemBase{T}"/> that wraps a <see cref="StudyNode"/> in a <see cref="StudyBuilder"/> tree.
	/// </summary>
	public class StudyItem : StudyComposerItemBase<StudyNode>
	{
		private readonly SeriesItemCollection _series;

		public StudyItem(StudyNode study)
		{
			base.Node = study;
			_series = new SeriesItemCollection(study.Series);
		}

		private StudyItem(StudyItem source) : this(source.Node.Copy(false))
		{
			this.Icon = (Image) source.Icon.Clone();
			foreach (SeriesItem series in source.Series)
			{
				this.Series.Add(series.Copy());
			}
		}

		public SeriesItemCollection Series
		{
			get { return _series; }
		}

		public void InsertItems(ImageItem[] images)
		{
			SeriesItem series = this.Series.AddNew();
			foreach (ImageItem item in images)
			{
				series.Images.Add(item);
			}
		}

		public void InsertItems(SeriesItem[] series)
		{
			foreach (SeriesItem item in series)
				this.Series.Add(item);
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public StudyItem Copy()
		{
			return new StudyItem(this);
		}

		// The key image is the middle image of the first series that has images
		private Image GetKeyImage()
		{
			if (_series != null && _series.Count > 0)
			{
				foreach (SeriesItem series in _series)
				{
					if (series.Images.Count > 0)
						return series.Images[(series.Images.Count - 1)/2].Icon;
				}
			}
			return null;
		}

		#region Overrides

		public override string Name
		{
			get { return base.Node.StudyId; }
			set { base.Node.StudyId = value; }
		}

		public override string Description
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(base.Node.Description))
					sb.AppendLine(base.Node.Description);
				if (!string.IsNullOrEmpty(base.Node.AccessionNumber))
					sb.AppendLine(base.Node.AccessionNumber);
				if (base.Node.DateTime.HasValue)
					sb.AppendLine(base.Node.DateTime.Value.ToString());
				sb.AppendLine(base.Node.InstanceUid);
				return sb.ToString();
			}
		}

		protected override void OnNodePropertyChanged(string propertyName)
		{
			base.OnNodePropertyChanged(propertyName);

			if (propertyName == "StudyId")
				base.FirePropertyChanged("Name");
			if (propertyName == "Description" || propertyName == "AccessionNumber" || propertyName == "DateTime" || propertyName == "InstanceUid")
				base.FirePropertyChanged("Description");
		}

		public override void UpdateIcon(Size iconSize)
		{
			Image keyImage = GetKeyImage();
			_helper.IconSize = iconSize;
			base.Icon = _helper.CreateStackIcon(keyImage);
		}

		public override StudyComposerItemBase<StudyNode> Clone()
		{
			return this.Copy();
		}

		#endregion

		#region Statics

		private static readonly IconHelper _helper = new IconHelper();

		static StudyItem()
		{
			_helper.IconSize = new Size(64, 64);
			_helper.StackSize = 5;
			_helper.StackOffset = new Size(5, -5);
		}

		#endregion
	}
}