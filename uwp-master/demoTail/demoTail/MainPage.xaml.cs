using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace demoTail
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            /*MediaElement DemoMusic = new MediaElement();
            DemoMusic.Source = new Uri("D:/work/web/be/demotail/demoTail/demoTail/Assets/music.mp3");
            DemoMusic.Source = new Uri("ms-appx:///Assets/music.mp3");
            DemoMusic.AreTransportControlsEnabled = true;

            //Configure transport control buttons.
            DemoMusic.TransportControls.IsZoomButtonVisible = false;
            DemoMusic.TransportControls.IsZoomEnabled = false;
            DemoMusic.TransportControls.IsPlaybackRateButtonVisible = true;
            DemoMusic.TransportControls.IsPlaybackRateEnabled = true;

            rootGrid.Children.Add(DemoMusic);*/
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the input focus to ensure that keyboard events are raised.
            this.Loaded += delegate { this.Focus(FocusState.Programmatic); };
        }

        private void MediaButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case "PlayButton": DemoMusic.Play(); break;
                case "PauseButton": DemoMusic.Pause(); break;
                case "StopButton": DemoMusic.Stop(); break;
            }
        }

        private static bool IsCtrlKeyPressed()
        {
            var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            return (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (IsCtrlKeyPressed())
            {
                switch (e.Key)
                {
                    case VirtualKey.P: DemoMusic.Play(); break;
                    case VirtualKey.A: DemoMusic.Pause(); break;
                    case VirtualKey.S: DemoMusic.Stop(); break;
                }
            }
        }
        private void SubmitClick(Object sender, RoutedEventArgs e)
        {
            string str = tbox.Text;
            tblock.Text = str;
        }
        private void tbox_enter(Object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                string str = tbox.Text;
                tblock.Text = str;
            }
        }
    }
}
