using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Controles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AZDebutPage : ContentPage
    {
        protected AZComboCS cboid_prs = null;
        protected int m_id_prs;
        public AZDebutPage()
        {
            InitializeComponent();
            cboid_prs = new AZComboCS();
            Grid.SetColumn(cboid_prs, 1);
            Grid.SetRow(cboid_prs, 1);
            cboid_prs.titre = "Login";
            cboid_prs.base_req = "select @top id_prs as @id,dbo.fct_rep('prs',id_prs) as @lib from prs where 1=1 order by 2";
            cboid_prs.base_filtre_lib = " and dbo.fct_rep('prs',id_prs) like '@valeur'";
            cboid_prs.base_filtre_id = " and id_prs like @valeur";
            cboid_prs.taille_texte = 15.0;
            grlogin.Children.Add(cboid_prs);
        }
        protected virtual void Init()
        {
        }
        protected byte[] Retrouver(string str_tab)
        {
            int nb_char = str_tab.Length;
            int nb_octets = nb_char / 3;
            byte[] tab = new byte[nb_octets];
            for (int i = 0; i < nb_char; i += 3)
            {
                string temp = str_tab.Substring(i, 3);
                byte b = Convert.ToByte(temp);
                tab[i / 3] = b;
            }
            return tab;
        }
        protected string Crypter3(string ch)
        {
            string cle = "007125189077123030227149079076133056205068134019246007053246074062040179076008235094142229142200";
            string iv = "145010139090021189211222000125238018165039162092";
            byte[] tab_cle = Retrouver(cle);
            byte[] tab_iv = Retrouver(iv);
            byte[] tab_ch_crypte;
            string ch_crypte = "";
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = tab_cle;
                aesAlg.IV = tab_iv;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(ch);
                        }
                        tab_ch_crypte = msEncrypt.ToArray();
                    }
                }
            }
            ch_crypte = System.Text.ASCIIEncoding.UTF8.GetString(tab_ch_crypte).Replace("'", "''");
            return ch_crypte;
        }
        protected virtual void MemoriserPrs()
        {

        }
        protected async void btnok_Clicked(object sender, EventArgs e)
        {
            int? id_prs = cboid_prs.CboId;
            if (id_prs.HasValue)
            {

                string pwd_usr = this.txtpwd_usr.Text;
                string cle = Crypter3(pwd_usr);
                string sql = "exec valider_prs " + id_prs.ToString() + ",'" + cle + "'";
                AccesBdClient.AccesBdClient ab = new AccesBdClient.AccesBdClient();
                string cnx_ok = await ab.LireChaine(sql);
                if (cnx_ok == "OK")
                {
                    //                    ((App)Application.Current).SpecifierIdPrs(id_prs.Value);
                    m_id_prs = id_prs.Value;
                    MemoriserPrs();
                    //                    ((MainWindow)((App)Application.Current).MaMainWindow()).ActiverMenus();
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erreur", "Erreur de connexion", "Cancel");
                }
            }
        }
        protected async void btncancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}