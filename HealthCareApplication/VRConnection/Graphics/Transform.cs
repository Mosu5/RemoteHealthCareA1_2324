namespace VRConnection.Graphics
{
    public class Transform
    {
        //Attributes NOT capitalized, to correstpond with server commands
        public int[] position { get; set; }
        public int scale { get; set; }
        public int[] rotation { get; set; }
        public Transform(int scale, int[] pos, int[] rot)
        {
            rotation = rot;
            this.scale = scale;
            position = pos;
        }
    }
}