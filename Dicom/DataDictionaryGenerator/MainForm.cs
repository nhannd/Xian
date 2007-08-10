using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;

namespace ClearCanvas.ImageServer.Dicom.DataDictionaryGenerator
{
    public partial class MainForm : Form
    {
        XmlDocument _transferSyntaxDoc = null;
        Parser _parse = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            _parse = new Parser();

            _parse.ParseFile(openFileDialog1.FileName);

            AddGroupZeroTags(_parse._tags);
        }

        private void OpenFile_MouseDown(object sender, MouseEventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void CreateNames(ref Tag thisTag)
        {
                // Now create the variable name
                char[] charSeparators = new char[] { '(', ')', ',', ' ', '\'', '’', '–', '-', '/', '&' };

                String[] nodes = thisTag.name.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

                thisTag.varName = "";
                foreach (String node in nodes)
                    thisTag.varName += node;

                // Handling leading digits in names
                if (thisTag.varName.Length > 0 && char.IsDigit(thisTag.varName[0]))
                    thisTag.varName = "Tag" + thisTag.varName;

                if (thisTag.retired != null && thisTag.retired.Equals("RET"))
                    thisTag.varName += "Retired";
        }

        private void AddGroupZeroTags(SortedList<uint, Tag> tags)
        {
            Tag thisTag = new Tag();
            thisTag.name = "Affected SOP Class UID";
            thisTag.tag = "(0000,0002)";
            thisTag.vr = "UI";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000002;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Requested SOP Class UID";
            thisTag.tag = "(0000,0003)";
            thisTag.vr = "UI";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000003;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Command Field";
            thisTag.tag = "(0000,0100)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000100;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag.name = "Message ID";
            thisTag.tag = "(0000,0110)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000110;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Message ID Being Responded To";
            thisTag.tag = "(0000,0120)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000120;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Move Destination";
            thisTag.tag = "(0000,0600)";
            thisTag.vr = "AE";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000600;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Priority";
            thisTag.tag = "(0000,0700)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000700;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Data Set Type";
            thisTag.tag = "(0000,0800)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000800;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Status";
            thisTag.tag = "(0000,0900)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000900;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Offending Element";
            thisTag.tag = "(0000,0901)";
            thisTag.vr = "AT";
            thisTag.vm = "1-n";
            thisTag.retired = "";
            thisTag.nTag = 0x00000901;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Error Comment";
            thisTag.tag = "(0000,0902)";
            thisTag.vr = "LO";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000902;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Error ID";
            thisTag.tag = "(0000,0903)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00000903;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Affected SOP Instance UID";
            thisTag.tag = "(0000,1000)";
            thisTag.vr = "UI";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001000;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Requested SOP Instance UID";
            thisTag.tag = "(0000,1001)";
            thisTag.vr = "UI";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001001;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Event Type ID";
            thisTag.tag = "(0000,1002)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x000001002;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Attribute Identifier List";
            thisTag.tag = "(0000,1005)";
            thisTag.vr = "AT";
            thisTag.vm = "1-n";
            thisTag.retired = "";
            thisTag.nTag = 0x000001005;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Action Type ID";
            thisTag.tag = "(0000,1008)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x000001008;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Number of Remaining Sub-operations";
            thisTag.tag = "(0000,1020)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001020;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Number of Completed Sub-operations";
            thisTag.tag = "(0000,1021)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001021;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Number of Failed Sub-operations";
            thisTag.tag = "(0000,1022)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001022;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Number of Warning Sub-operations";
            thisTag.tag = "(0000,1023)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001023;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Move Originator Application Entity Title";
            thisTag.tag = "(0000,1030)";
            thisTag.vr = "AE";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001030;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Move Originator Message ID";
            thisTag.tag = "(0000,1031)";
            thisTag.vr = "US";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00001031;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);

            thisTag = new Tag();
            thisTag.name = "Group 2 Length";
            thisTag.tag = "(0002,0000)";
            thisTag.vr = "UL";
            thisTag.vm = "1";
            thisTag.retired = "";
            thisTag.nTag = 0x00020000;
            CreateNames(ref thisTag);
            tags.Add(thisTag.nTag, thisTag);
            
        }

        private void OpenTransferSyntax_Click(object sender, EventArgs e)
        {
            openFileDialog_TransferSyntax.ShowDialog();

            _transferSyntaxDoc = new XmlDocument();

            _transferSyntaxDoc.Load(openFileDialog_TransferSyntax.FileName);

        }

        private void GenerateCode_Click(object sender, EventArgs e)
        {
            if ((_transferSyntaxDoc == null) || (_parse == null))
                return;

            CodeGenerator gen = new CodeGenerator(_parse._tags, _parse._tranferSyntaxes, _parse._sopClasses, _parse._metaSopClasses, _transferSyntaxDoc);


            gen.WriteTags("DicomTags.cs");

            gen.WriteTransferSyntaxes("TransferSyntax.cs");

            gen.WriteSopClasses("SopClass.cs");

            gen.WriteTagDictionary("DicomTagDictionary.cs");
        }
    }
}