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

using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Command for compressing a DICOM Sop Instance.
	/// </summary>
	public class DicomCompressCommand : ServerCommand
	{
		private readonly DicomMessageBase _file;
		private readonly IDicomCodec _codec;
		private readonly DicomCodecParameters _parms;
		private readonly TransferSyntax _syntax;
		private bool _lossy;
		private TimeSpanStatistics _timeSpan = new TimeSpanStatistics("CompressTime");

		public TimeSpanStatistics CompressTime
		{
			get { return _timeSpan; }
		}

		public DicomCompressCommand(DicomMessageBase file, TransferSyntax syntax, IDicomCodec codec, DicomCodecParameters parms, bool lossy)
			: base("DICOM Compress Command", true)
		{
			_file = file;
			_syntax = syntax;
			_codec = codec;
			_parms = parms;
			_lossy = lossy;
		}

		protected override void OnExecute()
		{
			// Check if its already in the right syntax.
			if (_file.TransferSyntax.Equals(_syntax))
				return;

			_timeSpan.Start();

			// Check for decompression first
			if (_file.TransferSyntax.Encapsulated)
				_file.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

			_file.ChangeTransferSyntax(_syntax, _codec, _parms);

			_timeSpan.End();
		}

		protected override void OnUndo()
		{
			
		}
	}
}