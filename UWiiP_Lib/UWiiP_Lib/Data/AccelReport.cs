using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace De.Cefoot.UWiiP.Data
{
    /// <summary>
    /// 0x30:_Core_Buttons
    /// </summary>
    public class AccelReport : WiimoteReport
    {
        public override ushort ReportID => 0x31;

        public override string ReadReport(IBuffer buffer)
        {
            var bytes = new byte[6];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            return BitConverter.ToString(bytes);
        }

        public override void ReadReport(IBuffer buffer, ref Wiimote data)
        {
            var bytes = new byte[6];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            ButtonReport.ParseButtons(bytes[1], bytes[2], ref data);
            data.AccelX = bytes[3];
            data.AccelY = bytes[4];
            data.AccelZ = bytes[5];
        }

    }
}
