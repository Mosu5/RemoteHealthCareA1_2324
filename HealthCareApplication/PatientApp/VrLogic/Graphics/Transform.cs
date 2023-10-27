using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientApp.VrLogic.Graphics
{
    public class Transform
    {
        //Attributes NOT capitalized, to correstpond with server commands
        public double[] position { get; set; }
        public double scale { get; set; }
        public double[] rotation { get; set; }
        public Transform(double scale, double[] pos, double[] rot)
        {
            rotation = rot;
            this.scale = scale;
            position = pos;
        }
    }
}
