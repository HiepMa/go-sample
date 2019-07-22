using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AudioUWP
{
    public class AudioRecorder
    {
        private const string DEFAULT_AUDIO_FILENAME = "audio_clip.mp3";
        private string _fileName;
        private MediaCapture _mediaCapture;
        public static InMemoryRandomAccessStream _memoryBuffer;
        public static int _bufferSize = 320;
        private static bool isCalling = false;
        private string ipAddress = "192.168.82.21";
        private static IRandomAccessStream sendingStream;


        public bool IsRecording { get; set; }

        public async void Record()
        {
            if (IsRecording)
            {
                throw new InvalidOperationException("Recording already in progress!");
            }

            Initialize();
            await DeleteExistingFile();

            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Audio
            };

            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync(settings);
            await _mediaCapture.StartRecordToStreamAsync( MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto), _memoryBuffer);
            IsRecording = true;
        }

        //public async void Talk()
        //{
        //    isCalling = true;
        //    Initialize();
        //    MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
        //    {
        //        StreamingCaptureMode = StreamingCaptureMode.Audio
        //    };

        //    _mediaCapture = new MediaCapture();
        //    await _mediaCapture.InitializeAsync(settings);
        //    await _mediaCapture.StartRecordToStreamAsync(MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Low), _memoryBuffer);
        //    sendingStream = _memoryBuffer.CloneStream();
        //    Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
        //    sendAudioThread = new Thread(SendPackage);
        //    sendAudioThread.Start();            
        //}

        public async void Talk()
        {

            Initialize();
            
            await Task.Run(StartRecording);
        }

        private void StartRecording()
        {
            var recorder = new WasapiCaptureRT();
            //recorder.WaveFormat = new WaveFormat(44100, 16, 2);
            //var devices = await DeviceInformation.FindAllAsync();
            //foreach (var device in devices)
            //{
            //    Debug.WriteLine(device.Name + "  " + device.Id);
            //}
            //recorder.WaveFormat = new WaveFormat();
            //recorder.WaveFormat = new WaveFormat(8000, 16, 1);
            recorder.WaveFormat = new WaveFormat(48000, 16, 1);
            recorder.DataAvailable += OnDataAvailable;
            Debug.WriteLine("Wave format: " + recorder.WaveFormat);
            Debug.WriteLine(WasapiCaptureRT.GetDefaultCaptureDevice());
            recorder.StartRecording();
        }


        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            Debug.WriteLine(e.BytesRecorded);
            UdpService.Instance.SendMessage(e.Buffer, e.BytesRecorded);
        }

        public async static void SendPackage() {
            ulong currentOffset = 0;
            while (isCalling)
            {
                if (sendingStream.Size > currentOffset + (ulong)_bufferSize)
                {
                    Debug.WriteLine(currentOffset);
                    sendingStream.Seek(currentOffset);
                    byte[] tempBuffer = new byte[_bufferSize];
                    await sendingStream.ReadAsync(tempBuffer.AsBuffer(), (uint)_bufferSize, InputStreamOptions.None);
                    UdpService.Instance.SendMessage(tempBuffer, tempBuffer.Length);
                    Debug.WriteLine(BitConverter.ToString(tempBuffer));
                    currentOffset += (ulong)_bufferSize;
                    Debug.WriteLine("");
                }
                Thread.Sleep(20);
            }

            //int currentOffset = 0;

            //while (isCalling)
            //{
            //    byte[] tempBuffer = new byte[_bufferSize];

            //    if ((int)_memoryBuffer.Size - currentOffset > _bufferSize)
            //    {
            //        var bytesRead = await _memoryBuffer.AsStream().ReadAsync(tempBuffer, currentOffset, _bufferSize);
            //        Debug.WriteLine(currentOffset);
            //        Debug.WriteLine(BitConverter.ToString(tempBuffer));
            //        UdpService.Instance.SendMessage(tempBuffer, tempBuffer.Length);
            //        currentOffset += bytesRead;
            //        Debug.WriteLine("");

            //    }
            //}


            //while (isCalling)
            //{
            //    byte[] arrayByteBuffer = new byte[_bufferSize];

            //    //IRandomAccessStream audioStream = _memoryBuffer.CloneStream();
            //    // arrayByteBuffer = await ToBytesArray(_memoryBuffer, _bufferSize);
            //    //var byteWrite = await _memoryBuffer.WriteAsync(arrayByteBuffer.AsBuffer());
            //    var byteRead = await _memoryBuffer.ReadAsync(arrayByteBuffer.AsBuffer(), (uint)_bufferSize, Windows.Storage.Streams.InputStreamOptions.None);
            //    // Debug.WriteLine("Sending package, arraylength: " + arrayByteBuffer.Length + "Byte array: " + arrayByteBuffer);
            //    if (arrayByteBuffer.GetLength(0) == _bufferSize)
            //    {
            //        Debug.WriteLine("Sending" + arrayByteBuffer.Length);
            //        UdpService.Instance.SendMessage(arrayByteBuffer, arrayByteBuffer.Length);
            //        await _memoryBuffer.FlushAsync();
            //    }
            //    Thread.Sleep(100);
            //}



        }

        public async void Stop() {
            await _mediaCapture.StopRecordAsync();
            await _memoryBuffer.FlushAsync();
            //sendAudioThread.Abort();
        }


        public async void StopRecording()
        {
            await _mediaCapture.StopRecordAsync();
            IsRecording = false;
            IRandomAccessStream randomAccessStream = _memoryBuffer.CloneStream();

            Debug.WriteLine(randomAccessStream.Size);
            byte[] arrayByteBuffer = new byte[randomAccessStream.Size];
            await randomAccessStream.ReadAsync(arrayByteBuffer.AsBuffer(), (uint)randomAccessStream.Size, Windows.Storage.Streams.InputStreamOptions.None);
            Debug.WriteLine(BitConverter.ToString(arrayByteBuffer));
            InMemoryRandomAccessStream newStream = new InMemoryRandomAccessStream();
            await newStream.WriteAsync(arrayByteBuffer.AsBuffer());

            SaveAudioToFile(newStream);

            UdpService.Instance.SendMessage(arrayByteBuffer, arrayByteBuffer.Length);
        }

        public async Task PlayFromDisk(CoreDispatcher dispatcher)
        {

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                MediaElement playbackMediaElement = new MediaElement();
                StorageFolder storageFolder = Package.Current.InstalledLocation;
                StorageFile storageFile = await storageFolder.GetFileAsync(_fileName);
                IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);

                playbackMediaElement.SetSource(stream, storageFile.FileType);
                playbackMediaElement.Play();

            });
        }

        public async Task<StorageFile> GetStorageFile(CoreDispatcher dispatcher)
        {
            StorageFolder storageFolder = Package.Current.InstalledLocation;
            StorageFile storageFile = await storageFolder.GetFileAsync(_fileName);
            return storageFile;
        }

        public void Play()
        {
            MediaElement playbackMediaElement = new MediaElement();

            playbackMediaElement.SetSource(_memoryBuffer, "MP3");
            playbackMediaElement.Play();
        }


        private void Initialize()
        {
            if (_memoryBuffer != null)
            {
                _memoryBuffer.Dispose();
            }

            _memoryBuffer = new InMemoryRandomAccessStream();

            if (_mediaCapture != null)
            {
                _mediaCapture.Dispose();
            }
            _fileName = DEFAULT_AUDIO_FILENAME;
            UdpService.startup(ipAddress);
        }


        private async void SaveAudioToFile(InMemoryRandomAccessStream buffer)
        {
            try
            {
                IRandomAccessStream audioStream = buffer.CloneStream();
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

        public byte[] ToByte(Stream sourceStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                sourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        async Task<byte[]> ToBytesArray(IRandomAccessStream s, int size)
        {
            var dr = new DataReader(s.GetInputStreamAt(0));
            var bytes = new byte[size];
            await dr.LoadAsync((uint)size);
            dr.ReadBytes(bytes);
            await s.FlushAsync();
            return bytes;
        }

        public IRandomAccessStream ToRandomAccessStream(Byte[] bytesArray, int size)
        {
            Stream stream = new MemoryStream(bytesArray);
            IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
            return randomAccessStream;
        }

        internal static async Task<InMemoryRandomAccessStream> ConvertTo(byte[] arr)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            await randomAccessStream.WriteAsync(arr.AsBuffer());
            randomAccessStream.Seek(0); // Just to be sure.
                                        // I don't think you need to flush here, but if it doesn't work, give it a try.
            return randomAccessStream;
        }

        public void PlaySoundLive(int size) {
            Byte[] bytesArray = new byte[_bufferSize];
            MediaElement playbackMediaElement = new MediaElement();

            while (true) {
                bytesArray = UdpService.Instance.ReceiveMessage();
                playbackMediaElement.SetSource(ToRandomAccessStream(bytesArray,_bufferSize), "MP3");
                playbackMediaElement.Play();
            }
            
        }


    }
}
