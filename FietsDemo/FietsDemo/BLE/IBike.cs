using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

public interface IDataSubscriber
{
    event EventHandler ValueChanged;
    void Connect();       
}
