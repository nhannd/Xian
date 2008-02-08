using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client.Adt
{
    public class OrderAdditionalInfoComponent : DHtmlComponent
    {
        private IDictionary<string, string> _orderExtendedProperties;

        public OrderAdditionalInfoComponent(IDictionary<string, string> orderExtendedProperties)
        {
            _orderExtendedProperties = orderExtendedProperties;
        }

        public OrderAdditionalInfoComponent()
        {
            _orderExtendedProperties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the dictionary of order extended properties that this component will
        /// use to store data.
        /// </summary>
        public IDictionary<string, string> OrderExtendedProperties
        {
            get { return _orderExtendedProperties; }
            set { _orderExtendedProperties = value; }
        }

        public override void Start()
        {
            SetUrl(TechnologistDocumentationComponentSettings.Default.PreExamDetailsPageUrl);
            base.Start();
        }

        protected override IDictionary<string, string> TagData
        {
            get
            {
                return _orderExtendedProperties;
            }
        }

    }
}
