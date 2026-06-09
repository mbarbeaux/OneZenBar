// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

using OneZenBar.Core;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace OneZenBar.Widget
{
    /// <summary>
    /// Primary widget page for the application.
    /// </summary>
    public sealed partial class PrimaryWidget : Page
    {
        public PrimaryWidget()
        {
            InitializeComponent();

            // The version line is composed at runtime, so it cannot use x:Uid like the static
            // TextBlocks. Pull the localized format string from Strings/<lang>/Resources.resw and
            // fill in the version. ResourceLoader resolves the language at runtime, so the same
            // call yields the right text in every supported language.
            var resources = ResourceLoader.GetForCurrentView();
            var format = resources.GetString("LibraryVersionFormat");
            LibraryVersionText.Text = string.Format(CultureInfo.CurrentCulture, format, LibraryInfo.GetVersion());
        }
    }
}