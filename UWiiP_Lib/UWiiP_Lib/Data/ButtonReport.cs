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
    public class ButtonReport : WiimoteReport
    {
        public override ushort ReportID => 0x30;

        public override string ReadReport(IBuffer buffer)
        {
            var bytes = new byte[3];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            return BitConverter.ToString(bytes);
        }

        public override void ReadReport(IBuffer buffer, ref Wiimote data)
        {
            var bytes = new byte[3];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            ParseButtons(bytes[1], bytes[2], ref data);
        }

        /// <summary>
        /// http://wiibrew.org/wiki/Wiimote#Buttons
        /// </summary>
        /// <param name="bit1"></param>
        /// <param name="bit2"></param>
        /// <param name="data"></param>
        public static void ParseButtons(byte bit1, byte bit2, ref Wiimote data)
        {
            data.Button_DPadLeft = (0x01 & bit1) == 0x01;
            data.Button_DPadRight = (0x02 & bit1) == 0x02;
            data.Button_DPadDown = (0x04 & bit1) == 0x04;
            data.Button_DPadUp = (0x08 & bit1) == 0x08;
            data.Button_Plus = (0x10 & bit1) == 0x10;
            data.Button_Two = (0x01 & bit2) == 0x01;
            data.Button_One = (0x02 & bit2) == 0x02;
            data.Button_B = (0x04 & bit2) == 0x04;
            data.Button_A = (0x08 & bit2) == 0x08;
            data.Button_Minus = (0x10 & bit2) == 0x10;
            data.Button_Home = (0x80 & bit2) == 0x60;
        }
    }
}
