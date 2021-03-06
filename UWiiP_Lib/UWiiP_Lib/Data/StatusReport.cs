﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace De.Cefoot.UWiiP.Data
{
    /// <summary>
    /// http://wiibrew.org/wiki/Wiimote#0x20:_Status
    /// </summary>
    public class StatusReport : WiimoteReport
    {
        public override ushort ReportID => 0x20;

        public override string ReadReport(IBuffer buffer)
        {
            var bytes = new byte[7];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            return BitConverter.ToString(bytes);
        }

        public override void ReadReport(IBuffer buffer, ref Wiimote data)
        {
            var bytes = new byte[7];
            DataReader dataReader = DataReader.FromBuffer(buffer);
            dataReader.ReadBytes(bytes);
            ButtonReport.ParseButtons(bytes[1], bytes[2], ref data);
        }
    }
}
