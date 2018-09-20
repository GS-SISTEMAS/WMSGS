using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMS.Entities.Common
{
    public class RowItem
    {
        public object v01;
        public object v02;
        public object v03;
        public object v04;
        public object v05;
        public object v06;
        public object v07;
        public object v08;
        public object v09;
        public object v10;
        public object v11;
        public object v12;
        public object v13;
        public object v14;
        public object v15;
        public object v16;
        public object v17;
        public object v18;
        public object v19;
        public object v20;
        public object v21;
        public object v22;
        public object v23;
        public object v24;
        public object v25;
        public object v26;
        public object v27;
        public object v28;
        public object v29;
        public object v30;
        public object v31;
        public object v32;
        public object v33;
        public object v34;
        public object v35;
        public object v36;
        public object v37;
        public object v38;
        public object v39;
        public object v40;

        public void GetValue(string field, object value)
        {
            switch (field)
            {
                case "v01": this.v01 = value; break;
                case "v02": this.v02 = value; break;
                case "v03": this.v03 = value; break;
                case "v04": this.v04 = value; break;
                case "v05": this.v05 = value; break;
                case "v06": this.v06 = value; break;
                case "v07": this.v07 = value; break;
                case "v08": this.v08 = value; break;
                case "v09": this.v09 = value; break;
                case "v10": this.v10 = value; break;
                case "v11": this.v11 = value; break;
                case "v12": this.v12 = value; break;
                case "v13": this.v13 = value; break;
                case "v14": this.v14 = value; break;
                case "v15": this.v15 = value; break;
                case "v16": this.v16 = value; break;
                case "v17": this.v17 = value; break;
                case "v18": this.v18 = value; break;
                case "v19": this.v19 = value; break;
                case "v20": this.v20 = value; break;
                case "v21": this.v21 = value; break;
                case "v22": this.v22 = value; break;
                case "v23": this.v23 = value; break;
                case "v24": this.v24 = value; break;
                case "v25": this.v25 = value; break;
                case "v26": this.v26 = value; break;
                case "v27": this.v27 = value; break;
                case "v28": this.v28 = value; break;
                case "v29": this.v29 = value; break;
                case "v30": this.v30 = value; break;
                case "v31": this.v31 = value; break;
                case "v32": this.v32 = value; break;
                case "v33": this.v33 = value; break;
                case "v34": this.v34 = value; break;
                case "v35": this.v35 = value; break;
                case "v36": this.v36 = value; break;
                case "v37": this.v37 = value; break;
                case "v38": this.v38 = value; break;
                case "v39": this.v39 = value; break;
                case "v40": this.v40 = value; break;
            }
        }
    }

    public class Column
    {
        public string campo { get; set; }
        public string key { get; set; }
        public int index { get; set; }
    }

    public class gsInterfacePedidos_LeerResult
    {
        public object NroPedido;
        public object ID_Item;
        public object Lote;
        public object CantidadPedido;
        public object CantidadEntrega;
        public object CantidadPendiente;
        public object EstadoPedido;
        public object Id_Amarre;
        public object TransferidoTabla;
        public object Servicio;
  
        public void GetValue(string field, object value)
        {
            switch (field)
            {
                case "NroPedido": this.NroPedido = value; break;
                case "ID_Item": this.ID_Item = value; break;
                case "Lote": this.Lote = value; break;
                case "CantidadPedido": this.CantidadPedido = value; break;
                case "CantidadEntrega": this.CantidadEntrega = value; break;
                case "CantidadPendiente": this.CantidadPendiente = value; break;
                case "EstadoPedido": this.EstadoPedido = value; break;
                case "Id_Amarre": this.Id_Amarre = value; break;
                case "TransferidoTabla": this.TransferidoTabla = value; break;
                case "Servicio": this.Servicio = value; break;
            }
        }
    }

    public class gsItem_BuscarResult
    {
        public object Item_ID;
        public object Codigo;
        public object Item;
        public object DctoMax;
        public object PrecioInicial;
        public object ID_Moneda;
        public object UnidadPresentacion;
        public object ID_UnidadInv;
        public object FactorUnidadInv;
        public object ID_UnidadControl;
        public object Stock;
        public object Estado;
        public object Precio;
        public object Importe;
        public object Cantidad;
        public object Costo;
        public object CostoUnitario;
        public object TC;
        public object idCCosto;
        public object Fecha;
        public object correlativo;
        public object NombreMoneda;
        public object Signo;
        public object Observacion;
        public object Descuento;


        public void GetValue(string field, object value)
        {
            switch (field)
            {
                case "Item_ID": this.Item_ID = value; break;
                case "Codigo": this.Codigo = value; break;
                case "Item": this.Item = value; break;
                case "DctoMax": this.DctoMax = value; break;
                case "PrecioInicial": this.PrecioInicial = value; break;
                case "ID_Moneda": this.ID_Moneda = value; break;
                case "UnidadPresentacion": this.UnidadPresentacion = value; break;
                case "ID_UnidadInv": this.ID_UnidadInv = value; break;
                case "FactorUnidadInv": this.FactorUnidadInv = value; break;
                case "ID_UnidadControl": this.ID_UnidadControl = value; break;
                case "Stock": this.Stock = value; break;
                case "Estado": this.Estado = value; break;
                case "Precio": this.Precio = value; break;
                case "Importe": this.Importe = value; break;
                case "Cantidad": this.Cantidad = value; break;
                case "Costo": this.Costo = value; break;
                case "CostoUnitario": this.CostoUnitario = value; break;
                case "TC": this.TC = value; break;
                case "idCCosto": this.idCCosto = value; break;
                case "Fecha": this.Fecha = value; break;
                case "correlativo": this.correlativo = value; break;
                case "NombreMoneda": this.NombreMoneda = value; break;
                case "Signo": this.Signo = value; break;
                case "Observacion": this.Observacion = value; break;
                case "Descuento": this.Descuento = value; break;

            }
        }
    }
}
