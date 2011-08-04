#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion


namespace ClearCanvas.Enterprise.Desktop
{
    public interface IPasswordDialog
    {   
        bool Show();    
        string Password { get; }
    }
}
