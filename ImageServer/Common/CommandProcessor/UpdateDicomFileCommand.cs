using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    public class UpdateDicomFileCommand : ServerCommand
    {
        private DicomFileUpdateCommandActionList _actionList;
        private string _outputFilePath;
        private string _backupExistingFileName;
        private bool _backup = false;
        bool _saved;

        private DicomFile _file;

        public UpdateDicomFileCommand(DicomFileUpdateCommandActionList actionList)
            : base("UpdateDicomFileCommand", true)
        {
            _actionList = actionList;
        }

        public DicomFile DicomFile
        {
            get { return _file; }
            set { _file = value; }
        }

        public string OutputFilePath
        {
            get { return _outputFilePath; }
            set { _outputFilePath = value; }
        }


        public override void Dispose()
        {
            if (File.Exists(_backupExistingFileName))
                File.Delete(_backupExistingFileName); 
            base.Dispose();
        }

        protected override void OnExecute()
        {
            Platform.CheckForNullReference(DicomFile, "DicomFile");

            foreach(IDicomFileUpdateCommandAction action in _actionList)
            {
                action.Apply(DicomFile);
            }

            if (!String.IsNullOrEmpty(OutputFilePath))
            {
                if (File.Exists(OutputFilePath))
                {
                    // backup the file first for undo purpose
                    _backupExistingFileName = OutputFilePath + "." + Path.GetRandomFileName();
                    File.Move(OutputFilePath, _backupExistingFileName);
                    _backup = true;
                } 
                
                DicomFile.Save(OutputFilePath);

                _saved = true;
                Debug.Assert(_backupExistingFileName != null);
            }

            Platform.Log(LogLevel.Debug, "Dicom file updated");
        }


        protected override void OnUndo()
        {
            if (!String.IsNullOrEmpty(OutputFilePath) && _saved)
            {
                File.Delete(OutputFilePath);

                if (_backup)
                {
                    // restore the backup file
                    Debug.Assert(_backupExistingFileName != null);
                    File.Move(_backupExistingFileName, OutputFilePath);
                }
            }

            //TODO: We may want to also undo changes the DicomFile object.
        }
    }

    /// <summary>
    /// Defines the interface of an Dicom file level update action that can be applied to a <see cref="DicomFile"/>
    /// </summary>
    public interface IDicomFileUpdateCommandAction
    {
        void Apply(DicomFile file);
    }

    /// <summary>
    /// List of <see cref="IDicomFileUpdateCommandAction"/>
    /// </summary>
    public class DicomFileUpdateCommandActionList : List<IDicomFileUpdateCommandAction> { }



}
