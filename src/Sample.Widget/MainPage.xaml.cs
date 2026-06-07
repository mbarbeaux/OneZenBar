// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

using Windows.UI.Xaml.Controls;

namespace Sample.Widget
{
    /// <summary>
    /// Page that opens if the widget app is launched as a regular UWP app instead of Xbox Game Bar widget (for example, from the Start menu or when debugging).
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}