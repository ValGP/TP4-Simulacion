using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP4_Final.Distribuciones
{
    public interface Distribucion
    {
        List<double> generarVariables();
        double generarValor(double v);
    }
}
