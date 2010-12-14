#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls
{
    public partial class ThrottlePanel : UserControl
    {
        public ThrottlePanel()
        {
            InitializeComponent();
            DataContext = ThrottleSettings.Default;
        }

        private void Strategies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Strategies != null)
            {
                ConstantRatePanel.Visibility = ThrottleSettings.Default.Strategy == ThrottleStrategy.ConstantRate ? Visibility.Visible : Visibility.Collapsed;

            }
        }
    }

    public class EnumValuesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            else
                return value.GetType().GetFields().Where(f=>f.IsLiteral).Select(f => f.GetValue(null)).ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
