using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Xml;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using WMS.Entities.Common;

namespace WMS.DataAccess.Configuration
{
    public class DAO
    {
        public IDataReader GetCollectionIReader(Query query) 
        {
            Database db;
            IDataReader dr;
            //if (query.connection == null || query.connection == string.Empty)
            //    db = DatabaseFactory.CreateDatabase("Default");
            //else
            //    db = DatabaseFactory.CreateDatabase(query.connection);
            db = new SqlDatabase(GS.configuration.Init.GetValue(Constant.sistema, Constant.key, Constant.BD));
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbCommand dbCommand;
                if(query.input.Count()==0)
                {
                    dbCommand = db.GetStoredProcCommand(query.method);
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand(query.method, query.input.ToArray());
                }

                if (query.Timeout != 0)
                {
                    dbCommand.CommandTimeout = query.Timeout;
                }
 
                dr = db.ExecuteReader(dbCommand);
                connection.Close();
            }
            return dr;
        }

        public int ExecuteTransactions(Query query) 
        {
            Database db;
            int result;
            //if (query.connection == null || query.connection == string.Empty)
            //    db = DatabaseFactory.CreateDatabase("Default");
            //else
            //    db = DatabaseFactory.CreateDatabase(query.connection);
            db = new SqlDatabase(GS.configuration.Init.GetValue(Constant.sistema, Constant.key, Constant.BD));
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                DbCommand dbCommand;
                if (query.input.Count() == 0)
                    dbCommand = db.GetStoredProcCommand(query.method);
                else
                    dbCommand = db.GetStoredProcCommand(query.method, query.input.ToArray());
                if (query.Timeout != 0)
                    dbCommand.CommandTimeout = query.Timeout;

                result = db.ExecuteNonQuery(dbCommand);
                connection.Close();
            }
            return result;      
        }
    }
    
    public static class DAO_Temp 
    {
        public static string cnx;
    }
}
