using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace De.Cefoot.UWiiP.Data
{
    //See http://wiibrew.org/wiki/Wiimote
    public abstract class WiimoteReport
    {
        private static Dictionary<UInt16, WiimoteReport> _knownDictionarys = new Dictionary<ushort, WiimoteReport>();

        static WiimoteReport()
        {
            Type reportType = typeof(WiimoteReport);
            var reportTypes = from typ in reportType.Assembly.GetTypes()
                              where typ != reportType
                              where reportType.IsAssignableFrom(typ)
                              select typ;
            reportTypes.ToList().ForEach(typ =>
            {
                var knownReport = (WiimoteReport)Activator.CreateInstance(typ);
                _knownDictionarys[knownReport.ReportID] = knownReport;
            });
        }

        public static WiimoteReport GetReportByID(UInt16 id)
        {
            if (!_knownDictionarys.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException("id", id, "No Report with this ID specified");
            }
            return _knownDictionarys[id];
        }

        public abstract ushort ReportID { get; }
        public abstract String ReadReport(IBuffer buffer);
        public abstract void ReadReport(IBuffer buffer, ref Wiimote data);
    }
}
