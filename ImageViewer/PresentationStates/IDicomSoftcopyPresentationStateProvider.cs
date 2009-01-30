using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.PresentationStates {
	public interface IDicomSoftcopyPresentationStateProvider {
		DicomSoftcopyPresentationState PresentationState { get; set; }
	}
}
