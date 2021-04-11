using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace WordCross
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AddDictionary : Page
    {
        MainPage MainPage;

        public AddDictionary()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainPage = (MainPage)e.Parameter;
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var isValidUrl = Uri.IsWellFormedUriString(urlBox.Text, UriKind.Absolute);

            if (isValidUrl)
            {
                MainPage.AddNewDictionary(new DictionaryInfo(nameBox.Text, urlBox.Text, separatorBox.Text));
                Window.Current.Close();
            }
            else
            {
                var invalidUrlDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Ivalid URL",
                    CloseButtonText = "Ok"
                };

                await invalidUrlDialog.ShowAsync();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }
    }
}
