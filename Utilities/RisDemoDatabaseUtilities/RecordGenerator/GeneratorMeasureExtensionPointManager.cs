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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    [ExtensionPoint()]
    public class GeneratorMeasureExtensionPoint : ExtensionPoint<IGeneratorMeasure>
    {
    }

    public class GeneratorMeasureExtensionPointManager : BasicExtensionPointManager<IGeneratorMeasure>
    {
        private bool _isLoaded = false;

        public IEnumerable<IGeneratorMeasure> EntityRecordGeneratorCollection
        {
            get
            {
                if (_isLoaded == false)
                {
                    _isLoaded = true;

                    try
                    {
                        this.LoadExtensions();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(e); //handle properly
                    }
                }

                return this.Extensions.AsReadOnly();
            }
        }

        protected override IExtensionPoint GetExtensionPoint()
        {
            return new GeneratorMeasureExtensionPoint();
        }
    }
}
