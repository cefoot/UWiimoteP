using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace De.Cefoot.UWiiP.Data
{
    public class ExtensionBytes8Report : WiimoteReport
    {
        public override ushort ReportID => 0x32;

        public override string ReadReport(IBuffer buffer)
        {
            var bytes = new byte[11];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            return BitConverter.ToString(bytes);
        }

        public override void ReadReport(IBuffer buffer, ref Wiimote data)
        {
            var bytes = new byte[11];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            ButtonReport.ParseButtons(bytes[1], bytes[2], ref data);
            data.ExtData = new ArraySegment<byte>(bytes, 3, 8).Array;
        }
    }
}
