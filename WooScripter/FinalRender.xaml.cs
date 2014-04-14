using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;

namespace WooScripter
{
    /// <summary>
    /// Interaction logic for FinalRender.xaml
    /// </summary>
    public partial class FinalRender : Window
    {
        public double _MinR
        {
            get { return (double)GetValue(_MinRProperty); }
            set { SetValue(_MinRProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MinR.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MinRProperty =
            DependencyProperty.Register("_MinR", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MinG
        {
            get { return (double)GetValue(_MinGProperty); }
            set { SetValue(_MinGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MinG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MinGProperty =
            DependencyProperty.Register("_MinG", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MinB
        {
            get { return (double)GetValue(_MinBProperty); }
            set { SetValue(_MinBProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MinB.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MinBProperty =
            DependencyProperty.Register("_MinB", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MaxR
        {
            get { return (double)GetValue(_MaxRProperty); }
            set { SetValue(_MaxRProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MaxR.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxRProperty =
            DependencyProperty.Register("_MaxR", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MaxG
        {
            get { return (double)GetValue(_MaxGProperty); }
            set { SetValue(_MaxGProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MaxG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxGProperty =
            DependencyProperty.Register("_MaxG", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MaxB
        {
            get { return (double)GetValue(_MaxBProperty); }
            set { SetValue(_MaxBProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MaxB.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxBProperty =
            DependencyProperty.Register("_MaxB", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MaxValue
        {
            get { return (double)GetValue(_MaxValueProperty); }
            set { SetValue(_MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxValueProperty =
            DependencyProperty.Register("_MaxValue", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _Factor
        {
            get { return (double)GetValue(_FactorProperty); }
            set { SetValue(_FactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Factor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _FactorProperty =
            DependencyProperty.Register("_Factor", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _ToneFactor
        {
            get { return (double)GetValue(_ToneFactorProperty); }
            set { SetValue(_ToneFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ToneFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ToneFactorProperty =
            DependencyProperty.Register("_ToneFactor", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _GammaFactor
        {
            get { return (double)GetValue(_GammaFactorProperty); }
            set { SetValue(_GammaFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _GammaFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GammaFactorProperty =
            DependencyProperty.Register("_GammaFactor", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _GammaContrast
        {
            get { return (double)GetValue(_GammaContrastProperty); }
            set { SetValue(_GammaContrastProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _GammaContrast.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GammaContrastProperty =
            DependencyProperty.Register("_GammaContrast", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public int _ImageWidth
        {
            get { return (int)GetValue(_ImageWidthProperty); }
            set { SetValue(_ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ImageWidthProperty =
            DependencyProperty.Register("_ImageWidth", typeof(int), typeof(FinalRender), new UIPropertyMetadata(960));

        public int _ImageHeight
        {
            get { return (int)GetValue(_ImageHeightProperty); }
            set { SetValue(_ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ImageHeightProperty =
            DependencyProperty.Register("_ImageHeight", typeof(int), typeof(FinalRender), new UIPropertyMetadata(480));

        public int _SamplesPerPixel
        {
            get { return (int)GetValue(_SamplesPerPixelProperty); }
            set { SetValue(_SamplesPerPixelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _SamplesPerPixel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _SamplesPerPixelProperty =
            DependencyProperty.Register("_SamplesPerPixel", typeof(int), typeof(FinalRender), new UIPropertyMetadata(0));

        
        string _XML = @"";
        Scene _Scene;
        Camera _Camera;

        private void BuildXML()
        {
            _XML = @"<VIEWPORT width=" + image1.Width + @" height=" + image1.Height + @"/>";
            _XML += _Camera.CreateElement().ToString();
            _XML += _Scene.CreateElement(false).ToString();
        }

        public FinalRender(ref Scene scene, ref Camera camera)
        {
            _Scene = scene;
            _Camera = camera;

            DataContext = this;
            InitializeComponent();
            _MaxValue = 3;
            _Factor = 1;
            _ToneFactor = 1;
            _GammaFactor = 1;
            _GammaContrast = 1;

            if (_Camera._AAEnabled)
                checkBox1.IsChecked = true;
            if (_Camera._DOFEnabled)
                checkBox2.IsChecked = true;
            if (_Scene._PathTracer)
                checkBox3.IsChecked = true;
            _SamplesPerPixel = _Camera._MinSamples;

            BuildXML();
        }

        ImageRenderer _ImageRenderer;
        bool _ImageRendering;
        DispatcherTimer _Timer;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _Camera._MinSamples = _Camera._MaxSamples = _SamplesPerPixel;
            BuildXML();

            int width = 960;
            int height = 540;
            if (radioButton3.IsChecked.HasValue && radioButton3.IsChecked.Value)
            {
                width = 480;
                height = 270;
            }
            if (radioButton5.IsChecked.HasValue && radioButton5.IsChecked.Value)
            {
                width = 1920;
                height = 1080;
            }
            if (radioButton6.IsChecked.HasValue && radioButton6.IsChecked.Value)
            {
                width = _ImageWidth;
                height = _ImageHeight;
            }
            _ImageRenderer = new ImageRenderer(image1, _XML, width, height, true);
            _ImageRendering = true;
            _ImageRenderer.Render();

            // set up animation thread for the camera movement
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(100);
            _Timer.Tick += this.timer_Tick;
            _Timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            updateRender();
        }

        void updateRender()
        {
            if (_ImageRenderer == null)
                return;
            if (radioButton2.IsChecked.Value)
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Exposure;
            else if (radioButton8.IsChecked.Value)
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Gamma;
            else if (radioButton9.IsChecked.Value)
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Tone;
            else
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Ramp;

            _ImageRenderer._MaxValue = _MaxValue;
            _ImageRenderer._ExposureFactor = _Factor;
            _ImageRenderer._ToneFactor = (float)_ToneFactor;
            _ImageRenderer._GammaFactor = (float)_GammaFactor;
            _ImageRenderer._GammaContrast = (float)_GammaContrast;

            _ImageRenderer.TransferLatest();

            Colour minColour = _ImageRenderer._MinColour;
            _MinR = minColour._Red; _MinG = minColour._Green; _MinB = minColour._Blue;
            Colour maxColour = _ImageRenderer._MaxColour;
            _MaxR = maxColour._Red; _MaxG = maxColour._Green; _MaxB = maxColour._Blue;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _ImageRenderer.Stop();
            _Timer.Stop();
            _ImageRendering = false;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            _Camera._AAEnabled = true;
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            _Camera._AAEnabled = false;
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            _Camera._DOFEnabled = true;
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            _Camera._DOFEnabled = false;
        }

        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            _Scene._PathTracer = true;
        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            _Scene._PathTracer = false;
        }

        private void radioButton6_Checked(object sender, RoutedEventArgs e)
        {
            textBox12.IsEnabled = true;
            textBox13.IsEnabled = true;
        }

        private void radioButton6_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox12.IsEnabled = false;
            textBox13.IsEnabled = false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            _ImageRenderer.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _ImageRenderer.Stop();
            _Timer.Stop();
        }

        private void refreshRender(object sender, RoutedEventArgs e)
        {
            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void refreshRender(object sender, TextChangedEventArgs e)
        {
            if (!_ImageRendering)
            {
                updateRender();
            }
        }
    }
}
