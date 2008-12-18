using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	public sealed class KeyObjectSelectionCodeSequences
	{
		public static readonly Code DocumentTitleModifier = new Code(113011, "Document Title Modifier");
		public static readonly Code KeyObjectDescription = new Code(113012, "Key Object Description");

		public sealed class Code : ContextGroupBase<Code>.ContextGroupItemBase
		{
			internal Code(int codeValue, string codeMeaning) : base("DCM", codeValue.ToString(), codeMeaning) {}
		}
	}
}