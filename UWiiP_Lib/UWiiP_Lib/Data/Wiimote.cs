using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.Cefoot.UWiiP.Data
{
    public class Wiimote
    {
        public bool Button_DPadLeft { get; internal set; }
        public bool Button_DPadRight { get; internal set; }
        public bool Button_DPadDown { get; internal set; }
        public bool Button_DPadUp { get; internal set; }
        public bool Button_Plus { get; internal set; }
        public bool Button_Two { get; internal set; }
        public bool Button_One { get; internal set; }
        public bool Button_B { get; internal set; }
        public bool Button_A { get; internal set; }
        public bool Button_Minus { get; internal set; }
        public bool Button_Home { get; internal set; }
        public byte[] ExtData { get; internal set; }
        private byte _accelX, _accelY, _accelZ;
        private bool _accelSet = false;
        public byte AccelX
        {
            get
            {
                return _accelX;
            }
            internal set
            {
                _accelX = value;
                _accelSet = true;
            }
        }
        public byte AccelY
        {
            get
            {
                return _accelY;
            }
            internal set
            {
                _accelY = value;
                _accelSet = true;
            }
        }
        public byte AccelZ
        {
            get
            {
                return _accelZ;
            }
            internal set
            {
                _accelZ = value;
                _accelSet = true;
            }
        }

        public override string ToString()
        {
            var props = GetType().GetProperties();
            return "Wiimote:\r\n"
                + String.Join('\n', props.Where(p => p.Name.StartsWith("Button_")).Select(p => p.Name.Substring(7) + ":" + p.GetValue(this)))
                + (_accelSet ? "\r\nAccel:X:" + AccelX + ";Y:" + AccelY + ";Z:" + AccelZ : "")
                + (ExtData != null && ExtData.Length > 0 ? "\r\nExtData:" + String.Join(':', ExtData.Select(b => b.ToString())) : "");
        }
    }
}
