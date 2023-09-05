using System;

public class DataHandler
{
    public event Action<byte[]> DataReceived;

    public static void HandleData(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            Console.WriteLine("Invalid data");
            return;
        }

        // Process the data


        // Notify the client
        DataReceived?.Invoke(data);
    }

}