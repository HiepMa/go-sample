using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using System;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Sensors;
using Windows.Networking.Proximity;
using Windows.UI.Xaml.Media.Imaging;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace AudioUWP.ViewModel
{
    public class DemoViewModel :  ViewModelBase
    {
        private RelayCommand _click_start;
        private RelayCommand _click_stop;
        private bool _bs1;
        private bool _bs2;
        private DeviceInfo _index;
        private string _text;
        private string _vs_co;

        private List<DeviceInfo> deviceInfo = new List<DeviceInfo>();

        private List<DeviceInfoSelection> deviceInfoSelections = new List<DeviceInfoSelection>();
        public List<DeviceInfo> CreateList()
        {
            List<DeviceInfo> selectors = new List<DeviceInfo>();

            // Pre-canned device class selectors
            selectors.Add(new DeviceInfo() { DisplayName = "All Device Interfaces (default)", DeviceClassSelector = DeviceClass.All, Selection = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Audio Capture", DeviceClassSelector = DeviceClass.AudioCapture, Selection = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Audio Render", DeviceClassSelector = DeviceClass.AudioRender, Selection = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Image Scanner", DeviceClassSelector = DeviceClass.ImageScanner, Selection = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Location", DeviceClassSelector = DeviceClass.Location, Selection = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Portable Storage", DeviceClassSelector = DeviceClass.PortableStorageDevice, Selection = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Video Capture", DeviceClassSelector = DeviceClass.VideoCapture, Selection = null });

            // A few examples of selectors built dynamically by windows runtime apis.
            selectors.Add(new DeviceInfo() { DisplayName = "Human Interface (HID)", Selection = HidDevice.GetDeviceSelector(0, 0) });
            selectors.Add(new DeviceInfo() { DisplayName = "Activity Sensor", Selection = ActivitySensor.GetDeviceSelector() });
            selectors.Add(new DeviceInfo() { DisplayName = "Pedometer", Selection = Pedometer.GetDeviceSelector() });
            selectors.Add(new DeviceInfo() { DisplayName = "Proximity", Selection = ProximityDevice.GetDeviceSelector() });
            selectors.Add(new DeviceInfo() { DisplayName = "Proximity Sensor", Selection = ProximitySensor.GetDeviceSelector() });

            return selectors;
        }

/*        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                RaisePropertyChanged("Channel");
            }
        }*/
        public List<DeviceInfoSelection> ItemList
        {
            get { return deviceInfoSelections; }
            set
            {
                deviceInfoSelections = value;
                RaisePropertyChanged("ItemList");
            }
        }
        public DemoViewModel()
        {
            Bs_1 = true;
            Bs_2 = false;
            Vs_Co = "Collapsed";
            Info = CreateList();
        }

        public string Text
        {
            get { return _text; }
            set { _text = value;
                RaisePropertyChanged("Text");
            }
        }
        public string Vs_Co
        {
            get { return _vs_co; }
            set { _vs_co = value;
                RaisePropertyChanged("Vs_Co");
            }
        }
        public bool Bs_1
        {
            get { return _bs1; }
            set
            {
                _bs1 = value;
                RaisePropertyChanged("Bs_1");
            }
        }

        public bool Bs_2
        {
            get { return _bs2; }
            set
            {
                _bs2 = value;
                RaisePropertyChanged("Bs_2");
            }
        }

        public DeviceInfo Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        public List<DeviceInfo> Info
        {
            get { return deviceInfo; }
            set { deviceInfo = value;
                RaisePropertyChanged("Info");
            }
        }

        public RelayCommand Click_Start
        {
            get
            {
                return _click_start
                    ?? (_click_start = new RelayCommand(Start));
            }
        }

        public RelayCommand Click_Stop
        {
            get
            {
                return _click_stop
                    ?? (_click_stop = new RelayCommand(Stop));
            }
        }

        private void Stop()
        {
            Bs_1 = true;
            Bs_2 = false;
            Vs_Co = "Collapsed";
        }

        async void Start()
        {
            ItemList = null;
            Bs_1 = false;
            Bs_2 = true;

            /*// First get the device selector chosen by the UI.
            DeviceInfo dv = Index;
            *//*var dev = await DeviceInformation.FindAllAsync();*//*
            DeviceInformationCollection dev = null;
            if (null == dv.Selection)
            {
                dev = await DeviceInformation.FindAllAsync(dv.DeviceClassSelector);
            }
            else if(dv.Kind == DeviceInformationKind.Unknown)
            {
                dev = await DeviceInformation.FindAllAsync(dv.Selection, null);
            }
            else
            {
                dev = await DeviceInformation.FindAllAsync(dv.Selection, null, dv.Kind);
            }
            Output(dev);*/
            WaveOutCapabilities list = new WaveOutCapabilities();
            
            Debug.WriteLine(list.ProductName);

        }

        public void Output(DeviceInformationCollection dev)
        {
            ItemList = null;
            List<DeviceInfoSelection> liste = new List<DeviceInfoSelection>();
            foreach (var device in dev)
            {
                Debug.WriteLine(device.Name + " " + device.Id + " " + device.Pairing.IsPaired + " "+ device.Properties);
                liste.Add(new DeviceInfoSelection(device));
            }
            ItemList = liste;
            Vs_Co = "Visible";
            Text = liste.Count.ToString() + " Device Found ....";
        }
    }

    public class DeviceInfo
    {
        public string DisplayName { get; set; }
        public DeviceClass DeviceClassSelector { get; set; }
        public string Selection { get; set; }
        public DeviceInformationKind Kind { get; set; }
        public DeviceInfo()
        {
            Kind = DeviceInformationKind.Unknown;
            DeviceClassSelector = DeviceClass.All;
        }
    }

    public class DeviceInfoSelection : ViewModelBase
    {
        private DeviceInformation deviceInfo;
        /*public string Channel
        {
            get
            {
                
            }
        }*/
        public BitmapImage GlyphBitmapImage { get; private set; }
        public string Name { get { return deviceInfo.Name; } }
        public string Id {
            get
            {
                return deviceInfo.Id;
            }
        }

        public DeviceInformationKind Kind
        {
            get { return deviceInfo.Kind; }
        }

        public DeviceInfoSelection(DeviceInformation deviceInfo)
        {
            this.deviceInfo = deviceInfo;
            UpdateGlyphBitmapImage();
        }

        private async void UpdateGlyphBitmapImage()
        {
            DeviceThumbnail deviceThumbnail = await deviceInfo.GetGlyphThumbnailAsync();
            BitmapImage glyphBitmapImage = new BitmapImage();
            await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
            GlyphBitmapImage = glyphBitmapImage;
            RaisePropertyChanged("GlyphBitmapImage");
        }
    }

}
