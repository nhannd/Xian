﻿#region License

//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearCanvas.Common
{
    public class FrameHeader
    {

        public ushort Planar { get; set; }
        public bool IsPlaner { get; set; }
        public float LossyImageCompressionRatio { get; set; }
        public string TransferSyntaxUid { get; set; }
        public ushort BitsAllocated { get; set; }
        public ushort BitsStored { get; set; }
        public ushort Height { get; set; }
        public ushort Width { get; set; }
        public ushort Samples { get; set; }
        public ushort HighBit { get; set; }
        public string PhotometricInterpretation { get; set; }
        public ushort PixelRepresentation { get; set; }
        public string DerivationDescription { get; set; }
        public string LossyImageCompression { get; set; }
        public string LossyImageCompressionMethod { get; set; }
    }
}
