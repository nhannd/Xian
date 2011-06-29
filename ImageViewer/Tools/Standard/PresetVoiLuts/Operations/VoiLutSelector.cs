using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Layout;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
    public interface IVoiLutSelector
    {
        IUndoableOperation<IPresentationImage> GetOperation();
    }

    public abstract class VoiLutSelector : IVoiLutSelector
    {
        #region IVoiLutSelector Members

        public abstract IUndoableOperation<IPresentationImage> GetOperation();

        #endregion
    }

    public class AutoVoiLutSelector : VoiLutSelector
    {
        public bool IsData { get; set; }
        public string LutExplanation { get; set; }
        public int? LutIndex { get; set; }

        public static AutoVoiLutSelector CreateFrom(IPresentationImage image)
        {
            var applicator = AutoVoiLutApplicator.Create(image);
            if (applicator == null)
                return null;

            var autoLut = applicator.GetAppliedLut();
            if (autoLut == null)
                return null;

            return !autoLut.IsHeader ? null
                       : new AutoVoiLutSelector
                       {
                           IsData = autoLut.IsData,
                           LutExplanation = autoLut.Explanation,
                           LutIndex = autoLut.Index
                       };
        }

        public override IUndoableOperation<IPresentationImage> GetOperation()
        {
            return new BasicImageOperation(GetOriginator, Apply);
        }

        private static IMemorable GetOriginator(IPresentationImage image)
        {
            return !AutoVoiLutApplicator.CanCreate(image) ? null : ((IVoiLutProvider) image).VoiLutManager;
        }

        private void Apply(IPresentationImage image)
        {
            var applicator = AutoVoiLutApplicator.Create(image);
            if (applicator == null)
                return;

            if (IsData)
            {
                if (!String.IsNullOrEmpty(LutExplanation))
                    applicator.ApplyDataLut(LutExplanation);
                else
                    applicator.ApplyDataLut(LutIndex ?? 0); //just apply the first one.
            }
            else
            {
                if (!String.IsNullOrEmpty(LutExplanation))
                    applicator.ApplyLinearLut(LutExplanation);
                else if (LutIndex.HasValue)
                    applicator.ApplyLinearLut(LutIndex ?? 0); //just apply the first one.
            }
        }
    }

    [HpDataContract("{9F548831-7669-4abb-8CFC-A3AB676C09E6}")]
    internal class LinearPresetVoiLut
    {
        public string Modality { get; set; }
        public string Name { get; set; }
        public double WindowWidth { get; set; }
        public double WindowCenter { get; set; }
    }

    public class LinearPresetVoiLutSelector : VoiLutSelector
    {
        private readonly LinearPresetVoiLut _dataContract;

        internal LinearPresetVoiLutSelector(LinearPresetVoiLut dataContract)
        {
            _dataContract = dataContract;
        }

        public LinearPresetVoiLutSelector(object dataContract)
            : this((LinearPresetVoiLut)dataContract)
        {
        }

        public object DataContract { get{ return _dataContract; } }

        internal string Modality { get { return _dataContract.Modality; } }
        public string Name { get { return _dataContract.Name; } }
        public double WindowWidth { get { return _dataContract.WindowWidth; } }
        public double WindowCenter { get { return _dataContract.WindowCenter; } }

        public string Description
        { 
            get
            {
                return String.Format(SR.FormatLinearPresetDescription
                                     , String.IsNullOrEmpty(Modality) ? "?" : Modality
                                     , String.IsNullOrEmpty(Name) ? "?" : Name);
            }
        }

        public static LinearPresetVoiLutSelector CreateFrom(IPresentationImage image)
        {
            string modality;
            var lut = GetAppliedLut(image, out modality);
            if (lut == null)
                return null;

            return new LinearPresetVoiLutSelector(new LinearPresetVoiLut
                    {
                        Modality = modality,
                        Name = lut.Name,
                        WindowWidth = lut.WindowWidth,
                        WindowCenter = lut.WindowCenter
                    });
        }

        public static List<LinearPresetVoiLutSelector> GetAllSelectors()
        {
            var selectors = new List<LinearPresetVoiLutSelector>();
            var groups = PresetVoiLutSettings.Default.GetPresetGroups();
            foreach (var @group in groups)
            {
                foreach (var preset in @group.Presets)
                {
                    var dataContract = new LinearPresetVoiLut { Modality = @group.Modality };
                    var operation = (LinearPresetVoiLutOperationComponent)preset.Operation;
                    dataContract.Name = preset.Operation.Name;
                    dataContract.WindowWidth = operation.WindowWidth;
                    dataContract.WindowCenter = operation.WindowCenter;
                    selectors.Add(new LinearPresetVoiLutSelector(dataContract));
                }
            }

            return selectors;
        }
        
        public override IUndoableOperation<IPresentationImage> GetOperation()
        {
            return new BasicImageOperation(GetOriginator, Apply);
        }

        private IPresetVoiLutOperation GetRealOperation(IPresentationImage image)
        {
            var sopProvider = image as IImageSopProvider;
            if (sopProvider == null)
                return null;

            var imageSop = sopProvider.ImageSop;
            var groups = PresetVoiLutSettings.Default.GetPresetGroups();
            foreach (var @group in groups)
            {
                if (!group.AppliesTo(imageSop))
                    continue;

                foreach (var preset in @group.Presets)
                {
                    if (Equals(Name, preset.Operation.Name) && preset.Operation.AppliesTo(image))
                        return preset.Operation;
                }
            }

            return null;
        }

        private static NamedVoiLutLinear GetAppliedLut(IPresentationImage image, out string modality)
        {
            modality = null;

            var lutProvider = image as IVoiLutProvider;
            if (lutProvider == null)
                return null;

            var sopProvider = image as IImageSopProvider;
            if (sopProvider == null)
                return null;

            var namedLut = lutProvider.VoiLutManager.VoiLut as NamedVoiLutLinear;
            if (namedLut == null)
                return null;

            var imageSop = sopProvider.ImageSop;
            modality = imageSop.Modality;

            var groups = PresetVoiLutSettings.Default.GetPresetGroups();
            foreach (var @group in groups)
            {
                if (!group.AppliesTo(imageSop))
                    continue;

                foreach (var preset in @group.Presets)
                {
                    if (!Equals(preset.Operation.Name, namedLut.Name) || !preset.Operation.AppliesTo(image))
                        continue;

                    var operation = (LinearPresetVoiLutOperationComponent)preset.Operation;
                    if (operation.WindowCenter == namedLut.WindowCenter || operation.WindowWidth == namedLut.WindowWidth)
                        return namedLut;
                }
            }

            return null;
        }

        private IMemorable GetOriginator(IPresentationImage image)
        {
            return GetRealOperation(image) == null ? null : ((IVoiLutProvider)image).VoiLutManager;
        }

        private void Apply(IPresentationImage image)
        {
            var operation = GetRealOperation(image);
            if (operation == null)
                return;

            operation.Apply(image);
        }
    }
}
