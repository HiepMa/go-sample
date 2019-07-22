using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Windows.UI.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Devices.Enumeration.Pnp;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Management; // reference required
using Windows.Devices.HumanInterfaceDevice;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AudioUWP.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _countDevices;
        public static int count = 0;
        public static DeviceInformation deviceInformation;

        public MainViewModel()
        {
            this._audioRecorder = new AudioRecorder();
        }

        private RelayCommand _click;
        public RelayCommand Click_Count
        {
            get
            {
                return _click
                    ?? (_click = new RelayCommand(Count_Devices));
            }
        }

        async void Count_Devices()
        {
            string classGUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";
            var s = "System.Devices.InterfaceClassGuid:=\"" + classGUID + "\"";
            var dev = await DeviceInformation.FindAllAsync();
            DeviceWatcher deviceWatcher;
            deviceWatcher = DeviceInformation.CreateWatcher(
                    null,
                    null, // don't request additional properties for this sample
                    DeviceInformationKind.DeviceInterface);
            foreach (var device in dev)
            {
                /*if (device.Kind.Equals(0))
                {
                    Debug.WriteLine(device.Name + " " + device.Id + " " + device.Pairing.IsPaired);
                }*/
                Debug.WriteLine(device.Name + " " + device.Id + " " + device.Pairing.IsPaired);
            }
            CountDevices = dev.Count.ToString();

            /*var info = GetInfo();
            foreach (var sound in info)
            {
                Debug.WriteLine("Device ID: {0}, PNP Device ID: {1}, Description: {2}",
                                 sound.Id, sound.PnpId, sound.Des);
            }*/
            /*var objSearcher = new ManagementObjectSearcher(
                   "SELECT * FROM Win32_SoundDevice");

            var objCollection = objSearcher.Get();
            foreach (var d in objCollection)
            {
                Console.WriteLine("=====DEVICE====");
                foreach (var p in d.Properties)
                {
                    Console.WriteLine($"{p.Name}:{p.Value}");
                }
            }*/
            /*EnumerateHidDevices();*/
        }

        class SoundInfo
        {
            public string Id { get; set; }
            public string PnpId { get; set; }
            public string Des { get; set; }
            
            public SoundInfo(string id,string pbpid,string des)
            {
                Id = id;
                PnpId = pbpid;
                Des = des;
            }
        }

        static List<SoundInfo> GetInfo()
        {
            List<SoundInfo> devices = new List<SoundInfo>();
            ManagementObjectCollection collection;
            // Win32_SoundDevice "SELECT * FROM Win64_PnPSignedDriver"
            using (var search = new ManagementObjectSearcher("Select * From Win32_USBHub"))
                collection = search.Get();
            foreach(var dev in collection)
            {
                devices.Add(new SoundInfo(
                    (string)dev.GetPropertyValue("DeviceID"),
                    (string)dev.GetPropertyValue("PNPDeviceID"),
                    (string)dev.GetPropertyValue("Description")
                    ));
            }
            collection.Dispose();
            return devices;
        }
        // Find HID devices.
        private async void EnumerateHidDevices()
        {
            // Microsoft Input Configuration Device.
            ushort vendorId = 0x045E;
            ushort productId = 0x07CD;
            ushort usagePage = 0x000D;
            ushort usageId = 0x000E;

            // Create the selector.
            string selector =
                HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);

            // Enumerate devices using the selector.
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Any())
            {
                // At this point the device is available to communicate with
                // So we can send/receive HID reports from it or 
                // query it for control descriptions.
                CountDevices = "HID devices found: " + devices.Count;

                // Open the target HID device.
                HidDevice device =
                    await HidDevice.FromIdAsync(devices.ElementAt(0).Id,
                    FileAccessMode.ReadWrite);

                if (device != null)
                {
                    // Input reports contain data from the device.
                    device.InputReportReceived += async (sender, args) =>
                    {
                        HidInputReport inputReport = args.Report;
                        IBuffer buffer = inputReport.Data;

                        // Create a DispatchedHandler as we are interracting with the UI directly and the
                        // thread that this function is running on might not be the UI thread; 
                        // if a non-UI thread modifies the UI, an exception is thrown.

                        /*await this.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            new DispatchedHandler(() =>
                            {
                                CountDevices += "\nHID Input Report: " + inputReport.ToString() +
                                "\nTotal number of bytes received: " + buffer.Length.ToString();
                            }));*/
                    };
                }

            }
            else
            {
                // There were no HID devices that met the selector criteria.
                CountDevices = "HID device not found";
            }
        }

        public string CountDevices
        {
            get { return _countDevices; }
            set
            {
                _countDevices = value;
                RaisePropertyChanged("CountDevices");
            }
        }

        

        AudioRecorder _audioRecorder;       

        private RelayCommand _record;
        public RelayCommand Record
        {
            get
            {
                return _record
                    ?? (_record = new RelayCommand(RecordMethod));
            }
        }

        private RelayCommand _runServer;
        public RelayCommand RunServer
        {
            get
            {
                return _runServer
                    ?? (_runServer = new RelayCommand(RunServerMethod));
            }
        }
        

        private RelayCommand _play;
        public RelayCommand Play
        {
            get
            {
                return _play
                    ?? (_play = new RelayCommand(PlayMethod));
            }
        }

        private RelayCommand _test;
        public RelayCommand Test
        {
            get
            {
                return _test
                    ?? (_test = new RelayCommand(TestMethod));
            }
        }

        private string _recordText = "Record";
        public string RecordText
        {
            get
            {
                return _recordText;
            }
            set
            {
                _recordText = value;
                RaisePropertyChanged("RecordText");
            }
        }

        private RelayCommand _stop;
        public RelayCommand Stop
        {
            get
            {
                return _stop
                    ?? (_stop = new RelayCommand(StopMethod));
            }
        }

        public void TestMethod() {
            _audioRecorder.Talk();
        }

        public void RecordMethod()
        {
            if (_audioRecorder.IsRecording)
            {
                RecordText = "Record";
                _audioRecorder.StopRecording();
            }
            else
            {
                RecordText = "Stop";
                _audioRecorder.Record();
            }
        }

        public void PlayMethod() {
            _audioRecorder.Play();

            //await _audioRecorder.PlayFromDisk(CoreWindow.GetForCurrentThread().Dispatcher);
        }

        public void StopMethod() {
            _audioRecorder.Stop();
        }

        public void RunServerMethod()
        {
            UdpService.Instance.StartServerThread();
        }

    }
}