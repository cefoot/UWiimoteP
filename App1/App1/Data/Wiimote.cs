using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace De.Cefoot.UWiimoteP.Data
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

        public override string ToString()
        {
            var props = GetType().GetProperties();
            return "Wiimote:\r\n" + String.Join('\n', props.Select(p => p.Name + ":" + p.GetValue(this)));
        }
    }
}
