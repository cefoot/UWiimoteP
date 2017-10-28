using De.Cefoot.UWiimoteP.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace App1
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        HidDevice wiimote;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnName_Click(object sender, RoutedEventArgs e)
        {
            var selector = HidDevice.GetDeviceSelector(1, 5);
            var devices = await DeviceInformation.FindAllAsync(selector);
            if (devices.Count > 0)
            {
                foreach (var device in devices)
                {
                    try
                    {

                        var deviceId = device.Id;
                        var foundDevice = await HidDevice.FromIdAsync(deviceId, FileAccessMode.Read); // Does not work always returns null
                        if (foundDevice == null) continue;
                        // if the vendor and product IDs match up
                        wiimote = foundDevice;
                        Debug.WriteLine(foundDevice);
                        foundDevice.InputReportReceived += FoundDevice_InputReportReceived;
                        //for (ushort i = 46765; i <= ushort.MaxValue; i++)
                        //{
                        try
                        {
                            var report = await wiimote.GetInputReportAsync(0x20);
                            ReadReport(report);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

                        //}
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error:" + ex);
                        throw ex;
                    }
                }
            }
        }

        private async void ReadReport(HidInputReport report)
        {
            var res = WiimoteReport.GetReportByID(report.Id);
            var wiimote = new Wiimote();
            res.ReadReport(report.Data, ref wiimote);
            //var txt = report.Id + ":" + res.ReadReport(report.Data);
            var txt = wiimote.ToString();
            await tbStatus.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => tbStatus.Text = txt);
        }

        private void FoundDevice_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            ReadReport(args.Report);
        }
    }
}
