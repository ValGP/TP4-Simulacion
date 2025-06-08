using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TP4_Final.Distribuciones;
using TP4_Final;

namespace TP4_Final
{
    public partial class Simulacion : Form
    {
     
        public Simulacion()
        {
            InitializeComponent();
        }

        private void Simulacion_Load(object sender, EventArgs e)
        {

        }

        private void btnSimular_Click(object sender, EventArgs e)
        {
            // 1) Leemos y parseamos las tasas de llegada y atención
            double mdLlegMostrador = double.Parse(tasaLlegadaMostrador.Text);
            double mdLlegAutoservicio = double.Parse(tasaLlegadaAutoservicio.Text);
            double mdLlegOnline = double.Parse(tasaLlegadaOnline.Text);
            double mdLlegDelivery = double.Parse(tasaLlegadaDelivery.Text);
            double mdLlegLlevar = double.Parse(tasaLlegadaLlevar.Text);

            double mdAtendMostrador = double.Parse(tasaAtencionMostrador.Text);
            double mdAtendAutoservicio = double.Parse(tasaAtencionAutoservicio.Text);
            double mdAtendOnline = double.Parse(tasaAtencionOnline.Text);
            double mdAtendDelivery = double.Parse(tasaAtencionDelivery.Text);
            double mdAtendLlevar = double.Parse(tasaAtencionLlevar.Text);

            int mostrarDesde = int.Parse(txtMostrarDesde.Text);
            int mostrarHasta = int.Parse(txtMostrarHasta.Text);
            int totalIteraciones = int.Parse(txtCantIteraciones.Text);

            // 2) Construimos el diccionario de distribuciones
            var distribs = new Dictionary<int, Distribucion>
            {
                {1,  new Exponencial(mdLlegMostrador)},
                {2,  new Exponencial(mdLlegAutoservicio)},
                {3,  new Exponencial(mdLlegOnline)},
                {4,  new Exponencial(mdLlegDelivery)},
                {5,  new Exponencial(mdLlegLlevar)},
                {6,  new Exponencial(mdAtendMostrador)},
                {7,  new Exponencial(mdAtendAutoservicio)},
                {8,  new Exponencial(mdAtendOnline)},
                {9,  new Exponencial(mdAtendLlevar)},
                {10, new Exponencial(mdAtendDelivery)},
            };

            // 3) Creamos el gestor y simulamos
            var gestor = new GestorSimulacion(
                numIteraciones: totalIteraciones,
                mostrarDesde: mostrarDesde,
                mostrarHasta: mostrarHasta,
                distribuciones: distribs
            );
            gestor.Ejecutar();

            // 4) Volcamos al DataGridView
            var resultados = gestor.Registros.ToArray();
            CargarSimulacion(resultados);
        }

