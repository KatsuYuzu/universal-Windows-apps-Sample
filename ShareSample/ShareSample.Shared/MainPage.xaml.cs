using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ShareSample
{
    /// <summary>
    /// Frame 内へナビゲートするために利用する空欄ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        /// <summary>
        /// Page がアンロードされて親 Frame の現在のソースではなくなった直後に呼び出されます。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }

        /// <summary>
        /// 「共有を表示」ボタンをクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowShareButton_OnClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// 現在のウィンドウで共有操作が開始されたときに呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = "データのタイトル";
            e.Request.Data.Properties.Description = "データの説明";
            e.Request.Data.SetText("共有するテキスト！");

            // 共有データの拡張子
            e.Request.Data.Properties.FileTypes.Add(".jpg");

            // デリゲートの共有
            e.Request.Data.SetDataProvider(
                StandardDataFormats.StorageItems,
                this.OnDeferredImageRequestedHandler);
        }

        async void OnDeferredImageRequestedHandler(DataProviderRequest request)
        {
            // 非同期処理の開始（非同期の場合に必要）
            var deferral = request.GetDeferral();
            try
            {
#if WINDOWS_APP
                // 適当なファイルを用意
                var files = (await KnownFolders.PicturesLibrary.GetFilesAsync())
                    .Where(x => Path.GetExtension(x.Name) == ".jpg")
                    .Take(1);
#else
                // 適当なファイルを用意
                var files = (await KnownFolders.CameraRoll.GetFilesAsync())
                    .Where(x => Path.GetExtension(x.Name) == ".jpg")
                    .Take(1);
#endif
                // 共有データをセット
                request.SetData(files);
            }
            finally
            {
                // 非同期処理の終了（非同期の場合に必要）
                deferral.Complete();
            }
        }
    }
}
