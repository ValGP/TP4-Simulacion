using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP4_Final.Distribuciones
{
    public class Exponencial : Distribucion
    {
        public double Lambda { get; private set; }
        public int SizeMuestra { get; set; }
        public List<double> RNDs { get; set; }
        public List<double> Variables { get; set; }

        public double Media => 1.0 / Lambda;

        public Exponencial(double media)
        {
            Lambda = 1.0 / media;
        }

        public Exponencial(double lambda, int sizeMuestra, List<double> rnds)
        {
            Lambda = lambda;
            SizeMuestra = sizeMuestra;
            RNDs = rnds;
        }

        public List<double> generarVariables()
        {
            Variables = new List<double>();
            for (int i = 0; i < SizeMuestra; i++)
            {
                Variables.Add(generarValor(RNDs[i]));
            }
            return Variables;
        }

        public double generarValor(double rnd)
        {
            double valor = -1.0 / Lambda * Math.Log(1 - rnd);
            return valor; // truncar solo en UI si hace falta
        }
    }

}
