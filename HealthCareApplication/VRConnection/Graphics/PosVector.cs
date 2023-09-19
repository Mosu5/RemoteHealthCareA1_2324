namespace VRConnection.Graphics{

public class PosVector
{
    public int[] pos { get; set; }
    public int[] dir { get; set; }

    public PosVector(int[] position, int[] direction)
    {
        pos = position;
        dir = direction;
    }
}
}