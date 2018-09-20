using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WMS.Entities.Common;
using WMS.Logic.Tareo;

namespace WMS.Tareo
{
    public partial class frmTareo : Form
    {
        const string estado_msg_sininiciar = "...Presionar iniciar";
        const string estado_msg_buscando = "...Buscando en ftp://";
        const string estado_msg_buscando_bd = "...Buscando en base de datos";
        const string estado_msg_detenido = "...Proceso detenido";
        const string estado_msg_esperando = "...Esperando proxima busqueda";


        const string estado_btn_iniciar = "Iniciar";
        const string estado_btn_cancelar = "Cancelar";
        const string estado_btn_detenido = "Detenido";
        const string ruta_ftp_inventario = "ftp://10.10.1.8/SILVESTRE/IN/INVENTARIO/";
        const string ruta_ftp_lotes = "ftp://10.10.1.8/SILVESTRE/IN/MAESTRO/LOTES/";
        string ruta_ftp_lotes_dinamico = "ftp://10.10.1.9/WMS/{0}/IN/MAESTRO/LOTES/";

        private Thread hiloTrasladoGuia;
        private Thread hiloSincronizarLotes;
    
        public frmTareo()
        {
            InitializeComponent();
            SincronizarLotes();
            //tmSincronizacionLotes.Interval = minuto * Convert.ToInt32(UpInterval_sincronizarlotes.Text);
            //tmTrasladoGuia.Interval = minuto * Convert.ToInt32(UpInterval_TrasladoGuia.Text);

            //lblSincronzarLotes.Text = lblTrasladoGuia.Text = estado_msg_sininiciar;
            //btnTrasladoGuia.Text = btnTrasladoGuia.Text = estado_btn_iniciar;

            //hiloTrasladoGuia = new Thread(RealizarTraslado);
            //hiloSincronizarLotes = new Thread(SincronizarLotes);
        }

