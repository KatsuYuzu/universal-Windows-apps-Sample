# universal Windows apps - さまざまなウィンドウ サイズに対応する

ユニバーサルWindowsアプリのさまざまなウィンドウ サイズに対応するサンプルです。

## 詳細

### さまざまなウィンドウ サイズに適したレイアウト

必要になるレイアウトは3つです。

1. 通常の横長のレイアウト
1. 画面を分割した時や端末を縦に持った時の縦長のレイアウト
1. マニフェストで最小幅320pxを指定した時の狭い幅のレイアウト

ウィンドウ サイズが変化した時に、現在の幅に合わせて画面のレイアウトを切り替えます。ここでは、レイアウトの切り替えは`VisualStateManager`を使うこととします。

```xml
<Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">

    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="LayoutStateGroup">
            <VisualState x:Name="LandscapeState" />
            <VisualState x:Name="MinimalState">
                <Storyboard>
                    <ObjectAnimationUsingKeyFrames Storyboard.TargetProperty="(TextBlock.Text)"
                                                   Storyboard.TargetName="textBlock">
                        <DiscreteObjectKeyFrame KeyTime="0"
                                                Value="幅の狭いレイアウト" />
                    </ObjectAnimationUsingKeyFrames>
                </Storyboard>
            </VisualState>
            <VisualState x:Name="PortraitState">
                <Storyboard>
                    <ObjectAnimationUsingKeyFrames Storyboard.TargetProperty="(TextBlock.Text)"
                                                   Storyboard.TargetName="textBlock">
                        <DiscreteObjectKeyFrame KeyTime="0"
                                                Value="縦長のレイアウト" />
                    </ObjectAnimationUsingKeyFrames>
                </Storyboard>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
    
    <TextBlock x:Name="textBlock"
               Style="{StaticResource HeaderTextBlockStyle}"
               Text="横長のレイアウト"
               Margin="10"
               VerticalAlignment="Center"
               HorizontalAlignment="Center" />
</Grid>
```

### ウィンドウ サイズの変化を購読する

ウィンドウ サイズの変化を購読します。
```csharp
// VisualState を切り替えるために、購読は各ページで行います

Window.Current.SizeChanged += OnSizeChanged;
```

```csharp
void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
{
    this.GoToState(e.Size.Width, e.Size.Height);
}
```

また、ページのロード時はウィンドウ サイズの変化が発生しないため、ロード時のレイアウトを切り替えるにはページのロードも購読します。

```csharp
page.Loaded += OnLoaded;
```

```csharp
void OnLoaded(object sender, RoutedEventArgs e)
{
    var rect = Window.Current.Bounds;
    this.GoToState(rect.Width, rect.Height);
}
```

### ウィンドウ サイズによってレイアウトを切り替える

```csharp
void GoToState(double width, double height)
{
    string stateName;

    if (width < 500)
    {
        // Windows PhoneアプリではWindowsストアアプリに比べて幅が狭くなるので、
        // この対応をしておくことで2つのレイアウトで済みます。

        // 狭い幅のレイアウトを指定していない場合は、縦長のレイアウトとする。
        stateName = this.MinimalLayoutState ?? this.PortraitLayoutState;
    }
    else if (width < height)
    {
        stateName = this.PortraitLayoutState;
    }
    else
    {
        stateName = this.LandscapeLayoutState;
    }

    VisualStateManager.GoToState(this.resolvedSource, stateName, true);
}
```

### ビヘイビアー化する

これらの処理はウィンドウ サイズに対応するすべてのページに必要になるため、ビヘイビアー化して再利用します。

ビヘイビアーを作成するには、`DependencyObject`と`IBehavior`の2つを継承して、`IBehavior.Attach/Detach`で購読と解除を行います。その他に、レイアウトを切り替えるための状態名を表すプロパティを作成します。

```csharp
/// <summary>
/// 横長のレイアウトをあらわす状態名を取得または設定します。
/// </summary>
[CustomPropertyValueEditor(CustomPropertyValueEditor.StateName)]
public string LandscapeLayoutState
{
    get { return (string)GetValue(LandscapeLayoutStateProperty); }
    set { SetValue(LandscapeLayoutStateProperty, value); }
}

public static readonly DependencyProperty LandscapeLayoutStateProperty =
    DependencyProperty.Register(
        "LandscapeLayoutState",
        typeof(string),
        typeof(LayoutChangeBehavior),
        new PropertyMetadata(null));
```

ビヘイビアー化することで、ページ毎の処理は不要になり、簡単にさまざまなウィンドウ サイズに対応することができます。

```xml
<Interactivity:Interaction.Behaviors>
    <local:LayoutChangeBehavior LandscapeLayoutState="LandscapeState"
                                MinimalLayoutState="MinimalState"
                                PortraitLayoutState="PortraitState" />
</Interactivity:Interaction.Behaviors>
```


## 関連情報
詳細はブログにまとめています。

1. さまざまなウィンドウ サイズに対応する（未投稿）
1. [独自のビヘイビアーを作成する](http://katsuyuzu.hatenablog.jp/entry/2013/12/13/080723)

## License

under [MIT License](http://opensource.org/licenses/MIT)
