using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WMS.Entities.Common;
using WMS.Logic.Tareo;
using TESO.FacturacionSUNAT.WSComprobantes; 


namespace TESO.FacturacionSUNAT
{
    class Program
    {
        static void Main(string[] args)
        {
            string ruc_empresa = "";
            int idEmpresa = 0;

            for (int x = 1; x <= 3; x++)
            {

                if (x == 1)
                {
 
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_silvestre"].ToString();
                    ruc_empresa = "20191503482";
                    idEmpresa = 1;

                    Actualizar_RespuestaElectronica(idEmpresa, ruc_empresa, esquemaREC);
                }

                if (x == 2)
                {
                  
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_neoagrum"].ToString();
                    ruc_empresa = "20509089923";
                    idEmpresa = 2;
                    Actualizar_RespuestaElectronica(idEmpresa, ruc_empresa, esquemaREC);
                }

                if (x == 3)
                {
 
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_inatec"].ToString();
                    ruc_empresa = "20505467214";
                    idEmpresa = 6;
                    Actualizar_RespuestaElectronica(idEmpresa, ruc_empresa, esquemaREC);
                }



            }
            var format = DateTime.Now.GetDateTimeFormats();
        }

        static string getFormatDate()
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var format = culture.DateTimeFormat.ShortDatePattern;
            //Console.WriteLine(format);
            switch (format)
            {
                case "dd/MM/yyyy": return "DMY";
                case "dd/M/yyyy": return "DMY";
                case "d/M/yyyy": return "DMY";
                case "d/MM/yyyy": return "DMY";
                case "MM/d/yyyy": return "MDY";
                case "MM/dd/yyyy": return "MDY";
                case "M/dd/yyyy": return "MDY";
                case "M/d/yyyy": return "MDY";
                default: return "DMY";
            }
        }

 
        public static void Actualizar_RespuestaElectronica(int id_Empresa, string ruc_empresa, string esquema)
        {
            var error = string.Empty;
            DateTime fecha1 = DateTime.Now.AddYears(-60);
            DateTime fecha2 = DateTime.Now.AddYears(+60);
            DateTime fechaActual = Convert.ToDateTime(DateTime.Now.ToShortDateString());


            ENEmpresa objEmpresa = new ENEmpresa();
            WSComprobantes.WSComprobanteSoapClient oServicio = new WSComprobantes.WSComprobanteSoapClient();
            General oGeneral = new General();
            ENComprobante objComprobante = new ENComprobante();

            try
            {
                DataTable tblDocumentos = Documentos_Listar(id_Empresa, ruc_empresa, esquema, 0, 0);
                //DataTable lstTipoCambio = Listar_TipoCambio(id_Empresa, ruc_empresa, esquema);
                //DataTable lstTipoDocumento = Listar_TipoDocumento(id_Empresa, ruc_empresa, esquema);
                //DataTable lstSedes = Listar_Sedes(id_Empresa, ruc_empresa, esquema);
                int Bloqueo = 0;

                foreach (DataRow Documentos in tblDocumentos.Rows)
                {
                    int count = 0;
                    string mensajeEtapa = "";
                    RowItem itemProceso = new RowItem();
                    itemProceso.v01 = Documentos["RUC_Emisor"];
                    itemProceso.v02 = Documentos["Serie"];
                    itemProceso.v03 = Documentos["Numero"];
                    itemProceso.v04 = Documentos["RUC_Cliente"];
                    itemProceso.v05 = Documentos["DocSunat"];
                    itemProceso.v06 = Documentos["OpOrigen"];
                    itemProceso.v07 = Documentos["Op"];
                    itemProceso.v08 = Documentos["TablaOrigen"];

                    //------------------------ Etapa 1 ------------------------------
                    Console.WriteLine("Iniciando Proceso");
                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("ID_Proceso: " + itemProceso.v01.ToString());

                    //// Iniciando 
                    ////    Buscar detalle proceso
                    //DataTable tblProcesoDetalle = ProcesoDetalle_Listar(id_Empresa, ruc_empresa, esquema, Convert.ToInt32(itemProceso.v01), 0, 0);
                    //DataTable tblFechas = Listar_Fechas(id_Empresa, ruc_empresa, esquema, Convert.ToInt32(itemProceso.v01));


                    ENComprobanteConsulta oConsulta = new ENComprobanteConsulta();
                    ENRespuesta oRespuesta = new ENRespuesta();
                    string strCadena = "";

                    ENComprobanteN Comprobante = new ENComprobanteN();
                    ENComprobanteComunicadoBaja ComunicadoBaja = new ENComprobanteComunicadoBaja();
                    ENComprobanteResumenDiario ResumenDiario = new ENComprobanteResumenDiario();

                    // Comprobante
                    string strTipo = "";
                    strTipo = "0" + itemProceso.v05.ToString();

                    Comprobante.TipoComprobante = strTipo; 
                    Comprobante.Serie = itemProceso.v02.ToString();
                    Comprobante.Numero = itemProceso.v03.ToString();
                    Comprobante.IdComprobanteCliente = 0; 

                    // Consulta 
                    oConsulta.NComprobante = Comprobante;
                    oConsulta.NComunicacionBaja = ComunicadoBaja;
                    oConsulta.NResumenDiario = ResumenDiario;
                    oConsulta.TipoConsulta = 1;
                    oConsulta.RucEmpresa = itemProceso.v01.ToString();
                   // oConsulta.PuntoVentaConfiguracion = 0;
 
                    oRespuesta = oServicio.ConsultarComprobanteIndividual(oConsulta , ref strCadena);

                    int Respueta = 100;
                    string strRespuesta = "";

                    Respueta = oRespuesta.Respuesta;
                    strRespuesta = oRespuesta.DescripcionRespuesta; 

                    if(itemProceso.v06.ToString() == "39324")
                    {
                        Respueta = oRespuesta.Respuesta;
                        strRespuesta = oRespuesta.DescripcionRespuesta;
                    }


                    try
                    {
                        if (Respueta == 0 || Respueta == 3)
                        {
                            Actualizar_Documentos(id_Empresa, esquema, Convert.ToInt32(itemProceso.v07), Convert.ToInt32(itemProceso.v06), itemProceso.v08.ToString(), strRespuesta, 1);
                        }
                        else
                        {
                            //Actualizar_Documentos(id_Empresa, esquema, Convert.ToInt32(itemProceso.v07), Convert.ToInt32(itemProceso.v06), itemProceso.v08.ToString(), strRespuesta, 0);
                        }
                    }
                    catch(Exception ex)
                    {
                        error = error;
                        error = ex.Message;
                        Console.WriteLine("Error: " + ex.Message.ToString());
                    }
 

                }

            }
            catch (Exception ex)
            {
                error = error;
                error = ex.Message;
                Console.WriteLine("Error: " + ex.Message.ToString());
            }

        }
 
