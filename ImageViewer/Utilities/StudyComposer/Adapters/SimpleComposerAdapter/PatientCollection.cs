using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	public sealed class PatientCollection : BindingListWrapper<PatientItem>
	{
		internal PatientCollection(BindingList<PatientItem> list)
			: base(list) {}

		public PatientItem GetFirstPatient()
		{
			if (this.Count == 0)
				return null;
			return this[0];
		}
	}
}