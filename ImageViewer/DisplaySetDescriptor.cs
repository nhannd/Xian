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
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public interface IDicomDisplaySetDescriptor : IDisplaySetDescriptor
	{
		ISeriesIdentifier SourceSeries { get; }

		//TODO: put this stuff back when we actually support dynamically updating the viewer.
		//bool Update(Sop sop);
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

		//bool IDicomDisplaySetDescriptor.Update(Sop sop)
		//{
		//    Platform.CheckForNullReference(sop, "sop");
		//    return Update(sop);
		//}

		internal virtual bool ShouldAddSop(Sop sop)
		{
			return false;
		}

		internal virtual bool Update(Sop sop)
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