using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a grayscale lookup table pipeline.
	/// </summary>
	/// <remarks>Supplement 33 of the DICOM standard, <i>Softcopy Presentation State</i>
	/// defines a sequence of three lookup tables (LUT) that transform raw pixel
	/// values into presentation values: 1) Modality LUT 2) VOI LUT
	/// and 3) Presentation LUT.  At present, this class allows you to define
	/// the Modality and VOI LUTs.</remarks>
	//public class GrayscaleLUTPipeline
	//{
	//    private LUTComposer _compositeLUT = new LUTComposer();

	//    /// <summary>
	//    /// Initializes a new instance of the <see cref="GrayscaleLUTPipeline"/> class.
	//    /// </summary>
	//    public GrayscaleLUTPipeline()
	//    {
	//        _compositeLUT.LUTCollection.Add(null);
	//        _compositeLUT.LUTCollection.Add(null);
	//        //_compositeLUT.LUTCollection.Add(null);
	//    }

	//    /// <summary>
	//    /// Gets or set a value indicating whether the output of the pipeline
	//    /// should be inverted.
	//    /// </summary>
	//    public bool Invert
	//    {
	//        get { return _compositeLUT.Invert; }
	//        set { _compositeLUT.Invert = value; }
	//    }

	//    /// <summary>
	//    /// Gets the output LUT of the pipeline.
	//    /// </summary>
	//    public byte[] OutputLUT
	//    {
	//        get { return _compositeLUT.OutputLUT; }
	//    }

	//    /// <summary>
	//    /// Gets or sets the modality LUT.
	//    /// </summary>
	//    public IComposableLUT ModalityLUT
	//    {
	//        get { return _compositeLUT.LUTCollection[0]; }
	//        set	{ _compositeLUT.LUTCollection[0] = value; }
	//    }

	//    /// <summary>
	//    /// Gets or sets the value-of-interest (VOI) LUT.
	//    /// </summary>
	//    public IComposableLUT VoiLUT
	//    {
	//        get { return _compositeLUT.LUTCollection[1]; }
	//        set { _compositeLUT.LUTCollection[1] = value; }
	//    }

	//    //public IGrayscaleLUT PresentationLUT
	//    //{
	//    //    get { return _compositeLUT.LUTCollection[2]; }
	//    //    set { _compositeLUT.LUTCollection[2] = value; }
	//    //}

	//    /// <summary>
	//    /// Executes the pipeline.
	//    /// </summary>
	//    /// <remarks>Once executed, the <see cref="OutputLUT"/> will
	//    /// reflect any changes to any of the LUTs in the pipeline.</remarks>
	//    public void Execute()
	//    {
	//        _compositeLUT.Compose();
	//    }
	//}
}
