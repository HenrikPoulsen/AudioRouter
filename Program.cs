using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Win32;

namespace AudioRouter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("partialsourcename partialtargetname");
                return;
            }

            var sourceName = args[0];
            var targetName = args[1];
            var captureMode = CaptureMode.LoopbackCapture;
            MMDevice sourceDevice = null;
            MMDevice targetDevice = null;
            using (var deviceEnumerator = new MMDeviceEnumerator())
            using (var deviceCollection = deviceEnumerator.EnumAudioEndpoints(
                captureMode == CaptureMode.Capture ? DataFlow.Capture : DataFlow.Render, DeviceState.Active))
            {
                foreach (var device in deviceCollection)
                {
                    if (sourceDevice == null && DeviceMatches(device, sourceName))
                    {
                        Console.WriteLine("Found matching source device: " + device.FriendlyName);
                        sourceDevice = device;
                    }
                    if (targetDevice == null && DeviceMatches(device, targetName))
                    {
                        targetDevice = device;
                        Console.WriteLine("Found matching target device: " + device.FriendlyName);
                    }
                }
            }

            if (sourceDevice == targetDevice)
            {
                Console.WriteLine("Source device and target device is the same. Shutting down");
            }

            Console.WriteLine("Starting capture");

            StartCapture(sourceDevice, targetDevice);
        }

        private static void StartCapture(MMDevice sourceDevice, MMDevice targetDevice)
        {
            var soundIn = new WasapiLoopbackCapture {Device = sourceDevice};

            soundIn.Initialize();

            var soundOut = new WasapiOut() { Latency = 100, Device = targetDevice };
            soundOut.Initialize(new SoundInSource(soundIn));

            soundIn.Start();
            soundOut.Play();
            while (true)
            {
                if(soundOut.PlaybackState == PlaybackState.Playing)
                    Thread.Sleep(500);
                soundOut.Play();
            }
        }

        private static bool DeviceMatches(MMDevice device, string name)
        {
            return device.FriendlyName.ToLower().Contains(name.ToLower());
        }

        public enum CaptureMode
        {
            Capture,
            LoopbackCapture
        }
    }
}
