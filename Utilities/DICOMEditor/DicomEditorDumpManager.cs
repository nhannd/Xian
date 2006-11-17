using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomEditorDumpManager
    {
        
        public void MoveNext()
        {
            if (_position >= (_loadedDicomDumps.GetLength(0) - 1))
            {
                return;
            }
            
            _position++;
        }

        public void MovePrevious()
        {
            if (_position <= 0)
            {
                return;
            }
            _position--;
        }

        public void ApplyEdit(DicomTag Tag, EditType Type, bool ApplyToAll)
        {
            if (ApplyToAll == false)
            {
                _loadedDicomDumps[_position].AddEditItem(new EditItem(Tag, Type));
            }
            else
            {
                for (int i = 0; i < _loadedDicomDumps.Length; i++)
                {
                    if (_loadedDicomDumps[i].TagExists(Tag.Key))
                    {
                        _loadedDicomDumps[i].AddEditItem(new EditItem(Tag, Type == EditType.Create ? EditType.Update : Type));
                    }
                    else
                    {
                        if (Type != EditType.Delete)
                        {
                            _loadedDicomDumps[i].AddEditItem(new EditItem(Tag, Type == EditType.Update ? EditType.Create : Type));
                        }
                    }
                }
            }
        }

        public void RemoveAllPrivateTags()
        {
            for (int i = 0; i < _loadedDicomDumps.Length; i++)
            {
                List<DicomTag> tagList = new List<DicomTag>(_loadedDicomDumps[i].DisplayTagList);
                foreach (DicomTag currentTag in tagList)
                {
                    if (currentTag.Group % 2 != 0)
                    {
                        _loadedDicomDumps[i].AddEditItem(new EditItem(currentTag, EditType.Delete));
                    }
                }
            }
        }

        public void RevertAll()
        {
            for (int i = 0; i < _loadedDicomDumps.Length; i++)
            {
                _loadedDicomDumps[i].RevertEdits();
            }
        }


        //part of component directly
        public void SetPaths(IEnumerable<string> rawPaths)
        {
            List<string> paths = new List<string>();

            foreach (string rawPath in rawPaths)
            {
                FileProcessor.ProcessFile process = new FileProcessor.ProcessFile(delegate(string path) { paths.Add(path); });
                FileProcessor.Process(rawPath, "*.*", process, true);
            }
            DicomFileAccessor accessor = new DicomFileAccessor();

            _loadedDicomDumps = accessor.LoadDicomFiles(paths);
            _position = 0;
        }



        //part of tool context

        public DicomDump Current
        {
            get { return _loadedDicomDumps[_position]; }
        }

        public bool IsCurrentFirst()
        {
           return _position == 0;
        }

        public bool IsCurrentLast()
        {
            return _position == (_loadedDicomDumps.GetLength(0) - 1);
        }

        public bool IsCurrentTheOnly()
        {
            return _loadedDicomDumps.GetLength(0) == 1;
        }

        private DicomDump[] _loadedDicomDumps = null;
        private int _position = 0;
    }
}
