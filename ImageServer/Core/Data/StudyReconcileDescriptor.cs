#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents information encoded in the xml of a <see cref="StudyHistory"/> record of type <see cref="StudyHistoryTypeEnum.StudyReconciled"/>.
	/// </summary>
	/// <remarks>
	/// </remarks>
	[XmlRoot("Reconcile")]
	public class StudyReconcileDescriptor
	{
		#region Private Members

		private List<BaseImageLevelUpdateCommand> _commands;

		#endregion

		#region Public Properties

		/// <summary>
		/// Reconciliation option
		/// </summary>
		public StudyReconcileAction Action { get; set; }

		/// <summary>
		/// User-defined description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether the reconciliation was automatic or manual.
		/// </summary>
		public bool Automatic { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="StudyInformation"/>
		/// </summary>
		public StudyInformation ExistingStudy { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="ImageSetDescriptor"/>
		/// </summary>
		public ImageSetDescriptor ImageSetData { get; set; }

		/// <summary>
		/// Gets or sets the commands that are part of the reconciliation process.
		/// </summary>
		[XmlArray("Commands")]
		[XmlArrayItem("Command", Type = typeof(AbstractProperty<BaseImageLevelUpdateCommand>))]
		public List<BaseImageLevelUpdateCommand> Commands
		{
			get
			{
				if (_commands == null)
					_commands = new List<BaseImageLevelUpdateCommand>();
				return _commands;
			}
			set { _commands = value; }
		}

		public string UserName { get; set; }

		public List<SeriesMapping> SeriesMappings { get; set; }

		#endregion
	}

	[XmlRoot("Reconcile")]
	public class ReconcileCreateStudyDescriptor : StudyReconcileDescriptor
	{
		public ReconcileCreateStudyDescriptor()
		{
			Action = StudyReconcileAction.CreateNewStudy;
		}
	}

	[XmlRoot("Reconcile")]
	public class ReconcileDiscardImagesDescriptor : StudyReconcileDescriptor
	{
		public ReconcileDiscardImagesDescriptor()
		{
			Action = StudyReconcileAction.Discard;
		}
	}

	[XmlRoot("Reconcile")]
	public class ReconcileMergeToExistingStudyDescriptor : StudyReconcileDescriptor
	{
		public ReconcileMergeToExistingStudyDescriptor()
		{
			Action = StudyReconcileAction.Merge;
		}
	}

	[XmlRoot("Reconcile")]
	public class ReconcileProcessAsIsDescriptor : StudyReconcileDescriptor
	{
		public ReconcileProcessAsIsDescriptor()
		{
			Action = StudyReconcileAction.ProcessAsIs;
		}
	}
}