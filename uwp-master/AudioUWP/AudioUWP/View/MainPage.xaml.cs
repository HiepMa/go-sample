﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AudioUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AudioRecorder _audioRecorder;


        public MainPage()
        {
            this.InitializeComponent();
            //this._audioRecorder = new AudioRecorder();
        }

        //private void btnRecord_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this._audioRecorder.IsRecording)
        //    {
        //        this.btnRecord.Content = "Record";
        //        this._audioRecorder.StopRecording();
        //    }
        //    else
        //    {
        //        this.btnRecord.Content = "Stop";
        //        this._audioRecorder.Record();
        //    }
        //}

        //private async void btnPlay_Click(object sender, RoutedEventArgs e)
        //{
        //    this._audioRecorder.Play();

        //    await this._audioRecorder.PlayFromDisk(Dispatcher);


        //}

        //private async void btnSpecialEffectPlay_Click(object sender, RoutedEventArgs e)
        //{
        //    var storageFile = await this._audioRecorder.GetStorageFile(Dispatcher);
        //    AudioEffects effects = new AudioEffects();
        //    await effects.InitializeAudioGraph();
        //    await effects.LoadFileIntoGraph(storageFile);
        //    effects.Play();
        //}

        //private void BtnTest_Click(object sender, RoutedEventArgs e)
        //{
        //    Thread thread = new Thread(UdpService.);
        //    thread.Start();
        //}

    }
}
