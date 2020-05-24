using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Re_TTSCat.Windows
{
    /// <summary>
    /// IconTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class IconTextBlock : UserControl
    {
        public IconTextBlock()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconGlyphProperty = DependencyProperty.Register("IconGlyph", typeof(string), typeof(IconTextBlock), new PropertyMetadata(new PropertyChangedCallback(OnIconGlyphChanged)));
        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register("IconForeground", typeof(Brush), typeof(IconTextBlock), new PropertyMetadata(new PropertyChangedCallback(OnIconForegroundChanged)));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IconTextBlock), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));
        public static new readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(IconTextBlock), new PropertyMetadata(new PropertyChangedCallback(OnFontSizeChanged)));

        private static void OnIconGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconTextBlock)d).OnIconGlyphChanged(e);
        private static void OnIconForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconTextBlock)d).OnIconForegroundChanged(e);
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconTextBlock)d).OnTextChanged(e);
        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconTextBlock)d).OnFontSizeChanged(e);
        private void OnIconGlyphChanged(DependencyPropertyChangedEventArgs e) => tbIconGlyph.Text = (string)e.NewValue;
        private void OnIconForegroundChanged(DependencyPropertyChangedEventArgs e) => tbIconGlyph.Foreground = (Brush)e.NewValue;
        private void OnTextChanged(DependencyPropertyChangedEventArgs e) => tbText.Text = (string)e.NewValue;
        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            tbIconGlyph.FontSize = (double)e.NewValue;
            tbText.FontSize = (double)e.NewValue;
        }

        public string IconGlyph
        {
            get => (string)GetValue(IconGlyphProperty);
            set => SetValue(IconGlyphProperty, value);
        }

        public Brush IconForeground
        {
            get => (Brush)GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
    }
}
