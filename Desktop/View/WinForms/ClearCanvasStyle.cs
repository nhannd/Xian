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

using System.Drawing;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class ClearCanvasStyle
	{
		public static Color ClearCanvasDarkBlue
		{
			get { return Color.FromArgb(61, 152, 209); }
		}

		public static Color ClearCanvasBlue
		{
			get { return Color.FromArgb(124, 177, 221); }
		}

		public static Color ClearCanvasLightBlue
		{
			get { return Color.FromArgb(186, 210, 236); }
		}

		public static void SetTitleBarStyle(TitleBar titleBar)
		{
			titleBar.BackColor = ClearCanvasDarkBlue;
			titleBar.ForeColor = Color.White;
			titleBar.GradientActiveColor = ClearCanvasDarkBlue;
			titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToGradientColor;
			titleBar.GradientDirection = Crownwood.DotNetMagic.Controls.GradientDirection.TopToBottom;
		}
	}
}
