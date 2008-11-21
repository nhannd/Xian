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
using ClearCanvas.Common;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Rules.Specifications
{
    public class NoSuchDicomTagException : Exception
    {
        private readonly string _tagName;

        public NoSuchDicomTagException(string tagName)
        {
            _tagName = tagName;
        }

        public string TagName
        {
            get { return _tagName; }
        }
    }
    /// <summary>
    /// Expression factory for evaluating expressions that reference attributes within a <see cref="DicomMessageBase"/>.
    /// </summary>
    [ExtensionOf(typeof (ExpressionFactoryExtensionPoint))]
    [LanguageSupport("dicom")]
    public class DicomExpressionFactory : IExpressionFactory
    {
        #region IExpressionFactory Members

        public Expression CreateExpression(string text)
        {
            if (text.StartsWith("$"))
            {
                DicomTag tag = DicomTagDictionary.GetDicomTag(text.Substring(1));
                if (tag == null)
                    throw new XmlSpecificationCompilerException("Invalid DICOM tag: " + text);
            }
            return new DicomExpression(text);
        }

        #endregion
    }

    /// <summary>
    /// An expression handler for <see cref="DicomAttributeCollection"/> or 
    /// <see cref="DicomMessageBase"/> classes.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// This expression filter will evaluate input text with a leading $ as the name of a DICOM Tag.
    /// It will lookup the name of the tag, and retrieve the value of the tag and return it.  if there is no
    /// leading $, the value will be passed through.
    /// </para>
    /// </remarks>
    public class DicomExpression : Expression
    {
        public DicomExpression(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Evaluate 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override object Evaluate(object arg)
        {
            if (string.IsNullOrEmpty(Text))
                return null;

            if (Text.StartsWith("$"))
            {
                DicomMessageBase msg = arg as DicomMessageBase;
                DicomAttributeCollection collection = arg as DicomAttributeCollection;
                if (collection == null && msg == null)
                    return null;

                DicomTag tag = DicomTagDictionary.GetDicomTag(Text.Substring(1));
                if (tag == null)
                {
                    throw new NoSuchDicomTagException(Text.Substring(1));
                }
                    

                if (msg != null)
                {
                    if (msg.DataSet.Contains(tag))
                        collection = msg.DataSet;
                    else if (msg.MetaInfo.Contains(tag))
                        collection = msg.MetaInfo;
                }
				if (collection == null)
					return null;

            	DicomAttribute attrib;
				if (collection.TryGetAttribute(tag,out attrib))
				{
				/*	if (attrib.Tag.VR.Equals(DicomVr.SLvr))
						return attrib.GetInt32(0, 0);
					else if (attrib.Tag.VR.Equals(DicomVr.SSvr))
						return attrib.GetInt16(0, 0);
					else if (attrib.Tag.VR.Equals(DicomVr.ATvr) || attrib.Tag.VR.Equals(DicomVr.ULvr))
						return attrib.GetUInt32(0, 0);
					else if (attrib.Tag.VR.Equals(DicomVr.DSvr) || attrib.Tag.VR.Equals(DicomVr.FDvr))
						return attrib.GetFloat64(0, 0f);
					else if (attrib.Tag.VR.Equals(DicomVr.FLvr))
						return attrib.GetFloat32(0, 0f);
					else if (attrib.Tag.VR.Equals(DicomVr.ISvr))
						return attrib.GetInt64(0, 0);
					else if (attrib.Tag.VR.Equals(DicomVr.USvr))
						return attrib.GetUInt16(0, 0);
					else if (attrib.Tag.VR.Equals(DicomVr.SLvr))
						return attrib.GetInt32(0, 0);
					else
				*/		return attrib.ToString().Trim();
				}

            	return null;
            }

            return Text;
        }
    }
}