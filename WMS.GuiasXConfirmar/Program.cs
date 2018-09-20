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
using WMS.GuiasXConfirmar.AgendaWCF;
using WMS.GuiasXConfirmar.GuiaWCF;
using WMS.GuiasXConfirmar.OrdenVentaWCF;
using WMS.GuiasXConfirmar.WmsWCF;

namespace WMS.GuiasXConfirmar
{
    class Program
    {
        public static void Main(string[] args)
        {
            string EmpresaPT = ""; 
            string ruc_empresa = "";
            int idEmpresa = 0;

            string esquemaREC = ConfigurationSettings.AppSettings["BDERP"].ToString();
            ruc_empresa = ConfigurationSettings.AppSettings["RUC"].ToString();
            EmpresaPT = ConfigurationSettings.AppSettings["SIGLAS"].ToString();
            idEmpresa = Convert.ToInt32(ConfigurationSettings.AppSettings["IDEMPRESA"]);

            LeerPedidosTXT_FTP(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);


            //for (int x = 1; x <= 2; x++)
            //{
            //    if (x == 1)
            //    {
            //        string esquemaREC = ConfigurationSettings.AppSettings["BD_silvestre"].ToString();
            //        ruc_empresa = "20191503482";
            //        EmpresaPT = "SILPT";
            //        idEmpresa = 1;

            //        LeerPedidosTXT_FTP(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);
            //        x = x + 10;
            //    }

            //    if (x == 2)
            //    {
            //        string esquemaREC = ConfigurationSettings.AppSettings["BD_neoagrum"].ToString();
            //        ruc_empresa = "20509089923";
            //        EmpresaPT = "NEOPT";
            //        idEmpresa = 2;

            //        LeerPedidosTXT_FTP(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);
            //        x = x + 10;
            //    }


            //if (x == 1)
            //{
            //    string esquemaREC = ConfigurationSettings.AppSettings["BD_silvestre"].ToString();
            //    ruc_empresa = "20191503482";
            //    EmpresaPT = "SILPT";
            //    idEmpresa = 1;

            //    //ruc_empresa = "SIL";
            //    //idEmpresa = 3;
            //    LeerPedidosTXT_FTP(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);
            //    //x = x + 10;
            //}

            //if (x == 2)
            //{
            //    string esquemaREC = ConfigurationSettings.AppSettings["BD_REC_Sil"].ToString();
            //    ruc_empresa = "20191503482";
            //    EmpresaPT = "SILPT";
            //    idEmpresa = 3;

            //    //ruc_empresa = "SIL";
            //    //idEmpresa = 3;
            //    LeerPedidosTXT_FTP(idEmpresa, ruc_empresa, esquemaREC, EmpresaPT);
            //    //x = x + 10;
            //}
            //}
        }

        //public static void LeerPedidosTXT_FTP(string RutaOrigen, string RutaSubmit, string RutaFail, string Login, string Password, string fileName, int idEmpresa, string NomEmpresa)
        public static void LeerPedidosTXT_FTP(int idEmpresa, string NomEmpresa, string esquema, string EmpresaPT)
        {
            Reproceso_Pedidos_Confirmacion(idEmpresa, EmpresaPT, esquema);

            DataTable Lista = Listar_Pedidos_Confirmacion(idEmpresa, NomEmpresa, esquema);

            //DateTime fecha =   Convert.ToDateTime(DateTime.Now.ToShortDateString());
            //Console.WriteLine("La fecha es: " + fecha); 

            if (Lista.Rows.Count > 0)
            {
                    ReadFileFromFTP(idEmpresa, Lista, esquema, EmpresaPT);
            }
            else
            {
                Console.WriteLine("No se tiene pedidos confirmados para " + NomEmpresa);
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
                    "exec " + esquema + "..Cargar_Pedidos_Confirmacion {0}",
                    ruc_empresa
                    )
                    );
                //Busca registro de interfaz
                dtTabla = ds1.Tables[0];

                //lista = dcg.Cargar_Pedidos_Confirmacion(ruc_empresa).ToList();
                //LstPedidos = objWMS.WmsPedidos_Confirmacion_Listar(idEmpresa, 1, ruc_empresa).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }

