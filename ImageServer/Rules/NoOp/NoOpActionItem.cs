#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Rules.NoOp
{
    public class NoOpActionItem : ServerActionItemBase
    {
        public NoOpActionItem()
            : base("No-Op Action")
        {
        }

        protected override bool OnExecute(ServerActionContext context)
        {
            return true;
        }
    }
}