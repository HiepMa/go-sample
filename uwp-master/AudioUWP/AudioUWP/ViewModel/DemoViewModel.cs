using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.UI.Core;

namespace AudioUWP.ViewModel
{
    public class DemoViewModel :  ViewModelBase
    {
        private RelayCommand _click_start;
        private RelayCommand _click_stop;
        private bool _bs1;
        private bool _bs2;
        private string _name;
        private DeviceInfo _index;
        private DeviceWatcherHelper deviceWatcherHelper;

        private List<DeviceInfo> deviceInfo = new List<DeviceInfo>();

        private List<DeviceInfoSelection> deviceInfoSelections = new List<DeviceInfoSelection>();
        public List<DeviceInfo> CreateList()
        {
            List<DeviceInfo> selectors = new List<DeviceInfo>();

            // Pre-canned device class selectors
            selectors.Add(new DeviceInfo("All Device Interfaces (default)", DeviceClass.All, null));
            selectors.Add(new DeviceInfo("Audio Capture", DeviceClass.AudioCapture,  null ));
            selectors.Add(new DeviceInfo("Audio Render", DeviceClass.AudioRender, null ));
            selectors.Add(new DeviceInfo("Image Scanner",DeviceClass.ImageScanner,null ));
           /* selectors.Add(new DeviceInfo() { DisplayName = "Location", DeviceClassSelector = DeviceClass.Location, Selector = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Portable Storage", DeviceClassSelector = DeviceClass.PortableStorageDevice, Selector = null });
            selectors.Add(new DeviceInfo() { DisplayName = "Video Capture", DeviceClassSelector = DeviceClass.VideoCapture, Selector = null });
*/
            // A few examples of selectors built dynamically by windows runtime apis.
/*            selectors.Add(new DeviceInfo("Human Interface (HID)", null ,HidDevice.GetDeviceSelector(0, 0));*/
/*            selectors.Add(new DeviceInfo() { DisplayName = "Activity Sensor", Selector = ActivitySensor.GetDeviceSelector() });
            selectors.Add(new DeviceInfo() { DisplayName = "Pedometer", Selector = Pedometer.GetDeviceSelector() });
            selectors.Add(new DeviceInfo() { DisplayName = "Proximity", Selector = ProximityDevice.GetDeviceSelector() });
            selectors.Add(new DeviceInfo() { DisplayName = "Proximity Sensor", Selector = ProximitySensor.GetDeviceSelector() });
*/
            return selectors;
        }
/*
        public List<DeviceInfoSelection> CreateSelection()
        {
            List<DeviceInfoSelection> deviceInfoSelections = new List<DeviceInfoSelection>();
            deviceInfoSelections.Add(new DeviceInfoSelection("123", "456", "789"));
            return deviceInfoSelections;
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
            Info = CreateList();
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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
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
        }

        private void Start()
        {
            ItemList = null;
            Bs_1 = false;
            Bs_2 = true;

            // First get the device selector chosen by the UI.
            DeviceInfo dv = Index;
            DeviceWatcher deviceWatcher;
            if(null == dv.Selection)
            {
                deviceWatcher = DeviceInformation.CreateWatcher(dv.DeviceClassSelector);
            }
            else if(dv.Kind == DeviceInformationKind.Unknown)
            {
                deviceWatcher = DeviceInformation.CreateWatcher(dv.Selection, null);
            }
            else
            {
                deviceWatcher = DeviceInformation.CreateWatcher(dv.Selection, null, dv.Kind);
            }
            Run(deviceWatcher);
        }

        public void Run(DeviceWatcher deviceWatcher)
        {
            deviceWatcher.Added += Watcher_DeviceAdded;
            deviceWatcher.Updated += Watcher_DeviceUpdated;
            deviceWatcher.Removed += Watcher_DeviceRemoved;
/*            deviceWatcher.EnumerationCompleted += Watcher_EnumerationCompleted;
            deviceWatcher.Stopped += Watcher_Stopped;*/

            deviceWatcher.Start();
        }

        private void Watcher_DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }

        private void Watcher_DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }
        CoreDispatcher dispatcher;
        private async void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
            await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                List<DeviceInfoSelection> dv = ItemList;
                // Watcher may have stopped while we were waiting for our chance to run.
                if (IsWatcherStarted(sender))
                {
                    dv.Add(new DeviceInfoSelection(deviceInfo));
                    ItemList = dv;
                }
            });
        }

        private bool IsWatcherStarted(DeviceWatcher watcher)
        {
            return (watcher.Status == DeviceWatcherStatus.Started) ||
                (watcher.Status == DeviceWatcherStatus.EnumerationCompleted);
        }
    }

    public class DeviceInfo
    {
        public string DisplayName { get; set; }
        public DeviceClass DeviceClassSelector { get; set; }
        public string Selection { get; set; }

        public DeviceInformationKind Kind { get; set; }

        public DeviceInfo(string displayName, DeviceClass deviceClassSelector, string selection)
        {
            DisplayName = displayName;
            DeviceClassSelector = deviceClassSelector;
            Selection = selection;
        }
    }

    public class DeviceInfoSelection
    {
        private DeviceInformation deviceInfo;

        public string GlyphBitmapImage { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public DeviceInfoSelection(string glyphBitmapImage, string name, string id)
        {
            GlyphBitmapImage = glyphBitmapImage;
            Name = name;
            Id = id;
        }

        public DeviceInfoSelection(DeviceInformation deviceInfo)
        {
            this.deviceInfo = deviceInfo;
        }
    }

}
