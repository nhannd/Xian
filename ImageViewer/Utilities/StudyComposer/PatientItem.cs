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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	/// <summary>
	/// A <see cref="StudyComposerItemBase{T}"/> that wraps a <see cref="PatientNode"/> in a <see cref="StudyBuilder"/> tree.
	/// </summary>
	public class PatientItem : StudyComposerItemBase<PatientNode>
	{
		private readonly StudyItemCollection _studies;

		public PatientItem(PatientNode patient)
		{
			base.Node = patient;
			_studies = new StudyItemCollection(patient.Studies);
			_studies.ListChanged += OnChildListChanged;
		}

		private PatientItem(PatientItem source) : this(source.Node.Copy(false))
		{
			this.Icon = (Image) source.Icon.Clone();
			foreach (StudyItem study in source.Studies)
			{
				this.Studies.Add(study.Copy());
			}
		}

		public StudyItemCollection Studies
		{
			get { return _studies; }
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public PatientItem Copy()
		{
			return new PatientItem(this);
		}

		private void OnChildListChanged(object sender, ListChangedEventArgs e) {
			switch (e.ListChangedType) {
				case ListChangedType.Reset:
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemDeleted:
				case ListChangedType.ItemMoved:
				case ListChangedType.ItemChanged:
					UpdateIcon();
					break;
			}
		}

		#region Misc Helpers

		private string GetAgeSexString()
		{
			string age = "";
			if (this.Node.BirthDate.HasValue)
			{
				DateTime now = DateTime.Now;
				DateTime bd = this.Node.BirthDate.Value;

				if (now.Year - bd.Year > 0)
				{
					int diffYears = now.Year - bd.Year;
					age = string.Format("{0} Year Old", diffYears);
				}
				else
				{
					if (now.Month - bd.Month > 0)
					{
						int diffMonths = now.Month - bd.Month;
						age = string.Format("{0} Month Old", diffMonths);
					}
					else
					{
						TimeSpan diff = now.Subtract(bd);
						if (diff.Days > 0)
						{
							age = string.Format("{0} Day Old", diff.Days);
						}
						else
						{
							if (diff.Hours > 0)
							{
								age = string.Format("{0} Hour Old", diff.Hours);
							}
							else
							{
								age = "Newborn";
							}
						}
					}
				}
			}

			string sex = "";
			switch (this.Node.Sex)
			{
				case PatientSex.Male:
					sex = "Male";
					break;
				case PatientSex.Female:
					sex = "Female";
					break;
				case PatientSex.Other:
					sex = "Other";
					break;
			}

			string result = string.Format("{0} {1}", age, sex).Trim();
			if (result == "")
				return "Unknown";
			return result;
		}

		// The key image is the middle image of the first series that has images, of the first study that has series
		private Image GetKeyImage()
		{
			if (_studies != null && _studies.Count > 0)
			{
				foreach (StudyItem study in _studies)
				{
					foreach (SeriesItem series in study.Series)
					{
						if (series.Images.Count > 0)
							return series.Images[(series.Images.Count - 1)/2].Icon;
					}
				}
			}
			return null;
		}

		#endregion

		#region Overrides

		public override string Name
		{
			get { return base.Node.PatientId; }
			set { base.Node.PatientId = value; }
		}

		public override string Description
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrEmpty(base.Node.Name))
					sb.AppendLine(base.Node.Name);
				sb.AppendLine(GetAgeSexString());
				return sb.ToString();
			}
		}

		protected override void OnNodePropertyChanged(string propertyName)
		{
			base.OnNodePropertyChanged(propertyName);
			if (propertyName == "PatientId")
				base.FirePropertyChanged("Name");
			else if (propertyName == "BirthDate" || propertyName == "Sex" || propertyName == "Name")
				base.FirePropertyChanged("Description");
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override StudyComposerItemBase<PatientNode> Clone()
		{
			return this.Copy();
		}

		public override void UpdateIcon(Size iconSize)
		{
			Image keyImage = GetKeyImage();
			_helper.IconSize = iconSize;
			base.Icon = _helper.CreateStackIcon(keyImage);
		}

		#endregion

		#region Collection Insert Helpers

		public void InsertItems(ImageItem[] images)
		{
			StudyItem study = this.Studies.AddNew();
			SeriesItem series = study.Series.AddNew();
			foreach (ImageItem item in images)
			{
				series.Images.Add(item);
			}
		}

		public void InsertItems(SeriesItem[] series)
		{
			StudyItem study = this.Studies.AddNew();
			foreach (SeriesItem item in series)
			{
				study.Series.Add(item);
			}
		}

		public void InsertItems(StudyItem[] studies)
		{
			foreach (StudyItem item in studies)
			{
				this.Studies.Add(item);
			}
		}

		/// <summary>
		/// Inserts an <see cref="ImageItem"/> into the study tree under the patient represented by this item,
		/// creating a new intermediary study and series.
		/// </summary>
		/// <param name="image"></param>
		public void InsertImage(ImageItem image)
		{
			StudyItem study = this.Studies.AddNew();
			SeriesItem series = study.Series.AddNew();
			series.Images.Add(image);
		}

		#endregion

		#region Statics

		private static readonly IconHelper _helper = new IconHelper();

		static PatientItem()
		{
			_helper.IconSize = new Size(64, 64);
			_helper.StackSize = 7;
			_helper.StackOffset = new Size(3, -3);
		}

		#endregion
	}
}