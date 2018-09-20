using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Configuration;

using WMS.DataAccess.Configuration;
using WMS.Entities.Common;

namespace WMS.DataAccess.Tareo
{
    public class daTareo
    {

        public int ProcesarTrasladoGuia(RowItemCollection itemcollection)
        {
            Database db = DatabaseFactory.CreateDatabase("");
            int nresult = -1;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                try
                {
                    DbCommand dbCommand;
                    var header = (from p in itemcollection.rows
                                  group p by new { Origen = p.v06, Destino = p.v07 } //or group by new {p.ID, p.Name, p.Whatever}
                                 into mygroup
                                  select mygroup.FirstOrDefault()).ToList();

                    dbCommand = db.GetStoredProcCommand("USP_WMS_TASK_HEAD_GUIATRASLADO");
                    db.AddInParameter(dbCommand, "@ID_ALMACEN_ORIGEN", System.Data.DbType.Decimal, Convert.ToDecimal(header[0].v06));
                    db.AddInParameter(dbCommand, "@ID_ALMACEN_DESTINO", System.Data.DbType.Decimal, Convert.ToDecimal(header[0].v07));
                    object primarykey = db.ExecuteScalar(dbCommand, transaction);
                    if (primarykey == null)
                        throw new Exception("Generación del Traslado: Primary Key no generada");
                    foreach (var item in itemcollection.rows)
                    {
                        dbCommand = db.GetStoredProcCommand("USP_WMS_TASK_DETAIL_GUIATRASLADO");
                        db.AddInParameter(dbCommand, "@ID_CABECERA", System.Data.DbType.Int32, primarykey);
                        db.AddInParameter(dbCommand, "@ID_ALMACEN_ORIGEN", System.Data.DbType.Decimal, Convert.ToDecimal(item.v06));
                        db.AddInParameter(dbCommand, "@ID_ALMACEN_DESTINO", System.Data.DbType.Decimal, Convert.ToDecimal(item.v07));
                        db.AddInParameter(dbCommand, "@ID_SKU_PRODUCTO", System.Data.DbType.String, item.v02);
                        db.AddInParameter(dbCommand, "@CANTIDAD", System.Data.DbType.Int32, Convert.ToInt32(item.v05));
                        db.AddInParameter(dbCommand, "@LOTE", System.Data.DbType.String, item.v04);
                        nresult = db.ExecuteNonQuery(dbCommand, transaction);
                    }

                    dbCommand = db.GetStoredProcCommand("USP_WMS_TASK_HEAD_CONFIRMTRASLADO");
                    db.AddInParameter(dbCommand, "@ID_ALMACEN_ORIGEN", System.Data.DbType.Decimal, Convert.ToDecimal(header[0].v06));
                    db.AddInParameter(dbCommand, "@ID_ALMACEN_DESTINO", System.Data.DbType.Decimal, Convert.ToDecimal(header[0].v07));
                    object primarykey_confirmacion = db.ExecuteScalar(dbCommand, transaction);
                    if (primarykey_confirmacion == null)
                        throw new Exception("Confirmación del Traslado: Primary Key no generada");

                    foreach (var item in itemcollection.rows)
                    {
                        dbCommand = db.GetStoredProcCommand("USP_WMS_TASK_DETAIL_CONFIRMTRASLADO");
                        //db.AddInParameter(dbCommand, "@ID_AMARRE", System.Data.DbType.Int32, primarykey);
                        db.AddInParameter(dbCommand, "@ID_CABECERA", System.Data.DbType.Int32, primarykey_confirmacion);
                        db.AddInParameter(dbCommand, "@ID_ALMACEN_ORIGEN", System.Data.DbType.Decimal, Convert.ToDecimal(item.v06));
                        db.AddInParameter(dbCommand, "@ID_ALMACEN_DESTINO", System.Data.DbType.Decimal, Convert.ToDecimal(item.v07));
                        db.AddInParameter(dbCommand, "@ID_SKU_PRODUCTO", System.Data.DbType.String, item.v02);
                        db.AddInParameter(dbCommand, "@CANTIDAD", System.Data.DbType.Int32, Convert.ToInt32(item.v05));
                        db.AddInParameter(dbCommand, "@LOTE", System.Data.DbType.String, item.v04);
                        nresult = db.ExecuteNonQuery(dbCommand, transaction);
                    }

                    if (nresult == -1)
                        transaction.Rollback();
                    else
                        transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    nresult = -1;
                    throw ex;
                }
                connection.Close();
            }
            db = null;
            return nresult;

        }

