using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitLib
{
    internal class Wire
    {
        public Pin StartPin;
        public Pin EndPin;

        public Wire(Pin start,Pin end)
        {
            StartPin = start;
            EndPin = end;   
        }
    }
}