        private void CargarSimulacion(VectorEstado[] resultados)
        {
            // 1) Limpiar cualquier contenido previo
            TablaVectorEstado.Rows.Clear();
            TablaVectorEstado.Columns.Clear();

            // 2) Columnas de datos generales
            TablaVectorEstado.Columns.Add("Evento", "Evento");
            TablaVectorEstado.Columns.Add("Reloj", "Reloj (min)");

            // 3) Columnas de Llegadas (5 servicios)
            var servicios = new[] { "Mostrador", "Autoservicio", "Online", "Delivery", "Llevar" };
            foreach (var s in servicios)
            {
                TablaVectorEstado.Columns.Add($"RND_Llegada{s}", $"RND Llegada {s}");
                TablaVectorEstado.Columns.Add($"Tiempo_Llegada{s}", $"Tiempo Llegada {s}");
                TablaVectorEstado.Columns.Add($"Hora_Llegada{s}", $"Hora Llegada {s}");
            }

            // 4) Columnas de Fines de atención (uno por empleado y servicio)
            //    Tomamos los conteos de la primera iteración
            int nMostrador = resultados[0].FinesMostrador.Count;
            int nAutoservicio = resultados[0].FinesAutoservicio.Count;
            int nOnline = resultados[0].FinesOnline.Count;
            int nDelivery = resultados[0].FinesDelivery.Count;
            int nLlevar = resultados[0].FinesLlevar.Count;

            for (int i = 0; i < nMostrador; i++)
            {
                TablaVectorEstado.Columns.Add($"RND_FinMostrador_{i}", $"Rnd Fin Mostrador E{i}");
                TablaVectorEstado.Columns.Add($"Tiempo_FinMostrador_{i}", $"Tiempo Fin Mostrador E{i}");
                TablaVectorEstado.Columns.Add($"Hora_FinMostrador_{i}", $"Hora Fin Mostrador E{i}");
            }
            for (int i = 0; i < nAutoservicio; i++)
            {
                TablaVectorEstado.Columns.Add($"RND_FinAutoservicio_{i}", $"Rnd Fin Autoserv E{i}");
                TablaVectorEstado.Columns.Add($"Tiempo_FinAutoservicio_{i}", $"Tiempo Fin Autoserv E{i}");
                TablaVectorEstado.Columns.Add($"Hora_FinAutoservicio_{i}", $"Hora Fin Autoserv E{i}");
            }
            for (int i = 0; i < nOnline; i++)
            {
                TablaVectorEstado.Columns.Add($"RND_FinOnline_{i}", $"Rnd Fin Online E{i}");
                TablaVectorEstado.Columns.Add($"Tiempo_FinOnline_{i}", $"Tiempo Fin Online E{i}");
                TablaVectorEstado.Columns.Add($"Hora_FinOnline_{i}", $"Hora Fin Online E{i}");
            }
            for (int i = 0; i < nDelivery; i++)
            {
                TablaVectorEstado.Columns.Add($"RND_FinDelivery_{i}", $"Rnd Fin Delivery E{i}");
                TablaVectorEstado.Columns.Add($"Tiempo_FinDelivery_{i}", $"Tiempo Fin Delivery E{i}");
                TablaVectorEstado.Columns.Add($"Hora_FinDelivery_{i}", $"Hora Fin Delivery E{i}");
            }
            for (int i = 0; i < nLlevar; i++)
            {
                TablaVectorEstado.Columns.Add($"RND_FinLlevar_{i}", $"Rnd Fin Llevar E{i}");
                TablaVectorEstado.Columns.Add($"Tiempo_FinLlevar_{i}", $"Tiempo Fin Llevar E{i}");
                TablaVectorEstado.Columns.Add($"Hora_FinLlevar_{i}", $"Hora Fin Llevar E{i}");
            }

            // 5) Columnas de estado de empleados, hora inicio de ocupación y tiempo ocupado, y cola general por servicio
            for (int i = 0; i < resultados[0].Mostrador.Empleados.Count; i++)
            {
                TablaVectorEstado.Columns.Add($"Estado_Mostrador_{i}", $"Est M E{i}");
                TablaVectorEstado.Columns.Add($"HoraInicio_Mostrador_{i}", $"Inicio Ocu M E{i}");
                TablaVectorEstado.Columns.Add($"TiempoOcu_Mostrador_{i}", $"Tiempo Ocu M E{i}");
            }
            TablaVectorEstado.Columns.Add("ColaMostrador", "Cola Mostrador");

            for (int i = 0; i < resultados[0].Autoservicio.Empleados.Count; i++)
            {
                TablaVectorEstado.Columns.Add($"Estado_Autoserv_{i}", $"Est A E{i}");
                TablaVectorEstado.Columns.Add($"HoraInicio_Autoserv_{i}", $"Inicio Ocu A E{i}");
                TablaVectorEstado.Columns.Add($"TiempoOcu_Autoserv_{i}", $"Tiempo Ocu A E{i}");
            }
            TablaVectorEstado.Columns.Add("ColaAutoservicio", "Cola Autoservicio");

            for (int i = 0; i < resultados[0].Online.Empleados.Count; i++)
            {
                TablaVectorEstado.Columns.Add($"Estado_Online_{i}", $"Est O E{i}");
                TablaVectorEstado.Columns.Add($"HoraInicio_Online_{i}", $"Inicio Ocu O E{i}");
                TablaVectorEstado.Columns.Add($"TiempoOcu_Online_{i}", $"Tiempo Ocu O E{i}");
            }
            TablaVectorEstado.Columns.Add("ColaOnline", "Cola Online");

            for (int i = 0; i < resultados[0].Delivery.Empleados.Count; i++)
            {
                TablaVectorEstado.Columns.Add($"Estado_Delivery_{i}", $"Est D E{i}");
                TablaVectorEstado.Columns.Add($"HoraInicio_Delivery_{i}", $"Inicio Ocu D E{i}");
                TablaVectorEstado.Columns.Add($"TiempoOcu_Delivery_{i}", $"Tiempo Ocu D E{i}");
            }
            TablaVectorEstado.Columns.Add("ColaDelivery", "Cola Delivery");

            for (int i = 0; i < resultados[0].Llevar.Empleados.Count; i++)
            {
                TablaVectorEstado.Columns.Add($"Estado_Llevar_{i}", $"Est L E{i}");
                TablaVectorEstado.Columns.Add($"HoraInicio_Llevar_{i}", $"Inicio Ocu L E{i}");
                TablaVectorEstado.Columns.Add($"TiempoOcu_Llevar_{i}", $"Tiempo Ocu L E{i}");
            }
            TablaVectorEstado.Columns.Add("ColaLlevar", "Cola Llevar");

            // 6) Columnas de tiempo total de espera en cola por servicio
            TablaVectorEstado.Columns.Add("EsperaMostrador", "Espera Total M");
            TablaVectorEstado.Columns.Add("EsperaAutoservicio", "Espera Total A");
            TablaVectorEstado.Columns.Add("EsperaOnline", "Espera Total O");
            TablaVectorEstado.Columns.Add("EsperaDelivery", "Espera Total D");
            TablaVectorEstado.Columns.Add("EsperaLlevar", "Espera Total L");

            // 7) Columnas de métricas de servicio
            TablaVectorEstado.Columns.Add("OcupCompMostrador", "Ocup. Completa M");
            TablaVectorEstado.Columns.Add("OcupCompAutoservicio", "Ocup. Completa A");
            TablaVectorEstado.Columns.Add("OcupCompOnline", "Ocup. Completa O");
            TablaVectorEstado.Columns.Add("OcupCompDelivery", "Ocup. Completa D");
            TablaVectorEstado.Columns.Add("OcupCompLlevar", "Ocup. Completa L");

            // 8) Columnas de total de clientes atendidos por servicio y total general
            TablaVectorEstado.Columns.Add("TotalMostrador", "Total M");
            TablaVectorEstado.Columns.Add("TotalAutoservicio", "Total A");
            TablaVectorEstado.Columns.Add("TotalOnline", "Total O");
            TablaVectorEstado.Columns.Add("TotalDelivery", "Total D");
            TablaVectorEstado.Columns.Add("TotalLlevar", "Total L");
            TablaVectorEstado.Columns.Add("TotalGeneral", "Total General");

            // 9) Columnas de estado de cada cliente (hasta el máximo observado)
            int maxClientes = resultados.Last().ListaClientes.Count;
            for (int c = 0; c < maxClientes; c++)
            {
                TablaVectorEstado.Columns.Add($"Estado_Cliente_{c + 1}", $"Cliente {c + 1} Estado");
            }

            // 10) Finalmente, agregamos las filas
            foreach (var ve in resultados)
            {
                TablaVectorEstado.Rows.Add(ve.ToLista(maxClientes));
            }
        }

    }
}
