using System;

public class DataHandler

    public IBike Bike { get; set; }

    public DataHandler(IBike bike)
    {
        Bike = bike;
        bike.ValueChanged += OnValueChanged;

        bike.Connect();
        bike.ValueChanged();
    }

    private static void OnValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
    {
    byte[] receivedData = e.Data;
    HandleData(receivedData);
    }

  

    public static void HandleData(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            Console.WriteLine("Invalid data");
            return;
        }

        
    // Process the data
    byte dataType = data[4];
    int speed;
    int distance;
    
    if (dataType == 0x10)
    {
        // calculate values
        
    } else if (dataType == 0x19)
    {
        // calculate values
    }
    
    
    
    
    }



// public static string ByteArrayToString(byte[] ba)
// {
//     String hexString = BitConverter.ToString(ba).ToLower().Replace("-", " 0x");
//     StringBuilder sb = new StringBuilder();
//     sb.Append("0x");
//
//     sb.Append(hexString);
//
//     return sb.ToString();
// }

}