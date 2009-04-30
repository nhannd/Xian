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

using System;
using System.Windows.Forms;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestCompressionForm : Form
    {
        public TestCompressionForm()
        {
            InitializeComponent();

			//TODO: this can now just use DicomCodecRegistry.GetCodecTransferSyntaxes()
            this.comboBoxCompressionType.Items.Add(TransferSyntax.Jpeg2000ImageCompression);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.RleLossless);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegExtendedProcess24);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegBaselineProcess1);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegLosslessNonHierarchicalProcess14);
        }

        private void buttonDecompress_Click(object sender, EventArgs e)
        {
            if (this.textBoxSourceFile.Text.Length == 0 || this.textBoxDestinationFile.Text.Length == 0)
            {
                MessageBox.Show("Invalid source or destination filename");
                return;
            }

            DicomFile dicomFile = new DicomFile(textBoxSourceFile.Text);

            dicomFile.Load();

            dicomFile.Filename = textBoxDestinationFile.Text;

            dicomFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);

            dicomFile.Save();
        }

        private void buttonBrowseSourceFile_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = textBoxSourceFile.Text;
            openFileDialog.ShowDialog();
            this.textBoxSourceFile.Text = this.openFileDialog.FileName;
        }

        private void buttonBrowseDestinationFile_Click(object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = this.textBoxDestinationFile.Text;
            saveFileDialog.ShowDialog();
            textBoxDestinationFile.Text = saveFileDialog.FileName;
        }

        private void buttonCompress_Click(object sender, EventArgs e)
        {
            TransferSyntax syntax = this.comboBoxCompressionType.SelectedItem as TransferSyntax;
            if (syntax == null)
            {
                MessageBox.Show("Transfer syntax not selected");
                return;
            }

            DicomFile dicomFile = new DicomFile(textBoxSourceFile.Text);

            dicomFile.Load();
            if (dicomFile.TransferSyntax.Encapsulated)
            {
                MessageBox.Show(String.Format("Message encoded as {0}, cannot compress.", dicomFile.TransferSyntax));
                return;
            }

            dicomFile.Filename = textBoxDestinationFile.Text;

            dicomFile.ChangeTransferSyntax(syntax);

            dicomFile.Save();

        }
    }
}