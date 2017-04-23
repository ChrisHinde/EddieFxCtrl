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

namespace EddieFxCtrl
{
    public class CenterOnPoint
    {
        public static readonly DependencyProperty CenterPointProperty =
           DependencyProperty.RegisterAttached("CenterPoint", typeof(Point), typeof(CenterOnPoint),
           new PropertyMetadata(default(Point), OnPointChanged));

        public static void SetCenterPoint(UIElement element, Point value)
        {
            element.SetValue(CenterPointProperty, value);
        }

        public static Point GetCenterPoint(UIElement element)
        {
            return (Point)element.GetValue(CenterPointProperty);
        }

        private static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)d;
            element.SizeChanged -= OnSizeChanged;
            element.SizeChanged += OnSizeChanged;
            var newPoint = (Point)e.NewValue;
            element.SetValue(Canvas.LeftProperty, newPoint.X - (element.ActualWidth / 2));
            element.SetValue(Canvas.TopProperty, newPoint.Y - (element.ActualHeight / 2));
        }

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var newPoint = GetCenterPoint(element);
            element.SetValue(Canvas.LeftProperty, newPoint.X - (e.NewSize.Width / 2));
            element.SetValue(Canvas.TopProperty, newPoint.Y - (e.NewSize.Height / 2));
        }
    }

    /// <summary>
    /// Interaction logic for efcChannelPreviewCtrl.xaml
    /// </summary>
    public partial class EfcChannelPreviewCtrl : UserControl
    {
        protected UInt16 _ChannelCount = 128;
        protected UInt16 _ChannelStart = 1;
        protected SolidColorBrush _FillEmptyBrush;
        protected SolidColorBrush _FillUsedBrush;
        protected SolidColorBrush _FillDisabledBrush;
        protected SolidColorBrush _BorderBrush;
        protected SolidColorBrush _BorderPatchedBrush;
        protected SolidColorBrush _TextBrush;
        protected SolidColorBrush _TextDisabledBrush;
        protected double _FontSizeA = 12;
        protected double _FontSizeB = 15;

        public UInt16 ChannelCount
        {
            get { return _ChannelCount; }
            set { _ChannelCount = value; Draw(); }
        }
        public UInt16 ChannelStart
        {
            get { return _ChannelStart; }
            set { _ChannelStart = value; Draw(); }
        }

        public EfcChannelPreviewCtrl()
        {
            InitializeComponent();


            _FillEmptyBrush = new SolidColorBrush(Colors.WhiteSmoke);
            _FillUsedBrush = new SolidColorBrush(Colors.LightSkyBlue);
            _FillDisabledBrush = new SolidColorBrush(Colors.LightSalmon);
            _BorderBrush = new SolidColorBrush(Colors.DarkBlue);
            _BorderPatchedBrush = new SolidColorBrush(Colors.Orange);
            _TextBrush = new SolidColorBrush(Colors.Black);
            _TextDisabledBrush = new SolidColorBrush(Colors.Gray);
        }
         
        public void Draw()
        {
            ChannelsCanvas.Children.Clear();

            double rowSize = 8;// Math.Sqrt(_ChannelCount);
            double colSize = _ChannelCount / rowSize;
            double width = this.ActualWidth / rowSize;
            double height = this.ActualHeight / colSize;
            int channel = _ChannelStart;

            Canvas canvas;

            for ( int y=0; y < colSize; y++ )
            {
                if (channel >= (_ChannelStart + _ChannelCount))
                    break;

                for ( int x=0; x < rowSize; x++ )
                {
                    if (channel >= (_ChannelStart + _ChannelCount))
                        break;

                    canvas = MakeChannelBox(channel, x, y, width, height );
                    ChannelsCanvas.Children.Add(canvas);

                    channel++;
                }
            }
        }

        protected Canvas MakeChannelBox( int channel, int x, int y,  double width, double height )
        {
            Canvas canvas = new Canvas();
            Rectangle rect = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = _BorderBrush,
                Fill = _FillEmptyBrush
            };
            Canvas.SetTop(rect, y * height);
            Canvas.SetLeft(rect, x * width);

           /* if (channel < 6)
                rect.Fill = _FillUsedBrush;
            /*else if (x % 2 != y % 2)
                rect.Fill = _FillDisabledBrush;*/

          /*  if (channel == 32)
                rect.Stroke = _BorderPatchedBrush;*/

            canvas.Children.Add(rect);


            TextBlock textBlock = new TextBlock()
            {
                Text = channel.ToString(),
                Foreground = _TextBrush,
                FontSize = _FontSizeA,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.SetValue(CenterOnPoint.CenterPointProperty, new Point(x*width + width / 2, y * height + height / 4) );
            
            /*if (x % 2 != y % 2)
                textBlock.Foreground = _TextDisabledBrush;*/

            canvas.Children.Add(textBlock);

            textBlock = new TextBlock()
            {
                Text = "0%",
                Foreground = _TextBrush,
                FontWeight = FontWeight.FromOpenTypeWeight(600),
                FontSize = _FontSizeB,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.SetValue(CenterOnPoint.CenterPointProperty, new Point(x * width + width / 2, y * height + (3 * height / 4) ));

            canvas.Children.Add(textBlock);

            return canvas;
        }

        private void ChannelsCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        private void ChannelsCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }
    }
}
