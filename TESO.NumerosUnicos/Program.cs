using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WMS.Entities.Common;
using WMS.Logic.Tareo;

namespace TESO.NumerosUnicos
{
    class Program
    {
        static void Main(string[] args)
        {
         
            string ruc_empresa = "";
            int idEmpresa = 0;
            int idCtaBanco = 0; 
            

            for (int x = 1; x <= 2; x++)
            {
 
                if (x == 1)
                {
                    //string esquemaREC = ConfigurationSettings.AppSettings["BD_REC_Sil"].ToString();
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_silvestre"].ToString();
                    ruc_empresa = "20191503482";
                    idEmpresa = 1;
                    idCtaBanco = 5;
                    LeerLetrasPendiente_Recibir(idEmpresa, ruc_empresa, esquemaREC, idCtaBanco);
                }

                if (x == 2)
                {
                    //string esquemaREC = ConfigurationSettings.AppSettings["BD_REC_NEO"].ToString();
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_neoagrum"].ToString();
                    ruc_empresa = "20509089923";
                    idEmpresa = 2;
                    idCtaBanco = 53;

                    LeerLetrasPendiente_Recibir(idEmpresa, ruc_empresa, esquemaREC, idCtaBanco);
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

        public static void LeerLetrasPendiente_Recibir(int id_Empresa, string ruc_empresa, string esquema, int idCtaBanco)
        {
            ReadFileFromFTP_NumerosUnicos(id_Empresa, ruc_empresa, esquema, idCtaBanco);
        }

        public static void ReadFileFromFTP_NumerosUnicos(int id_Empresa, string ruc_empresa, string esquema, int idCtaBanco)
        {
            var error = string.Empty;
            DateTime fecha1 = DateTime.Now.AddYears(-60);
            DateTime fecha2 = DateTime.Now.AddYears(+60);
            DateTime fechaActual = Convert.ToDateTime( DateTime.Now.ToShortDateString()); 
          

            try
            {
                Console.WriteLine("-----------------------------------------");
                DataTable listaProcesos = Listar_NumerosUnicos_Procesos(id_Empresa, ruc_empresa, esquema);
                Console.WriteLine("Procesos Pendientes: " + listaProcesos.Rows.Count.ToString());
                Console.WriteLine("-----------------------------------------");

                foreach (DataRow lineaProceso in listaProcesos.Rows)
                {

                    int count = 0;
                    string mensajeEtapa = ""; 
                    RowItem itemP = new RowItem();
                    itemP.v01 = lineaProceso["id"];
                    Console.WriteLine("Iniciando Proceso: " + itemP.v01.ToString());
                  

                    blTareo bl0 = new blTareo();
                    DataTable listaArchivo = Listar_NumerosUnicos(id_Empresa, int.Parse(itemP.v01.ToString()), esquema);

                    Console.WriteLine("Total: " + listaArchivo.Rows.Count.ToString());

                    if(listaArchivo.Rows.Count > 0)
                    {
                        foreach (DataRow lineaArchivo in listaArchivo.Rows)
                        {
                            try
                            {
                                count++;

                                RowItem item = new RowItem();
                                item.v01 = lineaArchivo["Correlativo"];
                                item.v02 = lineaArchivo["ID_LetraL"];
                                item.v03 = lineaArchivo["EstadoLetra"];
                                item.v04 = lineaArchivo["Cliente"];
                                item.v05 = lineaArchivo["Importe"];
                                item.v06 = lineaArchivo["NumeroUnico"];
                                item.v07 = lineaArchivo["VencimientoL"];

                                item.v08 = lineaArchivo["id_proceso"];
                                item.v09 = "";
                                item.v10 = "";
                                item.v11 = "";
                                item.v12 = lineaArchivo["Banco"];
                                item.v13 = lineaArchivo["ID"];

                                Console.WriteLine(count.ToString() + ".  ID_Letra: " + item.v02.ToString());
                                error = item.v02.ToString();


                                blTareo bl = new blTareo();

                                mensajeEtapa = "Al buscar letras. ";

                                // Buscar Letra
                                DataSet ds1 = bl.ListarDataSet(
                                string.Format(
                                //"set dateformat DMY\n" +
                                "set dateformat " + getFormatDate() + "\n" +
                                "exec " + esquema + "..VBG00591_NumerosUnicos {0}, NULL,610,NULL,NULL ,'{1}','{2}'",
                                item.v02.ToString(), fecha1, fecha2
                                )
                                );
                                //Busca registro de interfaz
                                //DataRow[] result1 = ds1.Tables[0].Select(string.Format("Kardex={0} AND Op={1}", itemGroupKardex[0].v01, itemGroupKardex[0].v02));
                                DataTable dtTabla = ds1.Tables[0];
                                DataRow[] result1 = ds1.Tables[0].Select();


                                mensajeEtapa = "Asignación de cobranza";

                                //Asignacion de cobranza
                                DataSet ds2 = bl.ListarDataSet(
                                    string.Format(
                                    //"set dateformat DMY\n" +
                                    "set dateformat " + getFormatDate() + "\n" +
                                    "declare @p1 numeric(18, 0)\n" +
                                    "set @p1 = null\n" +
                                    "exec " + esquema + "..VBG00626 @p1 output, {2}, 'Credito BCP', {0}, 'IntranetGS', 683, '{1}'\n" +
                                    "select @p1", result1[0]["Importe"], DateTime.Now.ToShortDateString(), idCtaBanco
                                    ));
                                var @p1 = ds2.Tables[0].Rows[0][0];


                                mensajeEtapa = "Agregar Numero Unico v1";
                                // Agregar Numero Unico
                                DataSet ds3 = bl.ListarDataSet(
                                   string.Format(
                                   //"set dateformat DMY\n" +
                                   "set dateformat " + getFormatDate() + "\n" +
                                   "declare @p1 numeric(18, 0)\n" +
                                   "set @p1 = null\n" +
                                   "exec " + esquema + "..VBG00627 @p1 output, {0}, '{1}'\n" +
                                   "select @p1", @p1, item.v02.ToString()
                                   ));
                                var @p2 = ds3.Tables[0].Rows[0][0];

                                mensajeEtapa = "Agregar Numero Unico v2";
                                DataSet ds4 = bl.ListarDataSet(
                                 string.Format(
                                 //"set dateformat DMY\n" +
                                 "set dateformat " + getFormatDate() + "\n" +
                                 "exec " + esquema + "..VBG02557 {0}, '{2}', '{1}'"
                                  , item.v02.ToString(), item.v06.ToString(), item.v12.ToString()
                                 ));

                                //Aprobar Letra Cobranza 

                                mensajeEtapa = "Aprobar Letra Cobranza";
                                DataSet ds5 = bl.ListarDataSet(
                                string.Format(
                                //"set dateformat DMY\n" +
                                "set dateformat " + getFormatDate() + "\n" +
                                "exec " + esquema + "..VBG00851 {0}, '{1}'"
                                 , @p1, DateTime.Now.ToShortDateString()
                                ));


                                // Actualizar Fecha Proceso
                                mensajeEtapa = " Actualizar Fecha Proceso de Letra";
                                DataSet ds6 = bl.ListarDataSet(
                                string.Format(
                                //"set dateformat DMY\n" +
                                "set dateformat " + getFormatDate() + "\n" +
                                "exec " + esquema + "..gsLetrasNumerosUnicos_Update {0} "
                                , item.v13
                                ));

                                Console.WriteLine("Mensaje: Se actualizo correctamente. ");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Mensaje: Error en la etapa: " + mensajeEtapa);
                                error = error;
                                error = ex.Message;
                            }

                        }
                        // Actualizar Fecha Proceso

                        DataSet ds7 = bl0.ListarDataSet(
                        string.Format(
                        //"set dateformat DMY\n" +
                        "set dateformat " + getFormatDate() + "\n" +
                        "exec " + esquema + "..gsProcesoLetras_Update {0} "
                        , itemP.v01
                        ));

                        DataSet ds8 = bl0.ListarDataSet(
                        string.Format(
                        //"set dateformat DMY\n" +
                        "set dateformat " + getFormatDate() + "\n" +
                        "exec " + esquema + "..uspNOTI_ProcesoNumerosUnicos {0} "
                        , itemP.v01
                        ));
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

        public static DataTable Listar_NumerosUnicos(int idEmpresa, int id_proceso, string esquema)
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
                    "exec " + esquema + "..gsNumerosUnicos_Listar {0} ",
                    id_proceso
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


        public static DataTable Listar_NumerosUnicos_Procesos(int idEmpresa, string ruc_empresa, string esquema)
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
                    "exec " + esquema + "..gsNumerosUnicos_ProcesosListar {0} ",
                    ruc_empresa
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


    }
}