        public static void Reproceso_Pedidos_Confirmacion(int idEmpresa, string ruc_empresa, string esquema)
        {

            try
            {
                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet(
                    string.Format(
                    //"set dateformat DMY\n" +
                    "set dateformat " + getFormatDate() + "\n" +
                    "exec " + esquema + "..Cargar_Pedidos_ReprocesoEstilos '{0}'",
                    ruc_empresa
                    )
                    );
 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public static void ReadFileFromFTP(int idEmpresa, List<string> files, string RutaOrigen, string RutaSubmit, string RutaFail, string user, string pass)
        public static void ReadFileFromFTP(int idEmpresa, DataTable ListaPedido, string esquema, string EmpresaPT)
        {
            
            //string Nombre; 
            int codigoUsuario = 1;
            int idOrdenVenta;
           
            try
            {
                List<gsInterfacePedidos_LeerResult> lstPedidos = new List<gsInterfacePedidos_LeerResult>();

                foreach (DataRow file in ListaPedido.Rows)
                {
                    gsInterfacePedidos_LeerResult Pedido = new gsInterfacePedidos_LeerResult();
                    //Pedido.NroPedido = "30313"; // file.NumeroDeDocumento; // Campos[0];                     //Op OV
                    //Pedido.ID_Item = "10210109003514"; //( file.CodigoDeArticulo;  //  Campos[1];                       //Item
                    //Pedido.Lote = "170831-0171-2050"; // file.CodigoDeLote; // Campos[2];                          //Lote
                    //Pedido.CantidadPedido = 6; // (decimal)file.UnidadesPedido; // decimal.Parse(Campos[3]);    //Cantidad
                    //Pedido.CantidadEntrega = 6; // (decimal)file.UnidadesEntregadas; // int.Parse(Campos[4]);       //Cantidad entregada
                    //Pedido.CantidadPendiente = 0; // (decimal)(file.Diferencia); // decimal.Parse(Campos[5]); //Diferencia
                    //Pedido.EstadoPedido = "D"; // file.Anticipado; // Campos[7];             // Estado
                    //Pedido.Id_Amarre = 76851; // decimal.Parse(file.IDDeLinea); // decimal.Parse(Campos[8]); // Id_Amarre
                    //Pedido.TransferidoTabla = "SILPT"; // file.Empresa;
                    //Pedido.Servicio = file.NumeroDeAlbaran;

                    //-------------------------------------------------
                    Pedido.NroPedido = file["NumeroDeDocumento"]; // Campos[0];                     //Op OV
                    Pedido.ID_Item = file["CodigoDeArticulo"];  //  Campos[1];                       //Item
                    Pedido.Lote = file["CodigoDeLote"]; // Campos[2];                          //Lote
                    Pedido.CantidadPedido = decimal.Parse(file["UnidadesPedido"].ToString()); // decimal.Parse(Campos[3]);    //Cantidad
                    Pedido.CantidadEntrega = decimal.Parse(file["UnidadesEntregadas"].ToString()); // int.Parse(Campos[4]);       //Cantidad entregada
                    Pedido.CantidadPendiente = decimal.Parse(file["Diferencia"].ToString()); // decimal.Parse(Campos[5]); //Diferencia
                    Pedido.EstadoPedido = file["Anticipado"]; // Campos[7];             // Estado
                    Pedido.Id_Amarre = decimal.Parse(file["IDDeLinea"].ToString()); // decimal.Parse(Campos[8]); // Id_Amarre
                    Pedido.TransferidoTabla = file["CIF"];
                    Pedido.Servicio = file["NumeroDeAlbaran"];

                    lstPedidos.Add(Pedido);

                }


                lstPedidos = lstPedidos.OrderBy(x => x.NroPedido).ToList();

                foreach (gsInterfacePedidos_LeerResult pedido in lstPedidos)
                {
                    try
                    {
                        WmsWCF.WmsWCFClient objWmsInsert = new WmsWCF.WmsWCFClient();
                        objWmsInsert.WmsPedidosPendientes_Insertar(idEmpresa, 1, pedido.NroPedido.ToString(), pedido.ID_Item.ToString(), pedido.Lote.ToString(),
                                                                    decimal.Parse(pedido.CantidadPedido.ToString()), decimal.Parse(pedido.CantidadEntrega.ToString()),
                                                                    decimal.Parse(pedido.CantidadPendiente.ToString()),
                                                                    pedido.EstadoPedido.ToString(), int.Parse(pedido.Id_Amarre.ToString())); //1=SILVESTRE; 2=NEOAGRUM; 6=INATEC

                        //objWmsInsert.WmsPedidosPendientes_UpdateEstilos(idEmpresa, codigoUsuario, pedido.TransferidoTabla, pedido.Servicio);

                        Console.Write("Se registro Op" + pedido.NroPedido + " - " + pedido.Id_Amarre);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Error: Registrar TXT: " + pedido.NroPedido.ToString() + "-" + pedido.ID_Item.ToString() + ", " + ex.Message.ToString());
                    }
                }

                Console.Write("Se registro los pedidos consumidos.");
                Console.Write("..:Procesando Guias:..");

                //---------------------------Pedido--------------------------------

                //var lstOpOV = lstPedidos.Select(x => x.NroPedido).Distinct();


                foreach (gsInterfacePedidos_LeerResult pedido in lstPedidos)
                {

                    try
                    {
                        GuiaWCFClient objGuiaVentaWCF = new GuiaWCFClient();
                        OrdenVentaWCFClient objOrdenVentaWCF = new OrdenVentaWCFClient();

                        gsOV_BuscarCabeceraResult objOrdenVentaCab;
                        gsOV_BuscarDetalleResult[] objOrdenVentaDet = null;
                        gsGuia_BuscarCabeceraResult objGuiaVentaCab = new gsGuia_BuscarCabeceraResult();
                        gsGuia_BuscarDetalleResult[] objGuiaVentaDet = null;
                        gsGuia_BuscarDetalleResult objGuiaVentaDetUpdate = null;
                        gsOV_BuscarDetalleResult objOrdenVenta_Linea = null;
                        List<gsItem_BuscarResult> lstProductos = new List<gsItem_BuscarResult>();
                        gsOV_BuscarImpuestoResult[] lstImpuestos = null;
                        GuiaVenta_LotesItemsResult[] lstLotes = null;

                        bool? bloqueado = false;
                        string mensajeBloqueo = null;
                        AgendaWCFClient objAgendaWCFClient;

                        VBG01134Result objAgendaCliente;

                        decimal? lineaCredito = null;
                        decimal? Id_Amarre = 0;
                        decimal? TC = 0;


                        int variable = Convert.ToInt32(pedido.NroPedido); 

                        if (variable == 23876)
                        {
                            TC = TC;
                        }


                        DateTime? fechaVecimiento = null;

                        Console.Write("OP:" + pedido.NroPedido);
                        Console.Write(" ");

                        
                        objAgendaWCFClient = new AgendaWCFClient();
                        objAgendaCliente = new VBG01134Result();

                        idOrdenVenta = int.Parse(pedido.NroPedido.ToString());
                        Console.WriteLine("Inicio: objOrdenVentaWCF.OrdenVenta_Buscar_Guia");
                        objOrdenVentaCab = objOrdenVentaWCF.OrdenVenta_Buscar_Guia(idEmpresa, codigoUsuario, idOrdenVenta, ref objOrdenVentaDet, ref lstImpuestos, ref bloqueado, ref mensajeBloqueo);

                        Console.WriteLine("Inicio: objAgendaWCFClient.Agenda_BuscarCliente");
                        objAgendaCliente = objAgendaWCFClient.Agenda_BuscarCliente(idEmpresa, codigoUsuario, objOrdenVentaCab.ID_Agenda,
                            ref lineaCredito, ref fechaVecimiento, ref TC);
                        Id_Amarre = (decimal)pedido.Id_Amarre;

                        objOrdenVenta_Linea = objOrdenVentaDet.ToList().FindAll(x => x.ID_Amarre == Id_Amarre).Single();

                        int idGuiaOp = 0;
                        int idGuiaOpLinea = 0;
                        int item_id = 0;

                        idGuiaOp = int.Parse(objOrdenVentaCab.OpGuia.ToString());
                        idGuiaOpLinea = int.Parse(objOrdenVenta_Linea.OpGuia.ToString());
                        item_id = int.Parse(objOrdenVenta_Linea.Item_ID.ToString());

                        if (objOrdenVenta_Linea == null)
                        {
                        }
                        else
                        {
                            if (idGuiaOpLinea > 0)
                            {
                                Console.WriteLine("Inicio: objGuiaVentaWCF.GuiaVenta_Buscar");
                                objGuiaVentaCab = objGuiaVentaWCF.GuiaVenta_Buscar(idEmpresa, codigoUsuario, idGuiaOp, ref objGuiaVentaDet, ref bloqueado, ref mensajeBloqueo);
                            }
                            else
                            {
                                Console.WriteLine("Inicio: GuiaVenta_ObtenerCabecera");
                                objGuiaVentaCab = GuiaVenta_ObtenerCabecera(objOrdenVentaCab, idGuiaOp);
                                Console.WriteLine("Inicio: GuiaVenta_ObtenerDetalle");
                                objGuiaVentaDet = GuiaVenta_ObtenerDetalle(objOrdenVentaCab, objOrdenVentaDet, idEmpresa, codigoUsuario).ToArray();
                            }

                            if (idGuiaOp > 0)
                            {
                                Console.WriteLine("Inicio: objGuiaVentaWCF.GuiaVenta_LotesItemBuscar");
                                lstLotes = objGuiaVentaWCF.GuiaVenta_LotesItemBuscar(idEmpresa, codigoUsuario, int.Parse(idGuiaOp.ToString()), int.Parse(objOrdenVenta_Linea.Item_ID.ToString()));
                            }




                            List<GuiaVenta_LotesItemsResult> LotesUp = new List<GuiaVenta_LotesItemsResult>();
                            if (lstLotes == null)
                            {
                                LotesUp = new List<GuiaVenta_LotesItemsResult>();
                            }
                            else
                            {
                                LotesUp = ((GuiaVenta_LotesItemsResult[])lstLotes).ToList();
                            }
                            Console.WriteLine("Inicio: GuiaVenta_ObtenerDetalle_Update");
                            objGuiaVentaDetUpdate = GuiaVenta_ObtenerDetalle_Update(objGuiaVentaDet, lstPedidos, Id_Amarre, ref LotesUp, item_id);

                            lstLotes = (GuiaVenta_LotesItemsResult[])LotesUp.ToArray();



                            try
                            {
                                int Error = 0;
                                int cont = 0;
                                WmsWCF.WmsWCFClient objWmsInsert = new WmsWCF.WmsWCFClient();

                                //List<VBG00971Result> Lista_LoteVar = objGuiaVentaWCF.GuiaVenta_BuscarLotesxItem(idEmpresa, 1, int.Parse(pedido.NroPedido), (int)item_id, (int)objGuiaVentaCab.ID_AlmacenAnexo, (int)pedido.Id_Amarre).ToList();

                                //List<VBG00971Result> Lista_LoteVar = dcg.VBG00971(item_id, ID_AlmacenAnexo, "GuiaVenta", 0, "OV", id_amarre).ToList();


                                DataTable dtTabla = new DataTable();
                                blTareo bl = new blTareo();
                                Console.WriteLine("Inicio de VBG00971_WMS");
                                //Lista Producción por recibir
                                DataSet ds1 = bl.ListarDataSet(
                                    string.Format(
                                    //"set dateformat DMY\n" +
                                    "set dateformat " + getFormatDate() + "\n" +
                                    "exec " + esquema + "..VBG00971_WMS {0},{1},'GuiaVenta', 0, 'OV',{2},'{3}'",
                                    (int)item_id, (int)objGuiaVentaCab.ID_AlmacenAnexo, pedido.Id_Amarre.ToString(), pedido.Lote.ToString()
                                    )
                                    );
                                //Busca registro de interfaz
                                dtTabla = ds1.Tables[0];
                                dtTabla.Columns.Add("Consumo", typeof(int));
                                Console.WriteLine("Fin de VBG00971_WMS");
 
                             
                                int Pendiente = 0;
                                Pendiente =  Convert.ToInt32(pedido.CantidadEntrega); 

                                foreach (DataRow Lote in dtTabla.Rows)//traer solo lotes iguales al lote del kardex
                                {
                                    string strLote = Lote["Lote"].ToString();

                                    if (strLote == pedido.Lote.ToString())
                                    {
                                        int Consumo = 0;
                                        int CantLote = Convert.ToInt32(Lote[10]) ;

                                        if(CantLote >= Pendiente)
                                        {
                                            Consumo = Pendiente;
                                            Pendiente = 0; 
                                        }
                                        else
                                        {
                                            Consumo = CantLote; 
                                            Pendiente = Pendiente - CantLote; 

                                        }

                                        Lote["Consumo"] = Consumo; 
                                        cont++;
                                        if(Pendiente <= 0)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (cont == 0)
                                {
                                    //objWmsInsert.WmsPedidosPendientes_Update(idEmpresa, codigoUsuario, pedido.Lote, (int)pedido.Id_Amarre, "F", "No se encuentra Lote para el Item");

                                    DataSet ds11 = bl.ListarDataSet(
                                    string.Format(
                                    "set dateformat DMY\n" +
                                    "exec  " + esquema + "..gsInterfacePedidos_Update '{0}',{1},{2},'{3}'",
                                    pedido.Lote.ToString(), pedido.Id_Amarre.ToString(), "'F'", "No se encuentra Lote para el Item"
                                    ));

                                    //dcg.gsInterfacePedidos_Update(Lote, Id_Amarre, transferido, observacion);

                                }
                                else
                                {
                                    try
                                    {
                                        Console.WriteLine("Registrando Guia y moviendo el Inv.");
                                        objGuiaVentaWCF.GuiaVenta_Registrar_wms(idEmpresa, 1, objGuiaVentaCab, objGuiaVentaDetUpdate,
                                            decimal.Parse(idGuiaOp.ToString()), lstLotes, EmpresaPT, pedido.Servicio.ToString(), pedido.Lote.ToString(), pedido.Id_Amarre.ToString());

                                        Console.WriteLine("Finalizado");
                                        //DataSet ds13 = bl.ListarDataSet(
                                        // string.Format(
                                        // "set dateformat DMY\n" +
                                        // "exec  " + esquema + "..Cargar_Pedidos_UpdateEstilos '{0}','{1}'",
                                        //   EmpresaPT, pedido.Servicio.ToString()
                                        // ));

                                        //DataSet ds14 = bl.ListarDataSet(
                                        //   string.Format(
                                        //   "set dateformat DMY\n" +
                                        //   "exec  " + esquema + "..gsInterfacePedidos_Update '{0}',{1},'{2}','{3}'",
                                        //   pedido.Lote.ToString(), pedido.Id_Amarre.ToString(), "S", "Se registro correctamente."
                                        //   ));
                                    }
                                    catch (Exception ex)
                                    {
                                        //objWmsInsert.WmsPedidosPendientes_Update(idEmpresa, codigoUsuario, pedido.Lote, (int)pedido.Id_Amarre, "F", "Error, al registrar la Guia.");


                                        //DataSet ds12 = bl.ListarDataSet(
                                        //string.Format(
                                        //"set dateformat DMY\n" +
                                        //"exec  " + esquema + "..gsInterfacePedidos_Update '{0}',{1},'{2}','{3}'",
                                        //pedido.Lote.ToString(), pedido.Id_Amarre.ToString(), "F", "Error: al registrar Guía"  
                                        //));

                                        Console.Write("Error al registrar guias: " + ex.Message.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                blTareo bl = new blTareo();
                                DataSet ds15 = bl.ListarDataSet(
                                       string.Format(
                                       "set dateformat DMY\n" +
                                        "exec  " + esquema + "..gsInterfacePedidos_Update '{0}',{1},'{2}','{3}'",
                                       pedido.Lote.ToString(), pedido.Id_Amarre.ToString(), "F", "Error: al registrar Guía"
                                       ));
                                Console.Write("Error al registrar guias: " + ex.Message.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        blTareo bl = new blTareo();
                        DataSet ds16 = bl.ListarDataSet(
                                       string.Format(
                                       "set dateformat DMY\n" +
                                       "exec  " + esquema + "..gsInterfacePedidos_Update '{0}',{1},'{2}','{3}'",
                                       pedido.Lote.ToString(), pedido.Id_Amarre.ToString(), "F", "Error: al registrar Guía"
                                       ));
                        Console.Write("Error: Registrar Guia: " + pedido.NroPedido.ToString() + "-" + pedido.ID_Item.ToString() + ", " + ex.Message.ToString());
                    }
                }

                ////------------------Mover archivos---------

                //    //MoverArchivos(RutaOrigen, RutaSubmit, file, user, pass);
                ////}
            }
            catch (Exception ex)
            {
                Console.Write("Error: Registrar Guia, " + ex.Message.ToString());
                //MoverArchivos(RutaOrigen, RutaFail, file, user, pass);
            }
            //}
        }

        static gsGuia_BuscarCabeceraResult GuiaVenta_ObtenerCabecera(gsOV_BuscarCabeceraResult objOrdenVentaCab_R, decimal idGuiaVenta)
        {
            gsGuia_BuscarCabeceraResult objGuiaVentaCab;

            try
            {
                objGuiaVentaCab = new gsGuia_BuscarCabeceraResult();

                objGuiaVentaCab.Op = idGuiaVenta;
                objGuiaVentaCab.ID_Almacen = objOrdenVentaCab_R.ID_AgendaOrigen.ToString();
                objGuiaVentaCab.Fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objGuiaVentaCab.FechaInicioTraslado = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objGuiaVentaCab.ID_MotivoTraslado = 1;
                objGuiaVentaCab.Serie = string.Empty;   // Revisar
                objGuiaVentaCab.Numero = 0;           // Revisar
                objGuiaVentaCab.ID_Agenda = objOrdenVentaCab_R.ID_Agenda;

                objGuiaVentaCab.ID_Envio = (decimal)objOrdenVentaCab_R.ID_Envio; // 2 
                objGuiaVentaCab.Observaciones = objOrdenVentaCab_R.Observaciones;

                objGuiaVentaCab.Transportista = objOrdenVentaCab_R.Transporte;  // Revisar
                objGuiaVentaCab.Chofer = null;
                objGuiaVentaCab.ID_AgendaAnexo = objOrdenVentaCab_R.ID_AgendaAnexo;

                objGuiaVentaCab.ID_AlmacenAnexo = (decimal)objOrdenVentaCab_R.ID_Almacen; // ID_AlmacenAnexo
                objGuiaVentaCab.ID_AgendaDireccion = objOrdenVentaCab_R.ID_AgendaDireccion;
                objGuiaVentaCab.ID_AgendaDireccion2 = objOrdenVentaCab_R.ID_AgendaDireccion2;
                objGuiaVentaCab.ID_Transportista = objOrdenVentaCab_R.ID_Transportista;

                objGuiaVentaCab.ID_Vehiculo = objOrdenVentaCab_R.ID_Vehiculo1;
                objGuiaVentaCab.ID_Vehiculo2 = objOrdenVentaCab_R.ID_Vehiculo2;
                objGuiaVentaCab.ID_Vehiculo3 = objOrdenVentaCab_R.ID_Vehiculo3;
                objGuiaVentaCab.ID_Chofer = objOrdenVentaCab_R.ID_Chofer;
                objGuiaVentaCab.NotasDespacho = objOrdenVentaCab_R.NotasDespacho;

                objGuiaVentaCab.ID_CondicionCredito = objOrdenVentaCab_R.ID_CondicionCredito;
                objGuiaVentaCab.TransportistaRUC = objOrdenVentaCab_R.ID_Transportista; // Revisar

                objGuiaVentaCab.TransportistaDomicilio = string.Empty; // Revisar
                objGuiaVentaCab.TransportistaMarca = null;
                objGuiaVentaCab.TransportistaModelo = null;
                objGuiaVentaCab.TransportistaPlaca = null;
                objGuiaVentaCab.TransportistaCertInscripcion = null;
                objGuiaVentaCab.TransportistaChofer = null;
                objGuiaVentaCab.TransportistaLicencia = null;
                objGuiaVentaCab.CompPagoTipo = null;
                objGuiaVentaCab.CompPagoNro = null;

                objGuiaVentaCab.CompPagoFechaEmision = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                objGuiaVentaCab.ID_AgendaOrigen = objOrdenVentaCab_R.ID_AgendaOrigen;
                objGuiaVentaCab.DireccionOrigenSucursal = objOrdenVentaCab_R.ID_Almacen;
                objGuiaVentaCab.DireccionOrigenReferencia = objOrdenVentaCab_R.DireccionOrigenReferencia;
                objGuiaVentaCab.DireccionOrigenDireccion = objOrdenVentaCab_R.DireccionOrigenDireccion;

                objGuiaVentaCab.ID_AgendaDestino = objOrdenVentaCab_R.ID_AgendaDestino;
                objGuiaVentaCab.DireccionDestinoSucursal = objOrdenVentaCab_R.DireccionDestinoSucursal;
                objGuiaVentaCab.DireccionDestinoReferencia = objOrdenVentaCab_R.DireccionDestinoReferencia;
                objGuiaVentaCab.DireccionDestinoDireccion = objOrdenVentaCab_R.DireccionDestinoDireccion;

                objGuiaVentaCab.HoraAtencionOpcion1_Desde = objOrdenVentaCab_R.HoraAtencionOpcion1_Desde;
                objGuiaVentaCab.HoraAtencionOpcion1_Hasta = objOrdenVentaCab_R.HoraAtencionOpcion1_Hasta;
                objGuiaVentaCab.HoraAtencionOpcion2_Desde = objOrdenVentaCab_R.HoraAtencionOpcion2_Desde;
                objGuiaVentaCab.HoraAtencionOpcion2_Hasta = objOrdenVentaCab_R.HoraAtencionOpcion2_Hasta;
                objGuiaVentaCab.HoraAtencionOpcion3_Desde = objOrdenVentaCab_R.HoraAtencionOpcion3_Desde;
                objGuiaVentaCab.HoraAtencionOpcion3_Hasta = objOrdenVentaCab_R.HoraAtencionOpcion3_Hasta;



                return objGuiaVentaCab;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static List<gsGuia_BuscarDetalleResult> GuiaVenta_ObtenerDetalle(gsOV_BuscarCabeceraResult objOrdenVentaCab_R,
        gsOV_BuscarDetalleResult[] objOrdenVentaDet, int idEmpresa, int codigoUsuario)
        {

            List<gsGuia_BuscarDetalleResult> lstPedidoDet = new List<gsGuia_BuscarDetalleResult>();
            gsGuia_BuscarDetalleResult objProducto;

            try
            {

                foreach (gsOV_BuscarDetalleResult producto in objOrdenVentaDet)
                {
                    objProducto = new gsGuia_BuscarDetalleResult();

                    // exec VBG00495 @p1 output,19250,'OV',51646,'10210109009418',NULL,
                    // NULL,NULL,NULL,202,48.0000,0.0000,0.0000,48.0000,0.0000,'Unidad',1.0000,48.0000,'Unidad',48.0000,'Unidad',48.0000,NULL	

                    objProducto.ID_Amarre = 0; //Amarre Guia
                    objProducto.Op = 0;
                    objProducto.TablaOrigen = "OV";
                    objProducto.Linea = producto.ID_Amarre;
                    objProducto.ID_Item = producto.ID_Item;
                    objProducto.ID_ItemAnexo = producto.ID_ItemAnexo;

                    objProducto.ID_CCosto = producto.ID_CCosto;
                    objProducto.ID_UnidadGestion = producto.ID_UnidadGestion;
                    objProducto.ID_UnidadProyecto = producto.ID_UnidadProyecto;
                    objProducto.Item_ID = producto.Item_ID;

                    objProducto.CantidadBruta = producto.Cantidad;
                    objProducto.Bultos = 0;
                    objProducto.Tara = 0;
                    objProducto.Cantidad = producto.Cantidad;
                    objProducto.Ajuste = 0;

                    objProducto.ID_UnidadInv = producto.ID_UnidadInv;
                    objProducto.FactorUnidadInv = producto.FactorUnidadInv;
                    objProducto.CantidadUnidadInv = producto.CantidadUnidadInv; // Consultar como se calcula realmente

                    objProducto.ID_UnidadDoc = producto.ID_UnidadDoc;
                    objProducto.CantidadUnidadDoc = producto.CantidadUnidadDoc; // Consultar como se calcula realmente
                    objProducto.ID_UnidadControl = producto.ID_UnidadDoc;
                    objProducto.CantidadUnidadControl = producto.CantidadUnidadDoc;
                    objProducto.Observaciones = producto.Observaciones;

                    objProducto.Estado = (int)producto.Estado;

                    lstPedidoDet.Add(objProducto);
                }
                return lstPedidoDet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static gsGuia_BuscarDetalleResult GuiaVenta_ObtenerDetalle_Update(gsGuia_BuscarDetalleResult[] objGuiaVentaDet,
            List<gsInterfacePedidos_LeerResult> lstPedido, decimal? Id_AmarreUp, ref List<GuiaVenta_LotesItemsResult> lstLotesI, int item_id)
        {

            decimal? id_amarreOV;
            decimal? id_amarreGuia;
            id_amarreOV = Id_AmarreUp;
            List<GuiaVenta_LotesItemsResult> LotesUp = new List<GuiaVenta_LotesItemsResult>();

            try
            {

                foreach (gsInterfacePedidos_LeerResult producto in lstPedido)
                {
                    id_amarreGuia = 0;
                    if (id_amarreOV == (decimal)producto.Id_Amarre)
                    {
                        GuiaVenta_LotesItemsResult Lote = new GuiaVenta_LotesItemsResult();

                        Lote.Linea = (decimal)producto.Id_Amarre;
                        Lote.Lote = producto.Lote.ToString();
                        Lote.Cantidad = (decimal)producto.CantidadEntrega;
                        Lote.CantidadUnidadControl = (decimal)producto.CantidadEntrega;
                        Lote.Item_ID = item_id;

                        foreach (GuiaVenta_LotesItemsResult lt in lstLotesI)
                        {
                            if (lt.Lote == Lote.Lote)
                            {
                                id_amarreGuia = lt.ID_Amarre;
                            }
                        }

                        Lote.ID_Amarre = (decimal)id_amarreGuia;

                        lstLotesI.RemoveAll(x => x.Lote == Lote.Lote);
                        lstLotesI.Add(Lote);
                    }
                }

                decimal CantidaEntrega = 0;

                foreach (GuiaVenta_LotesItemsResult Lote in lstLotesI)
                {
                    if (id_amarreOV == Lote.Linea)
                    {
                        CantidaEntrega = CantidaEntrega + Lote.CantidadUnidadControl;
                    }
                }

                gsGuia_BuscarDetalleResult GuiaDetalle = new gsGuia_BuscarDetalleResult();

                foreach (gsGuia_BuscarDetalleResult GuiaProducto in objGuiaVentaDet)
                {
                    if (id_amarreOV == GuiaProducto.Linea)
                    {
                        GuiaProducto.CantidadBruta = CantidaEntrega;
                        GuiaProducto.Bultos = 0;
                        GuiaProducto.Tara = 0;
                        GuiaProducto.Cantidad = CantidaEntrega;
                        GuiaProducto.Ajuste = 0;

                        GuiaProducto.CantidadUnidadInv = CantidaEntrega; // Cantidad
                        GuiaProducto.CantidadUnidadDoc = CantidaEntrega; // Cantidad
                        GuiaProducto.CantidadUnidadControl = CantidaEntrega;   // Cantidad

                        GuiaDetalle = GuiaProducto;

                        break;
                    }
                }

                return GuiaDetalle;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
    }
}
