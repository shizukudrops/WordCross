using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WordCross
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly ObservableCollection<DictionaryInfo> dictView = new ObservableCollection<DictionaryInfo>();

        object RightClickedItem;

        public MainPage()
        {
            this.InitializeComponent();
            
            dictView.Add(new DictionaryInfo("Longman", "https://www.ldoceonline.com/jp/dictionary/", "-"));
            dictView.Add(new DictionaryInfo("Collins Thesaurus", "https://www.collinsdictionary.com/dictionary/english-thesaurus/", "-"));
            dictView.Add(new DictionaryInfo("Merriam Webster", "https://www.merriam-webster.com/dictionary/", "%20"));
            dictView.Add(new DictionaryInfo("英辞郎", "https://eow.alc.co.jp/", "+"));
            dictView.Add(new DictionaryInfo("DictJuggler", "https://www.dictjuggler.net/yakugo/?word=", "%20"));

            dictList.ItemsSource = dictView;
        }

        public void AddNewDictionary(DictionaryInfo dict)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.dictView.Add(dict));
        }

        private void dictList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            search((DictionaryInfo)dictList.SelectedItem, searchBox.Text);
        }

        private void searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                search((DictionaryInfo)dictList.SelectedItem, searchBox.Text);
            }
        }

        private void search(DictionaryInfo dict, string input)
        {
            //どの辞書も選ばれていなかったら一番上の辞書で検索する。辞書が存在しなければ戻る。
            if (dict == null)
            {
                if (dictList.Items.Count > 0) dict = (DictionaryInfo)dictList.Items.First();
                else return;
            }

            var words = input.Split(" ");
            var target = string.Join(dict.Separator, words);
            var final = dict.BaseUrl + target;
            webView1.Source = new Uri(final);
        }

        private void dictList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            DictListContextMenu.ShowAt(listView, e.GetPosition(listView));
            RightClickedItem = ((FrameworkElement)e.OriginalSource).DataContext;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            dictView.Remove((DictionaryInfo)RightClickedItem);
        }

        async private void AddDictionary_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(AddDictionary), this);
                Window.Current.Content = frame;

                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
    }
}
