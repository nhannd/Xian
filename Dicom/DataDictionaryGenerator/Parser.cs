using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Globalization;


namespace ClearCanvas.Dicom.DataDictionaryGenerator
{
    public struct Tag
    {
        public uint nTag;
        public String tag;
        public String name;
        public String vr;
        public String vm;
        public String retired;
        public String varName;
    }

    public struct SopClass
    {
        public String name;
        public String uid;
        public String type;
        public String varName;
    }

    public class Parser
    {
        public SortedList<uint, Tag> _tags = new SortedList<uint, Tag>();
        public SortedList _sopClasses = new SortedList();
        public SortedList _metaSopClasses = new SortedList();
        public SortedList _tranferSyntaxes = new SortedList();

        public Parser()
        {
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

        public void ParseFile(String filename)
        {
            TextReader tReader = new StreamReader(filename);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            settings.ValidationType = ValidationType.None;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreProcessingInstructions = true;
            XmlReader reader = XmlReader.Create(tReader, settings);
            String[] columnArray = new String[10];
            int colCount = -1;
            bool isFirst = true;
            bool isTag = true;
            bool isUid = true;
            try
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        isFirst = true;
                        if (reader.Name == "w:tbl")
                        {
                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    if (reader.Name == "w:tc")
                                    {
                                        colCount++;
                                    }
                                    else if (reader.Name == "w:t")
                                    {
                                        String val = reader.ReadString();
                                        //if (val != "(")
                                        if (columnArray[colCount] == null)
                                            columnArray[colCount] = val;
                                        else
                                            columnArray[colCount] += val;
                                    }
                                }
                                if ((reader.NodeType == XmlNodeType.EndElement)
                                    && (reader.Name == "w:tr"))
                                {
                                    if (isFirst)
                                    {
                                        if (columnArray[0] == "Tag")
                                        {
                                            isTag = true;
                                            isUid = false;
                                        }
                                        else
                                        {
                                            isTag = false;
                                            isUid = true;
                                        }

                                        isFirst = false;
                                    }
                                    else
                                    {
                                        if (isTag)
                                        {
                                            Tag thisTag = new Tag();
                                            if (columnArray[0] != null && columnArray[0] != "Tag")
                                            {
                                                thisTag.tag = columnArray[0];
                                                thisTag.name = columnArray[1];
                                                thisTag.vr = columnArray[2];
                                                thisTag.vm = columnArray[3];
                                                thisTag.retired = columnArray[4];

                                                // Handle repeating groups
                                                if (thisTag.tag[3] == 'x')
                                                    thisTag.tag = thisTag.tag.Replace("xx", "00");

                                                char[] charSeparators = new char[] { '(', ')', ',', ' ' };

                                                String[] nodes = thisTag.tag.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                                                thisTag.nTag = UInt32.Parse(nodes[1], NumberStyles.HexNumber) | (UInt32.Parse(nodes[0], NumberStyles.HexNumber) << 16);


                                                if (thisTag.name != null)
                                                {
                                                    CreateNames(ref thisTag);

                                                    if (!thisTag.varName.Equals("Item")
                                                     && !thisTag.varName.Equals("ItemDelimitationItem")
                                                     && !thisTag.varName.Equals("SequenceDelimitationItem")
                                                     && !thisTag.varName.Equals("GroupLength"))

                                                        _tags.Add(thisTag.nTag, thisTag);
                                                }
                                            }
                                        }
                                        else if (isUid)
                                        {

                                            if (columnArray[0] != null)
                                            {
                                                SopClass thisUid = new SopClass();

                                                thisUid.uid = columnArray[0];
                                                thisUid.name = columnArray[1];
                                                thisUid.type = columnArray[2];

                                                char[] charSeparators = new char[] { '(', ')', ',', ' ', '\'', '’', '–', '-', '/', '&', '@', '[', ']' };

                                                String[] nodes = thisUid.name.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

                                                thisUid.varName = "";
                                                foreach (String node in nodes)
                                                    thisUid.varName += node;


                                                if (thisUid.type == "SOP Class")
                                                {
                                                    // Handling leading digits in names
                                                    if (thisUid.varName.Length > 0 && char.IsDigit(thisUid.varName[0]))
                                                        thisUid.varName = "Sop" + thisUid.varName;
                                                    _sopClasses.Add(thisUid.name, thisUid);
                                                }
                                                else if (thisUid.type == "Transfer Syntax")
                                                {
                                                    int index = thisUid.varName.IndexOf(':');
                                                    if (index != -1)
                                                        thisUid.varName = thisUid.varName.Remove(index);

                                                    _tranferSyntaxes.Add(thisUid.name, thisUid);
                                                }
                                                else if (thisUid.type == "Meta SOP Class")
                                                {
                                                    // Handling leading digits in names
                                                    if (thisUid.varName.Length > 0 && char.IsDigit(thisUid.varName[0]))
                                                        thisUid.varName = "Sop" + thisUid.varName;
                                                    _metaSopClasses.Add(thisUid.name, thisUid);
                                                }
                                            }
                                        }
                                    }

                                    colCount = -1;
                                    for (int i = 0; i < columnArray.Length; i++)
                                        columnArray[i] = null;
                                }

                                if ((reader.NodeType == XmlNodeType.EndElement)
                                 && (reader.Name == "w:tbl"))
                                    break; // end of table
                            }
                        }
                    }
                }
            }
            catch (XmlException)
            {

            }

        }

    }
}
