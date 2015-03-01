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
        public double _Min
        {
            get { return (double)GetValue(_MinProperty); }
            set { SetValue(_MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MinProperty =
            DependencyProperty.Register("_Min", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _Max
        {
            get { return (double)GetValue(_MaxProperty); }
            set { SetValue(_MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxProperty =
            DependencyProperty.Register("_Max", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

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
        
        public int _Iterations
        {
            get { return (int)GetValue(_IterationsProperty); }
            set { SetValue(_IterationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Iterations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _IterationsProperty =
            DependencyProperty.Register("_Iterations", typeof(int), typeof(FinalRender), new UIPropertyMetadata(1));

        public double _BoostPower
        {
            get { return (double)GetValue(_BoostPowerProperty); }
            set { SetValue(_BoostPowerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _BoostPower.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _BoostPowerProperty =
            DependencyProperty.Register("_BoostPower", typeof(double), typeof(FinalRender), new UIPropertyMetadata(10.0));

        public double _SourceWeight
        {
            get { return (double)GetValue(_SourceWeightProperty); }
            set { SetValue(_SourceWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _SourceWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _SourceWeightProperty =
            DependencyProperty.Register("_SourceWeight", typeof(double), typeof(FinalRender), new UIPropertyMetadata(0.9));

        public double _TargetWeight
        {
            get { return (double)GetValue(_TargetWeightProperty); }
            set { SetValue(_TargetWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _TargetWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _TargetWeightProperty =
            DependencyProperty.Register("_TargetWeight", typeof(double), typeof(FinalRender), new UIPropertyMetadata(0.1));

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
            _MaxValue = 1;
            _Factor = 1;
            _ToneFactor = 1.4;
            _GammaFactor = 0.7;
            _GammaContrast = 0.7;

            if (_Camera._AAEnabled)
                checkBox1.IsChecked = true;
            if (_Camera._DOFEnabled)
                checkBox2.IsChecked = true;
            if (_Scene._PathTracer)
                checkBox3.IsChecked = true;
            if (_Scene._Caustics)
                checkBox4.IsChecked = true;
            _SamplesPerPixel = _Camera._MinSamples;

            SetGaussian();

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
            UpdateKernel();
            _ImageRenderer.SetPostProcess(_Iterations, (float)_BoostPower, (float)_SourceWeight, (float)_TargetWeight);
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

        bool _RefreshingRender = false;

        void updateRender()
        {
            if (_ImageRenderer == null)
                return;
            if (_RefreshingRender == true)
                return;

            _RefreshingRender = true;

            if (radioButton2.IsChecked.Value)
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Exposure;
            else if (radioButton8.IsChecked.Value)
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Gamma;
            else if (radioButton9.IsChecked.Value)
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Tone;
            else
                _ImageRenderer._TransferType = ImageRenderer.Transfer.Ramp;

            _ImageRenderer._RampValue = _MaxValue;
            _ImageRenderer._ExposureFactor = _Factor;
            _ImageRenderer._ToneFactor = (float)_ToneFactor;
            _ImageRenderer._GammaFactor = (float)_GammaFactor;
            _ImageRenderer._GammaContrast = (float)_GammaContrast;

            UpdateKernel();
            _ImageRenderer.SetPostProcess(_Iterations, (float)_BoostPower, (float)_SourceWeight, (float)_TargetWeight);

            _ImageRenderer.TransferLatest(!_ImageRendering);

            Colour minColour = _ImageRenderer._MinColour;
            _Min = Math.Min(minColour._Red, Math.Min(minColour._Green, minColour._Blue));
            Colour maxColour = _ImageRenderer._MaxColour;
            _Max = Math.Max(maxColour._Red, Math.Max(maxColour._Green, maxColour._Blue));

            _RefreshingRender = false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _ImageRenderer.Stop(); // synchronous?
            _Timer.Stop();
            _ImageRendering = false;
            _ImageRenderer.TransferLatest(true);
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

        private void checkBox4_Checked(object sender, RoutedEventArgs e)
        {
            _Scene._Caustics = true;
        }

        private void checkBox4_Unchecked(object sender, RoutedEventArgs e)
        {
            _Scene._Caustics = false;
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

        bool _KernelUpdating = false;
        private void refreshRenderk(object sender, TextChangedEventArgs e)
        {
            if (!_ImageRendering && !_KernelUpdating)
            {
                updateRender();
            }
        }

        private void UpdateKernel()
        {
            float[] kernel = new float[25];
            int kidx = 0;
            float.TryParse(g11.Text, out kernel[kidx++]);
            float.TryParse(g12.Text, out kernel[kidx++]);
            float.TryParse(g13.Text, out kernel[kidx++]);
            float.TryParse(g14.Text, out kernel[kidx++]);
            float.TryParse(g15.Text, out kernel[kidx++]);
            float.TryParse(g21.Text, out kernel[kidx++]);
            float.TryParse(g22.Text, out kernel[kidx++]);
            float.TryParse(g23.Text, out kernel[kidx++]);
            float.TryParse(g24.Text, out kernel[kidx++]);
            float.TryParse(g25.Text, out kernel[kidx++]);
            float.TryParse(g31.Text, out kernel[kidx++]);
            float.TryParse(g32.Text, out kernel[kidx++]);
            float.TryParse(g33.Text, out kernel[kidx++]);
            float.TryParse(g34.Text, out kernel[kidx++]);
            float.TryParse(g35.Text, out kernel[kidx++]);
            float.TryParse(g41.Text, out kernel[kidx++]);
            float.TryParse(g42.Text, out kernel[kidx++]);
            float.TryParse(g43.Text, out kernel[kidx++]);
            float.TryParse(g44.Text, out kernel[kidx++]);
            float.TryParse(g45.Text, out kernel[kidx++]);
            float.TryParse(g51.Text, out kernel[kidx++]);
            float.TryParse(g52.Text, out kernel[kidx++]);
            float.TryParse(g53.Text, out kernel[kidx++]);
            float.TryParse(g54.Text, out kernel[kidx++]);
            float.TryParse(g55.Text, out kernel[kidx++]);

            if (_ImageRenderer != null)
                _ImageRenderer.SetKernel(kernel);
        }

        private void SetGaussian()
        {
            _KernelUpdating = true;
            g11.Text = "1"; g12.Text = "4"; g13.Text = "7"; g14.Text = "4"; g15.Text = "1";
            g21.Text = "4"; g22.Text = "16"; g23.Text = "26"; g24.Text = "16"; g25.Text = "4";
            g31.Text = "7"; g32.Text = "26"; g33.Text = "41"; g34.Text = "26"; g35.Text = "7";
            g41.Text = "4"; g42.Text = "16"; g43.Text = "26"; g44.Text = "16"; g45.Text = "4";
            g51.Text = "1"; g52.Text = "4"; g53.Text = "7"; g54.Text = "4"; g55.Text = "1";
            _KernelUpdating = false;

            UpdateKernel();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetGaussian();

            refreshRender(sender, e);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _KernelUpdating = true;
            g11.Text = "0"; g12.Text = "0"; g13.Text = "0"; g14.Text = "0"; g15.Text = "0";
            g21.Text = "0"; g22.Text = "0"; g23.Text = "0"; g24.Text = "0"; g25.Text = "0";
            g31.Text = "1"; g32.Text = "4"; g33.Text = "7"; g34.Text = "4"; g35.Text = "1";
            g41.Text = "0"; g42.Text = "0"; g43.Text = "0"; g44.Text = "0"; g45.Text = "0";
            g51.Text = "0"; g52.Text = "0"; g53.Text = "0"; g54.Text = "0"; g55.Text = "0";
            _KernelUpdating = false;

            UpdateKernel();

            refreshRender(sender, e);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _KernelUpdating = true;
            g11.Text = "1"; g12.Text = "0"; g13.Text = "0"; g14.Text = "0"; g15.Text = "1";
            g21.Text = "0"; g22.Text = "4"; g23.Text = "0"; g24.Text = "4"; g25.Text = "0";
            g31.Text = "0"; g32.Text = "0"; g33.Text = "7"; g34.Text = "0"; g35.Text = "0";
            g41.Text = "0"; g42.Text = "4"; g43.Text = "0"; g44.Text = "4"; g45.Text = "0";
            g51.Text = "1"; g52.Text = "0"; g53.Text = "0"; g54.Text = "0"; g55.Text = "1";
            _KernelUpdating = false;

            UpdateKernel();

            refreshRender(sender, e);
        }
    }
}
