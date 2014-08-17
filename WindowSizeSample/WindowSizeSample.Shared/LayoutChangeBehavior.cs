using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace WindowSizeSample
{
    /// <summary>
    /// ウィンドウ サイズに合わせてレイアウトを変更するビヘイビアー。
    /// </summary>
    [TypeConstraint(typeof(Page))]
    class LayoutChangeBehavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// 解決されたソース。
        /// </summary>
        private Page resolvedSource;

        /// <summary>
        /// 関連付けられているオブジェクトを取得します。
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get
            {
                return this.associatedObject;
            }
        }
        private DependencyObject associatedObject;

        #region DefaultLayoutState
        /// <summary>
        /// 通常のレイアウトをあらわす状態名を取得または設定します。
        /// </summary>
        [CustomPropertyValueEditor(CustomPropertyValueEditor.StateName)]
        public string DefaultLayoutState
        {
            get { return (string)GetValue(DefaultLayoutStateProperty); }
            set { SetValue(DefaultLayoutStateProperty, value); }
        }

        public static readonly DependencyProperty DefaultLayoutStateProperty =
            DependencyProperty.Register(
                "DefaultLayoutState",
                typeof(string),
                typeof(LayoutChangeBehavior),
                new PropertyMetadata(null));
        #endregion

        #region MinimalLayoutState
        /// <summary>
        /// 狭いの幅のレイアウトをあらわす状態名を取得または設定します。
        /// </summary>
        [CustomPropertyValueEditor(CustomPropertyValueEditor.StateName)]
        public string MinimalLayoutState
        {
            get { return (string)GetValue(MinimalLayoutStateProperty); }
            set { SetValue(MinimalLayoutStateProperty, value); }
        }

        public static readonly DependencyProperty MinimalLayoutStateProperty =
            DependencyProperty.Register(
                "MinimalLayoutState",
                typeof(string),
                typeof(LayoutChangeBehavior),
                new PropertyMetadata(null));
        #endregion

        #region PortraitLayoutState
        /// <summary>
        /// 縦長のレイアウトをあらわす状態名を取得または設定します。
        /// </summary>
        [CustomPropertyValueEditor(CustomPropertyValueEditor.StateName)]
        public string PortraitLayoutState
        {
            get { return (string)GetValue(PortraitLayoutStateProperty); }
            set { SetValue(PortraitLayoutStateProperty, value); }
        }

        public static readonly DependencyProperty PortraitLayoutStateProperty =
            DependencyProperty.Register(
                "PortraitLayoutState",
                typeof(string),
                typeof(LayoutChangeBehavior),
                new PropertyMetadata(null));
        #endregion

        /// <summary>
        /// 指定されたオブジェクトにアタッチします。
        /// </summary>
        /// <param name="associatedObject"></param>
        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == this.associatedObject || DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (this.associatedObject != null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        CannotAttachBehaviorMultipleTimesExceptionMessage,
                        new object[]
                        {
                            associatedObject,
                            this.associatedObject
                        }));
            }

            this.associatedObject = associatedObject;

            var page = associatedObject as Page;
            if (page == null)
            {
                throw new ArgumentException("associatedObject type is not a Page.");
            }

            this.resolvedSource = page;
            this.resolvedSource.Loaded += OnLoaded;
            Window.Current.SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// このインスタンスを関連オブジェクトからデタッチします。
        /// </summary>
        public void Detach()
        {
            this.resolvedSource.Loaded -= OnLoaded;
            Window.Current.SizeChanged -= OnSizeChanged;
            this.resolvedSource = null;

            this.associatedObject = null;
        }

        /// <summary>
        /// ページがロードされたときに呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var rect = Window.Current.Bounds;
            this.GoToState(rect.Width, rect.Height);
        }

        /// <summary>
        /// ウィンドウ サイズが変更されたときに呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.GoToState(e.Size.Width, e.Size.Height);
        }

        /// <summary>
        /// 状態を遷移させます。
        /// </summary>
        /// <param name="width">ウィンドウの幅。</param>
        /// <param name="height">ウインドウの高さ。</param>
        private void GoToState(double width, double height)
        {
            string stateName;

            if (width < 500)
            {
                stateName = this.MinimalLayoutState;
            }
            else if (width < height)
            {
                stateName = this.PortraitLayoutState;
            }
            else
            {
                stateName = this.DefaultLayoutState;
            }

            if (string.IsNullOrWhiteSpace(stateName))
            {
                stateName = this.DefaultLayoutState;
            }

            VisualStateManager.GoToState(this.resolvedSource, stateName, true);
        }

        #region ResourceHelper(ビヘイビアー SDK 既定のメッセージを取得するヘルパー)
        private static string CannotAttachBehaviorMultipleTimesExceptionMessage
        {
            get
            {
                return GetString("CannotAttachBehaviorMultipleTimesExceptionMessage");
            }
        }
        private static string GetString(string resourceName)
        {
            var loader = ResourceLoader.GetForCurrentView("Microsoft.Xaml.Interactions/Strings");
            return loader.GetString(resourceName);
        }
        #endregion
    }
}
