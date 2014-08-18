# universal Windows apps - 共有

ユニバーサルWindowsアプリの`[共有]`機能のサンプルです。

## 詳細


### 共有を呼び出す

任意の操作から共有を呼び出します。
```csharp
DataTransferManager.ShowShareUI();
```

### 共有を送信する

共有操作の開始を購読します。

```csharp
// アプリケーション内で1度のみ購読する、または、画面毎などで適切に購読と解除を行う
DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
```

共有を処理します。

```csharp
void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
{
    // 必須
    e.Request.Data.Properties.Title = "共有するデータのタイトル";
    e.Request.Data.Properties.Description = "共有するデータの説明";

    // 以下、適宜

    // 共有する文字列
    e.Request.Data.SetText("共有するテキスト！");
}
```

ファイルなどの時間や処理を伴う共有はデリゲートを通して共有します。

```csharp
void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
{
    // 必須
    e.Request.Data.Properties.Title = "共有するデータのタイトル";
    e.Request.Data.Properties.Description = "共有するデータの説明";

    // 以下、適宜

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
        var files = await ...

        // 共有するファイル
        request.SetData(files);
    }
    finally
    {
        // 非同期処理の終了
        deferral.Complete();
    }
}
```

### 共有を受信する

共有によるアプリケーションのアクティブ化を処理します。

```csharp
// App.xaml.cs

protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
{
    var rootFrame = new Frame();
    rootFrame.Navigate(typeof(HogePage), args.ShareOperation);
    Window.Current.Content = rootFrame;
    Window.Current.Activate();
}
```

共有されたデータを処理します。

```csharp
// HogePage.xaml.cs

protected async override void OnNavigatedTo(NavigationEventArgs e)
{
    var shareOperation = e.Parameter as ShareOperation;
    if (shareOperation == null)
    {
        return;
    }

    // 共有されたデータのタイトルと説明
    var receivedDataTitle = shareOperation.Data.Properties.Title;
    var receivedDataDescription = shareOperation.Data.Properties.Description;

    // StorageItems の共有を確認
    if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
    {
        // 共有されたデータの取得
        var storageItems= await shareOperation.Data.GetStorageItemsAsync();

        // 以下、適宜
    }
}
```

## 関連情報

各機能の詳細はブログにまとめています。

1. [共有を呼び出す](http://katsuyuzu.hatenablog.jp/entry/2014/07/31/001537)
1. [共有を送信する](http://katsuyuzu.hatenablog.jp/entry/2014/08/01/005625)
1. [共有を受信する](http://katsuyuzu.hatenablog.jp/entry/2014/08/07/080000)

## License

under [MIT License](http://opensource.org/licenses/MIT)
