using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace ShareSample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ReceivedPage : Page
    {
        public ReceivedPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var shareOperation = e.Parameter as ShareOperation;
            if (shareOperation == null)
            {
                return;
            }

            // 共有されたデータのタイトルと説明
            this.receivedDataTitle.Text = shareOperation.Data.Properties.Title;
            this.receivedDataDescription.Text = shareOperation.Data.Properties.Description;

            // StorageItems の共有を確認
            if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                // 共有されたデータの取得
                var storageItems= await shareOperation.Data.GetStorageItemsAsync();

                // 以下、適宜

                // ファイルの抽出
                var file = storageItems
                    .OfType<StorageFile>()
                    .First();

                // 画像の読み込み
                using (var stream = await file.OpenReadAsync())
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(stream);
                    this.receivedDataImage.Source = bitmapImage;
                }
            }
        }
    }
}
