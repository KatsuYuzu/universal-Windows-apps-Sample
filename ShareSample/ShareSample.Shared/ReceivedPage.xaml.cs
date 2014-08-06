using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

            this.receivedDataTitle.Text = shareOperation.Data.Properties.Title;
            this.receivedDataDescription.Text = shareOperation.Data.Properties.Description;

            // StorageItems の共有の確認
            if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                // 共有されたアイテムの取得
                IReadOnlyList<IStorageItem> storageItems= await shareOperation.Data.GetStorageItemsAsync();

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
