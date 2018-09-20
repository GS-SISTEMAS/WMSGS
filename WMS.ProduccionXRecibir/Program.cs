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

namespace WMS.ProduccionXRecibir
{
    class Program
    {
        static void Main(string[] args)
        {
            string EmpresaPT = "";
            string ruc_empresa = "";
            int idEmpresa = 0;

            for (int x = 1; x <= 2; x++)
            {
 
                if (x == 1)
                {
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_silvestre"].ToString();
                    ruc_empresa = "20191503482";
                    idEmpresa = 1;
                    EmpresaPT = "SILPT";

                    LeerProduccionPendienteRecibir(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);
                }

                if (x == 2)
                {
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_neoagrum"].ToString();
                    ruc_empresa = "20509089923";
                    idEmpresa = 2;
                    EmpresaPT = "NEOPT";

                    LeerProduccionPendienteRecibir(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);
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
                case "dd/MM/yyyy" : return "DMY";
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

        public static void LeerProduccionPendienteRecibir(int id_Empresa, string ruc_empresa, string esquema, string EmpresaPT)
        {
            ReadFileFromFTP(id_Empresa, ruc_empresa, esquema, EmpresaPT);
        }

        public static void ReadFileFromFTP(int id_Empresa, string ruc_empresa, string esquema, string EmpresaPT)
        {
            var error = string.Empty;
            try
            {
                DataTable lista = Listar_Pedidos_Confirmacion(id_Empresa, ruc_empresa, esquema);

                foreach (DataRow linea in lista.Rows)
                {
                   
                    List<RowItem> itemlist = new List<RowItem>();
                    RowItem item = new RowItem();
                    item.v01 = linea["CodigoAlternativo"];
                    item.v02 = linea["DOCNumeroDePackinglist"];
                    item.v03 = linea["CodigoDeArticulo"];
                    item.v04 = ""; //Nombre Articulo
                    item.v05 = linea["CodigoDeLote"];
                    item.v06 = linea["FechaFIFO"];
                    item.v07 = linea["FechaDeCaducidad"];
                    item.v08 = linea["IDUnidad"];
                    item.v09 = linea["Expr1"];
                    item.v10 = "6"; //Codigo Alamacén 
                    itemlist.Add(item);
 
 
                    var itemGroupKardex = itemlist
                    .GroupBy(be => be.v01)
                    .Select(se => new RowItem()
                    {
                        v01 = se.First().v01,
                        v02 = se.First().v02,
                        v03 = se.First().v03,
                        v04 = se.First().v04,
                        v05 = se.First().v05,
                        v06 = se.First().v06,
                        v07 = se.First().v07,
                        v08 = se.First().v08,
                        v09 = se.Sum(x => Convert.ToDouble(x.v09)),
                        v10 = se.First().v10,
                    })
                    .ToList();
              
                    try
                    {
                        blTareo bl = new blTareo();
                        //Lista Producción por recibir
                        DataSet ds1 = bl.ListarDataSet(
                            string.Format(
                            //"set dateformat DMY\n" +
                            "set dateformat " + getFormatDate() + "\n" +
                            "exec " + esquema + "..VBG01941_WMSV1 {0}, NULL,NULL,NULL,NULL,NULL,NULL,default,default,default,default,default,default,default,default,NULL,default,default,default,default,default,default,default,default,default",
                            itemGroupKardex[0].v02
                            )
                            );
                        //Busca registro de interfaz
                        //DataRow[] result1 = ds1.Tables[0].Select(string.Format("Kardex={0} AND Op={1}", itemGroupKardex[0].v01, itemGroupKardex[0].v02));
                        DataRow[] result1 = ds1.Tables[0].Select();


                        DataSet ds2 = bl.ListarDataSet(
                            string.Format(
                            //"set dateformat DMY\n" +
                            "set dateformat " + getFormatDate() + "\n" +
                            "declare @p1 numeric(18, 0)\n" +
                            "set @p1 = null\n" +
                            "exec " + esquema + "..VBG00599 @p1 output, '{0}', {3}, NULL, NULL, '{1}', NULL, 0, 0, 478, 0.0000, '{2}'\n" +
                            "select @p1", result1[0]["ClienteCodigo"], DateTime.Now.ToShortDateString(), "Interfaz WMS - Test", itemGroupKardex[0].v10
                            ));
                        var @p1 = ds2.Tables[0].Rows[0][0];


                        DataSet ds3 = bl.ListarDataSet(
                            string.Format(
                            //"set dateformat DMY\n" +
                            "set dateformat " + getFormatDate() + "\n" +
                            "exec " + esquema + "..VBG01942_WMSV01 {0},NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL",
                            itemGroupKardex[0].v02
                            ));


                        //-------------------------------

                        DataSet va1 = bl.ListarDataSet(
                        string.Format(
                        "select isnull(convert(varchar,MAX(ID_Amarre)),'null') 'IdAmarre' from  " + esquema + "..ParteProduccionLinea where TablaOrigen = 'MovProduccion' and Id_Item = '{0}'",
                        itemGroupKardex[0].v03
                        ));

                        DataRow[] resultva1 = va1.Tables[0].Select();



                        DataSet ds4 = bl.ListarDataSet(
                            string.Format(
                            "declare @p1 numeric(18, 0)\n" +
                            //"set @p1 = {6}\n"+
                            "set @p1 = null\n" +
                            "exec  " + esquema + "..VBG00597 @p1 output,{0},'MovProduccion',{1},'{2}',NULL,NULL,'5103','1020',{3},{4}.0000,0.0000,0.0000,{4}.0000,0.0000,'{5}',1.0000,{4}.0000,'{5}',{4}.0000,'{5}',{4}.0000,'...'\n" +
                            "select @p1", @p1, itemGroupKardex[0].v02, itemGroupKardex[0].v03, itemGroupKardex[0].v01, itemGroupKardex[0].v09, ds3.Tables[0].Rows[0]["UnidadPresentacion"] //, resultva1[0]["IdAmarre"]
                            ));
                        var @p2 = ds4.Tables[0].Rows[0][0];

                        DataSet ds5 = bl.ListarDataSet(
                            string.Format(
                            "set dateformat DMY\n" +
                            "exec  " + esquema + "..VBG00969 'ParteProduccion',{0}\n" +
                            "exec  " + esquema + "..VBG00972 'ParteProduccion',{0}",
                            @p1
                            ));

                        DataSet ds6 = bl.ListarDataSet(
                            string.Format(
                            "exec  " + esquema + "..VBG00167 '{0}',10,284" ,
                            ruc_empresa
                            ));
                        //Busca registro de interfaz
                        DataRow[] result2 = ds6.Tables[0].Select(string.Format("ID={0}", itemGroupKardex[0].v10));

                        //Busca el Id del lote
                        DataSet va2 = bl.ListarDataSet(
                        string.Format(
                        "select isnull(convert(varchar,max(Id_Lote)),'null') 'Id_Lote' from  " + esquema + "..tblLote where No_Lote = '{0}'",
                        itemGroupKardex[0].v05
                        ));
                        DataRow[] resultva2 = va2.Tables[0].Select();

                        DataSet ds7 = bl.ListarDataSet(
                            string.Format(
                            //"set dateformat DMY\n" +
                            "set dateformat " + getFormatDate() + "\n" +
                            "declare @p1 numeric(18, 0)\n" +
                            "set @p1 = {6}\n" +
                            "declare @p3 numeric(18, 0)\n" +
                            "set @p3 = {6}\n" +
                            "exec  " + esquema + "..VBG01024 @p1 output, {0}, @p3 output, '{1}', '{4}', 0, 0, 0, '{2}', '{3}', {5}, {5}, '...', '...'\n" +
                            "select @p1, @p3"
                            , @p2, itemGroupKardex[0].v05, itemGroupKardex[0].v06, itemGroupKardex[0].v07, result2[0]["ID_Agenda"], itemGroupKardex[0].v09, resultva2[0]["Id_Lote"]
                            ));

                        //Busca el Id del lote
                        va2 = bl.ListarDataSet(
                        string.Format(
                        "select isnull(convert(varchar,max(Item_ID)),'null') 'Item_ID', isnull(convert(varchar,max(Id_Lote)),'null') 'Id_Lote'  from  " + esquema + "..tblLote where No_Lote = '{0}'",
                        itemGroupKardex[0].v05
                        ));
                        resultva2 = va2.Tables[0].Select();

                        string query = "declare @existe int \n" +
                            "select @existe = count(1) from  " + esquema + "..ItemAlmacen where ID_Item = {2} and Id_Anexo = {1} \n" +
                            "if @existe = 0 \n" +
                            "   insert into  " + esquema + "..ItemAlmacen values('{0}',{2}, {1},0,0,0,0,NULL,NULL,NULL,NULL,NULL,1,0,0) \n";


                        string strID_Agenda = result2[0]["ID_Agenda"].ToString();
                        string strID = result2[0]["ID"].ToString();

                        DataSet ds8 = bl.ListarDataSet(
                            string.Format(query, result2[0]["ID_Agenda"], result2[0]["ID"], resultva2[0]["Item_ID"], itemGroupKardex[0].v09
                            ));


                        query = "declare @existe int \n" +
                            "select @existe = count(1) from  " + esquema + "..tblKardexLoteTotal where Item_Id = {2} and ID_Lote = {3} \n" +
                            "if @existe = 0 \n" +
                            "   insert into  " + esquema + "..tblKardexLoteTotal values('{0}',{1},{2},{3},0,{4}.00,0.00,{4}.00,{4}.00,0.00,{4}.00, null) \n" +
                            "else \n" +
                            "   update  " + esquema + "..tblKardexLoteTotal \n" +
                            "   set Und1Entrada =  {4}.00, \n" +
                            "   Und1Saldo = Und1Entrada - Und1Salida, \n" +
                            "   Und2Entrada = {4}.00, \n" +
                            "   Und2Saldo = Und2Entrada - Und2Salida \n" +
                            "   where Item_Id = {2} and ID_Lote = {3} \n";


                        DataSet ds9 = bl.ListarDataSet(
                        string.Format(query, result2[0]["ID_Agenda"], result2[0]["ID"], itemGroupKardex[0].v01, resultva2[0]["Id_Lote"], itemGroupKardex[0].v09
                        ));


                        DataSet ds10 = bl.ListarDataSet(
                        string.Format(
                        "set dateformat DMY\n" +
                        "exec  " + esquema + "..VBG00871 'ParteProduccion',{0}",
                        @p1
                        ));

                        DataSet ds11 = bl.ListarDataSet(
                         string.Format(
                         "set dateformat DMY\n" +
                         "exec  " + esquema + "..Cargar_Recepcion_UpdateEstilos '{1}','{0}'",
                         linea["NumeroDeAlbaran"], EmpresaPT
                         ));


                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        public static DataTable Listar_Pedidos_Confirmacion(int idEmpresa, string ruc_empresa, string esquema)
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
                    "exec " + esquema + "..Cargar_Recepcion_Confirmacion {0}",
                    ruc_empresa
                    )
                    );
                //Busca registro de interfaz
                dtTabla  = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }
    }
}
