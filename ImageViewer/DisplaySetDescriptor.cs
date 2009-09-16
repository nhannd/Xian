using System;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public interface IDicomDisplaySetDescriptor : IDisplaySetDescriptor
	{
		ISeriesIdentifier SourceSeries { get; }

		bool Update(Sop sop);
	}

	public interface IDisplaySetDescriptor
	{
		IDisplaySet DisplaySet { get; }

		string Name { get; }
		string Description { get; }
		int Number { get; }
		string Uid { get; }
	}

	[Cloneable(false)]
	public abstract class DicomDisplaySetDescriptor : DisplaySetDescriptor, IDicomDisplaySetDescriptor
	{
		[CloneCopyReference]
		private readonly ISeriesIdentifier _sourceSeries;
		[CloneCopyReference]
		private readonly IPresentationImageFactory _presentationImageFactory;

		private string _name;
		private string _description;
		private int? _number;
		private string _uid;

		protected DicomDisplaySetDescriptor(ISeriesIdentifier sourceSeries)
			: this(sourceSeries, null)
		{
		}

		protected DicomDisplaySetDescriptor(ISeriesIdentifier sourceSeries, IPresentationImageFactory presentationImageFactory)
		{
			Platform.CheckForNullReference(sourceSeries, "sourceSeries");
			_sourceSeries = sourceSeries;
			_presentationImageFactory = presentationImageFactory;
		}

		protected DicomDisplaySetDescriptor(DicomDisplaySetDescriptor source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		#region IDicomDisplaySetDescriptor Members

		public ISeriesIdentifier SourceSeries
		{
			get { return _sourceSeries; }
		}

		#endregion

		public override string Name
		{
			get
			{
				if (_name == null)
					_name = GetName() ?? "";
				return _name;
			}
			set { throw new InvalidOperationException("The Name property cannot be set publicly."); }
		}

		public override string Description
		{
			get
			{
				if (_description == null)
					_description = GetDescription() ?? "";
				return _description;
			}
			set { throw new InvalidOperationException("The Description property cannot be set publicly."); }
		}

		public override string Uid
		{
			get
			{
				if (_uid == null)
					_uid = GetUid() ?? "";
				return _uid;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

		public override int Number
		{
			get
			{
				if (!_number.HasValue)
					_number = GetNumber();
				return _number.Value;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

		protected abstract string GetName();
		protected abstract string GetDescription();
		protected abstract string GetUid();

		protected virtual int GetNumber()
		{
			return SourceSeries.SeriesNumber ?? default(int);
		}

		bool IDicomDisplaySetDescriptor.Update(Sop sop)
		{
			Platform.CheckForNullReference(sop, "sop");
			return Update(sop);
		}

		protected virtual bool ShouldAddSop(Sop sop)
		{
			return false;
		}

		protected virtual bool Update(Sop sop)
		{
			bool updated = false;
			if (_presentationImageFactory != null && sop.SeriesInstanceUid == SourceSeries.SeriesInstanceUid && ShouldAddSop(sop))
			{
				foreach (IPresentationImage image in _presentationImageFactory.CreateImages(sop))
				{
					base.DisplaySet.PresentationImages.Add(image);
					updated = true;
				}
			}

			return updated;
		}
	}

	[Cloneable(true)]
	public class BasicDisplaySetDescriptor : DisplaySetDescriptor
	{
		private string _name = "";
		private string _description = "";
		private int _number;
		private string _uid = "";

		public BasicDisplaySetDescriptor()
		{
		}

		public override string Name
		{
			get { return _name ?? ""; }
			set { _name = value; }
		}

		public override string Description
		{
			get { return _description ?? ""; }
			set { _description = value; }
		}

		public override int Number
		{
			get { return _number; }
			set { _number = value; }
		}

		public override string Uid
		{
			get { return _uid ?? ""; }
			set { _uid = value; }
		}	
	}

	[Cloneable(true)]
	public abstract class DisplaySetDescriptor : IDisplaySetDescriptor
	{
		[CloneIgnore]
		private DisplaySet _displaySet;

		public DisplaySetDescriptor()
		{
		}

		#region IDisplaySetDescriptor Members

		IDisplaySet IDisplaySetDescriptor.DisplaySet
		{
			get { return _displaySet; }
		}

		public virtual DisplaySet DisplaySet
		{
			get { return _displaySet; }
			internal set { _displaySet = value; }
		}

		#endregion

		#region IDisplaySetDescriptor Members

		public abstract string Name { get; set; }

		public abstract string Description { get; set; }

		public abstract int Number { get; set; }

		public abstract string Uid { get; set; }

		#endregion

		public DisplaySetDescriptor Clone()
		{
			return (DisplaySetDescriptor)CloneBuilder.Clone(this);
		}

		public override string ToString()
		{
			return StringUtilities.Combine(new string[] { Name, Uid}, " | ", true);
		}
	}
}