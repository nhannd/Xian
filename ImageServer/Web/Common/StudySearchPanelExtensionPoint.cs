#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common
{
    public interface IStudySearchPage
    {
        Control SearchFieldsContainer { get; }
    }

    public interface IStudySearchPageExtension
    {

        void OnPageLoad(IStudySearchPage page);
    }

    [ExtensionPoint]
    public sealed class StudySearchPageExtensionPoint : ExtensionPoint<IStudySearchPageExtension>
    {
    }

    
}