namespace ClearCanvas.Dicom.Utilities.StudyBuilder
{
	/// <summary>
	/// A generic DICOM study manipulator class. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="StudyBuilder"/> class abstracts the code required to construct new DICOM studies from scratch, or from existing instances.
	/// All resultant instances carry new UIDs that distinguish them from the original instances (if used), generated to conform with the structure defined
	/// in the builder and thereby erasing the original study structure.</para>
	/// <para>The hierarchial tree-based view defines standard tags on data nodes at four different levels in the study tree:
	/// <see cref="PatientNode">patient</see>, <see cref="StudyNode">study</see>, <see cref="SeriesNode">series</see>, and <see cref="SopInstanceNode">SOP</see>.
	/// By inserting instances, editing node properties and moving the individual nodes around the tree, an entirely new study structure can be
	/// created programmatically and henceforth exported to a filesystem or to a DICOM server.</para>
	/// </remarks>
	public sealed class StudyBuilder
	{
		private readonly PatientNodeCollection _patients;
		private readonly RootNode _rootNode;

		/// <summary>
		/// Constructs a new instance of the <see cref="StudyBuilder"/> using the default options.
		/// </summary>
		public StudyBuilder()
		{
			_patients = new PatientNodeCollection(this);
			_rootNode = new RootNode();
		}

		/// <summary>
		/// Gets a collection of all the <see cref="PatientNode"/>s in the study builder tree.
		/// </summary>
		public PatientNodeCollection Patients
		{
			get { return _patients; }
		}

		/// <summary>
		/// Gets the root node for this builder tree.
		/// </summary>
		internal StudyBuilderNode Root
		{
			get { return _rootNode; }
		}

		/// <summary>
		/// Generates new instance UIDs and rebuilds the UID relationships of all nodes in the study builder according to the current tree structure.
		/// </summary>
		/// <remarks>
		/// This method does not need to be explicitly called before any export operations. It is provided to allow access to the updated, underlying
		/// data sets of the individual SOP instances.
		/// </remarks>
		public void RebuildTree()
		{
			foreach (PatientNode patient in _patients)
			{
				foreach (StudyNode study in patient.Studies)
				{
					study.RegenerateUids();
					foreach (SeriesNode series in study.Series)
					{
						series.RegenerateUids();
						foreach (SopInstanceNode image in series.Images)
						{
							image.RegenerateUids();
						}
					}
				}
			}
		}

		/// <summary>
		/// Rebuilds the study builder tree and exports all the individual SOP instances to the specified directory.
		/// </summary>
		/// <remarks>
		/// <para>The exported files use the SOP instance UID as the filename with a &quot;.dcm&quot; extension.</para>
		/// <para>The <see cref="RebuildTree"/> method is called automatically, and hence does not need to be explicitly called before invoking this method.</para>
		/// </remarks>
		/// <param name="path">The path of the directory to which the SOP instances are exported.</param>
		public void ExportToDirectory(string path)
		{
			// we must rebuild the entire tree before we export it, lest we have left over UIDs that mess up the structure
			// so we can't combine these operations into a single giant iteration (they must be separate!)
			RebuildTree();

			foreach (PatientNode patient in _patients)
			{
				foreach (StudyNode study in patient.Studies)
				{
					foreach (SeriesNode series in study.Series)
					{
						int seriesNum = study.Series.IndexOf(series) + 1;
						foreach (SopInstanceNode image in series.Images)
						{
							int imageNum = series.Images.IndexOf(image) + 1;

							patient.Update(image.DicomData);
							study.Update(image.DicomData);
							series.Update(image.DicomData, seriesNum);
							image.Update(image.DicomData, imageNum);

							image.ExportToDirectory(path);
						}
					}
				}
			}
		}

		#region Root Node Class

		private class RootNode : StudyBuilderNode {}

		#endregion
	}
}