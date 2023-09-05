using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

public interface IBike
{
    event EventHandler<BLESubscriptionValueChangedEventArgs> SubcriptionValueChanged;
    void Connect();       
}
