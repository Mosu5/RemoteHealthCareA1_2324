using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

public class BLEBike : Bike
{
	private BLE bleDevice;
    public event EventHandler<BLESubscriptionValueChangedEventArgs> ValueChanged;


    public BLEBike()
	{
		bleDevice = new BLE();
	}

	public void Connect()
	{
        List<String> bleBikeList = bleBike.ListDevices();
        Console.WriteLine("Devices found: ");
        foreach (var name in bleBikeList)
        {
            Console.WriteLine($"Device: {name}");
        }

        // Connecting
        errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 01140"); // change with variable

        // Set service
        errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e"); // change with variable

  

        // Subscribe
      
        errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");

    }




}
