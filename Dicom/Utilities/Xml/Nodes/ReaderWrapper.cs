using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Utilities.Xml.Nodes
{
    public class ReaderWrapper
    {
        private readonly XmlTextReader _textReader;
        private readonly XmlReader _reader;
        private int _nodeCount;

        public ReaderWrapper(string xml)
        {
            var textStream = new StringReader(xml);
            var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
            _textReader = new XmlTextReader(textStream);
            _reader = XmlReader.Create(_textReader, settings);
            
        }

        public ReaderWrapper(Stream xmlStream)
        {
            var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
            _textReader = new XmlTextReader(xmlStream);
            _reader = XmlReader.Create(_textReader, settings);

        }

        public XmlReader GetXmlReader{ get{ return _reader;}}
        public int NodeCount { get { return _nodeCount; } }

        public string ReadString()
        {
            string inner = null;
            try
            {
                inner = _reader.ReadString();
                ThrowAwayEndElements();
            }
            catch (System.Exception e)
            {

                Platform.Log(LogLevel.Error, e, "MyReader: exception while reading string");
            }
            return inner;

        }

        public string ReadInnerXml()
        {
            string inner = null;
            try
            {
                inner = _reader.ReadInnerXml();
            }
            catch (System.Exception e)
            {

                Platform.Log(LogLevel.Error, e, "MyReader: exception while reading inner xml");
            }
            return inner;

        }

        public bool HasChildNodes()
        {
            return !_reader.IsEmptyElement;
        }

        public bool MoveFirstChild()
        {

            if (_reader.EOF || _reader.IsEmptyElement)
                return false;

            bool foundChild = false;
            try
            {
                //advance to next node
                _reader.ReadStartElement();
                if (_reader.EOF)
                    return false;
                //found a child, so increment node count
                foundChild = _reader.IsStartElement();
                if (foundChild)
                    _nodeCount++;
                //skip over end elements
                else
                    ThrowAwayEndElements();
            }
            catch (System.Exception e)
            {
                
                 Platform.Log(LogLevel.Error, e, "MyReader: exception while moving to first child");
            }
            return foundChild;
        }

        public bool MoveNextSibling(int siblingDepth, int siblingNodeCount)
        {
            if (_reader.EOF)
                return false;

            //someone else advanced the reader - do nothing!!
            if (_reader.Depth == siblingDepth && _nodeCount > siblingNodeCount)
                return true;
            try
            {

                //simply read the next element
                if (_reader.IsEmptyElement)
                {
                    _reader.ReadStartElement();
                    if (_reader.EOF)
                        return false;
                    //found sibling
                    if (_reader.IsStartElement())
                        _nodeCount++;
                    else
                        ThrowAwayEndElements();
                }
                else
                {
                    //ignore all sub-nodes
                    while (!_reader.EOF && _reader.Depth > siblingDepth)
                    {
                        _reader.Read();
                        if (_reader.IsStartElement())
                            _nodeCount++;
                    }
                    ThrowAwayEndElements();
                }
            }
            catch (System.Exception e)
            {
                
                Platform.Log(LogLevel.Error, e, "MyReader: exception while moving to next sibling");
            }

            //we have a sibling if reader.Depth is equal to sibling depth
            return !_reader.EOF && (_reader.Depth == siblingDepth);
        }
        public IDictionary<string, string> GetAttributes()
        {
            IDictionary<string, string> attributes = null;
            //get attributes
            if (_reader.HasAttributes)
            {
                attributes = new Dictionary<string, string>();
                _reader.MoveToFirstAttribute();
                attributes[_reader.LocalName] = _reader.Value;
                while (_reader.MoveToNextAttribute())
                {
                    attributes[_reader.LocalName] = _reader.Value;
                }
                _reader.MoveToContent();
                ThrowAwayEndElements();
            }
            return attributes;
        }


        private void ThrowAwayEndElements()
        {
            var readEndElement = false;
            try
            {
                //ignore end elements
                while (!_reader.EOF && _reader.NodeType == XmlNodeType.EndElement)
                {
                    _reader.Read();
                    readEndElement = true;
                }
            }
            catch (System.Exception e)
            {
                
                 Platform.Log(LogLevel.Error, e, "MyReader: exception while tidying");
            }
            if (readEndElement  && _reader.IsStartElement())
                _nodeCount++;
        }
    }
}