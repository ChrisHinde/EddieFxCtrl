using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using EddieFxCtrl;
using EddieFxCtrl.Classes;

namespace EddieFxCtrl.Controls
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

        private EfcMainWindow _MainWin;

        protected UInt16 _ChannelCount = 128;
        protected UInt16 _ChannelStart = 1;
        protected byte _Universe = 1;
        protected SolidColorBrush _FillEmptyBrush;
        protected SolidColorBrush _FillUsedBrush;
        protected SolidColorBrush _FillDisabledBrush;
        protected SolidColorBrush _FillErrorBrush;
        protected SolidColorBrush _BorderBrush;
        protected SolidColorBrush _BorderPatchedBrush;
        protected SolidColorBrush _TextBrush;
        protected SolidColorBrush _TextDisabledBrush;
        protected double _FontSizeA = 12;
        protected double _FontSizeB = 15;
        protected bool _Paused = false;
        protected bool _DisplayPercentage = true;

        public UInt16 ChannelCount
        {
            get { return _ChannelCount; }
            set { _ChannelCount = value; Draw(); }
        }
        public UInt16 ChannelStart
        {
            get { return _ChannelStart; }
            set
            {
                if (value > 512)
                {
                    throw new Exception("The start channel value is too high (>512)! This shouldn't happen!");
                }
                _ChannelStart = value;
                Draw();
            }
        }
        public byte Universe
        {
            get { return _Universe; }
            set { _Universe = value; }
        }

        public bool DisplayPercantage { get => _DisplayPercentage; set => _DisplayPercentage = value; }

        public bool IsPaused { get => _Paused; set => _Paused = value;  }
        public void Pause() { _Paused = true; }
        public void Unpause() { _Paused = false; }
        
        public EfcMainWindow MainWindow
        {
            set {
                _MainWin = value;
                _MainWin.OnUpdate += _OnUpdate;
            }
        }

        public EfcChannelPreviewCtrl()
        {
            InitializeComponent();

            _FillEmptyBrush = new SolidColorBrush(Colors.WhiteSmoke);
            _FillUsedBrush = new SolidColorBrush(Colors.LightSkyBlue);
            _FillDisabledBrush = new SolidColorBrush(Colors.LightSalmon);
            _FillErrorBrush = new SolidColorBrush(Colors.Red);
            _BorderBrush = new SolidColorBrush(Colors.DarkBlue);
            _BorderPatchedBrush = new SolidColorBrush(Colors.Orange);
            _TextBrush = new SolidColorBrush(Colors.Black);
            _TextDisabledBrush = new SolidColorBrush(Colors.Gray);
        }

        private void _OnUpdate(object sender, EfcUpdateEventArgs e)
        {
            if (_Paused)
                return;

            switch (e.Type)
            {
                case EfcEventType.ChannelChanged:
                    if ( ((EfcChannelChangedEventArgs)e.Args).Channel > 0 &&
                            !InRange(((EfcChannelChangedEventArgs)e.Args).Channel))
                        return;
                    break;
                case EfcEventType.PatchChanged:
                  //  if (!InRange(((EfcPatchChangedEventArgs)e.Args).Patch.Channel))
                        return;
                    break;
                case EfcEventType.SoftPatchChanged:
                    if ( !InRange(((EfcSoftPatchChangedEventArgs)e.Args).ChannelIn) &&
                         !InRange(((EfcSoftPatchChangedEventArgs)e.Args).ChannelOut))
                        return;
                    break;
            }

            //var thread = new Thread(Draw);
            //thread.Start();
            Draw();
        }

        private bool InRange(ushort channel)
        {
            return (channel >= _ChannelStart) && (channel < _ChannelStart + _ChannelCount);
        }

        public void Draw()
        {
            ChannelsCanvas.Children.Clear();

            double rowSize = 8;// Math.Sqrt(_ChannelCount);
            double colSize = _ChannelCount / rowSize;
            double width = this.ActualWidth / rowSize;
            double height = this.ActualHeight / colSize;
            UInt16 channel = _ChannelStart;

            Canvas canvas;

            for ( int y=0; y < colSize; y++ )
            {
                if (channel >= (_ChannelStart + _ChannelCount))
                    break;

                for ( int x=0; x < rowSize; x++ )
                {
                    if (channel >= (_ChannelStart + _ChannelCount))
                        break;

                    //MakeChannelBox(ChannelsCanvas,channel, x, y, width, height);
                    canvas = MakeChannelBox(channel, x, y, width, height);
                    ChannelsCanvas.Children.Add(canvas);

                    channel++;
                }
            }
        }

        protected Canvas MakeChannelBox(UInt16 channel, int x, int y, double width, double height)
        {

            Brush foreground = _TextBrush;
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

            String valueStr = "0" + (_DisplayPercentage ? "%" : "");
            if (_MainWin != null)
            {
                EfcUniverse universe = _MainWin.CurrentShow.Universes[_Universe];
                EfcPatch patch = universe.GetPatch(channel, out bool isSoftPatch, out EfcSoftPatch softPatch);

                if (patch != null)
                {
                    rect.Fill = _FillUsedBrush;

                    if (!patch.Enabled)
                    {
                        foreground = _TextDisabledBrush;
                        rect.Fill = _FillDisabledBrush;
                    }
                }

                if (isSoftPatch)
                    rect.Stroke = _BorderPatchedBrush;

                if (_DisplayPercentage)
                    valueStr = ((int)Math.Round((universe.ChannelValues[channel] / 255.0) * 100)).ToString();
                else
                    valueStr = universe.ChannelValues[channel].ToString();
            }
            else
            {
                rect.Fill = _FillErrorBrush;
            }

            canvas.Children.Add(rect);

            TextBlock textBlock = new TextBlock()
            {
                Text = channel.ToString(),
                Foreground = foreground,
                FontSize = _FontSizeA,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.SetValue(CenterOnPoint.CenterPointProperty, new Point(x * width + width / 2, y * height + height / 4));


            canvas.Children.Add(textBlock);

            textBlock = new TextBlock()
            {
                Text = valueStr,
                Foreground = _TextBrush,
                FontWeight = FontWeight.FromOpenTypeWeight(600),
                FontSize = _FontSizeB,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.SetValue(CenterOnPoint.CenterPointProperty, new Point(x * width + width / 2, y * height + (3 * height / 4)));

            canvas.Children.Add(textBlock);

            return canvas;
        }
        protected void MakeChannelBox(Canvas canvas, UInt16 channel, int x, int y, double width, double height)
        {

            Brush foreground = _TextBrush;
            //Canvas canvas = new Canvas();
            Rectangle rect = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = _BorderBrush,
                Fill = _FillEmptyBrush
            };
            Canvas.SetTop(rect, y * height);
            Canvas.SetLeft(rect, x * width);

            String valueStr = "0" + (_DisplayPercentage ? "%" : "");
            if (_MainWin != null)
            {
                EfcUniverse universe = _MainWin.CurrentShow.Universes[_Universe];
                EfcPatch patch = universe.GetPatch(channel, out bool isSoftPatch, out EfcSoftPatch softPatch);

                if (patch != null)
                {
                    rect.Fill = _FillUsedBrush;

                    if (!patch.Enabled)
                    {
                        foreground = _TextDisabledBrush;
                        rect.Fill = _FillDisabledBrush;
                    }
                }

                if (isSoftPatch)
                    rect.Stroke = _BorderPatchedBrush;

                if (_DisplayPercentage)
                    valueStr = ((int)Math.Round((universe.ChannelValues[channel] / 255.0) * 100)).ToString();
                else
                    valueStr = universe.ChannelValues[channel].ToString();
            }
            else
            {
                rect.Fill = _FillErrorBrush;
            }

            canvas.Children.Add(rect);

            TextBlock textBlock = new TextBlock()
            {
                Text = channel.ToString(),
                Foreground = foreground,
                FontSize = _FontSizeA,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.SetValue(CenterOnPoint.CenterPointProperty, new Point(x * width + width / 2, y * height + height / 4));


            canvas.Children.Add(textBlock);

            textBlock = new TextBlock()
            {
                Text = valueStr,
                Foreground = _TextBrush,
                FontWeight = FontWeight.FromOpenTypeWeight(600),
                FontSize = _FontSizeB,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.SetValue(CenterOnPoint.CenterPointProperty, new Point(x * width + width / 2, y * height + (3 * height / 4)));

            canvas.Children.Add(textBlock);

          //  return canvas;
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
