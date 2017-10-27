using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace De.Cefoot.UWiimoteP.Data
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

        public override void ReadReport(ref Wiimote data)
        {
            throw new NotImplementedException();
        }
    }
}