        public RowItemCollection GetLotesActivos(int esquema)
        {
            Database db = new SqlDatabase(GS.configuration.Init.GetValue(Constant.sistema, Constant.key, "genesys"));
            RowItemCollection ocol = new RowItemCollection();
            int nresult = -1;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbCommand dbCommand = db.GetStoredProcCommand(Entities.Common.Constant.getEsquemaBD(esquema) +  "USP_WMS_TASK_SEL_LOTEACTIVO");
                RowItem be;
                using (IDataReader dr = db.ExecuteReader(dbCommand))
                {
                    be = new RowItem();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        be.v01 = string.Format("{0}{1}", be.v01, dr.GetName(i) + "|");
                    }
                    ocol.rows.Add(be);

                    while (dr.Read())
                    {
                        be = new RowItem();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            be.v01 = string.Format("{0}{1}", be.v01, dr[i] + "|");
                        }

                        ocol.rows.Add(be);
                    }
                }
                ocol.rowsCount = ocol.rows.Count();
                connection.Close();
                return ocol;
            }
        }

        public RowItemCollection GetLotesActivos()
        {
            Database db = new SqlDatabase(GS.configuration.Init.GetValue(Constant.sistema, Constant.key, Constant.BD));
            RowItemCollection ocol = new RowItemCollection();
            int nresult = -1;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbCommand dbCommand = db.GetStoredProcCommand("USP_WMS_TASK_SEL_LOTEACTIVO");                    
                RowItem be;
                using (IDataReader dr = db.ExecuteReader(dbCommand))
                {
                    be = new RowItem();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        be.v01 = string.Format("{0}{1}", be.v01, dr.GetName(i) + "|");
                    }
                    ocol.rows.Add(be);

                    while (dr.Read())
                    {
                        be = new RowItem();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            be.v01 = string.Format("{0}{1}", be.v01, dr[i] + "|");
                        }
                        
                        ocol.rows.Add(be);
                    }
                }
                ocol.rowsCount = ocol.rows.Count();
                connection.Close();
                return ocol;
            }
        }

        public RowItemCollection ListarRowItemCollection(string procedimiento, List<object> parametros)
        {
            RowItemCollection ocol = new RowItemCollection();
            RowItem be;
            Query query = new Query(procedimiento);
            foreach (var item in parametros)
                query.input.Add(item);
            //query.connection = Constant.connectionWMSprod;
            using (IDataReader dr = new DAO().GetCollectionIReader(query))
            {
                ocol.nroColumns = dr.FieldCount;
                for (int idx = 0; idx < ocol.nroColumns; idx++)
                {
                    ocol.columns.Add(new Column()
                    {
                        campo = string.Format("v{0}", (idx + 1).ToString().PadLeft(2, '0')),
                        key = dr.GetName(idx),
                        index = idx,
                    });
                }
                int nroRows = 0;
                while (dr.Read())
                {
                    be = new RowItem();
                    foreach (Column c in ocol.columns)
                        be.GetValue(c.campo, dr[ocol.columns[c.index].index].ToString());
                    ocol.rows.Add(be);
                    nroRows++;
                }
                ocol.rowsCount = nroRows;
            }
            return ocol;
        }

        public DataSet ListarDataSet(string query)
        {

            Database db;
            //db = string.Format(ConfigurationManager.ConnectionStrings[dci.Empresa.SingleOrDefault(x => x.idEmpresa == idEmpresa).baseDatos].ConnectionString, "usrGEN" + (10000 + codigoUsuario).ToString().Substring(1, 4));
            //string coneccion;
            //coneccion = GS.configuration.Init.GetValue(Constant.sistema, Constant.key, Constant.BD);
            //coneccion = ConfigurationManager.ConnectionStrings["genesys"].ConnectionString; 

            //db = new SqlDatabase(ConfigurationManager.ConnectionStrings["genesys"].ConnectionString);
            db = new SqlDatabase(GS.configuration.Init.GetValue(Constant.sistema, Constant.key, Constant.BD));
   
            DataSet ds = null;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                ds = db.ExecuteDataSet(CommandType.Text, query);
                connection.Close();
            }
            return ds;
        }

        public void RegistrarLetraEmail(string query)
        {
            Database db;
            db = new SqlDatabase(GS.configuration.Init.GetValue(Constant.sistema, Constant.key, Constant.BD));
   
            DataSet ds = null;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                db.ExecuteNonQuery(CommandType.Text, query);
                connection.Close();
            }
        }

       
    }
}
