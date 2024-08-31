using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI;
using System.Diagnostics;
using Windows.Media.Devices.Core;
using Windows.Storage;
using System.Drawing;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace pixelpaw
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public MainViewModel _viewModel;

        //dispatcher
        private DispatcherTimer _timer;

        //used to calculate delta time 
        private DateTime _lastTickTime;

        //changes speed of increase
        private double _speed;


        public MainWindow()
        {
            //getting the app window
            AppWindow appWindow = this.AppWindow;

            //resizing the app window
            appWindow.Resize(new SizeInt32 { Width = 500, Height = 870 });

            //setting color of title bar
            if (AppWindowTitleBar.IsCustomizationSupported()) {
                AppWindowTitleBar m_TitleBar = appWindow.TitleBar;

                m_TitleBar.BackgroundColor = Colors.LightGray;
                m_TitleBar.ButtonBackgroundColor = Colors.LightGray;

            }

            //20 pixels per second
            _speed = 100.0;

            this.InitializeComponent();


            //creating a view model (needed to populate the sessions list)
            _viewModel = new MainViewModel();
            ExpanderStack.DataContext = _viewModel;
            
            //init the Dispatcher
            _timer=new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += Tick;

            //attaching handlers to the AccelerateButton
            AccelerateButton.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(Button_Pressed), true);
            AccelerateButton.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(Button_Released), true);

            //_viewModel.RemoveAllLoginTimes();

            //adding login time to the session list
            _viewModel.AddLoginTime(DateTime.Now);
            
        }

        //function is executed per tick
        private void Tick(object sender, object e)
        {
            DateTime currentTickTime = DateTime.Now;
            //this will be in seconds, should have enough precision for our purpose
            double deltaTime = (currentTickTime - _lastTickTime).TotalSeconds;


            manipulateRectangleHeight(deltaTime);

            _lastTickTime = DateTime.Now;
        }

        private void manipulateRectangleHeight(double deltaTime) { 
           
            double deltaHeight= _speed * deltaTime;
            double tempHeight = AccelerationBar.Height + deltaHeight;
            if (tempHeight > 155) {
                tempHeight = 155;
            }
            AccelerationBar.Height =tempHeight;

            Canvas.SetTop(AccelerationBar,155-AccelerationBar.Height);
        }

        //handles button pressed
        public void Button_Pressed(object sender, PointerRoutedEventArgs e) {
            _lastTickTime = DateTime.Now;
            _timer.Start();
        }
        
        //handles button released
        public  void Button_Released(object sender, PointerRoutedEventArgs e) {
            _timer.Stop();
            AccelerationBar.Height = 0;
            Canvas.SetTop(AccelerationBar,10);
        }

    }

}
