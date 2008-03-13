using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Codec.Jpeg;
using ClearCanvas.Dicom.Codec.Jpeg2000;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestCompressionForm : Form
    {
        public TestCompressionForm()
        {
            InitializeComponent();
            this.comboBoxCompressionType.Items.Add(TransferSyntax.Jpeg2000ImageCompression);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.RleLossless);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegExtendedProcess24);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegBaselineProcess1);
            this.comboBoxCompressionType.Items.Add(TransferSyntax.JpegLosslessNonHierarchicalProcess14);

            DicomCodecRegistry.RegisterCodec(TransferSyntax.JpegBaselineProcess1,
                                 new DicomJpegProcess1CodecFactory());


            DicomCodecRegistry.RegisterCodec(TransferSyntax.JpegExtendedProcess24,
                                             new DicomJpegProcess24CodecFactory());


            DicomCodecRegistry.RegisterCodec(TransferSyntax.JpegLosslessNonHierarchicalProcess14,
                                             new DicomJpegLossless14CodecFactory());

            DicomCodecRegistry.RegisterCodec(
                TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1,
                new DicomJpegLossless14SV1CodecFactory());

            DicomCodecRegistry.RegisterCodec(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly,
                                 new DicomJpeg2000LosslessCodecFactory());
            DicomCodecRegistry.RegisterCodec(TransferSyntax.Jpeg2000ImageCompression,
                                 new DicomJpeg2000LossyCodecFactory());
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