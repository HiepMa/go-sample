using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace demoTail.Service
{
    public class Service
    {
        private const string DEFAULT_AUDIO_FILENAME = "voice_sound.mp3";
        private string _fileName;
        private static InMemoryRandomAccessStream _memoryBuffer;
        private bool _isReceiving;
        private BufferedWaveProvider WaveProvider { get; set; }

        public async void ServerStartAsync()
        {
            //UdpClient udpserver = new UdpClient(11111);
            Debug.WriteLine("=============================");
            byte[] ba = new byte[128];
            DatagramSocket server = new DatagramSocket();
            WaveProvider = new BufferedWaveProvider(new WaveFormat(48000, 16, 2));
            Socket SimpleSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //Debug.WriteLine(ba.GetLength(0));
            //Debug.WriteLine(ba.Count(s => s != null));
            _isReceiving = true;

            // new Task(StartPlayingUsingNAudio, TaskCreationOptions.LongRunning).Start();
            // StartPlayingUsingAudioGraph();
            StartPlayingUsingNAudio();
            //await StartWithDatagramSocket(server);
            //StartWithUdpClient(udpserver);
            var remoteEP = new IPEndPoint(IPAddress.Any, 11111);

            byte[] buffer = new Byte[2048];
            SimpleSocket.Bind(remoteEP);
            SocketAsyncEventArgs sockarg = new SocketAsyncEventArgs();
            sockarg.Completed += Sockarg_Completed;
            sockarg.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            sockarg.SetBuffer(buffer, 0, buffer.Length);
            if (!SimpleSocket.ReceiveFromAsync(sockarg))
            {
                Sockarg_Completed(this, sockarg);
            }
            // StartPlaying();

        }

        private void Sockarg_Completed(object sender, SocketAsyncEventArgs e)
        {
            WaveProvider.AddSamples(e.Buffer, 0, e.BytesTransferred);
            Debug.WriteLine(BitConverter.ToString(e.Buffer));
        }

        private void StartWithUdpClient(UdpClient udpserver)
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, 11111);
            while (_isReceiving)
            {
                var data = udpserver.Receive(ref remoteEP);
                //Debug.WriteLine("receice " + count + " package");
                //Debug.WriteLine("receice data from " + remoteEP.ToString());
                //Debug.WriteLine("receice data length " + data.Length);

                // count++;               
                WaveProvider.AddSamples(data, 0, data.Length);
                Debug.WriteLine(BitConverter.ToString(data));
                Debug.WriteLine(data.Length);
                // await _memoryBuffer.WriteAsync(data.AsBuffer());
                //_memoryBuffer.Seek(currentOffset);
                //currentOffset += (ulong)data.Length;
            }
        }

        private async Task StartWithDatagramSocket(DatagramSocket server)
        {
            try
            {
                var remoteHost = new HostName("127.0.0.1");
                await server.BindEndpointAsync(remoteHost, "11111");
                server.MessageReceived += OnMessageReceived;

            }
            catch { }
        }

        private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var reader = args.GetDataReader();
            byte[] data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);
            Debug.WriteLine("Receive from " + args.RemoteAddress + "/" + args.RemotePort);
            Debug.WriteLine(BitConverter.ToString(data));
            WaveProvider.AddSamples(data, 0, data.Length);
        }

        private void StartPlayingUsingNAudio()
        {
            //var device = await getDevice();
            //Debug.WriteLine(device);
            var waveOut = new WasapiOutRT(AudioClientShareMode.Shared, 100);
            waveOut.PlaybackStopped += OnPlaybackStopped;
            waveOut.Init(GetWaveProvider);
            waveOut.Play();
        }
        private IWaveProvider GetWaveProvider()
        {
            return WaveProvider;
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            Debug.WriteLine(sender + " stopped.");
            Debug.WriteLine("Exception: " + e.Exception);
        }
        public void StartPlaying() {
            //var devices = await DeviceInformation.FindAllAsync();
            //string beingUsedDeviceId = null;

            //foreach (var device in devices)
            //{
            //Debug.WriteLine(device.Name);
            //if (device.Name.Equals("Speakers (Logitech USB Headset H340)"))
            //{
            //    beingUsedDeviceId = device.Id;
            //    Debug.WriteLine("Used " + device.Name);
            //    break;
            //}
            //}

            //var beingUsedDevice = await DeviceInformation.CreateFromIdAsync(beingUsedDeviceId);
            IRandomAccessStream tempBuffer = _memoryBuffer.CloneStream();
            var source = MediaSource.CreateFromStream(tempBuffer, "audio/mpeg");
            source.StateChanged += OnStateChanged;

            MediaPlayer _player = new MediaPlayer
            {
                //AutoPlay = true,
                RealTimePlayback = true,
                //CanPause = false,
                //AudioDevice = beingUsedDevice,
                Source = source
            };

            //_player.PlaybackSession.BufferingProgressChanged += OnBufferingProgressChanged;
            _player.PlaybackSession.PositionChanged += OnPositionChanged;
            _player.CurrentStateChanged += OnCurrentStateChanged;
            _player.MediaEnded += OnMediaEnded;
            _player.Play();
            //_player.SourceChanged += OnSourceChanged;

            //new Task(() => StartBufferMonitoring(_player.PlaybackSession, source), TaskCreationOptions.LongRunning).Start();
        }

        private void OnMediaEnded(MediaPlayer sender, object args)
        {
            Debug.WriteLine("OnMediaEnded");
            //sender.Position = sender.NaturalDuration;
        }

        private void StartBufferMonitoring(MediaPlaybackSession playbackSession, MediaSource source)
        {
            {
                ulong previousPosition = 0;
                while (true)
                {
                    var currentPosition = _memoryBuffer.Position;
                    if (currentPosition > previousPosition)
                    {
                        if (!source.IsOpen)
                        {
                            source.Reset();
                        }
                        previousPosition = currentPosition;
                    }
                }
            }
        }
        private void OnCurrentStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine("OnCurrentStateChanged");
        }

        private void OnPositionChanged(MediaPlaybackSession sender, object args)
        {
            Debug.WriteLine("OnPositionChanged");
        }

        private void OnStateChanged(MediaSource sender, MediaSourceStateChangedEventArgs args)
        {
            Debug.WriteLine("OnStateChanged");
        }

        public async void SaveAudioToFile()
        {            
            try
            {
                //await DeleteExistingFile();
                MediaPlayer mediaPlayer = new MediaPlayer();
                mediaPlayer.Source = MediaSource.CreateFromStream(_memoryBuffer, "MP3");
                mediaPlayer.Play();
                IRandomAccessStream audioStream = _memoryBuffer.CloneStream();
                StorageFolder storageFolder = Package.Current.InstalledLocation;

                StorageFile storageFile = await storageFolder.CreateFileAsync(DEFAULT_AUDIO_FILENAME, CreationCollisionOption.GenerateUniqueName);
                this._fileName = storageFile.Name;

                using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAndCloseAsync(audioStream.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                    await audioStream.FlushAsync();
                    audioStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task DeleteExistingFile()
        {
            try
            {
                Debug.WriteLine("Filename: " + _fileName);
                StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                Debug.WriteLine("=========================");
                Debug.WriteLine("Folder: " + storageFolder + " Filename: " + _fileName);
                StorageFile existingFile = await storageFolder.GetFileAsync(this._fileName);
                await existingFile.DeleteAsync();
            }
            catch (FileNotFoundException)
            {

            }

        }

        public void StartClient()
        {
            UdpClient udpclient = new UdpClient();
            try
            {
                Debug.WriteLine("=============================");
                Debug.WriteLine("Client Start");
                Thread.Sleep(1000);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11002);
                udpclient.Connect(ep);

                udpclient.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
                var receicedata = udpclient.Receive(ref ep);

                Debug.WriteLine("receive data from " + ep.ToString());
                Console.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        internal static async Task<InMemoryRandomAccessStream> ByteArrayToRandomAccessStream(byte[] arr)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            await randomAccessStream.WriteAsync(arr.AsBuffer());
            randomAccessStream.Seek(0); // Just to be sure.
                                        // I don't think you need to flush here, but if it doesn't work, give it a try.
            return randomAccessStream;
        }
    }
}

