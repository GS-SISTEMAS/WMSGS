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
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using TESO.Amerra_Docs.CorreoWCF;
using TESO.Amerra_Docs.MerlinWCF;


//using SISGEGS.WIN.MERLIN.CorreoWCF;
//using ¿.SISGEGS.WIN.MERLIN.MerlinWCF;




namespace TESO.Amerra_Docs
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

                    Generar_Archivo(idEmpresa, ruc_empresa, esquemaREC);
                }
                
                if (x == 2)
                {

                    string esquemaREC = ConfigurationSettings.AppSettings["BD_neoagrum"].ToString();
                    ruc_empresa = "20509089923";
                    idEmpresa = 2;
                    Generar_Archivo(idEmpresa, ruc_empresa, esquemaREC);
                }
                
                //if (x == 3)
                //{

                //    string esquemaREC = ConfigurationSettings.AppSettings["BD_inatec"].ToString();
                //    ruc_empresa = "20505467214";
                //    idEmpresa = 6;
                //    Generar_Archivo(idEmpresa, ruc_empresa, esquemaREC);
                //}



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

        public static void Generar_Archivo(int id_Empresa, string ruc_empresa, string esquema)
        {
            var error = string.Empty;
            DateTime fecha1 = DateTime.Now.AddYears(-60);
            DateTime fecha2 = DateTime.Now.AddYears(+60);
            DateTime fechaActual = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            string FilePath;
            string strFecha;
            string strHora;
            string FechaNow;
            string HoraNow;
            int colum = 0;
            int row = 0;
            string observacion = "";
 
            string strVarchar;
            Stream strStreamW;
            StreamWriter strStreamWriter;

            try
            {

                DateTime FechaActual;
                DateTime HoraActual;
                FechaActual = DateTime.Now; 
                HoraActual = FechaActual.AddHours(0);

                strFecha = Convert.ToString(FechaActual.ToString("ddMMyyyy"));
                strHora = Convert.ToString(HoraActual.ToString("hhmm"));

                //FilePath = "C:\\Users\\USER\\Desktop\\Pruebas_TXT\\" + esquema + "_" + strFecha + "_" + strHora + ".xlsx";
                FilePath = "C:\\DirectorioServicios\\Amerra\\" + esquema + "_" + strFecha + "_" + strHora + ".xlsx";
                //FilePath = "\\\\10.10.1.9\\c$\\DirectorioServicios\\Amerra\\" + esquema + "_" + strFecha + "_" + strHora + ".xlsx";
                //FilePath = "C:\\DirectorioServicios\\Amerra\\Silvestre_Peru_SAC_11042018_0742.xlsx";
                
                Application xlApp = new Application();



                var file =  FilePath ;
                var fileName = Path.GetFileName(Path.ChangeExtension(file, ".xlsx"));

                DataSet xmlDataset = new DataSet();
                System.Data.DataTable tblDocumentos  = Documentos_Listar(id_Empresa, ruc_empresa, esquema, fechaActual);
                xmlDataset = tblDocumentos.DataSet;
                Console.WriteLine("-----------------------");
                Console.WriteLine("Iniciando: "+ esquema);
                Console.WriteLine("Processing...");

                ExportDataSetToExcel(xmlDataset, fileName, Path.GetDirectoryName(FilePath));
                Console.WriteLine("XLS Generado");
                
                string EmailTO = "ray.mendoza@gruposilvestre.com.pe";
                string EmailCC = "ray.mendoza@gruposilvestre.com.pe;TI@gruposilvestre.com.pe";
                string Asunto = "Información " + esquema;
                string Mensaje = "Buenos días Amerra, \n  Se envía la información solicitada."; 
                
                EnviarEmail("Amerra", EmailTO, EmailCC, Asunto, Mensaje, FilePath);
                Console.WriteLine("Corrreo Enviado");

            }
            catch (Exception ex)
            {
                error = error;
                error = ex.Message;
                Console.WriteLine("Error: " + ex.Message.ToString());
            }
        }

        public static System.Data.DataTable Documentos_Listar(int idEmpresa, string ruc_empresa, string esquema, DateTime FechaBuscar)
        {
            System.Data.DataTable dtTabla = new System.Data.DataTable();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet(
                    string.Format(
                    //"set dateformat DMY\n" +
                    "set dateformat " + getFormatDate() + "\n" +
                    "exec " + esquema + "..VBG00062_Amerra_Pendientes '{0}' ",
                    FechaBuscar.ToShortDateString()
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

        static void ExportDataSetToExcel(DataSet ds, string fileName, string FilePath)
        {
            var convertedFilesDirectory = ""; 

            convertedFilesDirectory = Path.Combine(Environment.CurrentDirectory + @"\Converted Files\");
            convertedFilesDirectory = FilePath; 

            if (!Directory.Exists(convertedFilesDirectory))
            {
                Directory.CreateDirectory(convertedFilesDirectory);
            }

            var convertedFilePath = Path.Combine(convertedFilesDirectory, fileName);

            //Creae an Excel application instance
            Excel.Application excelApp = new Excel.Application();

            // Create Wb
            var wb = excelApp.Workbooks.Add();
            wb.SaveAs(convertedFilePath);
            wb.Close();

            //Create an Excel workbook instance and open it from the predefined location
            Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(convertedFilePath);

            foreach ( System.Data.DataTable table in ds.Tables)
            {
                //Add a new worksheet to workbook with the Datatable name
                Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                excelWorkSheet.Name = table.TableName;

                for (int i = 1; i < table.Columns.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                }

                for (int j = 0; j < table.Rows.Count; j++)
                {

                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        if(k==1 || k==2)
                        {
                            excelWorkSheet.Cells[j + 2, k + 1] = "'" + table.Rows[j].ItemArray[k].ToString();
                        }
                        else
                        {
                            excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                        }
                    }

                    //if (j == 1000)
                    //{
                    //    j = 5000000;
                    //}

                }
            }

            excelWorkBook.Save();
            excelWorkBook.Close();
            excelApp.Quit();

        }

        static void EnviarEmail(string responsable, string to, string cc, string asunto, string mensaje, string FilePath )
        {

            CorreoWCFClient objCorreoWCF = new CorreoWCFClient();
            objCorreoWCF.MerlinEnvioCorreoAdjunto(responsable, to, "", cc, asunto, mensaje, FilePath);

        }
    }
}
