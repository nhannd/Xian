using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    public interface IModalityDisplaySetCreationOptions
    {
        string Modality { get; }

        bool CreateAllImagesDisplaySet { get; }
        bool CreateSingleImageDisplaySets { get; }
        bool ShowOriginalSeries { get; }

        bool SplitMultiEchoSeries { get; }
        bool ShowOriginalMultiEchoSeries { get; }

        bool SplitMixedMultiframes { get; }
        bool ShowOriginalMixedMultiframeSeries { get; }

        bool ShowGrayscaleInverted { get; }
    }

    public interface IDisplaySetCreationOptions : IEnumerable<IModalityDisplaySetCreationOptions>
    {
        IModalityDisplaySetCreationOptions this[string modality] { get; set; }
    }

    public class DisplaySetCreationOptions : IDisplaySetCreationOptions
    {
        private readonly SortedDictionary<string, IModalityDisplaySetCreationOptions> _options;

        public DisplaySetCreationOptions()
        {
            _options = new SortedDictionary<string, IModalityDisplaySetCreationOptions>();
            foreach (var storedSetting in DisplaySetCreationSettings.Default.GetStoredSettings())
                _options[storedSetting.Modality] = storedSetting;

        }

        public IModalityDisplaySetCreationOptions this[string modality]
        {
            get
            {
                IModalityDisplaySetCreationOptions value;
                return _options.TryGetValue(modality, out value) ? value : null;
            }
            set
            {
                _options[modality] = value;
            }
        }

        #region IEnumerable<IModalityDisplaySetCreationOptions> Members

        public IEnumerator<IModalityDisplaySetCreationOptions> GetEnumerator()
        {
            return _options.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}