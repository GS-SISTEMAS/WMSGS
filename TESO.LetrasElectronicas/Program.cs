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

namespace TESO.LetrasElectronicas
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
                    //string esquemaREC = ConfigurationSettings.AppSettings["BD_REC_Sil"].ToString();
                    string esquemaREC = ConfigurationSettings.AppSettings["BD_silvestre"].ToString();
                    ruc_empresa = "20191503482";
                    idEmpresa = 1;

                    Generar_LetrasElectornicas(idEmpresa, ruc_empresa, esquemaREC);
                }

                if (x == 2)
                {

                    string esquemaREC = ConfigurationSettings.AppSettings["BD_neoagrum"].ToString();
                    ruc_empresa = "20509089923";
                    idEmpresa = 2;

                    Generar_LetrasElectornicas(idEmpresa, ruc_empresa, esquemaREC);
                }

                //if (x == 3)
                //{

                //    string esquemaREC = ConfigurationSettings.AppSettings["BD_inatec"].ToString();
                //    ruc_empresa = "20505467214";
                //    idEmpresa = 6;

                //    Generar_LetrasElectornicas(idEmpresa, ruc_empresa, esquemaREC);
                //} 
                //USUARIO ANDY VERA



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

        public static void Generar_LetrasElectornicas(int id_Empresa, string ruc_empresa, string esquema)
        {
            var error = string.Empty;
            DateTime fecha1 = DateTime.Now.AddYears(-60);
            DateTime fecha2 = DateTime.Now.AddYears(+60);
            DateTime fechaActual = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            int auxId = 0;

            try
            {
                DataTable tblProceso = Proceso_Listar(id_Empresa, ruc_empresa, esquema, 0, 0);
                DataTable lstTipoCambio = Listar_TipoCambio(id_Empresa, ruc_empresa, esquema);
                DataTable lstTipoDocumento = Listar_TipoDocumento(id_Empresa, ruc_empresa, esquema);
                DataTable lstSedes = Listar_Sedes(id_Empresa, ruc_empresa, esquema);
                int Bloqueo = 0;

                Console.WriteLine("-----------------------------------------");
                Console.WriteLine(esquema);
                Console.WriteLine("-----------------------------------------");

                foreach (DataRow Proceso in tblProceso.Rows)
                {
                    int count = 0;
                    string mensajeEtapa = "";
                    RowItem itemProceso = new RowItem();
                    itemProceso.v01 = Proceso["Id_Proceso"];
                    auxId = Convert.ToInt32(itemProceso.v01);
                    try
                    {
                        //------------------------ Etapa 1 ------------------------------
                        Console.WriteLine("Iniciando Proceso");
                        Console.WriteLine("-----------------------------------------");
                        Console.WriteLine("ID_Proceso: " + itemProceso.v01.ToString());

                        // Iniciando 
                        //    Buscar detalle proceso
                        DataTable tblProcesoDetalle = ProcesoDetalle_Listar(id_Empresa, ruc_empresa, esquema, Convert.ToInt32(itemProceso.v01), 0, 0);
                        DataTable tblFechas = Listar_Fechas(id_Empresa, ruc_empresa, esquema, Convert.ToInt32(itemProceso.v01));


                        int CountDOC_PRO = 0;
                        int CountDOC_Gsys = 0;

                        //-----------------------------------
                        CountDOC_PRO = tblProcesoDetalle.Rows.Count;

                        foreach (DataRow iProceso_D in tblProcesoDetalle.Rows)
                        {
                            RowItem itemPEDIDO = new RowItem();
                            itemPEDIDO.v01 = iProceso_D["OP_OV"];
                            DataTable lstDocumentos = Listar_DocumentosPendiente(id_Empresa, ruc_empresa, esquema, Convert.ToInt32(itemPEDIDO.v01));
                            if (lstDocumentos.Rows.Count > 0)
                            {
                                CountDOC_Gsys++;
                            }
                        }

                        if (CountDOC_PRO == CountDOC_Gsys & CountDOC_PRO > 0 & CountDOC_Gsys > 0)
                        {
                            //-----------------------------------
                            decimal TotalImporte = 0;
                            //------------------------ Etapa 2 ------------------------------
                            if (tblFechas.Rows.Count > 0)
                            {

                                var @Id_Direccion = 0;
                                var @Total = 0.00;
                                double @NroPeriodos = 0;
                                double @Cuota = 0.00;
                                var @ID_DocumentoLetra = lstTipoDocumento.Rows[0]["ID"];
                                var @ID_Moneda = 0;
                                var @Id_Cliente = "";
                                double CuotaTemp = 0.00;


                                List<RowItem> listDocumentos = new List<RowItem>();

                                foreach (DataRow iProceso_D in tblProcesoDetalle.Rows)
                                {
                                    RowItem itemPEDIDO = new RowItem();
                                    itemPEDIDO.v01 = iProceso_D["OP_OV"];

                                    //    Buscar Documentos 
                                    DataTable lstDocumentos = Listar_DocumentosPendiente(id_Empresa, ruc_empresa, esquema, Convert.ToInt32(itemPEDIDO.v01));

                                    if (lstDocumentos.Rows.Count > 0)
                                    {
                                        foreach (DataRow Documento in lstDocumentos.Rows)
                                        {
                                            Console.WriteLine("-----------------------------------------");
                                            Console.WriteLine("Documentos Procesar: " + lstDocumentos.Rows.Count.ToString());
                                            Console.WriteLine("-----------------------------------------");

                                            RowItem itemDOC = new RowItem();
                                            itemDOC.v01 = Documento["Op"];
                                            itemDOC.v02 = Documento["Id_Agenda"];
                                            itemDOC.v03 = Documento["ID_Doc"];
                                            itemDOC.v04 = Documento["ID_Moneda"];
                                            itemDOC.v05 = Documento["OP_OV"];

                                            ///   Buscar Cliente
                                            DataTable tblCliente = Listar_Cliente(id_Empresa, ruc_empresa, esquema, itemDOC.v02.ToString());
                                            ///   Buscar Direccion Cliente
                                            DataTable tblDireccion = Listar_Direcciones(id_Empresa, ruc_empresa, esquema, itemDOC.v02.ToString());
                                            ///   Buscar Documentos
                                            DataTable tblDocumento = Listar_Documento(id_Empresa, ruc_empresa, esquema, itemDOC.v03.ToString());

                                            TotalImporte = TotalImporte + Convert.ToDecimal(tblDocumento.Rows[0]["Importe"]);

                                            itemDOC.v06 = Convert.ToDecimal(tblDocumento.Rows[0]["Importe"]);
                                            listDocumentos.Add(itemDOC);

                                            //-----------Varibles 
                                            @Id_Direccion = Convert.ToInt32(tblDireccion.Rows[0]["ID"]);
                                            @Total = Convert.ToDouble(TotalImporte);
                                            @NroPeriodos = tblFechas.Rows.Count;
                                            @Cuota = (@Total / @NroPeriodos);
                                            @ID_DocumentoLetra = lstTipoDocumento.Rows[0]["ID"];
                                            @ID_Moneda = Convert.ToInt32(itemDOC.v04);
                                            @Id_Cliente = itemDOC.v02.ToString();
                                            //----------------------------------------


                                            // Buscar Financiamiento de OP
                                            DataTable tblFinanOLD = Buscar_Financiamiento(id_Empresa, esquema, itemDOC.v02.ToString(), Convert.ToInt32(itemDOC.v03));
                                            var @Op_FinOld = 0;
                                            var @Id_AmarreOLD = 0;

                                            DataTable tblDocumentos = new DataTable();
                                            DataTable tblLetras = new DataTable();
                                            DataTable tblBloqueo = new DataTable();

                                            if (tblFinanOLD.Rows.Count > 0)
                                            {
                                                @Op_FinOld = Convert.ToInt32(tblFinanOLD.Rows[0][0]);
                                                DataSet ListaFinan = Listar_FinanciamientoTotal(id_Empresa, esquema, Convert.ToInt32(@Op_FinOld));

                                                tblDocumentos = ListaFinan.Tables[0];
                                                @Id_AmarreOLD = Convert.ToInt32(tblDocumentos.Rows[0][0]);

                                                tblLetras = ListaFinan.Tables[1];
                                                tblBloqueo = ListaFinan.Tables[2];
                                                /*
                                                foreach (DataRow rowB in tblBloqueo.Rows)
                                                {
                                                    string Respuesta = rowB[0].ToString();

                                                    if (Respuesta == "True")
                                                    {
                                                        Bloqueo++;
                                                    }
                                                }
                                                */
                                                if (Bloqueo == 0)
                                                {
                                                    // Eliminar Letras 
                                                    foreach (DataRow iletra in tblLetras.Rows)
                                                    {
                                                        RowItem itemletra = new RowItem();
                                                        itemletra.v01 = iletra["Id_Amarre"];
                                                        Eliminar_Letra(id_Empresa, esquema, Convert.ToInt32(itemletra.v01));
                                                    }
                                                    Eliminar_Financimiento(id_Empresa, esquema, @Op_FinOld);

                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("-----------------------------------------");
                                        Console.WriteLine("No existe la OP: " + itemPEDIDO.v01.ToString() + "  en documentos pendientes. ");
                                        Console.WriteLine("-----------------------------------------");
                                    }

                                }

                                if (Bloqueo == 0)
                                {

                                    //----------------------------------------
                                    // Registrar Cabecera Financiamiento
                                    DataTable tblFinanCab = Finan_Cabecera_Registrar(id_Empresa, ruc_empresa, esquema, 0, @ID_Moneda, @Id_Cliente, Convert.ToInt32(@Id_Direccion),
                                                                                 Convert.ToDecimal(@Total), Convert.ToInt32(@NroPeriodos), Convert.ToDecimal(@Cuota), Convert.ToInt32(@ID_DocumentoLetra));
                                    var @Op_Fin = tblFinanCab.Rows[0][0];


                                    // Registrar Detalle Financiamiento
                                    foreach (RowItem iDoc in listDocumentos)
                                    {
                                        DataTable tblFinanDet = Finan_Detalle_Registrar(id_Empresa, ruc_empresa, esquema, 0, Convert.ToInt32(@Op_Fin), Convert.ToInt32(iDoc.v03), Convert.ToDecimal(iDoc.v06));
                                        var @pId_Amarre_Fin = tblFinanDet.Rows[0][0];
                                    }

                                    //----------------------------------------
                                    // Registrar Letras 
                                    int Periodo = 1;

                                    // Registrar Letras
                                    //int Op_Pedido = 0;
                                    CuotaTemp = 0;

                                    foreach (DataRow Letras in tblFechas.Rows)
                                    {
                                        string mensajeLetra = "";
                                        RowItem itemLetra = new RowItem();
                                        DataTable tblLetra = new DataTable();
                                        itemLetra.v01 = Letras["Fecha"];
                                        //Op_Pedido = Convert.ToInt32(Letras["Op"]);

                                        var FechaEmision = DateTime.Now.ToShortDateString();

                                        if (@NroPeriodos == 1)
                                        {
                                            tblLetra = Letras_Registrar(id_Empresa, ruc_empresa, esquema, Periodo, Convert.ToInt32(@Op_Fin), @Id_Cliente,
                                                                @ID_Moneda, Convert.ToDecimal((@Cuota)), Convert.ToDateTime(FechaEmision), Convert.ToDateTime(itemLetra.v01), "");
                                        }
                                        else
                                        {
                                            if (@NroPeriodos == Periodo)
                                            {
                                                CuotaTemp = @Total - CuotaTemp;
                                                tblLetra = Letras_Registrar(id_Empresa, ruc_empresa, esquema, Periodo, Convert.ToInt32(@Op_Fin), @Id_Cliente,
                                                                @ID_Moneda, Convert.ToDecimal(CuotaTemp), Convert.ToDateTime(FechaEmision), Convert.ToDateTime(itemLetra.v01), "");
                                            }
                                            else
                                            {
                                                CuotaTemp = CuotaTemp + Math.Truncate(@Cuota);
                                                tblLetra = Letras_Registrar(id_Empresa, ruc_empresa, esquema, Periodo, Convert.ToInt32(@Op_Fin), @Id_Cliente,
                                                                @ID_Moneda, Convert.ToDecimal(Math.Truncate(@Cuota)), Convert.ToDateTime(FechaEmision), Convert.ToDateTime(itemLetra.v01), "");
                                            }
                                        }


                                        var @ID_Letra = tblLetra.Rows[0][0];

                                        //---------------------------------------
                                        // Actualizar Fechas Letras
                                        Actualizar_Fechas_Letra(id_Empresa, esquema, Convert.ToInt32(itemProceso.v01), Convert.ToInt32(@ID_Letra), 1, Convert.ToDateTime(itemLetra.v01));

                                        //---------------------------------------
                                        Periodo++;
                                    }



                                    // Actualizar proceso de letras
                                    Actualizar_Proceso_Letra(id_Empresa, esquema, Convert.ToInt32(itemProceso.v01), 1);

                                    foreach (RowItem iDoc in listDocumentos)
                                    {
                                        Actualizar_ProcesoDetalle_Letra(id_Empresa, esquema, Convert.ToInt32(itemProceso.v01), Convert.ToInt32(iDoc.v05), Convert.ToInt32(iDoc.v01), 1);
                                        //Convert.ToInt32(iDoc.v01), 1);
                                        RegistrarLetraEmail(id_Empresa, esquema, Convert.ToInt32(iDoc.v01).ToString(), @Id_Cliente);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string strerr = "La cantidad de documentos del financiamiento, es distinta a la cantidad en genesys. ";
                            Console.WriteLine("-----------------------------------------");
                            Console.WriteLine(strerr);
                            Console.WriteLine("-----------------------------------------");

                            blTareo bl = new blTareo();
                            DataSet ds1 = bl.ListarDataSet(
                                string.Format(
                                "set dateformat " + getFormatDate() + "\n" +
                                "update " + esquema + "..gsPedidoLetras_Proceso \n" +
                                "set Estado = 2 \n" +
                                "where Id_Proceso = {0}\n" +
                                "insert into " + esquema + "..gsPedidoLetras_Proceso_Transact(ID_Proceso, Error, DetalleError) values ({0}, {1}, '{2}') \n " +
                                "select {0} \n ",
                                Convert.ToInt32(itemProceso.v01), 1, strerr
                                )
                                );
                        }

                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        Console.WriteLine("Error: " + ex.Message.ToString());
                        string strerr = ex.Message;
                        strerr = strerr.Replace("\n", ". ").Replace("\"", "").Replace("'", "");
                        Console.WriteLine("-----------------------------------------");
                        Console.WriteLine(strerr);
                        Console.WriteLine("-----------------------------------------");

                        blTareo bl = new blTareo();
                        DataSet ds1 = bl.ListarDataSet(
                            string.Format(
                            "set dateformat " + getFormatDate() + "\n" +
                            "update " + esquema + "..gsPedidoLetras_Proceso \n" +
                            "set Estado = 2 \n" +
                            "where Id_Proceso = {0}\n" +
                            "insert into " + esquema + "..gsPedidoLetras_Proceso_Transact(ID_Proceso, Error, DetalleError) values ({0}, {1}, '{2}') \n " +
                            "select {0} \n ",
                            Convert.ToInt32(itemProceso.v01), 1, strerr
                            )
                            );
                    }
                }
            }
            catch (Exception ex)
            {

                error = ex.Message;
                Console.WriteLine("Error: " + ex.Message.ToString());
                string strerr = ex.Message;
                strerr = strerr.Replace("\n", ". ").Replace("\"", "").Replace("'", "");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine(strerr);
                Console.WriteLine("-----------------------------------------");

                blTareo bl = new blTareo();
                DataSet ds1 = bl.ListarDataSet(
                    string.Format(
                    "set dateformat " + getFormatDate() + "\n" +
                    "update " + esquema + "..gsPedidoLetras_Proceso \n" +
                    "set Estado = 2 \n" +
                    "where Id_Proceso = {0}\n" +
                    "insert into " + esquema + "..gsPedidoLetras_Proceso_Transact(ID_Proceso, Error, DetalleError) values ({0}, {1}, '{2}') \n " +
                    "select {0} \n ",
                    Convert.ToInt32(auxId), 1, strerr
                    )
                    );
            }

        }

        public static void RegistrarLetraEmail(int idempresa, string esquema, string op_docventa, string id_agenda)
        {
            try
            {
                blTareo bl = new blTareo();
                bl.RegistrarLetraEmail("exec " + esquema + "..USP_INS_EnvioLetrasEmail " + op_docventa.ToString() + ",'" + id_agenda + "'");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable Listar_DocumentosPendiente(int idEmpresa, string ruc_empresa, string esquema, int OP_OV)
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
                    "exec " + esquema + "..USP_SEL_CtaCte {0}",
                    //"exec " + esquema + "..VBG00062_LetrasElectronicas {0}, null, null, null, 506, null, default, default, default, default, default, default, default, default " ,
                    OP_OV
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

        public static DataTable Listar_TipoCambio(int idEmpresa, string ruc_empresa, string esquema)
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
                    "exec " + esquema + "..VBG00187"
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

        public static DataTable Listar_TipoDocumento(int idEmpresa, string ruc_empresa, string esquema)
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
                    "exec " + esquema + "..VBG01122 10"
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

        public static DataTable Listar_Sedes(int idEmpresa, string ruc_empresa, string esquema)
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
                    "exec " + esquema + "..VBG02689"
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

        public static DataTable Listar_Cliente(int idEmpresa, string ruc_empresa, string esquema, string Id_Cliente)
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
                    "declare @p3 varchar(50)\n " +
                    "declare @p4 bit \n " +
                    "set @p3 = null\n" +
                    "set @p4 = null\n" +
                    "exec " + esquema + "..VBG01134 '{0}', 1, @p3 output, @p4 output ",
                    Id_Cliente
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

        public static DataTable Listar_Direcciones(int idEmpresa, string ruc_empresa, string esquema, string Id_Cliente)
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
                    "exec " + esquema + "..VBG00209 'Agenda', '{0}', 104 ",
                    Id_Cliente
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
        public static DataTable Listar_Documento(int idEmpresa, string ruc_empresa, string esquema, string ID_Documento)
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
                    "exec " + esquema + "..VBG01362_Letra  '{0}' ",
                    ID_Documento
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

        public static DataTable Listar_Fechas(int idEmpresa, string ruc_empresa, string esquema, int Id_Amarre)
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
                    "exec " + esquema + "..gsFechasOP_Listar {0}  ",
                    Id_Amarre
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

        public static DataTable Finan_Cabecera_Registrar(int idEmpresa, string ruc_empresa, string esquema, int OpOLD, int ID_Moneda, string ID_Aceptante, int ID_DireccionAceptante,
        decimal Total, int NroPeriodos, decimal Cuota, int ID_DocumentoLetra)
        {
            DataTable dtTabla = new DataTable();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir

                DataSet ds2 = bl.ListarDataSet(
                         string.Format(
                         //"set dateformat DMY\n" +
                         "set dateformat " + getFormatDate() + "\n" +
                         "declare @p1 numeric(18, 0)\n" +
                         "set @p1 = {9} \n" +
                         "exec " + esquema + "..VBG00686 @p1 output, '{0}', {1}, '{2}', null, {3}, NULL,{4}, null,null,null, '{5}', {6}, 0, {7},0,0, {8}, null,1 \n" +
                         "select @p1",
                         DateTime.Now.ToShortDateString(), ID_Moneda, ID_Aceptante, ID_DireccionAceptante, Total,
                         "Interfaz Intranet - Test", NroPeriodos, Cuota, ID_DocumentoLetra, OpOLD
                         ));

                dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }

        public static DataTable Finan_Detalle_Registrar(int idEmpresa, string ruc_empresa, string esquema, int Id_Amarre, int Op, int ID_Doc, decimal Importe)
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
                         "declare @p1 numeric(18, 0)  \n" +
                         "set @p1 = {3} \n" +
                         "exec " + esquema + "..VBG00687 @p1 output, '{0}', {1}, '{2}'  \n" +
                         "select @p1",
                         Op, ID_Doc, Importe, Id_Amarre
                         ));

                dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }

        public static DataTable Letras_Registrar(int idEmpresa, string ruc_empresa, string esquema, int Periodo, int Op, string ID_Agenda, int ID_Moneda, decimal Importe,
            DateTime FechaEmision, DateTime FechaVencimiento, string Glosa)
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
                         "declare @p1 numeric(18, 0)  \n" +
                         "set @p1 = null  \n" +
                         "exec " + esquema + "..VBG00575 @p1 output, {0}, {1}, '{2}', null, {3}, {4},'{5}', '{6}','Miraflores', '{7}'  \n" +
                         "select @p1",
                         Periodo, Op, ID_Agenda, ID_Moneda, Importe, FechaEmision.ToShortDateString(), FechaVencimiento.ToShortDateString(), Glosa
                         ));

                dtTabla = ds2.Tables[0];

                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }

        public static DataTable Buscar_Financiamiento(int idEmpresa, string esquema, string ID_Agenda, int Id_doc)
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
                    "exec " + esquema + "..VBG00679_Letras '{0}', {1} ",
                    ID_Agenda, Id_doc
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

        public static DataSet Listar_FinanciamientoTotal(int idEmpresa, string esquema, int Op)
        {
            DataTable dtTabla = new DataTable();
            DataSet ds1 = new DataSet();
            try
            {
                blTareo bl = new blTareo();
                //Lista Producción por recibir
                ds1 = bl.ListarDataSet(
                     string.Format(
                     //"set dateformat DMY\n" +
                     "set dateformat " + getFormatDate() + "\n" +
                     "declare @Bloqueado bit=null  \n" +
                     "declare @MensajeBloqueo varchar(100)=null  \n" +

                     "exec " + esquema + "..VBG00685_Letras {0}, @Bloqueado output, @MensajeBloqueo  output \n" +
                     "select @Bloqueado, @MensajeBloqueo",
                     Op
                     ));

                //declare
                //@Bloqueado  bit = Null ,  
                // @MensajeBloqueo varchar(100) = Null


                // exec VBG00685_Letras 35886, @Bloqueado, @MensajeBloqueo

                // select @Bloqueado, @MensajeBloqueo

                //Busca registro de interfaz
                //dtTabla = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds1;
        }

        public static void Eliminar_Letra(int idEmpresa, string esquema, int Id_Letra)
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
                         "exec " + esquema + "..VBG00576  '{0}' \n",
                         Id_Letra
                         ));

                //dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Actualizar_Proceso_Letra(int idEmpresa, string esquema, int ID_Proceso, int Usuario)
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
                         "exec " + esquema + "..gsPedidos_ProcesoUpdate   {0},{1}, 1 \n",
                         ID_Proceso, Usuario
                         ));

                //dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Actualizar_Fechas_Letra(int idEmpresa, string esquema, int ID_Proceso, int id_letra, int Usuario, DateTime Fecha)
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
                         "exec " + esquema + "..gsFactura_FechasLetrasUpdate   {0},{1},'{2}',{3} \n",
                         ID_Proceso, id_letra, Fecha, Usuario
                         ));

                //dtTabla = ds2.Tables[0];
                //var @p1 = ds2.Tables[0].Rows[0][0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable Proceso_Listar(int idEmpresa, string ruc_empresa, string esquema, int OP_OV, int OP_DOC)
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
                    "exec " + esquema + "..gsProcesoLetrasPendientes_Listar {0}, {1} ",
                    OP_OV, OP_DOC
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

        public static DataTable ProcesoDetalle_Listar(int idEmpresa, string ruc_empresa, string esquema, int Id_Amarre, int OP_OV, int OP_DOC)
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
                    "exec " + esquema + "..gsProcesoLetras_DetalleListar {0}, {1}, {2} ",
                     Id_Amarre, OP_OV, OP_DOC
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


        public static void Actualizar_ProcesoDetalle_Letra(int idEmpresa, string esquema, int ID_Proceso, int OP_OV, int OP_DOC, int Usuario)
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
                         "exec " + esquema + "..gsPedidos_ProcesoDetalleUpdate   {0},{1}, {2}, {3}, 1 \n",
                         ID_Proceso, OP_OV, OP_DOC, Usuario
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
