using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WMS.DataAccess.Configuration;
using WMS.DataAccess.Tareo;
using WMS.Entities.Common;

namespace WMS.Logic.Tareo
{
    public class blTareo
    {

        public bool ProcesarTrasladoGuia(RowItemCollection rows, out Transaction transaction)
        {
            try
            {
                DAO dao = new DAO();
                daTareo da = new daTareo();
                int result = da.ProcesarTrasladoGuia(rows);
                if (result == 0)
                {
                    transaction = Common.GetTransaction(TypeTransaction.ERR, "No se realizó la transacción");
                    return false;
                }
                else
                {
                    transaction = Common.GetTransaction(TypeTransaction.OK, "Operación realizada satisfactoriamente");
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                transaction = Common.GetTransaction(TypeTransaction.ERR, ex.Message);
                return false;
            }
        }

        public RowItemCollection GetLotesActivos(out Transaction transaction)
        {
            RowItemCollection ocol = new RowItemCollection();
            try
            {
                DAO dao = new DAO();
                daTareo da = new daTareo();
                ocol = da.GetLotesActivos();
                if (ocol.rowsCount == 0)
                {
                    transaction = Common.GetTransaction(TypeTransaction.ERR, "La consulta no muestra registros de lotes activos");
                    return ocol;
                }
                else
                {
                    transaction = Common.GetTransaction(TypeTransaction.OK, "");
                    return ocol;
                }

            }
            catch (Exception ex)
            {
                transaction = Common.GetTransaction(TypeTransaction.ERR, ex.Message);
                return ocol;
            }
        }

        public RowItemCollection GetLotesActivos(int esquema, out Transaction transaction)
        {
            RowItemCollection ocol = new RowItemCollection();
            try
            {
                DAO dao = new DAO();
                daTareo da = new daTareo();
                ocol = da.GetLotesActivos(esquema);
                if (ocol.rowsCount == 0)
                {
                    transaction = Common.GetTransaction(TypeTransaction.ERR, "La consulta no muestra registros de lotes activos");
                    return ocol;
                }
                else
                {
                    transaction = Common.GetTransaction(TypeTransaction.OK, "");
                    return ocol;
                }

            }
            catch (Exception ex)
            {
                transaction = Common.GetTransaction(TypeTransaction.ERR, ex.Message);
                return ocol;
            }
        }

        public DataSet ListarDataSet(string query)
        {
            return new daTareo().ListarDataSet(query);
        }

        public void RegistrarLetraEmail(string query)
        {
            new daTareo().RegistrarLetraEmail(query);
           
        }
    }
}