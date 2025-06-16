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
using TP4_Final.Objetos;

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

			double mdAtendEncuesta = 1.0;

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
                {9,  new Exponencial(mdAtendDelivery)},
                {10, new Exponencial(mdAtendLlevar)},
				{11, new Exponencial(mdAtendEncuesta)}
            };

            // 3) Creamos el gestor y simulamos
			bool encuestaActiva = radioEncuesta.Checked;
            bool limitarCola = chkLimitarCola.Checked;
            var gestor = new GestorSimulacion(
                numIteraciones: totalIteraciones,
                mostrarDesde: mostrarDesde,
                mostrarHasta: mostrarHasta,
                distribuciones: distribs,
				habilitarEncuesta: encuestaActiva,
                limitarCola: limitarCola
            );
            gestor.Ejecutar();

            // 4) Volcamos al DataGridView
            var resultados = gestor.Registros.ToArray();
            CargarSimulacion(resultados, encuestaActiva);

            //5) Mostrar estadisticas
            MostrarEstadisticas(resultados.Last());
        }

        private void CargarSimulacion(VectorEstado[] resultados, bool encuestaActiva)
        {
            // 1) Limpiar cualquier contenido previo
            TablaVectorEstado.Rows.Clear();
            TablaVectorEstado.Columns.Clear();

            // 2) Columnas de datos generales
            TablaVectorEstado.Columns.Add("Evento", "Evento");
            TablaVectorEstado.Columns.Add("Reloj", "Reloj (min)");
			TablaVectorEstado.Columns["Evento"].Frozen = true;
            TablaVectorEstado.Columns["Reloj"].Frozen = true;

            // 3) Columnas de Llegadas (5 servicios)
            var servicios = new List<string> { "Mostrador", "Autoservicio", "Online", "Delivery", "Llevar" };
            if (encuestaActiva) servicios.Add("Encuesta");
            foreach (var s in servicios)
            {
				if (s == "Encuesta") continue;

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
            int nEncuesta = resultados[0].FinesEncuesta.Count;

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
            if (encuestaActiva)
            {
                for (int i = 0; i < nEncuesta; i++)
                {
                    TablaVectorEstado.Columns.Add($"RND_FinEncuesta_{i}", $"Rnd Fin Encuesta E{i}");
                    TablaVectorEstado.Columns.Add($"Tiempo_FinEncuesta_{i}", $"Tiempo Fin Encuesta E{i}");
                    TablaVectorEstado.Columns.Add($"Hora_FinEncuesta_{i}", $"Hora Fin Encuesta E{i}");
                }
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

            if (encuestaActiva)
            {
                for (int i = 0; i < resultados[0].Encuesta.Empleados.Count; i++)
                {
                    TablaVectorEstado.Columns.Add($"Estado_Encuesta_{i}", $"Est En E{i}");
                    TablaVectorEstado.Columns.Add($"HoraInicio_Encuesta_{i}", $"Inicio Ocu En E{i}");
                    TablaVectorEstado.Columns.Add($"TiempoOcu_Encuesta_{i}", $"Tiempo Ocu En E{i}");
                }
                TablaVectorEstado.Columns.Add("ColaEncuesta", "Cola Encuesta");
            }

            // 6) Columnas de tiempo total de espera en cola por servicio
            TablaVectorEstado.Columns.Add("EsperaMostrador", "Espera Total M");
            TablaVectorEstado.Columns.Add("EsperaAutoservicio", "Espera Total A");
            TablaVectorEstado.Columns.Add("EsperaOnline", "Espera Total O");
            TablaVectorEstado.Columns.Add("EsperaDelivery", "Espera Total D");
            TablaVectorEstado.Columns.Add("EsperaLlevar", "Espera Total L");
            if (encuestaActiva)
                TablaVectorEstado.Columns.Add("EsperaEncuesta", "Espera Total En");

            // 7) Columnas de métricas de servicio
            TablaVectorEstado.Columns.Add("OcupCompMostrador", "Ocup. Completa M");
            TablaVectorEstado.Columns.Add("OcupCompAutoservicio", "Ocup. Completa A");
            TablaVectorEstado.Columns.Add("OcupCompOnline", "Ocup. Completa O");
            TablaVectorEstado.Columns.Add("OcupCompDelivery", "Ocup. Completa D");
            TablaVectorEstado.Columns.Add("OcupCompLlevar", "Ocup. Completa L");
            if (encuestaActiva)
                TablaVectorEstado.Columns.Add("OcupCompEncuesta", "Ocup. Completa En");

            // 8) Columnas de total de clientes atendidos por servicio y total general
            TablaVectorEstado.Columns.Add("TotalMostrador", "Total M");
            TablaVectorEstado.Columns.Add("TotalAutoservicio", "Total A");
            TablaVectorEstado.Columns.Add("TotalOnline", "Total O");
            TablaVectorEstado.Columns.Add("TotalDelivery", "Total D");
            TablaVectorEstado.Columns.Add("TotalLlevar", "Total L");
            if (encuestaActiva)
                TablaVectorEstado.Columns.Add("TotalEncuesta", "Total En");
            TablaVectorEstado.Columns.Add("TotalGeneral", "Total General");

            if (chkLimitarCola.Checked)
            {
                TablaVectorEstado.Columns.Add("PerdidosMostrador", "Perdidos M");
                TablaVectorEstado.Columns.Add("PerdidosAutoservicio", "Perdidos A");
                TablaVectorEstado.Columns.Add("PerdidosOnline", "Perdidos O");
                TablaVectorEstado.Columns.Add("PerdidosDelivery", "Perdidos D");
                TablaVectorEstado.Columns.Add("PerdidosLlevar", "Perdidos L");

                TablaVectorEstado.Columns.Add("PerdidosGeneral", "Perdidos General");
            }

            

            

            // 9) Columnas de estado de cada cliente (hasta el máximo observado)
            int maxClientes = resultados.Last().ListaClientes.Count;

            if (conListaClientes.Checked)
            {
                for (int c = 0; c < maxClientes; c++)
                {
                    TablaVectorEstado.Columns.Add($"Estado_Cliente_{c + 1}", $"Cliente {c + 1} Estado");
                }
            } else
            {

            }



                // 10) Finalmente, agregamos las filas
                foreach (var ve in resultados)
                {
					TablaVectorEstado.Rows.Add(ve.ToLista(maxClientes, encuestaActiva));
				}
        }

        private void MostrarEstadisticas(VectorEstado finalEstado)
        {
            // 1) Calcular estadisticas

            double esperaMostrador = finalEstado.ListaClientes
                .Where(c => c.Tipo == TipoCliente.Mostrador && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);

            int atendidosMostrador = finalEstado.Mostrador.TotalClientesAtendidos;

            double promedioMostrador = atendidosMostrador > 0 ? esperaMostrador / atendidosMostrador : 0.0;

            double esperaAutoservicio = finalEstado.ListaClientes
                .Where(c => c.Tipo == TipoCliente.Autoservicio && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);

            int atendidosAutoservicio = finalEstado.Autoservicio.TotalClientesAtendidos;

            double promedioAutoservicio = atendidosAutoservicio > 0 ? esperaAutoservicio / atendidosAutoservicio : 0.0;

            double esperaOnline = finalEstado.ListaClientes
                .Where(c => c.Tipo == TipoCliente.Online && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            int atendidosOnline = finalEstado.Online.TotalClientesAtendidos;
            double promedioOnline = atendidosOnline > 0 ? esperaOnline / atendidosOnline : 0.0;

            double esperaDelivery = finalEstado.ListaClientes
                .Where(c => c.Tipo == TipoCliente.Delivery && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            int atendidosDelivery = finalEstado.Delivery.TotalClientesAtendidos;
            double promedioDelivery = atendidosDelivery > 0 ? esperaDelivery / atendidosDelivery : 0.0;

            double esperaLlevar = finalEstado.ListaClientes
                .Where(c => c.Tipo == TipoCliente.Llevar && c.HoraFinEspera >= c.HoraInicioEspera)
                .Sum(c => c.TiempoEnCola);
            int atendidosLlevar = finalEstado.Llevar.TotalClientesAtendidos;
            double promedioLlevar = atendidosLlevar > 0 ? esperaLlevar / atendidosLlevar : 0.0;

            if (radioEncuesta.Checked)
            {
                double esperaEncuesta = finalEstado.ListaClientes
                    .Where(c => c.Tipo == TipoCliente.Encuesta && c.HoraFinEspera >= c.HoraInicioEspera)
                    .Sum(c => c.TiempoEnCola);
                int atendidosEncuesta = finalEstado.Encuesta.TotalClientesAtendidos;
                double promedioEncuesta = atendidosEncuesta > 0 ? esperaEncuesta / atendidosEncuesta : 0.0;
                txtEsperaEncuesta.Text = promedioEncuesta.ToString("F4");
            }
            else
            {
                txtEsperaEncuesta.Text = "0.0";
            }



                ////////////////////////////////////////////////////////////////////////////////////
                ///

                double ocupacionMostrador = finalEstado.Mostrador.Empleados
                .Sum(e => e.TiempoOcupado +
                            (e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));

            double porcentajeOcupacionMostrador = (ocupacionMostrador / (finalEstado.Reloj * finalEstado.Mostrador.Empleados.Count)) * 100;

            double ocupacionAutoservicio = finalEstado.Autoservicio.Empleados
                .Sum(e => e.TiempoOcupado +
                            (e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
            double porcentajeOcupacionAutoservicio = (ocupacionAutoservicio / (finalEstado.Reloj * finalEstado.Autoservicio.Empleados.Count)) * 100;

            double ocupacionOnline = finalEstado.Online.Empleados
                .Sum(e => e.TiempoOcupado +
                            (e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
            double porcentajeOcupacionOnline = (ocupacionOnline / (finalEstado.Reloj * finalEstado.Online.Empleados.Count)) * 100;

            double ocupacionDelivery = finalEstado.Delivery.Empleados
                .Sum(e => e.TiempoOcupado +
                            (e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
            double porcentajeOcupacionDelivery = (ocupacionDelivery / (finalEstado.Reloj * finalEstado.Delivery.Empleados.Count)) * 100;

            double ocupacionLlevar = finalEstado.Llevar.Empleados
                .Sum(e => e.TiempoOcupado +
                            (e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
            double porcentajeOcupacionLlevar = (ocupacionLlevar / (finalEstado.Reloj * finalEstado.Llevar.Empleados.Count)) * 100;

            if (radioEncuesta.Checked)
            {
                double ocupacionEncuesta = finalEstado.Encuesta.Empleados
                    .Sum(e => e.TiempoOcupado +
                                (e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
                double porcentajeOcupacionEncuesta = (ocupacionEncuesta / (finalEstado.Reloj * finalEstado.Encuesta.Empleados.Count)) * 100;
                txtOcupacionEncuesta.Text = porcentajeOcupacionEncuesta.ToString("F4");
            }
            else
            {
                txtOcupacionEncuesta.Text = "0.0";
            }

                // 2) Mostrar en el label

            txtEsperaMostrador.Text = promedioMostrador.ToString("F4");
            txtEsperaAutoservicio.Text = promedioAutoservicio.ToString("F4");
            txtEsperaOnline.Text = promedioOnline.ToString("F4");
            txtEsperaDelivery.Text = promedioDelivery.ToString("F4");
            txtEsperaLlevar.Text = promedioLlevar.ToString("F4");

            txtOcupacionMostrador.Text = porcentajeOcupacionMostrador.ToString("F4");
            txtOcupacionAutoservicio.Text = porcentajeOcupacionAutoservicio.ToString("F4");
            txtOcupacionOnline.Text = porcentajeOcupacionOnline.ToString("F4");
            txtOcupacionDelivery.Text = porcentajeOcupacionDelivery.ToString("F4");
            txtOcupacionLlevar.Text = porcentajeOcupacionLlevar.ToString("F4");

            var promedios = new Dictionary<string, double>
            {
                { "Mostrador", promedioMostrador },
                { "Autoservicio", promedioAutoservicio },
                { "Online", promedioOnline },
                { "Delivery", promedioDelivery },
                { "Para Llevar", promedioLlevar }
            };

            // Encontrar el par con el menor promedio
            var menor = promedios.Aggregate((x, y) => x.Value < y.Value ? x : y);

            // Resultado
            string servicioMinimo = menor.Key;
            double valorMinimo = menor.Value;

            // Mostrar en algún Label o consola
            minimoTiempoEspera.Text =  $"{servicioMinimo}";





        }

        private void button1_Click(object sender, EventArgs e)
        {
            tasaLlegadaAutoservicio.Text = "1.2";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tasaLlegadaAutoservicio.Text = "1.5";
        }


        /// Actividad 5 /

		private void btnSimularAct5_Click(object sender, EventArgs e)
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
				{9,  new Exponencial(mdAtendDelivery)},
				{10, new Exponencial(mdAtendLlevar)},
			};

			// 3) Creamos el gestor y simulamos
            
            var gestor = new GestorSimulacionAct5(
				numIteraciones: totalIteraciones,
				mostrarDesde: mostrarDesde,
				mostrarHasta: mostrarHasta,
				distribuciones: distribs
                );
			gestor.Ejecutar();

			// 4) Volcamos al DataGridView
			var resultados = gestor.Registros.ToArray();
			CargarSimulacionAct5(resultados);

			//5) Mostrar estadisticas
			MostrarEstadisticasAct5(resultados.Last());
		}

		private void CargarSimulacionAct5(VectorEstadoAct5[] resultados)
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

			if (conListaClientes.Checked)
			{
				for (int c = 0; c < maxClientes; c++)
				{
					TablaVectorEstado.Columns.Add($"Estado_Cliente_{c + 1}", $"Cliente {c + 1} Estado");
				}
			}
			else
			{

			}



			// 10) Finalmente, agregamos las filas
			foreach (var ve in resultados)
			{
				TablaVectorEstado.Rows.Add(ve.ToLista(maxClientes));
			}
		}

		private void MostrarEstadisticasAct5(VectorEstadoAct5 finalEstado)
		{
			// 1) Calcular estadisticas

			double esperaMostrador = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Mostrador && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);

			int atendidosMostrador = finalEstado.Mostrador.TotalClientesAtendidos;

			double promedioMostrador = atendidosMostrador > 0 ? esperaMostrador / atendidosMostrador : 0.0;

			double esperaAutoservicio = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Autoservicio && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);

			int atendidosAutoservicio = finalEstado.Autoservicio.TotalClientesAtendidos;

			double promedioAutoservicio = atendidosAutoservicio > 0 ? esperaAutoservicio / atendidosAutoservicio : 0.0;

			double esperaOnline = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Online && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);
			int atendidosOnline = finalEstado.Online.TotalClientesAtendidos;
			double promedioOnline = atendidosOnline > 0 ? esperaOnline / atendidosOnline : 0.0;

			double esperaDelivery = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Delivery && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);
			int atendidosDelivery = finalEstado.Delivery.TotalClientesAtendidos;
			double promedioDelivery = atendidosDelivery > 0 ? esperaDelivery / atendidosDelivery : 0.0;

			double esperaLlevar = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Llevar && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);
			int atendidosLlevar = finalEstado.Llevar.TotalClientesAtendidos;
			double promedioLlevar = atendidosLlevar > 0 ? esperaLlevar / atendidosLlevar : 0.0;

			////////////////////////////////////////////////////////////////////////////////////
			///

			double ocupacionMostrador = finalEstado.Mostrador.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));

			double porcentajeOcupacionMostrador = (ocupacionMostrador / (finalEstado.Reloj * finalEstado.Mostrador.Empleados.Count)) * 100;

			double ocupacionAutoservicio = finalEstado.Autoservicio.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionAutoservicio = (ocupacionAutoservicio / (finalEstado.Reloj * finalEstado.Autoservicio.Empleados.Count)) * 100;

			double ocupacionOnline = finalEstado.Online.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionOnline = (ocupacionOnline / (finalEstado.Reloj * finalEstado.Online.Empleados.Count)) * 100;

			double ocupacionDelivery = finalEstado.Delivery.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionDelivery = (ocupacionDelivery / (finalEstado.Reloj * finalEstado.Delivery.Empleados.Count)) * 100;

			double ocupacionLlevar = finalEstado.Llevar.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionLlevar = (ocupacionLlevar / (finalEstado.Reloj * finalEstado.Llevar.Empleados.Count)) * 100;

			// 2) Mostrar en el label

			txtEsperaMostrador.Text = promedioMostrador.ToString("F4");
			txtEsperaAutoservicio.Text = promedioAutoservicio.ToString("F4");
			txtEsperaOnline.Text = promedioOnline.ToString("F4");
			txtEsperaDelivery.Text = promedioDelivery.ToString("F4");
			txtEsperaLlevar.Text = promedioLlevar.ToString("F4");

			txtOcupacionMostrador.Text = porcentajeOcupacionMostrador.ToString("F4");
			txtOcupacionAutoservicio.Text = porcentajeOcupacionAutoservicio.ToString("F4");
			txtOcupacionOnline.Text = porcentajeOcupacionOnline.ToString("F4");
			txtOcupacionDelivery.Text = porcentajeOcupacionDelivery.ToString("F4");
			txtOcupacionLlevar.Text = porcentajeOcupacionLlevar.ToString("F4");

			var promedios = new Dictionary<string, double>
			{
				{ "Mostrador", promedioMostrador },
				{ "Autoservicio", promedioAutoservicio },
				{ "Online", promedioOnline },
				{ "Delivery", promedioDelivery },
				{ "Para Llevar", promedioLlevar }
			};

			// Encontrar el par con el menor promedio
			var menor = promedios.Aggregate((x, y) => x.Value < y.Value ? x : y);

			// Resultado
			string servicioMinimo = menor.Key;
			double valorMinimo = menor.Value;

			// Mostrar en algún Label o consola
			minimoTiempoEspera.Text = $"{servicioMinimo}";





		}

		private void btnSimularAct6_Click(object sender, EventArgs e)
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
				{9,  new Exponencial(mdAtendDelivery)},
				{10, new Exponencial(mdAtendLlevar)},
			};

			// 3) Creamos el gestor y simulamos
			var gestor = new GestorSimulacionAct6(
				numIteraciones: totalIteraciones,
				mostrarDesde: mostrarDesde,
				mostrarHasta: mostrarHasta,
				distribuciones: distribs
			);
			gestor.Ejecutar();

			// 4) Volcamos al DataGridView
			var resultados = gestor.Registros.ToArray();
			CargarSimulacionAct6(resultados);

			//5) Mostrar estadisticas
			MostrarEstadisticasAct6(resultados.Last());
		}

		private void CargarSimulacionAct6(VectorEstadoAct6[] resultados)
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

			if (conListaClientes.Checked)
			{
				for (int c = 0; c < maxClientes; c++)
				{
					TablaVectorEstado.Columns.Add($"Estado_Cliente_{c + 1}", $"Cliente {c + 1} Estado");
				}
			}
			else
			{

			}



			// 10) Finalmente, agregamos las filas
			foreach (var ve in resultados)
			{
				TablaVectorEstado.Rows.Add(ve.ToLista(maxClientes));
			}
		}

		private void MostrarEstadisticasAct6(VectorEstadoAct6 finalEstado)
		{
			// 1) Calcular estadisticas

			double esperaMostrador = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Mostrador && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);

			int atendidosMostrador = finalEstado.Mostrador.TotalClientesAtendidos;

			double promedioMostrador = atendidosMostrador > 0 ? esperaMostrador / atendidosMostrador : 0.0;

			double esperaAutoservicio = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Autoservicio && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);

			int atendidosAutoservicio = finalEstado.Autoservicio.TotalClientesAtendidos;

			double promedioAutoservicio = atendidosAutoservicio > 0 ? esperaAutoservicio / atendidosAutoservicio : 0.0;

			double esperaOnline = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Online && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);
			int atendidosOnline = finalEstado.Online.TotalClientesAtendidos;
			double promedioOnline = atendidosOnline > 0 ? esperaOnline / atendidosOnline : 0.0;

			double esperaDelivery = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Delivery && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);
			int atendidosDelivery = finalEstado.Delivery.TotalClientesAtendidos;
			double promedioDelivery = atendidosDelivery > 0 ? esperaDelivery / atendidosDelivery : 0.0;

			double esperaLlevar = finalEstado.ListaClientes
				.Where(c => c.Tipo == TipoCliente.Llevar && c.HoraFinEspera >= c.HoraInicioEspera)
				.Sum(c => c.TiempoEnCola);
			int atendidosLlevar = finalEstado.Llevar.TotalClientesAtendidos;
			double promedioLlevar = atendidosLlevar > 0 ? esperaLlevar / atendidosLlevar : 0.0;

			////////////////////////////////////////////////////////////////////////////////////
			///

			double ocupacionMostrador = finalEstado.Mostrador.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));

			double porcentajeOcupacionMostrador = (ocupacionMostrador / (finalEstado.Reloj * finalEstado.Mostrador.Empleados.Count)) * 100;

			double ocupacionAutoservicio = finalEstado.Autoservicio.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionAutoservicio = (ocupacionAutoservicio / (finalEstado.Reloj * finalEstado.Autoservicio.Empleados.Count)) * 100;

			double ocupacionOnline = finalEstado.Online.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionOnline = (ocupacionOnline / (finalEstado.Reloj * finalEstado.Online.Empleados.Count)) * 100;

			double ocupacionDelivery = finalEstado.Delivery.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionDelivery = (ocupacionDelivery / (finalEstado.Reloj * finalEstado.Delivery.Empleados.Count)) * 100;

			double ocupacionLlevar = finalEstado.Llevar.Empleados
				.Sum(e => e.TiempoOcupado +
							(e.Estado == EstadoEmpleado.Ocupado ? finalEstado.Reloj - e.HoraInicioOcupacion : 0.0));
			double porcentajeOcupacionLlevar = (ocupacionLlevar / (finalEstado.Reloj * finalEstado.Llevar.Empleados.Count)) * 100;

			// 2) Mostrar en el label

			txtEsperaMostrador.Text = promedioMostrador.ToString("F4");
			txtEsperaAutoservicio.Text = promedioAutoservicio.ToString("F4");
			txtEsperaOnline.Text = promedioOnline.ToString("F4");
			txtEsperaDelivery.Text = promedioDelivery.ToString("F4");
			txtEsperaLlevar.Text = promedioLlevar.ToString("F4");

			txtOcupacionMostrador.Text = porcentajeOcupacionMostrador.ToString("F4");
			txtOcupacionAutoservicio.Text = porcentajeOcupacionAutoservicio.ToString("F4");
			txtOcupacionOnline.Text = porcentajeOcupacionOnline.ToString("F4");
			txtOcupacionDelivery.Text = porcentajeOcupacionDelivery.ToString("F4");
			txtOcupacionLlevar.Text = porcentajeOcupacionLlevar.ToString("F4");

			var promedios = new Dictionary<string, double>
			{
				{ "Mostrador", promedioMostrador },
				{ "Autoservicio", promedioAutoservicio },
				{ "Online", promedioOnline },
				{ "Delivery", promedioDelivery },
				{ "Para Llevar", promedioLlevar }
			};

			// Encontrar el par con el menor promedio
			var menor = promedios.Aggregate((x, y) => x.Value < y.Value ? x : y);

			// Resultado
			string servicioMinimo = menor.Key;
			double valorMinimo = menor.Value;

			// Mostrar en algún Label o consola
			minimoTiempoEspera.Text = $"{servicioMinimo}";





		}

		private void label20_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
