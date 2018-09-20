using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WMS.Entities.Common
{
    public class Constant
    {
        public const string nameUser = "user";
        public const string nameOptions = "options";
        public const string quote = "\"";
        //public const string connectionWMStest = "connectionWMStest";
        //public const string connectionWMSprod = "connectionWMSprod";
        //public const string connectionWMS = "Data Source=10.10.1.8;Initial Catalog=REC_Sil;Persist Security Info=True;User ID=aplicacionesgs;Password=desarrollo2017";

        public static string sistema = ConfigurationManager.AppSettings["sistema"];
        public static string key = ConfigurationManager.AppSettings["key"];
        public static string BD = ConfigurationManager.AppSettings["BD"];

        public const int idaplicacion = 1;
        public const int idempresa = 1;

        public static string getEsquemaBD(int esquema)
        {
            switch(esquema)
            {
                case 1:
                    return "Silvestre_Peru_SAC..";
                case 2:
                    return "NeoAgrum_SAC..";
                default:
                    return "Silvestre_Peru_SAC..";
            }
        }

        public static string getesquemaFTP(int esquema)
        {
            switch (esquema)
            {
                case 1:
                    return "SILVESTRE";
                case 2:
                    return "NEOAGRUM";
                default:
                    return "SILVESTRE";
            }
        }
    }
}
