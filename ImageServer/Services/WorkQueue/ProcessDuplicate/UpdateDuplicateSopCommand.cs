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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Core.Edit;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ProcessDuplicate
{
    internal class UpdateDuplicateSopCommand : CommandBase
    {
        #region Private Members

        private readonly List<BaseImageLevelUpdateCommand> _commands;
        private readonly DicomFile _file;

        #endregion

        #region Constructors

        public UpdateDuplicateSopCommand(DicomFile file, List<BaseImageLevelUpdateCommand> commands)
            :base("Duplicate SOP demographic update command", true)
        {
            _file = file;
            _commands = commands;
        }

        #endregion

        #region Overridden Protected Methods

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            if (_commands!=null)
            {
                foreach (BaseImageLevelUpdateCommand command in _commands)
                {
                    if (!command.Apply(_file))
                        throw new ApplicationException(
                            String.Format("Unable to update the duplicate sop. Command={0}", command));
                }
            }
            
        }

        
        protected override void OnUndo()
        {
        }

        #endregion

    }
}