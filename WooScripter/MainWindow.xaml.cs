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

        public double _BackgroundRed
        {
            get { return (double)GetValue(BackgroundRedProperty); }
            set { SetValue(BackgroundRedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundRed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundRedProperty =
            DependencyProperty.Register("BackgroundRed", typeof(double), typeof(MainWindow), new UIPropertyMetadata(0.0));

        public double _BackgroundGreen
        {
            get { return (double)GetValue(BackgroundGreenProperty); }
            set { SetValue(BackgroundGreenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundGreen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundGreenProperty =
            DependencyProperty.Register("BackgroundGreen", typeof(double), typeof(MainWindow), new UIPropertyMetadata(0.0));

        public double _BackgroundBlue
        {
            get { return (double)GetValue(BackgroundBlueProperty); }
            set { SetValue(BackgroundBlueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundBlue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundBlueProperty =
            DependencyProperty.Register("BackgroundBlue", typeof(double), typeof(MainWindow), new UIPropertyMetadata(0.0));

        private void InitialiseCamera()
        {
            _Camera = new Camera(new Vector3(-10, 20, -20), new Vector3(0, 0, 0), 40);
            _FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();
            _ApertureSize = _Camera._ApertureSize;

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

            _Scene.GetBackground()._BackgroundColour = new Colour(_BackgroundRed, _BackgroundGreen, _BackgroundBlue);

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
//            _Scene.AddRenderObject(new Circle(new Vector3(0, -1, 0), 20, new Vector3(0,1,0)));
//            _Scene.AddRenderObject(new Cube(new Vector3(0, 5, 0), new Vector3(10, 10, 10), identity));
            _Scene.AddRenderObject(_BackgroundScript);
            _Scene.AddRenderObject(_SceneScript);
            _Scene.AddRenderObject(_LightingScript);
//            Light light1 = new Light();
 //           light1._Position = new Vector3(20, 20, 20);
  //          PointLight pl = light1._LightInstance as PointLight;
   //         pl._Position = new Vector3(20, 20, 20);
     //       pl._Colour = new Colour(100, 80, 30);
       //     _Scene.AddLight(new Light());
            _BackgroundRed = 0.6;
            _BackgroundGreen = 0.5;
            _BackgroundBlue = 0.4;
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // starting camera settings
            InitialiseCamera();

            // initialise the scene
            InitialiseScene();

            // initialise the script objects
            InitialiseScript();

            InitialiseTestScene();

            Compile();
        }

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
            bool success = script.Parse(ref log);
            if (success)
            {
                status.Background = Brushes.LightGreen;
                return true;
            }
            else
            {
                status.Background = Brushes.Red;
                MessageBox.Show(log);
                return false;
            }
        }

        private void Compile()
        {
            _BackgroundScript._Program = backgroundDesc.Text;
            _SceneScript._Program = sceneDesc.Text;
            _LightingScript._Program = lightingDesc.Text;

            _BackgroundScript.Save("background", "scratch");
            _SceneScript.Save("scene", "scratch");
            _LightingScript.Save("lighting", "scratch");

            bool success = CompileSingle(ref _BackgroundScript, ref backgroundStatus);
            success &= CompileSingle(ref _SceneScript, ref sceneStatus);
            success &= CompileSingle(ref _LightingScript, ref lightingStatus);
            
            if (success)
            {
                _Scale = 0.33f;
                _ImageRenderer = new ImageRenderer(image1, BuildXML(true), (int)((float)_Scale * 480), (int)((float)_Scale * 270), false);
                Preview(true);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _BackgroundScript.Save("background", "scratch");
            _SceneScript.Save("scene", "scratch");
            _LightingScript.Save("lighting", "scratch");
        }

        Vector3 _Velocity = new Vector3(0,0,0);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Button source = e.Source as Button;
            if (source != null)
            {
                if (e.Key == Key.Left)
                {
                    _Velocity.x -= 0.1 * _FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    _Velocity.x += 0.1 * _FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    _Velocity.z += 0.1 * _FocusDistance;
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    _Velocity.z -= 0.1 * _FocusDistance;
                    e.Handled = true;
                }
            }

            if (!_Timer.IsEnabled)
                _Timer.Start();
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
            newup.Mul(_Velocity.z);

//            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(to);
                _Camera._Target.Add(right);
                _Camera._Target.Add(to);
            }
  /*          else
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(newup);
            }
            */
            _FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();

            _Velocity *= 0.6;
            if (_ImageRenderer != null)
            {
                _ImageRenderer.UpdateCamera(_Camera.CreateElement().ToString());
                Preview(true);
            }
            if (_Velocity.MagnitudeSquared()<0.001)
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
            _Scale = 0.33f;
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
            Preview(false);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            _Camera._FocusDepth = (float)_FocusDistance;
            _Camera._ApertureSize = (float)_ApertureSize;
            FinalRender ownedWindow = new FinalRender(ref _Scene, ref _Camera);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();
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
    }
}
