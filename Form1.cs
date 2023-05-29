using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;

namespace Doviz_Ofis
{
    public partial class FrmGiris : Form
    {
        public FrmGiris()
        {
            InitializeComponent();
        }

        SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-1PLLIFRV\SQLEXPRESS;Initial Catalog=DbDovizIslem;Integrated Security=True");

        public void listele()
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("Select * From TBLISLEM", connection);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dgrwTotal.DataSource = dt;
            connection.Close();

        }

        public void guncelle()
        {
            connection.Open();
            SqlCommand sql = new SqlCommand("Select SUM(DOLAR),SUM(EURO),SUM(TL) from TDE", connection);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                tbxdolartotal.Text= dr[0].ToString()+' '+ "$";
                tbxeurototal.Text= dr[1].ToString()+' '+ "€";    
                tbxtltotal.Text= dr[2].ToString()+' '+ "₺";
            }
            connection.Close();
        }
        private void FrmGiris_Load(object sender, EventArgs e)
        {
            listele();
            guncelle();
           
            string today = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var xmlfile = new XmlDocument();
            xmlfile.Load(today);

            string dollarBuying = xmlfile.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteBuying").InnerXml;
            lblDolarAlis.Text = dollarBuying;

            string dollarSelling = xmlfile.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml;
            lblDolarSatis.Text = dollarSelling;

            string euroBuying = xmlfile.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteBuying").InnerXml;
            lblEuroAlis.Text = euroBuying;

            string euroSelling = xmlfile.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml;
            lblEuroSatis.Text = euroSelling;


        }

        private void button4_Click(object sender, EventArgs e)
        {
            tbxKur.Text = lblDolarAlis.Text;
            cbxMiktar.SelectedIndex = 0;
            btnBuy.Enabled = true;
            btnExchange.Enabled = false;            

            tbxMiktar.Clear();
            tbxTutar.Clear();
            tbxKalan.Clear();
                    
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tbxKur.Text = lblDolarSatis.Text;
            cbxMiktar.SelectedIndex = 2;
            btnExchange.Enabled = true;
            btnBuy.Enabled = false;         

            tbxMiktar.Clear();
            tbxTutar.Clear();
            tbxKalan.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tbxKur.Text = lblEuroAlis.Text;
            cbxMiktar.SelectedIndex = 0;
            btnBuy.Enabled = true;
            btnExchange.Enabled = false;     

            tbxMiktar.Clear();
            tbxTutar.Clear();
            tbxKalan.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbxKur.Text = lblEuroSatis.Text;
            cbxMiktar.SelectedIndex = 1;
            btnExchange.Enabled= true;
            btnBuy.Enabled = false;           

            tbxMiktar.Clear();
            tbxTutar.Clear();
            tbxKalan.Clear();
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            double kur, miktar, tutar, kalan;
            kur = Convert.ToDouble(tbxKur.Text);
            miktar = Convert.ToDouble(tbxMiktar.Text);
            tutar = miktar/kur;
            if (button4.Enabled)
            {
                tbxTutar.Text = tutar.ToString()+' '+"USD";
            }
            else if (button3.Enabled)
            {
                tbxTutar.Text = tutar.ToString()+' ' +"EUR";
            }
            else
            {
                Application.Exit();
            }
            kalan = miktar % kur;
            tbxKalan.Text = kalan.ToString()+' '+"TL";
            double eksimiktar = -1 * miktar;           

            if (button4.Enabled)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Insert into TBLISLEM (TL,DOLAR) values (@P1,@P2)", connection);
                cmd.Parameters.AddWithValue("@P1", eksimiktar + kalan);
                cmd.Parameters.AddWithValue("@P2", tutar);
                cmd.ExecuteNonQuery();
                connection.Close();

                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("update TDE set TL = TL + @P1,DOLAR= DOLAR - @P2", connection);
                sqlCommand.Parameters.AddWithValue("@P1", miktar);
                sqlCommand.Parameters.AddWithValue("@P2", tutar);
                sqlCommand.ExecuteNonQuery();
                connection.Close();

                listele();
                guncelle();
                
            }
            if (button3.Enabled)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Insert into TBLISLEM (TL,EURO) values (@P1,@P2)", connection);
                cmd.Parameters.AddWithValue("@P1", eksimiktar);
                cmd.Parameters.AddWithValue("@P2", tutar);
                cmd.ExecuteNonQuery();
                connection.Close();

                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("update TDE set TL= TL + @P1,EURO= EURO - @P2", connection);
                sqlCommand.Parameters.AddWithValue("@P1", miktar);
                sqlCommand.Parameters.AddWithValue("@P2", tutar);
                sqlCommand.ExecuteNonQuery();
                connection.Close();

                listele();
                guncelle();
                
            }
            
        }

        private void tbxKur_TextChanged(object sender, EventArgs e)
        {
            tbxKur.Text = tbxKur.Text.Replace(".", ",");
        }

        private void btnExchange_Click(object sender, EventArgs e)
        {
           double kur, miktar, tutar;
           kur = Convert.ToDouble(tbxKur.Text);
           miktar = Convert.ToDouble(tbxMiktar.Text);
           tutar = miktar * kur;
           if (button5.Enabled && button1.Enabled)
           {
               tbxTutar.Text = tutar.ToString()+' '+"TL";
           }
           else
            {
                Application.Exit();
            }
           double eksimiktar = -1 * miktar;

            if (button5.Enabled)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Insert into TBLISLEM(TL,DOLAR) values (@k1,@k2)", connection);
                cmd.Parameters.AddWithValue("@k1",  tutar);
                cmd.Parameters.AddWithValue("@k2", eksimiktar);
                cmd.ExecuteNonQuery();
                connection.Close();


                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("update TDE set TL = TL - @P1,DOLAR = DOLAR - @P2", connection);
                sqlCommand.Parameters.AddWithValue("@P1", tutar);
                sqlCommand.Parameters.AddWithValue("@P2", miktar);
                sqlCommand.ExecuteNonQuery();
                connection.Close();

                listele();
                guncelle();
                

            }

            if (button1.Enabled)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Insert into TBLISLEM(TL,EURO) values (@k1,@k2)", connection);
                cmd.Parameters.AddWithValue("@k1",  tutar);
                cmd.Parameters.AddWithValue("@k2",  eksimiktar);
                cmd.ExecuteNonQuery();
                connection.Close();

                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("update TDE set TL= TL - @P1,EURO= EURO - @P2", connection);
                sqlCommand.Parameters.AddWithValue("@P1", tutar);
                sqlCommand.Parameters.AddWithValue("@P2", miktar);
                sqlCommand.ExecuteNonQuery();
                connection.Close();

                listele();
                guncelle();
                
            }
        }

        
    }
}
