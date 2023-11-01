using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientApp.VrLogic.Graphics
{
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