        public static DataTable Documentos_Listar(int idEmpresa, string ruc_empresa, string esquema, int OP_OV, int OP_DOC)
        {
            DataTable dtTabla = new DataTable();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet(
                    string.Format(
                    //"set dateformat DMY\n" +
                    "set dateformat " + getFormatDate() + "\n" +
                    "exec " + esquema + "..gsDocumentosEletronicos_Listar"
                    )
                    );
                //Busca registro de interfaz
                dtTabla = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }
 
        public static void Eliminar_Financimiento(int idEmpresa, string esquema, int OP_Finan)
        {
            DataTable dtTabla = new DataTable();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir

                DataSet ds2 = bl.ListarDataSet(
                         string.Format(
                         //"set dateformat DMY\n" +
                         "set dateformat " + getFormatDate() + "  \n" +
                         "exec " + esquema + "..gsEliminar_Financimiento  '{0}' \n",
                         OP_Finan
                         ));

                //dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
 
        public static void Actualizar_Documentos(int idEmpresa, string esquema, int Op, int OpOrigen, string TablaOrigen, string Respuesta, int Activo)
        {
            DataTable dtTabla = new DataTable();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir

                DataSet ds2 = bl.ListarDataSet(
                         string.Format(
                         //"set dateformat DMY\n" +
                         "set dateformat " + getFormatDate() + "  \n" +
                         "exec " + esquema + "..gsDocumentosEletronicos_Update   {0},{1}, '{2}', '{3}', {4} \n",
                         Op, OpOrigen, TablaOrigen, Respuesta, Activo
                         ));

                //dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
