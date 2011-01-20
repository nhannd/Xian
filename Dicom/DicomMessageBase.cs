#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class for DICOM Files and Messages
    /// </summary>
    /// <seealso cref="DicomFile"/>
    /// <seealso cref="DicomMessage"/>
    public abstract class DicomMessageBase
    {
        /// <summary>
        /// The Transfer Syntax of the DICOM file or message
        /// </summary>
        public abstract TransferSyntax TransferSyntax { get; set; }
    
        /// <summary>
        /// The Meta information for the message.
        /// </summary>
        public DicomAttributeCollection MetaInfo
        {
            get;
            internal set;
        }

        /// <summary>
        /// The DataSet for the message.
        /// </summary>
        public DicomAttributeCollection DataSet
        {
            get; internal set;
        }

        public void ChangeTransferSyntax(TransferSyntax newTransferSyntax)
        {
            ChangeTransferSyntax(newTransferSyntax, null, null);
        }

        public void ChangeTransferSyntax(TransferSyntax newTransferSyntax, IDicomCodec inputCodec, DicomCodecParameters inputParameters)
        {
            IDicomCodec codec = inputCodec;
            DicomCodecParameters parameters = inputParameters;
            if (newTransferSyntax.Encapsulated && TransferSyntax.Encapsulated)
                throw new DicomCodecException("Source and destination transfer syntaxes encapsulated");

            if (newTransferSyntax.Encapsulated)
            {
                if (codec == null)
                {
                    codec = DicomCodecRegistry.GetCodec(newTransferSyntax);
                    if (codec == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to get registered codec for {0}", newTransferSyntax);
                        throw new DicomCodecException("No registered codec for: " + newTransferSyntax.Name);
                    }
                }
                if (parameters == null)
                    parameters = DicomCodecRegistry.GetCodecParameters(newTransferSyntax, DataSet);

            	DicomAttribute pixelData;
                if (DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData))
                {
                    if (pixelData.IsNull)
                        throw new DicomCodecException("Sop pixel data has no valid value and cannot be compressed.");

                    DicomUncompressedPixelData pd = new DicomUncompressedPixelData(DataSet);
                    DicomCompressedPixelData fragments = new DicomCompressedPixelData(pd);

                    // Check if Overlay is embedded in pixels
                    OverlayPlaneModuleIod overlayIod = new OverlayPlaneModuleIod(DataSet);
                    for (int i = 0; i < 16; i++)
                    {
                        if (overlayIod.HasOverlayPlane(i))
                        {
                            OverlayPlane overlay = overlayIod[i];
                            if (overlay.OverlayData == null)
                            {
                                Platform.Log(LogLevel.Debug,
                                             "SOP Instance has embedded overlay in pixel data, extracting");
                                overlay.ConvertEmbeddedOverlay(pd);
                            }
                        }
                    }

                    // Set before compression, the codecs need it.
                    fragments.TransferSyntax = newTransferSyntax;

                    codec.Encode(pd, fragments, parameters);

                    fragments.UpdateMessage(this);

                    //TODO: should we validate the number of frames in the compressed data?
                    if (!DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData) || pixelData.IsNull)
                        throw new DicomCodecException("Sop has no pixel data after compression.");
                }
                else
                {
                    //A bit cheap, but check for basic image attributes - if any exist
                    // and are non-empty, there should probably be pixel data too.

                    DicomAttribute attribute;
                    if (DataSet.TryGetAttribute(DicomTags.Rows, out attribute) && !attribute.IsNull)
                        throw new DicomCodecException("Suspect Sop appears to be an image (Rows is non-empty), but has no pixel data.");

                    if (DataSet.TryGetAttribute(DicomTags.Columns, out attribute) && !attribute.IsNull)
                        throw new DicomCodecException("Suspect Sop appears to be an image (Columns is non-empty), but has no pixel data.");

                    TransferSyntax = newTransferSyntax;
                }
            }
            else
            {
                if (codec == null)
                {
                    codec = DicomCodecRegistry.GetCodec(TransferSyntax);
                    if (codec == null)
                    {
                        Platform.Log(LogLevel.Error, "Unable to get registered codec for {0}", TransferSyntax);

                        throw new DicomCodecException("No registered codec for: " + TransferSyntax.Name);
                    }

                    if (parameters == null)
                        parameters = DicomCodecRegistry.GetCodecParameters(TransferSyntax, DataSet);
                }

				DicomAttribute pixelData;
				if (DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData))
				{
					if (pixelData.IsNull)
						throw new DicomCodecException("Sop pixel data has no valid value and cannot be decompressed.");

					DicomCompressedPixelData fragments = new DicomCompressedPixelData(DataSet);
                    DicomUncompressedPixelData pd = new DicomUncompressedPixelData(fragments);

                    codec.Decode(fragments, pd, parameters);

                    pd.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

                    pd.UpdateMessage(this);

					//TODO: should we validate the number of frames in the decompressed data?
					if (!DataSet.TryGetAttribute(DicomTags.PixelData, out pixelData) || pixelData.IsNull)
						throw new DicomCodecException("Sop has no pixel data after decompression.");
				}
                else
                {
					//NOTE: doing this for consistency, really.
					DicomAttribute attribute;
					if (DataSet.TryGetAttribute(DicomTags.Rows, out attribute) && !attribute.IsNull)
						throw new DicomCodecException("Suspect Sop appears to be an image (Rows is non-empty), but has no pixel data.");

					if (DataSet.TryGetAttribute(DicomTags.Columns, out attribute) && !attribute.IsNull)
						throw new DicomCodecException("Suspect Sop appears to be an image (Columns is non-empty), but has no pixel data.");
					
					TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
                }
            }
        }

        /// <summary>
        /// Load the contents of attributes in the message into a structure or class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will use reflection to look at the contents of the object specified by
        /// <paramref name="obj"/> and copy the values of attributes within the <see cref="MetaInfo"/>
        /// and <see cref="DataSet"/> for the message to fields in the object with 
        /// the <see cref="DicomFieldAttribute"/> attribute set for them.
        /// </para>
        /// </remarks>
        /// <param name="obj"></param>
        public void LoadDicomFields(object obj)
        {
            MetaInfo.LoadDicomFields(obj);
            DataSet.LoadDicomFields(obj);
        }

        #region Dump
        /// <summary>
        /// Dump the contents of the message to a StringBuilder.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        public abstract void Dump(StringBuilder sb, string prefix, DicomDumpOptions options);

        /// <summary>
        /// Dump the contents of the message to a string.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="options"></param>
        /// <returns>The dump of the message.</returns>
        public string Dump(string prefix, DicomDumpOptions options)
        {
            StringBuilder sb = new StringBuilder();
            Dump(sb, prefix, options);
            return sb.ToString();
        }

        /// <summary>
        /// Dump the contents of themessage to a string with the default dump options.
        /// </summary>
        /// <param name="prefix">A prefix to place in front of each dump line.</param>
        /// <returns>The dump of the message.</returns>
        public string Dump(string prefix)
        {
            return Dump(prefix, DicomDumpOptions.Default);
        }

        /// <summary>
        /// Dump the contents of the message to a string with the default options set.
        /// </summary>
        /// <returns>The dump of the message.</returns>
        public string Dump()
        {
            return Dump(String.Empty, DicomDumpOptions.Default);
        }
        #endregion

		/// <summary>
		/// Hash override that sums the hashes of the attributes within the message.
		/// </summary>
		/// <returns>The sum of the hashes of the attributes in the message.</returns>
		public override int GetHashCode()
		{
			if (MetaInfo.Count > 0 || DataSet.Count > 0)
			{
				int hash = 0;
				foreach (DicomAttribute attrib in MetaInfo)
					hash += attrib.GetHashCode();
				foreach (DicomAttribute attrib in DataSet)
					hash += attrib.GetHashCode();
				return hash;
			}
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
		    List<DicomAttributeComparisonResult> failureReasons = new List<DicomAttributeComparisonResult>();
		    return Equals(obj, ref failureReasons);
		}

        /// <summary>
		/// Check if the contents of the DicomAttributeCollection is identical to another DicomAttributeCollection instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method compares the contents of two attribute collections to see if they are equal.  The method
		/// will step through each of the tags within the collection, and compare them to see if they are equal.  The
		/// method will also recurse into sequence attributes to be sure they are equal.</para>
		/// </remarks>
		/// <param name="obj">The object to compare to.</param>
        /// <param name="comparisonResults">A list of <see cref="DicomAttributeComparisonResult"/>  describing why the objects are not equal.</param>
		/// <returns>true if the collections are equal.</returns>
		public bool Equals(object obj, ref List<DicomAttributeComparisonResult> comparisonResults)
		{
			DicomFile a = obj as DicomFile;
			if (a == null)
			{
			    DicomAttributeComparisonResult result = new DicomAttributeComparisonResult
			                                                {
			                                                    ResultType = ComparisonResultType.InvalidType,
			                                                    Details =
			                                                        String.Format("Comparison object is invalid type: {0}",
			                                                                      obj.GetType())
			                                                };
			    comparisonResults.Add(result);

				return false;
			}

            if (!MetaInfo.Equals(a.MetaInfo, ref comparisonResults))
				return false;
            if (!DataSet.Equals(a.DataSet, ref comparisonResults))
				return false;

			return true;
		}
    }
}