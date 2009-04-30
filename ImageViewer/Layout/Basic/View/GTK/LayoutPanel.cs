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


using Gtk;
using System;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.GTK
{
	public class LayoutPanel : Frame
	{
		public LayoutPanel() : base()
		{
			// Initialize controls
			Initialize();
			
			// Add controls
			Add(panel1);
			//Add(new Label("Layoutpanel"));
			
			// Set window size
			//SetSizeRequest(200, 200);
			
			// Initialization code & event handlers
			ShowAll();
		}
		
		public String GetLocalizedText(String text)
		{
			return text;
		}
		
		public int ImageBoxRows
		{
			get { return _imageBoxRows.ValueAsInt; }
			set { _imageBoxRows.Value = value; }
		}
		
		public int ImageBoxCols
		{
			get { return _imageBoxCols.ValueAsInt; }
			set { _imageBoxCols.Value = value; }
		}
		public int TileRows
		{
			get { return _tileRows.ValueAsInt; }
			set { _tileRows.Value = value; }
		}
		public int TileCols
		{
			get { return _tileCols.ValueAsInt; }
			set { _tileCols.Value = value; }
		}
		public Button ApplyImageBoxesButton
		{
			get { return _btnApplyImageBoxes; }
		}
		public Button ApplyTilesButton
		{
			get { return _btnApplyTiles; }
		}
		
		
		#region Components (X-develop GTK# designer code)
		
		private SpinButton _imageBoxRows; // Spinner,,3,,1,,,,,,Yes
		private SpinButton _imageBoxCols; // Spinner,,3,,3,,,,,,Yes
		private Button _btnApplyImageBoxes; // Button,Apply,6,,1
		private SpinButton _tileRows; // Spinner,,10,,1,,,,,,Yes
		private SpinButton _tileCols; // Spinner,,10,,3,,,,,,Yes
		private Button _btnApplyTiles; // Button,Apply,13,,1
		
		#endregion
		
		#region Panels (X-develop GTK# designer code)
		
		private Table panel1; // Panel,Title
		private Frame panel3; // Grouppanel,Image Boxes,1
		private Table panel3Content;
		private Table panel6; // Panel,,2
		private Label label1; // Label,Rows,3
		private Label label2; // Label,Cols,3,,2
		private Table panel4; // Panel,,2,,,1
		private Table panel9; // Panel,,6
		private Table panel7; // Panel,,6,,2
		private Frame panel2; // Grouppanel,Tiles,1,,,1
		private Table panel2Content;
		private Table panel8; // Panel,,9
		private Label label3; // Label,Rows,10
		private Label label4; // Label,Cols,10,,2
		private Table panel5; // Panel,,9,,,1
		private Table panel11; // Panel,,13
		private Table panel10; // Panel,,13,,2
		
		#endregion
		
		#region Implementation (X-develop GTK# designer code)
		
		private const String TITLE_TEXT = "Title";
		
		private const double ASPECT_RATIO = 1.7061611374407584; // Aspect Ratio
		private const double SCREEN_AREA_RATIO = 0.03875510204081633; // Screen Area Ratio
		
		private void Initialize()
		{
			CalculateScreenSize();
			CalculateWindowSize();
			CreateComponents();
			InitComponents();
			LayoutComponents();
		}
		
		private void CreateComponents()
		{
			panel1 = new Table(2, 1, false);
			panel3Content = new Table(2, 1, false);
			panel3 = new Frame(GetLocalizedText("Image Boxes"));
			panel6 = new Table(1, 4, false);
			label1 = new Label(GetLocalizedText("Rows"));
			_imageBoxRows = new SpinButton(0, 100, 1);
			label2 = new Label(GetLocalizedText("Cols"));
			_imageBoxCols = new SpinButton(0, 100, 1);
			panel4 = new Table(1, 3, false);
			panel9 = new Table(0, 0, false);
			_btnApplyImageBoxes = new Button(GetLocalizedText("Apply"));
			panel7 = new Table(0, 0, false);
			panel2Content = new Table(2, 1, false);
			panel2 = new Frame(GetLocalizedText("Tiles"));
			panel8 = new Table(1, 4, false);
			label3 = new Label(GetLocalizedText("Rows"));
			_tileRows = new SpinButton(0, 100, 1);
			label4 = new Label(GetLocalizedText("Cols"));
			_tileCols = new SpinButton(0, 100, 1);
			panel5 = new Table(1, 3, false);
			panel11 = new Table(0, 0, false);
			_btnApplyTiles = new Button(GetLocalizedText("Apply"));
			panel10 = new Table(0, 0, false);
		}
		
		private void InitComponents()
		{
			panel3.BorderWidth = 2;
			panel3Content.BorderWidth = 2;
			label1.SetAlignment(0, 0.5f);
			label2.SetAlignment(0, 0.5f);
			panel2.BorderWidth = 2;
			panel2Content.BorderWidth = 2;
			label3.SetAlignment(0, 0.5f);
			label4.SetAlignment(0, 0.5f);
		}
		
		private void LayoutComponents()
		{
			panel1.Attach(panel3, 0, 1, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel1.Attach(panel2, 0, 1, 1, 2, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel3.Add(panel3Content);
			panel3Content.Attach(panel6, 0, 1, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel3Content.Attach(panel4, 0, 1, 1, 2, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel6.Attach(label1, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel6.Attach(_imageBoxRows, 1, 2, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand, TranslateMarginX(2), TranslateMarginY(2));
			panel6.Attach(label2, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel6.Attach(_imageBoxCols, 3, 4, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand, TranslateMarginX(2), TranslateMarginY(2));
			panel4.Attach(panel9, 0, 1, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel4.Attach(_btnApplyImageBoxes, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel4.Attach(panel7, 2, 3, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel2.Add(panel2Content);
			panel2Content.Attach(panel8, 0, 1, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel2Content.Attach(panel5, 0, 1, 1, 2, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel8.Attach(label3, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel8.Attach(_tileRows, 1, 2, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand, TranslateMarginX(2), TranslateMarginY(2));
			panel8.Attach(label4, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel8.Attach(_tileCols, 3, 4, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand, TranslateMarginX(2), TranslateMarginY(2));
			panel5.Attach(panel11, 0, 1, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
			panel5.Attach(_btnApplyTiles, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(2), TranslateMarginY(2));
			panel5.Attach(panel10, 2, 3, 0, 1, AttachOptions.Expand|AttachOptions.Fill, AttachOptions.Expand|AttachOptions.Fill, TranslateMarginX(0), TranslateMarginY(0));
		}
		
		private int screenSize;
		private double pixelAspectRatio;
		private int windowWidth;
		private int windowHeight;
		
		private uint TranslateMarginX(int pixels)
		{
			return (uint) ((screenSize * pixels + 500) / 1000 / pixelAspectRatio);
		}
		
		private uint TranslateMarginY(int pixels)
		{
			return (uint) ((screenSize * pixels + 500) / 1000 * pixelAspectRatio);
		}
		
		private void CalculateScreenSize()
		{
			screenSize = Math.Min(Screen.Height * 4 / 3, Screen.Width);
			pixelAspectRatio = 1.0;
			if (Screen.Height == 1024 && Screen.Width == 1280) pixelAspectRatio = (double) 1024 * 4 / 3 / 1280;
		}
		
		private void CalculateWindowSize()
		{
			windowWidth = (int) (screenSize * Math.Sqrt(SCREEN_AREA_RATIO * ASPECT_RATIO) / pixelAspectRatio);
			windowHeight = (int) (screenSize * Math.Sqrt(SCREEN_AREA_RATIO / ASPECT_RATIO) * pixelAspectRatio);
		}
		
		#endregion Implementation (X-develop GTK# designer code)
	}
}

