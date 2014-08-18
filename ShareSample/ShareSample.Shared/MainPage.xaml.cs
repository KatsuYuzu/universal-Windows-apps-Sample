using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            // アプリケーション内で1度のみ購読する、または、画面毎などで適切に購読と解除を行う
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
            // 必須
            e.Request.Data.Properties.Title = "共有するデータのタイトル";
            e.Request.Data.Properties.Description = "共有するデータの説明";

            // 以下、適宜

            // 共有する文字列
            e.Request.Data.SetText("共有するテキスト！");

            // 共有するファイルの拡張子
            e.Request.Data.Properties.FileTypes.Add(".jpg");

            // 共有するファイルを処理するデリゲート
            e.Request.Data.SetDataProvider(
                StandardDataFormats.StorageItems,
                this.OnDeferredImageRequestedHandler);
        }

        async void OnDeferredImageRequestedHandler(DataProviderRequest request)
        {
            // 非同期処理の開始
            var deferral = request.GetDeferral();
            try
            {
                // ファイルの非同期操作
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
                // 共有するファイル
                request.SetData(files);
            }
            finally
            {
                // 非同期処理の終了
                deferral.Complete();
            }
        }
    }
}
