#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class for DICOM Files and Messages
    /// </summary>
    /// <seealso cref="DicomFile"/>
    /// <seealso cref="DicomMessage"/>
    public abstract class DicomMessageBase
    {
        internal DicomAttributeCollection _metaInfo = null;
        internal DicomAttributeCollection _dataSet = null;

        /// <summary>
        /// The Transfer Syntax of the DICOM file or message
        /// </summary>
        public abstract TransferSyntax TransferSyntax { get; set; }
    
        /// <summary>
        /// The Meta information for the message.
        /// </summary>
        public DicomAttributeCollection MetaInfo
        {
            get { return _metaInfo; }
        }

        /// <summary>
        /// The DataSet for the message.
        /// </summary>
        public DicomAttributeCollection DataSet
        {
            get { return _dataSet; }
        }

        public void ChangeTransferSyntax(TransferSyntax newTransferSyntax)
        {
            ChangeTransferSyntax(newTransferSyntax, null, null);
        }

        public void ChangeTransferSyntax(TransferSyntax newTransferSyntax, IDicomCodec inputCodec, DicomCodecParameters inputParameters)
        {
            IDicomCodec codec = inputCodec;
            DicomCodecParameters parameters = inputParameters;
            if (newTransferSyntax.Encapsulated && this.TransferSyntax.Encapsulated)
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

                if (DataSet.Contains(DicomTags.PixelData))
                {
                    DicomUncompressedPixelData pd = new DicomUncompressedPixelData(DataSet);
                    DicomCompressedPixelData fragments = new DicomCompressedPixelData(pd);

					// Set before compression, the codecs need it.
					fragments.TransferSyntax = newTransferSyntax;

					codec.Encode(pd, fragments, parameters);

                    fragments.UpdateMessage(this);
                }
                else
                    this.TransferSyntax = newTransferSyntax;
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

                if (DataSet.Contains(DicomTags.PixelData))
                {
                    DicomCompressedPixelData fragments = new DicomCompressedPixelData(DataSet);
                    DicomUncompressedPixelData pd = new DicomUncompressedPixelData(fragments);

                    codec.Decode(fragments, pd, parameters);

                    pd.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

                    pd.UpdateMessage(this);
                }
                else
                    this.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;
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
            _metaInfo.LoadDicomFields(obj);
            _dataSet.LoadDicomFields(obj);
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

    }
}