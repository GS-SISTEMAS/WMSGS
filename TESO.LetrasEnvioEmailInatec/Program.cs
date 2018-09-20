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
using System.Web.Script.Serialization;
using System.Net.Mail;

namespace TESO.LetrasEnvioEmailInatec
{

    class Program
    {
        static void Main(string[] args)
        {
            string ruc_empresa = "";
            int idEmpresa = 0;


            string esquemaREC = ConfigurationSettings.AppSettings["BD_inatec"].ToString();
            ruc_empresa = "20505467214";
            idEmpresa = 6;

            EnviarCorreos(idEmpresa, ruc_empresa, esquemaREC);



        }


        public static void EnviarCorreos(Int32 idEmpresa, string ruc_empresa, string esquemaREC)
        {

            

                DataTable clientes = new DataTable();
                List<string> EmailVendedores = new List<string>();
                List<string> EmailCliente = new List<string>();
                List<string> lstopDoc_Venta = new List<string>();
                string responseFromServer = string.Empty;

                clientes = ObtenerClientesPendientesEnvioEmail(esquemaREC);
                if (!Directory.Exists(@"C:\Email\Inatec\"))
                    Directory.CreateDirectory(@"C:\Email\Inatec\");

                string[] Archivos = Directory.GetFiles(@"C:\Email\Inatec\");
                foreach (string pdfs in Archivos)
                    File.Delete(pdfs);

                foreach (DataRow row in clientes.Rows)
                {
                    Archivos = Directory.GetFiles(@"C:\Email\Inatec\");
                    foreach (string pdfs in Archivos)
                        File.Delete(pdfs);

                    EmailCliente = ObtenerEmailsCliente(esquemaREC, row["ID_Agenda"].ToString());
                    EmailVendedores = ObtenerEmailsVendedor(esquemaREC, row["ID_Agenda"].ToString());
                    lstopDoc_Venta = ObtenerLetrasPendientesdeEnvio(esquemaREC, row["ID_Agenda"].ToString());

                    foreach (string opdoc_venta in lstopDoc_Venta)
                    {
                        responseFromServer = POSTResult(
                        //"http://localhost:49573/Comercial/Facturacion/Gestionar/frmFacturaElectronica_usuPDF.aspx/ObtenerPDFLetra",
                        "https://intranet.gruposilvestre.com.pe/IntranetGSqa/Comercial/Facturacion/Gestionar/frmFacturaElectronica_usuPDF.aspx/ObtenerPDFLetra",
                        Convert.ToInt32(opdoc_venta), idEmpresa
                        );
                        responseFromServer = responseFromServer.Replace("{\"d\":\"", "").Replace("\"}", "");
                        byte[] bytes = System.Convert.FromBase64String(responseFromServer);

                        string nombreletra = "Letra" + opdoc_venta + "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".pdf";
                        File.WriteAllBytes(@"C:\Email\Inatec\" + nombreletra, bytes);
                    }


                    try
                    {
                        EnviarCorreo(EmailCliente, EmailVendedores);

                        foreach (string opdoc_venta in lstopDoc_Venta)
                        {
                            RegistrarFlagEnvioEmail(esquemaREC, row["ID_Agenda"].ToString(), opdoc_venta);
                        }
                    }
                    catch (Exception ex)
                    {
                        foreach (string opdoc_venta in lstopDoc_Venta)
                        {
                            RegistrarErrorEnvioEmail(esquemaREC, row["ID_Agenda"].ToString(), opdoc_venta, ex.Message);
                        }
                    }

                }
            
        }

        public static void RegistrarErrorEnvioEmail(string esquema, string id_agenda, string op_docventa, string mensajerror)
        {

            try
            {
                blTareo bl = new blTareo();
                //Lista Producción por recibir
                bl.RegistrarLetraEmail("exec " + esquema + "..USP_INS_ErrorEnvioEmail '" + id_agenda + "'," + op_docventa + ",'" + mensajerror + "'");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void RegistrarFlagEnvioEmail(string esquema, string id_agenda, string op_docventa)
        {

            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                bl.RegistrarLetraEmail("exec " + esquema + "..USP_UDP_LetrasPorEnviarEmail '" + id_agenda + "'," + op_docventa);


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public static DataTable ObtenerClientesPendientesEnvioEmail(string esquema)
        {
            DataTable dtTabla = new DataTable();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet("exec " + esquema + "..USP_SEL_LetrasPendientesEnvio ");

                //Busca registro de interfaz
                dtTabla = ds1.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtTabla;
        }


        public static List<string> ObtenerEmailsCliente(string esquema, string id_agenda)
        {
            DataTable dtTabla = new DataTable();
            List<string> lst = new List<string>();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet("exec " + esquema + "..USP_SEL_CorreosCliente '" + id_agenda + "'");

                //Busca registro de interfaz
                dtTabla = ds1.Tables[0];
                foreach (DataRow row in dtTabla.Rows)
                    lst.Add(row["EMail"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lst;
        }
        public static List<string> ObtenerEmailsVendedor(string esquema, string id_agenda)
        {
            DataTable dtTabla = new DataTable();
            List<string> lst = new List<string>();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet("exec " + esquema + "..USP_SEL_CorreosVendedor '" + id_agenda + "'");

                //Busca registro de interfaz
                dtTabla = ds1.Tables[0];
                foreach (DataRow row in dtTabla.Rows)
                    lst.Add(row["EMail"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lst;
        }
        public static List<string> ObtenerLetrasPendientesdeEnvio(string esquema, string id_agenda)
        {
            DataTable dtTabla = new DataTable();
            List<string> lst = new List<string>();
            try
            {

                blTareo bl = new blTareo();
                //Lista Producción por recibir
                DataSet ds1 = bl.ListarDataSet("exec " + esquema + "..USP_SEL_LetrasPorEnviarEmail '" + id_agenda + "'");

                //Busca registro de interfaz
                dtTabla = ds1.Tables[0];
                foreach (DataRow row in dtTabla.Rows)
                    lst.Add(row["Op_DocVenta"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lst;
        }

        #region CREARPDF


        public static string POSTResult(string method, int Op, int Empresa)
        {
            string postData =
            "{" +
                "\"Op\": " + Op + "," +
                "\"Empresa\": " + Empresa +
            "}";

            WebRequest request = WebRequest.Create(method);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        private static void EnviarCorreo(List<string> lstclientes, List<string> lstvendedores)
        {
            try
            {

                MailMessage Mail = new MailMessage();
                Mail.From = new System.Net.Mail.MailAddress(ConfigurationSettings.AppSettings.Get("UsuarioMail"));

                foreach (string email in lstclientes)
                    Mail.To.Add(new MailAddress(email));

                foreach (string email in lstvendedores)
                    Mail.To.Add(new MailAddress(email));

                //Mail.To.Add(new MailAddress("andy_14_08_@hotmail.com"));
                //Mail.CC.Add(new MailAddress("ray.mendoza@gruposilvestre.com.pe"));

                Mail.Subject = "Envio de letras Generadas";

                Mail.Priority = MailPriority.Normal;
                Mail.IsBodyHtml = true;
                Mail.BodyEncoding = System.Text.Encoding.UTF8;

                string[] Archivos = Directory.GetFiles(@"C:\Email\Inatec\");
                foreach (string pdfs in Archivos)
                    Mail.Attachments.Add(new Attachment(pdfs));

                string body = string.Empty;
                body = "Adjuntamos sus letras  de compromiso,agradecemos las pueda imprimir y en coordinación con su respectivo representante de ventas para el recojo respectivo." + "<br/><br/><br/>" + " Atte." + "<br/>" + "";
                body = "<html><body>" + "<table cellpadding='0'; border='0'; cellspacing='1'; width='1000'; height='auto'; style=font-family:Calibri; font size:9px>" +
                       "<tr>" + "<td>" + "<font color=black>" + "<b>" + "Estimado Cliente:" + "</b> " + "</td>" + "</tr>" +
                       "<tr>" + "<td>" + "<font color=black>" + body + "</td>" + "</tr></table></body></html>";

                Mail.Body = body;
                System.Net.Mail.SmtpClient Smtp = new System.Net.Mail.SmtpClient();
                Smtp.Port = 587;
                Smtp.Host = "smtp.gmail.com";
                Smtp.EnableSsl = true;
                Smtp.Credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings.Get("UsuarioMail"), ConfigurationSettings.AppSettings.Get("PasswordMail"));
                Smtp.Send(Mail);
                Mail.Dispose();
            }

            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
        }
        #endregion  
    }
}