        private void btnTrasladoGuia_Click(object sender, EventArgs e)
        {
            if (chkTrasladoGuia.Checked == true)
            {
                if (
                    btnTrasladoGuia.Text == estado_btn_iniciar || 
                    btnTrasladoGuia.Text == estado_btn_detenido
                    )
                {
                    lblTrasladoGuia.Text = estado_msg_buscando;
                    btnTrasladoGuia.Text = estado_btn_cancelar;
                    if(!hiloTrasladoGuia.IsAlive)
                        RealizarTraslado();
                }
                else if (btnTrasladoGuia.Text == estado_btn_cancelar)
                {
                    lblTrasladoGuia.Text = estado_msg_detenido;
                    btnTrasladoGuia.Text = estado_btn_iniciar;
                }
            }
            else
            {
                MessageBox.Show("Ud. debe seleccionar el tareo \"Traslado de Guía\"", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void chkTrasladoGuia_CheckStateChanged(object sender, EventArgs e)
        {            
            if (btnTrasladoGuia.Text == estado_btn_cancelar)
            {
                chkTrasladoGuia.Checked = true;
                MessageBox.Show("Ud. debe detener el tareo \"Traslado de Guía\"", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                
            }
        }

        private void tmTrasladoGuia_Tick(object sender, EventArgs e)
        {
            if (chkTrasladoGuia.Checked && (btnTrasladoGuia.Text != estado_btn_iniciar))
            {
                RealizarTraslado();
            }
        }

        private void ReadFileFromFTPTrasladoGuia(List<string> files)
        {
            foreach (string file in files)
            {
                if (file != null && file.Contains(".txt"))
                {
                    
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ruta_ftp_inventario + file);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential("ftpwms", "wmsapia");
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    string contents = reader.ReadToEnd();

                    Transaction transaction = Common.InitTransaction();
                    List<RowItem> itemlist = new List<RowItem>();
                    string line = "";
                    using (StringReader strReader = new StringReader(contents))
                    {                        
                        int contador = 0;
                        while ((line = strReader.ReadLine()) != null)
                        {
                            if (contador > 0)
                            {
                                RowItem item = new RowItem();
                                item.v01 = line.Split('|')[0];
                                item.v02 = line.Split('|')[1];
                                item.v03 = line.Split('|')[2];
                                item.v04 = line.Split('|')[3];
                                item.v05 = line.Split('|')[4];
                                item.v06 = line.Split('|')[5];
                                itemlist.Add(item);
                            }
                            contador++;                            
                        }
                    }
                    
                    reader.Close();
                    reader.Dispose();
                    response.Close();                    


                    RowItemCollection itemcollection = new RowItemCollection();   
                    var itemlistOrder =
                        (from row in itemlist
                        orderby row.v02, row.v06
                        select row).ToList();
                    for (int i = 0; i < itemlistOrder.Count(); i++)
                    {
                        RowItem item = new RowItem();
                        item.v01 = itemlistOrder.ToList()[i].v01;
                        item.v02 = itemlistOrder.ToList()[i].v02;
                        item.v03 = itemlistOrder.ToList()[i].v03;
                        item.v04 = itemlistOrder.ToList()[i].v04;
                        item.v05 = Convert.ToInt32(Math.Abs(Convert.ToDecimal(itemlistOrder.ToList()[i].v05))).ToString();
                        item.v06 = itemlistOrder.ToList()[i].v06;
                        i++;
                        item.v07 = itemlistOrder.ToList()[i].v06;
                        itemcollection.rows.Add(item);                     
                    }

                    if(!Directory.Exists(@"C:\Data"))
                        Directory.CreateDirectory(@"C:\Data");
                    if (!Directory.Exists(@"C:\Data\temp"))
                        Directory.CreateDirectory(@"C:\Data\temp");
                    string name = string.Format(@"{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), file);
                    string temp = string.Format(@"C:\Data\temp\{0}", name);
                    //File.Create(temp);
                    List<string> lines = new List<string>();
                    foreach (var row in itemcollection.rows)
                        lines.Add(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", row.v01, row.v02, row.v03, row.v04, row.v05, row.v06, row.v07));
                    
                    bool result = new blTareo().ProcesarTrasladoGuia(itemcollection, out transaction);
                    lines.Add(transaction.type.ToString() + ":" + transaction.message);
                    File.WriteAllLines(temp, lines.ToArray());
                    
                    if (result)
                        UploadFTP(temp, ruta_ftp_inventario + "success/", "ftpwms", "wmsapia");
                    else
                        UploadFTP(temp, ruta_ftp_inventario + "failed/", "ftpwms", "wmsapia");

                    //Eliminar fichero
                    request = (FtpWebRequest)WebRequest.Create(ruta_ftp_inventario + file);
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new NetworkCredential("ftpwms", "wmsapia");
                    response = (FtpWebResponse)request.GetResponse();                    
                    response.Close();

                    File.Delete(temp);
                    

                }

            }
        }
        private void RealizarTraslado()
        {
            try
            {

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ruta_ftp_inventario);
                //FtpWebRequest ftpRequest = System.Net.FtpWebRequest.GetRequest
                ftpRequest.Credentials = new NetworkCredential("ftpwms", "wmsapia");
                //ftpRequest.UsePassive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                List<string> directories = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
                ReadFileFromFTPTrasladoGuia(directories);
                lblTrasladoGuia.Text = estado_msg_esperando + " " + DateTime.Now.ToString("HH:mm:ss");

                hiloTrasladoGuia.Interrupt();
                hiloTrasladoGuia = new Thread(RealizarTraslado);
            }
            catch (Exception ex)
            {
                hiloTrasladoGuia.Interrupt();
                hiloTrasladoGuia = new Thread(RealizarTraslado);
            }
        }

        private void SincronizarLotes()
        {
            try
            {
                for (int i = 1; i <= 2; i++)
                {
                        RowItemCollection ocol = new RowItemCollection();
                        blTareo bl = new blTareo();
                        Transaction transaction = Common.InitTransaction();
                        RowItemCollection itemcollection = bl.GetLotesActivos(i, out transaction);
                        const string file = "LotesActivos";

                        if (!Directory.Exists(@"C:\Data"))
                            Directory.CreateDirectory(@"C:\Data");
                        if (!Directory.Exists(@"C:\Data\temp"))
                            Directory.CreateDirectory(@"C:\Data\temp");
                        string name = string.Format(@"{0}_{1}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"), file);
                        string temp = string.Format(@"C:\Data\temp\{0}", name);
                        //File.Create(temp);
                        List<string> lines = new List<string>();
                        foreach (var row in itemcollection.rows)
                            lines.Add(string.Format("{0}", row.v01));
                        File.WriteAllLines(temp, lines.ToArray());
                        try
                        {
                            //string.Format(ruta_ftp_lotes_dinamico,Entities.Common.Constant.getesquemaFTP(i))
                            UploadFTP(temp, string.Format(ruta_ftp_lotes_dinamico, Entities.Common.Constant.getesquemaFTP(i)), "ftpwms", "wmsapia");
                        }
                        catch (Exception ex) { }
                        File.Delete(temp);
                        //lblSincronzarLotes.Text = estado_msg_esperando + " " + DateTime.Now.ToString("HH:mm:ss");
                }
                Environment.Exit(0);
                //lblSincronzarLotes.Text = estado_msg_esperando + " " + DateTime.Now.AddMinutes(Convert.ToDouble(UpInterval_sincronizarlotes.Text)).ToString("HH:mm:ss");
                //hiloSincronizarLotes.Interrupt();
                //hiloSincronizarLotes = new Thread(SincronizarLotes);

            }
                catch (Exception ex)
                {
                    Transaction transaction = Common.GetTransaction(TypeTransaction.ERR, ex.Message);
                }

        }
            
        public static void UploadFTP(string FilePath, string RemotePath, string Login, string Password)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string url = Path.Combine(RemotePath, Path.GetFileName(FilePath));
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(url);
                ftp.Credentials = new NetworkCredential(Login, Password);
                ftp.KeepAlive = false;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;
                ftp.UseBinary = true;
                ftp.ContentLength = fs.Length;
                ftp.Proxy = null;
                fs.Position = 0;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];

                int contentLen;
                using (Stream strm = ftp.GetRequestStream())
                {
                    contentLen = fs.Read(buff, 0, buffLength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
                }
            }
        }

        private void btnSincronizaLotes_Click(object sender, EventArgs e)
        {
            if (chkSincronizarLotes.Checked == true)
            {
                if (
                    btnSincronizaLotes.Text == estado_btn_iniciar ||
                    btnSincronizaLotes.Text == estado_btn_detenido
                    )
                {
                    lblSincronzarLotes.Text = estado_msg_buscando_bd;                    
                    btnSincronizaLotes.Text = estado_btn_cancelar;
                    if (!hiloSincronizarLotes.IsAlive)
                        SincronizarLotes();
                }
                else if (btnSincronizaLotes.Text == estado_btn_cancelar)
                {
                    lblSincronzarLotes.Text = estado_msg_detenido;
                    btnSincronizaLotes.Text = estado_btn_iniciar;
                }
            }
            else
            {
                MessageBox.Show("Ud. debe seleccionar el tareo \"Sincronizar Lotes\"", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void tmSincronizacionLotes_Tick(object sender, EventArgs e)
        {
            if (chkSincronizarLotes.Checked && (btnSincronizaLotes.Text != estado_btn_iniciar))
            {
                SincronizarLotes();
            }
        }

        const int minuto = 60000;
        private void UpInterval_sincronizarlotes_SelectedItemChanged(object sender, EventArgs e)
        {
            if (chkSincronizarLotes.Checked == false)
            {
                tmSincronizacionLotes.Interval = minuto * Convert.ToInt32(UpInterval_sincronizarlotes.Text);
            }
            else
            {
                UpInterval_sincronizarlotes.Text = (tmSincronizacionLotes.Interval / minuto).ToString();
                MessageBox.Show("Ud. primero debe desactivar el tareo \"Sincronizar Lotes\"", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void UpInterval_TrasladoGuia_SelectedItemChanged(object sender, EventArgs e)
        {
            if (chkTrasladoGuia.Checked == false)
            {
                tmTrasladoGuia.Interval = minuto * Convert.ToInt32(UpInterval_TrasladoGuia.Text);
            }
            else
            {
                UpInterval_TrasladoGuia.Text = (tmTrasladoGuia.Interval / minuto).ToString();
                MessageBox.Show("Ud. primero debe desactivar el tareo \"Traslado Guía\"", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
