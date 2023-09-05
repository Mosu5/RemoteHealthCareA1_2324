using System;

class Test
{
     static async Task main(string[] args)
    {
        IBike bleBike = new BLEBike();
        DataHandler dataHandler = new DataHandler(bleBike);
    }
}
