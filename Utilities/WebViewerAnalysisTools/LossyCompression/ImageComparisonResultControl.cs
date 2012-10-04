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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    public partial class ImageComparisonResultControl : UserControl
    {
        public ImageComparisonResultControl()
        {
            InitializeComponent();
        }

        public void SetImageComparisonResult(ImageComparisonResult result)
        {
            Bitmap diffImage = Difference(result.Image1, result.Image2);
            Width = result.Image1.Width + result.Image2.Width + diffImage.Width + 10;

            PictureBox pictureBox1 = new PictureBox();
            pictureBox1.Image = result.Image1;
            pictureBox1.Margin = new Padding(2, 2, 2, 2);
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 0);

            PictureBox pictureBox2 = new PictureBox();
            pictureBox2.Image = result.Image2;
            pictureBox2.Margin = new Padding(2, 2, 2, 2);
            tableLayoutPanel1.Controls.Add(pictureBox2, 1, 0);


            PictureBox pictureBox3 = new PictureBox();
            pictureBox3.Image = diffImage;
            pictureBox3.Margin = new Padding(2, 2, 2, 2);
            tableLayoutPanel1.Controls.Add(pictureBox3, 2, 0);


            pictureBox1.Size = new Size(result.Image1.Width, result.Image1.Height);
            pictureBox2.Size = new Size(result.Image2.Width, result.Image2.Height);
            pictureBox3.Size = new Size(diffImage.Width, diffImage.Height);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(result.Description);
            sb.AppendLine(String.Format("R: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[0].MeanError, result.Channels[0].StdDeviation, Math.Abs(result.Channels[0].MinError), Math.Abs(result.Channels[0].MaxError)));
            sb.AppendLine(String.Format("G: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[1].MeanError, result.Channels[1].StdDeviation, Math.Abs(result.Channels[1].MinError), Math.Abs(result.Channels[1].MaxError)));
            sb.AppendLine(String.Format("B: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[2].MeanError, result.Channels[2].StdDeviation, Math.Abs(result.Channels[2].MinError), Math.Abs(result.Channels[2].MaxError)));
            sb.AppendLine();

            if (result.CompressionRatio!=null)
            {
                sb.AppendLine(String.Format("Compression Ratio: {0}", result.CompressionRatio));
            }
            //Platform.ShowMessageBox(sb.ToString());

            ResultText.Text = sb.ToString();


        }

        Bitmap Difference(Bitmap bmp1, Bitmap bmp2)
        {
            Bitmap diff = new Bitmap(bmp1.Width, bmp1.Height);

            for(int x=0;x<bmp1.Width; x++)
            {
                for(int y=0; y<bmp1.Height; y++)
                {
                    Color c1 = bmp1.GetPixel(x, y);
                    Color c2 = bmp2.GetPixel(x, y);
                    Color c = Color.FromArgb(255,
                                             Math.Abs(c1.R - c2.R),
                                             Math.Abs(c1.G - c2.G),
                                             Math.Abs(c1.B - c2.B)
                        );

                    diff.SetPixel(x, y, c);
                }
            }

            return diff;
        }
    }

}
