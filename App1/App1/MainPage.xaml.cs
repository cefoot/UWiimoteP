using De.Cefoot.UWiiP;
using De.Cefoot.UWiiP.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
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

        private async void BtnName_Click(object sender, RoutedEventArgs e)
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


                        var foundDevice = await HidDevice.FromIdAsync(deviceId, chkWrite.IsChecked.HasValue && chkWrite.IsChecked.Value ? FileAccessMode.ReadWrite : FileAccessMode.Read); // Does not work always returns null
                        if (foundDevice == null)
                        {
                            await SetStatusText("No Device found");
                            continue;
                        }
                        // if the vendor and product IDs match up
                        wiimote = foundDevice;
                        Debug.WriteLine(foundDevice);
                        foundDevice.InputReportReceived += FoundDevice_InputReportReceived;
                        try
                        {
                            var report = await wiimote.GetInputReportAsync(0x20);
                            ReadReport(report);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error:" + ex);
                        throw ex;
                    }
                }
            }
        }

        private async void BtnSts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rep = wiimote.CreateOutputReport(0x16);
                var idx = 0;
                var buffer = new byte[22];
                buffer[idx++] = 0x16;//report ID
                buffer[idx++] = 0x04;//address space
                buffer[idx++] = 0xA4;//address  (4)A400F0
                buffer[idx++] = 0x00;//address
                buffer[idx++] = 0xF0;//address
                buffer[idx++] = 0x00;//size
                buffer[idx++] = 0x01;//size
                buffer[idx++] = 0x55;//data


                for (int i = idx; i < buffer.Length; i++)
                {
                    buffer[i] = 0x00;
                }

                DataWriter dataWriter = new DataWriter();
                dataWriter.WriteBytes(buffer);
                IBuffer buffer1 = dataWriter.DetachBuffer();
                Debug.WriteLine("size:" + buffer.Length + " contend:" + buffer);
                rep.Data = buffer1;
                await wiimote.SendOutputReportAsync(rep);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void ReadReport(HidInputReport report)
        {
            var res = WiimoteReport.GetReportByID(report.Id);
            var wiimote = new Wiimote();
            res.ReadReport(report.Data, ref wiimote);
            //var txt = report.Id + ":" + res.ReadReport(report.Data);
            var txt = report.Id + ":\r\n" + wiimote.ToString();
            await SetStatusText(txt);
        }

        private async System.Threading.Tasks.Task SetStatusText(string txt, bool append = false)
        {
            await tbStatus
                            .Dispatcher
                            .TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                if (append)
                                {
                                    tbStatus.Text += "\r\n" + txt;
                                }
                                else
                                {
                                    tbStatus.Text = txt;
                                }
                            });
        }

        private void FoundDevice_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            ReadReport(args.Report);
        }

        private async void BtnPin_Click(object sender, RoutedEventArgs e)
        {
            WiimoteUtil.PairWiimote(
                async (a, p) => await SetStatusText("Bluetooth search done!", true),
                async a => await SetStatusText("new Device:" + a.Name, true)
                );
            await SetStatusText("Bluetooth search started");
        }

        private void txtReport_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var c = (char)e.Key;
            e.Handled = !((c >= 48 && c <= 57) || c == 46 || c == 'X');
        }

        private async void btnReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = await wiimote.GetInputReportAsync(Convert.ToUInt16(tbReportNum.Text, 16));
                ReadReport(report);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/cefoot/UWiimoteP"));
        }
    }
}
