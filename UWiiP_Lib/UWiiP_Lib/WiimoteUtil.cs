using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;

namespace De.Cefoot.UWiiP
{
    public static class WiimoteUtil
    {
        public static async Task<HidDevice> GetWiimoteInstance(bool writePermission, Action<HidDevice, HidInputReportReceivedEventArgs> inputReportReceived = null)
        {
            HidDevice connectedDevice = null;
            var selector = HidDevice.GetDeviceSelector(1, 5);
            var devices = await DeviceInformation.FindAllAsync(selector);
            if (devices.Count > 0)
            {
                foreach (var device in devices)
                {
                    try
                    {
                        var deviceId = device.Id;

                        var foundDevice = await HidDevice.FromIdAsync(deviceId, writePermission ? FileAccessMode.ReadWrite : FileAccessMode.Read); // Does not work always returns null with ReadWrite
                        if (foundDevice == null)
                        {
                            continue;
                        }
                        // if the vendor and product IDs match up
                        connectedDevice = foundDevice;
                        Debug.WriteLine(foundDevice);
                        foundDevice.InputReportReceived += (a,b)=> inputReportReceived?.Invoke(a,b);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error:" + ex);
                        throw ex;
                    }
                }
            }
            return connectedDevice;
        }

        public static void PairWiimote(Action<DeviceWatcher, Object> enumerationCompleted = null, Action<DeviceInformation> newDeviceAdded = null, Action<DeviceInformation, DevicePairingResult> wiimotePaired = null)
        {
            DeviceWatcher watcher = DeviceInformation.CreateWatcher(
                    "System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\"",
                    null, // don't request additional properties for this sample
                    DeviceInformationKind.AssociationEndpoint);

            watcher.EnumerationCompleted += (a, b) => enumerationCompleted?.Invoke(a, b);
            watcher.Added += async (a, b) =>
            {
                newDeviceAdded?.Invoke(b);
                if (b.Name.ToLower().Contains("nintendo"))
                {
                    //ex:
                    //Bluetooth#Bluetooth4c:eb:42:de:4c:33-8c:56:c5:7c:fa:08
                    //SEE http://wiibrew.org/wiki/Wiimote#Bluetooth_Pairing for pairing process
                    var tmp = b.Id.Split("Bluetooth", StringSplitOptions.RemoveEmptyEntries);
                    tmp = tmp[1].Split('-', StringSplitOptions.RemoveEmptyEntries);
                    var address = tmp[1];
                    var addressSplit = address.Split(':');
                    var pw2 = new char[6];
                    pw2[5] = (char)(Convert.ToUInt32("0x" + addressSplit[0], 16));
                    pw2[4] = (char)(Convert.ToUInt32("0x" + addressSplit[1], 16));
                    pw2[3] = (char)(Convert.ToUInt32("0x" + addressSplit[2], 16));
                    pw2[2] = (char)(Convert.ToUInt32("0x" + addressSplit[3], 16));
                    pw2[1] = (char)(Convert.ToUInt32("0x" + addressSplit[4], 16));
                    pw2[0] = (char)(Convert.ToUInt32("0x" + addressSplit[5], 16));

                    DeviceInformationCustomPairing customPairing = b.Pairing.Custom;
                    customPairing.PairingRequested += (s, args) =>
                    {
                        args.Accept(new String(pw2));
                    };
                    var res = await customPairing.PairAsync(DevicePairingKinds.ProvidePin);
                    wiimotePaired?.Invoke(b, res);
                    Debug.WriteLine("Pairingresult:" + res.Status);
                }
            };
            watcher.Start();
        }
    }
}
