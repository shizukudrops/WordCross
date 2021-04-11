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
using Newtonsoft.Json;
using Windows.Storage;
using Windows.ApplicationModel;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WordCross
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly ObservableCollection<DictionaryInfo> dictView;

        object RightClickedItem;

        public bool IsAdsDisabled {
            get {
                var flag = ApplicationData.Current.LocalSettings.Values["disableAds"];
                if (flag == null) return false;
                else return (bool)flag;
            }
            set {
                ApplicationData.Current.LocalSettings.Values["disableAds"] = value;
            } }

        public MainPage()
        {
            this.InitializeComponent();

            //広告ブロック機能のオンオフをトグルに反映
            disableAdsToggle.IsChecked = IsAdsDisabled;

            //辞書リストを読み込む
            var dictionariesString = ApplicationData.Current.LocalSettings.Values["dictionaries"] as string;           

            if (dictionariesString == null)
            {
                dictView = new ObservableCollection<DictionaryInfo>();
                dictView.Add(new DictionaryInfo("Longman", "https://www.ldoceonline.com/jp/dictionary/", "-"));
                dictView.Add(new DictionaryInfo("Collins Thesaurus", "https://www.collinsdictionary.com/dictionary/english-thesaurus/", "-"));
                dictView.Add(new DictionaryInfo("Merriam Webster", "https://www.merriam-webster.com/dictionary/", "%20"));
                dictView.Add(new DictionaryInfo("英辞郎", "https://eow.alc.co.jp/", "+"));
                dictView.Add(new DictionaryInfo("DictJuggler", "https://www.dictjuggler.net/yakugo/?word=", "%20"));
            }
            else
            {
                dictView = JsonConvert.DeserializeObject<ObservableCollection<DictionaryInfo>>(dictionariesString);
            }

            dictList.ItemsSource = dictView;

            App.Current.Suspending += OnSuspending;
            webView.FrameNavigationStarting += WebView_FrameNavigationStarting;
        }

        #region mymethod

        //AddDictionaryウィンドウから呼び出されるメソッド
        public async void AddNewDictionary(DictionaryInfo dict)
        {
            //辞書リストに辞書を追加する
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.dictView.Add(dict));
        }

        private void Search(DictionaryInfo dict, string input)
        {
            //どの辞書も選ばれていなかったら一番上の辞書で検索する。辞書が存在しなければ戻る。
            if (dict == null)
            {
                if (dictList.Items.Count > 0) dict = (DictionaryInfo)dictList.Items.First();
                else return;
            }

            var words = input.Split(" ");
            string separator;

            //separatorが存在しなければホワイトスペースで代用する
            if (string.IsNullOrEmpty(dict.Separator))
            {
                separator = " ";
            }
            else
            {
                separator = dict.Separator;
            }
            var target = string.Join(separator, words);
            var final = dict.BaseUri + target;
            webView.Navigate(new Uri(final));
        }

        //登録された辞書サイト本体のホスト以外へのアクセス（広告など）かどうかを判定する
        private bool IsAllowedUri(Uri uri)
        {
            var dictUri =
                from d in dictView
                select new Uri(d.BaseUri);

            var dictUrlHost =
                from u in dictUri
                select u.Host;

            if (dictUrlHost.Contains(uri.Host))
            {
                return true;
            }

            return false;
        }

        #endregion

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var json = JsonConvert.SerializeObject(dictView);
            var localSettings = ApplicationData.Current.LocalSettings;

            localSettings.Values["dictionaries"] = json;
        }   

        private void DictList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Search((DictionaryInfo)dictList.SelectedItem, searchBox.Text);
        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Search((DictionaryInfo)dictList.SelectedItem, searchBox.Text);
            }
        }

        private void DictList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            DictListContextMenu.ShowAt(listView, e.GetPosition(listView));
            RightClickedItem = ((FrameworkElement)e.OriginalSource).DataContext;
        }    

        async private void AddDictionary_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;

            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(AddDictionaryPage), this);
                Window.Current.Content = frame;

                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });

            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId, ViewSizePreference.Default);   
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            dictView.Remove((DictionaryInfo)RightClickedItem);
        }

        private async void Property_Click(object sender, RoutedEventArgs e)
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            var versionString = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            var content = $@"{package.DisplayName}
{versionString}

© {package.PublisherDisplayName}
Released under MIT License

Icons made by Freepik (www.freepik.com) from Flaticon (www.flaticon.com/)";

            var propertyDialog = new ContentDialog
            {
                Title = "Property",
                Content = content,
                CloseButtonText = "Ok"
            };

            await propertyDialog.ShowAsync();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(webView.CanGoBack) webView.GoBack();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if(webView.CanGoForward) webView.GoForward();
        }

        private void WebView_FrameNavigationStarting(object sender, WebViewNavigationStartingEventArgs args)
        {
            if (IsAdsDisabled == false) return;

            // Cancel navigation if URL is not allowed.
            if (!IsAllowedUri(args.Uri))
                args.Cancel = true;
        }

        private void DisableAds_Click(object sender, RoutedEventArgs e)
        {
            IsAdsDisabled = (bool)disableAdsToggle.IsChecked;
        }
    }
}
