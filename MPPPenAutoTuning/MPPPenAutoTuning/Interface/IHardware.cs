using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserInterface
{
    interface IHardware
    {
        bool Connect();
        bool Disconnect();
    }
}
