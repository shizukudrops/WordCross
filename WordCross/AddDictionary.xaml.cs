using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            MainPage = (MainPage)e.Parameter;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            MainPage.AddNewDictionary(new DictionaryInfo(nameBox.Text, urlBox.Text, separatorBox.Text));
            Window.Current.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }
    }
}
