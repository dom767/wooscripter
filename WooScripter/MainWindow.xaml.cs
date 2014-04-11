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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WooScripter.Objects.WooScript;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using WooScripter.Objects;

namespace WooScripter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Camera _Camera;
        WooScript _BackgroundScript;
        WooScript _SceneScript;
        WooScript _LightingScript;
        Scene _Scene;

        public double _FocusDistance
        {
            get { return (double)GetValue(_DepthProperty); }
            set { SetValue(_DepthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _DepthProperty =
            DependencyProperty.Register("_FocusDistance", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)0));

        public double _ApertureSize
        {
            get { return (double)GetValue(_ApertureSizeProperty); }
            set { SetValue(_ApertureSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ApertureSizeProperty =
            DependencyProperty.Register("_ApertureSize", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)1.0));

        public double _FOV
        {
            get { return (double)GetValue(_FOVProperty); }
            set { SetValue(_FOVProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _FOVProperty =
            DependencyProperty.Register("_FOV", typeof(double), typeof(MainWindow), new UIPropertyMetadata((double)40));

        private void InitialiseCamera()
        {
            _Camera = new Camera(_AppSettings._CameraFrom, _AppSettings._CameraTo, _AppSettings._FOV);
            _FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();
            _ApertureSize = _AppSettings._ApertureSize;
            _FOV = _AppSettings._FOV;

            // set up animation thread for the camera movement
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(17);
            _Timer.Tick += this.timer_Tick;
        }

        private void InitialiseScene()
        {
            _Scene = new Scene(_Camera);
        }

        public void InitialiseScript()
        {
            _BackgroundScript = new WooScript();
            _SceneScript = new WooScript();
            _LightingScript = new WooScript();
            _BackgroundScript.Load("background", "scratch");
            _SceneScript.Load("scene", "scratch");
            _LightingScript.Load("lighting", "scratch");
            backgroundDesc.Text = _BackgroundScript._Program;
            sceneDesc.Text = _SceneScript._Program;
            lightingDesc.Text = _LightingScript._Program;
        }

        private string BuildXML(bool preview)
        {
            bool pt = _Scene._PathTracer;

            _Scene._PathTracer = false;

            string XML = @"
<VIEWPORT width=" + image1.Width + @" height=" + image1.Height + @"/>";

            Camera previewCamera = new Camera(_Camera._Position, _Camera._Target, _Camera._FOV);
            previewCamera._AAEnabled = false;
            previewCamera._DOFEnabled = false;

            XML += previewCamera.CreateElement().ToString();
            XML += _Scene.CreateElement(preview).ToString();

            _Scene._PathTracer = pt;

            return XML;
        }
        
        public void InitialiseTestScene()
        {
            Matrix3 identity = new Matrix3();
            identity.MakeIdentity();

            _Scene.AddRenderObject(_BackgroundScript);
            _Scene.AddRenderObject(_SceneScript);
            _Scene.AddRenderObject(_LightingScript);
        }

        string _SettingsLocation;
        AppSettings _AppSettings;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            _SettingsLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\Settings.xml";
            _AppSettings = AppSettings.Load(_SettingsLocation);

            // starting camera settings
            InitialiseCamera();

            // initialise the scene
            InitialiseScene();

            // initialise the script objects
            InitialiseScript();

            InitialiseTestScene();

            Distance.ReadDistanceSchema();

            Compile();
        }

        string _DistanceSchema;
        ImageRenderer _ImageRenderer;
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Preview(true);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Compile();
        }

        private void Preview(bool preview)
        {
            if (!preview)
            {
                _ImageRenderer = new ImageRenderer(image1, BuildXML(false), 480, 270, false);
                _ImageRenderer.Render();
                _ImageRenderer = new ImageRenderer(image1, BuildXML(true), (int)((float)480 * _Scale), (int)((float)270 * _Scale), false);
            }
            else
            {
                _ImageRenderer.Render();
            }
        }

        private bool CompileSingle(ref WooScript script, ref Label status)
        {
            string log = "";
            string error = "";
            bool success = script.Parse(ref log, ref error);
            if (success)
            {
                status.Background = Brushes.LightGreen;
                return true;
            }
            else
            {
                status.Background = Brushes.Red;
                MessageBox.Show(error);
                return false;
            }
        }

        private void Compile()
        {
            _BackgroundScript._Program = backgroundDesc.Text;
            _SceneScript._Program = sceneDesc.Text;
            _LightingScript._Program = lightingDesc.Text;

            SaveStatus();

            bool success = CompileSingle(ref _BackgroundScript, ref backgroundStatus);
            success &= CompileSingle(ref _SceneScript, ref sceneStatus);
            success &= CompileSingle(ref _LightingScript, ref lightingStatus);
            
//            if (success)
            {
                TriggerPreview();
            }
        }

        private void TriggerPreview()
        {
            _Scale = getPreviewResolution();
            _ImageRenderer = new ImageRenderer(image1, BuildXML(true), (int)((float)_Scale * 480), (int)((float)_Scale * 270), false);
            Preview(true);
        }

        Vector3 _Velocity = new Vector3(0,0,0);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Button source = e.Source as Button;
            if (source != null)
            {
                double Multiplier = 0.1;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    Multiplier = 0.01;
                }

                if (e.Key == Key.Left)
                {
                    _Velocity.x -= Multiplier * _FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    _Velocity.x += Multiplier * _FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _Velocity.z += Multiplier * _FocusDistance;
                        e.Handled = true;
                    }
                    else
                    {
                        _Velocity.y += Multiplier * _FocusDistance;
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Down)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _Velocity.z -= Multiplier * _FocusDistance;
                        e.Handled = true;
                    }
                    else
                    {
                        _Velocity.y -= Multiplier * _FocusDistance;
                        e.Handled = true;
                    }
                }

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }
        DispatcherTimer _Timer;
        void timer_Tick(object sender, EventArgs e)
        {
            Vector3 to = _Camera._Target - _Camera._Position;
            to.Normalise();
            Vector3 up = new Vector3(0, 1, 0);
            Vector3 right = up.Cross(to);
            right.Normalise();

            Vector3 newup = to.Cross(right);
            newup.Normalise();

            right.Mul(_Velocity.x);
            to.Mul(_Velocity.z);
            newup.Mul(_Velocity.y);

//            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(to);
                _Camera._Position.Add(newup);
                _Camera._Target.Add(right);
                _Camera._Target.Add(to);
                _Camera._Target.Add(newup);
            }
  /*          else
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(newup);
            }
            */
            _FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();
            _Camera._FOV = _FOV;

            _Velocity *= 0.6;
            if (_ImageRenderer != null)
            {
                _ImageRenderer.UpdateCamera(_Camera.CreateElement().ToString());
                Preview(true);
            }
            if (_Velocity.MagnitudeSquared()<0.0001)
                _Timer.Stop();
        }

        private void image1_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("GotFocus");
        }

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDepth(ref float depth, int x, int y);

        bool _ImageDrag = false;
        double _Pitch;
        double _Yaw;
        Point _DragStart;
        float _Scale = 1;
        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("LMBDown");
            float depth = 0;
            Point mousePos = e.GetPosition(image1);

            GetDepth(ref depth, (int)(_Scale * mousePos.X), (int)(_Scale * mousePos.Y));
            if (depth>0)
                _FocusDistance = (double)depth;

            Vector3 dir = (_Camera._Target - _Camera._Position);
            dir.Normalise();

            _Pitch = Math.Asin(dir.y);
            dir.y = 0;
            dir.Normalise();
            _Yaw = Math.Asin(dir.x);
            if (dir.z < 0)
                _Yaw = (Math.PI) - _Yaw;

            _DragStart = e.GetPosition(this);
            Debug.WriteLine("dragstart (x = " + _DragStart.X + ", y=" + _DragStart.Y + ")");

            dir = (_Camera._Target - _Camera._Position);
            dir.Normalise();
            dir *= _FocusDistance;
            _Camera._Target = _Camera._Position + dir;

            _ImageDrag = true;

        //    CaptureMouse();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("LMBUp");
            _ImageDrag = false;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ImageDrag)
            {
                Point dragPos = e.GetPosition(this);
                dragPos.X -= _DragStart.X;
                dragPos.Y -= _DragStart.Y;
                Debug.WriteLine("dragpos (x = " + dragPos.X + ", y=" + dragPos.Y + ")");

                double newyaw = _Yaw - 0.01 * dragPos.X;
                double newpitch = _Pitch + 0.01 * dragPos.Y;
                if (newpitch > Math.PI * 0.49f) newpitch = Math.PI * 0.49f;
                if (newpitch < -Math.PI * 0.49f) newpitch = -Math.PI * 0.49f;
                Vector3 dir = _Camera._Target - _Camera._Position;
                double length = dir.Magnitude();

                Vector3 newdir = new Vector3();
                newdir.y = Math.Sin(newpitch);
                newdir.x = Math.Cos(newpitch) * Math.Sin(newyaw);
                newdir.z = Math.Cos(newpitch) * Math.Cos(newyaw);

                double mag = newdir.Magnitude();
                newdir *= length;

                _Camera._Target = _Camera._Position + newdir;

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            TriggerPreview();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            SaveStatus(); 

            _Velocity = new Vector3(0, 0, 0);
            _Camera._FocusDepth = (float)_FocusDistance;
            _Camera._ApertureSize = (float)_ApertureSize;
            _Camera._FOV = (float)_FOV;
            FinalRender ownedWindow = new FinalRender(ref _Scene, ref _Camera);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();

            TriggerPreview();
        }

        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ButtonLMBUp");
            _ImageDrag = false;
            ReleaseMouseCapture();

        }

        private void saveBackground_Click(object sender, RoutedEventArgs e)
        {
            _BackgroundScript.SaveUserInput("background");
        }

        private void loadBackground_Click(object sender, RoutedEventArgs e)
        {
            _BackgroundScript.LoadUserInput("background");
            backgroundDesc.Text = _BackgroundScript._Program;
        }

        private void saveScene_Click(object sender, RoutedEventArgs e)
        {
            _SceneScript.SaveUserInput("scene");
        }

        private void loadScene_Click(object sender, RoutedEventArgs e)
        {
            _SceneScript.LoadUserInput("scene");
            sceneDesc.Text = _SceneScript._Program;
        }

        private void saveLighting_Click(object sender, RoutedEventArgs e)
        {
            _LightingScript.SaveUserInput("lighting");
        }

        private void loadLighting_Click(object sender, RoutedEventArgs e)
        {
            _LightingScript.LoadUserInput("lighting");
            lightingDesc.Text = _LightingScript._Program;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            string XML = BuildXML(false);

            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\XML";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "Scene XML (*.xml)|*.xml";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                // Save document
                string filename = saveFileDialog1.FileName;
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(XML);
                sw.Close();
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();

            helpWindow.Owner = Window.GetWindow(this);
            helpWindow.Show();            
        }

        private float getPreviewResolution()
        {
            if (radioButton1.IsChecked.HasValue && radioButton1.IsChecked.Value)
            {
                return 1.0f;
            }
            else if (radioButton2.IsChecked.HasValue && radioButton2.IsChecked.Value)
            {
                return 0.5f;
            }
            else if (radioButton3.IsChecked.HasValue && radioButton3.IsChecked.Value)
            {
                return 0.33333f;
            }
            else if (radioButton4.IsChecked.HasValue && radioButton4.IsChecked.Value)
            {
                return 0.1f;
            }
            return 0.1f;
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
                TriggerPreview();
        }

        private void SaveStatus()
        {
            _BackgroundScript.Save("background", "scratch");
            _SceneScript.Save("scene", "scratch");
            _LightingScript.Save("lighting", "scratch");
            _AppSettings.Save(_SettingsLocation, _Camera);
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveStatus();
        }

        private void RefreshRender(object sender, TextChangedEventArgs e)
        {
            if (!_Timer.IsEnabled)
                _Timer.Start();

        }
    }
}
