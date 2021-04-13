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
    public sealed partial class AddDictionaryPage : Page
    {
        MainPage MainPage;

        public AddDictionaryPage()
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
            //名前が空欄でないかを判定
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                var invalidNameDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please fill in a dictionary name.",
                    CloseButtonText = "OK"
                };

                await invalidNameDialog.ShowAsync();
                return;
            }
            
            //有効なURIかを判定
            if (!Uri.IsWellFormedUriString(urlBox.Text, UriKind.Absolute))
            {
                var invalidUrlDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Invalid URL. Please fill in a valid one.",
                    CloseButtonText = "OK"
                };

                await invalidUrlDialog.ShowAsync();
                return;
            }

            MainPage.AddNewDictionary(new DictionaryInfo(nameBox.Text, urlBox.Text, separatorBox.Text));
            Window.Current.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }
    }
}
