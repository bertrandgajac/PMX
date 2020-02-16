using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
//using Xamarin.Forms.Internals;
//using Newtonsoft.Json;
using AZ;

namespace AccesBdClient
{
    public class MyTraceHandler : DelegatingHandler
    {
        public MyTraceHandler() : this(new HttpClientHandler())
        {
        }

        public MyTraceHandler(HttpMessageHandler inner) : base(inner)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
    public class MyValidationHandler : DelegatingHandler
    {
        public MyValidationHandler() : this(new HttpClientHandler())
        {
        }

        public MyValidationHandler(HttpMessageHandler inner) : base(inner)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            /*
            if (!response.IsSuccessStatusCode)
                response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            */
            return response;
        }
    }
    /*
    public class TrustAllCertificatePolicy : ICertificatePolicy
    {
        public TrustAllCertificatePolicy() { }
        public bool CheckValidationResult(ServicePoint sp,
            System.Security.Cryptography.X509Certificates.X509Certificate cert,
            WebRequest req,
            int problem)
        {
            return cert.Issuer == "CN=www.bertrandgajac.hopto.org";
        }
    }
    */
    enum TYPE_ACCES_SERVICE_WEB { attendre = 0, definir_serveur_ou_bd = 1, valider_utilisateur = 2, lire_ensemble_de_tables = 3, ecrire_table = 4, lire_blob = 5, ecrire_blob=6, exec_sql=7, lire_une_valeur=8 }

    public class AccesBdClient
    {
        //        private string prefixe_url = "http://10.6.32.112:56248/";
        //        private string prefixe_url = "http://192.168.1.36:92/";
        //private string prefixe_url = "https://192.168.1.36:44338/";
//                private string prefixe_url = "http://localhost:60753/";
        //        private string prefixe_url = "http://localhost:80/";
        //private string prefixe_url = "https://localhost:44338/";
        // private string prefixe_url = "https://0.0.0.0:44338/";
                private string prefixe_url = "http://bertrandgajac.hopto.org:9003/AccesBdPm/";
        //        private string prefixe_url = "http://86.71.67.98:92/";
        private bool php = true;
        private const string separateur = "§";
        private const string serveur = "bertrand-pc\\bd_mssql";
        private const string bd = "gestion_pm";

        public AccesBdClient()
        {
        }

        private string NomRequetePhp(TYPE_ACCES_SERVICE_WEB tasw, string sql, string donnees)
        {
            string nom_php = "";
            switch (tasw)
            {
                case TYPE_ACCES_SERVICE_WEB.attendre:
                    nom_php = "Attendre.php";
                    break;
                case TYPE_ACCES_SERVICE_WEB.definir_serveur_ou_bd:
                    nom_php = "DefinirServeurOuBd.php?sql=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.valider_utilisateur:
                    nom_php = "ValiderUtilisateur.php?sql=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.lire_ensemble_de_tables:
                    nom_php = "LireEnsembleDeTables.php?sql=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.ecrire_table:
                    nom_php = "EcrireTable.php?sql=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.lire_blob:
                    nom_php = "LireBlob.php?sql=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.ecrire_blob:
                    nom_php = "EcrireBlob.php?sql=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.exec_sql:
                    nom_php = "ExecSql.php";
                    break;
                case TYPE_ACCES_SERVICE_WEB.lire_une_valeur:
                    nom_php = "LireUneValeur.php?sql=" + sql;
                    break;
            }
            return nom_php;
        }

        private string ContenuCSharp(TYPE_ACCES_SERVICE_WEB tasw, string sql, string donnees)
        {
            string contenu = "";
            switch (tasw)
            {
                case TYPE_ACCES_SERVICE_WEB.attendre:
                    contenu = "Attendre";
                    break;
                case TYPE_ACCES_SERVICE_WEB.definir_serveur_ou_bd:
                    contenu = sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.valider_utilisateur:
                    contenu = "valider_utilisateur=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.lire_ensemble_de_tables:
                    contenu = "LireEnsembleDeTables=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.ecrire_table:
                    contenu = "EcrireTable=" + sql + separateur + donnees;
                    break;
                case TYPE_ACCES_SERVICE_WEB.lire_blob:
                    contenu = "LireBlob=" + sql;
                    break;
                case TYPE_ACCES_SERVICE_WEB.ecrire_blob:
                    contenu = "EcrireBlob=" + sql + separateur + donnees;
                    break;
                case TYPE_ACCES_SERVICE_WEB.exec_sql:
                    contenu = "ExecSql=" + sql;
                    break;
            }
            return contenu;
        }

        private async Task<string> AccesDeBase(TYPE_ACCES_SERVICE_WEB tasw, string sql, string donnees)
        {
            System.Net.ServicePointManager.DnsRefreshTimeout = 0;
            var trace = new MyTraceHandler();
            var validation = new MyValidationHandler(trace);
            /*
            var client = new HttpClient(validation);
            var url_val = new Uri(prefixe_url + sql);
            HttpRequestMessage msg_val = new HttpRequestMessage(HttpMethod.Get, url_val);
            //                msg.Properties.Add
            //                msg.Headers.Add("Accept", "application/json");
            HttpResponseMessage reponse_val = await client.SendAsync(msg_val);
            */
            var client = new HttpClient();
            client.BaseAddress = new Uri(prefixe_url);
            client.MaxResponseContentBufferSize = 2000000000;
            string request = php ? NomRequetePhp(tasw, sql, donnees) : "api/accesbd";
//            request = "AccesBdPm/accesbd.php";
            string serveur_bd = "(" + serveur + "/" + bd + ")";
            // 20190414            string contenu = JsonConvert.SerializeObject(php ? "" : serveur_bd + ContenuCSharp(tasw, sql, donnees));
            //string contenu = JsonConvert.SerializeObject(php ? "" : serveur_bd + ContenuCSharp(tasw, sql, donnees));
            TraducJson tj = new TraducJson();
            string contenu_initial = php ? "" : serveur_bd + ContenuCSharp(tasw, sql, donnees);
            string contenu = tj.SerialiserObjet(contenu_initial);
            HttpRequestMessage msg_val = new HttpRequestMessage(HttpMethod.Get, request);
            msg_val.Content = new StringContent(contenu);
            msg_val.Content.Headers.ContentType.MediaType = "application/json";
            //            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, error) =>
            {
                return cert.Issuer == "CN=www.bertrandgajac.hopto.org";
            };
            HttpResponseMessage reponse_val = await client.SendAsync(msg_val);
            string json_val = "";
            if (reponse_val.StatusCode == System.Net.HttpStatusCode.OK)
            {
                json_val = await reponse_val.Content.ReadAsStringAsync();
                var json_val_stream = await reponse_val.Content.ReadAsStreamAsync();
                if (json_val.StartsWith("Erreur"))
                {
                    throw new Exception(json_val);
                }
                else if (json_val.Length == 0)
                {
                    throw new Exception("Erreur: réponse vide du Web Service à la requête (" + sql + ")");
                }
            }
            else
            {
                throw new Exception("Erreur: réponse du Web Service à la requête (" + sql + ")=" + reponse_val.StatusCode.ToString());
            }
            return json_val;
        }

        public async Task<string> LireUneValeurOuVide(string sql)
        {
            /*
            string val = "";
            DataTable dt = await LireTable(sql);
            if (dt.Rows.Count == 1)
            {
                val = dt.Rows[0][0].ToString();
                if (val.StartsWith("\""))
                {
                    val = val.Substring(1);
                }
                if (val.EndsWith("\""))
                {
                    val = val.Substring(0, val.Length - 1);
                }
            }
            */
            string val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.lire_une_valeur, sql, "");
            return val;
        }
        public async Task<string> LireUneValeur(string sql)
        {
            /*
            DataTable dt= await LireTable(sql);
            if(dt == null || dt.Rows == null && dt.Rows.Count == 0)
            {
                throw new Exception("LireUneValeur(" + sql + "): pas de valeur");
            }
            string val = dt.Rows[0][0].ToString();
            if (val.StartsWith("\""))
            {
                val = val.Substring(1);
            }
            if (val.EndsWith("\""))
            {
                val = val.Substring(0, val.Length - 1);
            }
            */
            string val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.lire_une_valeur, sql, "");
            return val;
        }

        public async Task<bool> Attendre()
        {
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.attendre, "", "");
            return json_val == "OK";
        }

        public async Task<bool> DefinirServeurOuBd(string cmd)
        {
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.definir_serveur_ou_bd, cmd, "");
            return json_val == "OK";
        }

        public async Task<bool> DefinirServeur(string serveur)
        {
            string cmd = "Serveur=" + serveur;
            bool ret = await DefinirServeurOuBd(cmd);
            return ret;
        }

        public async Task<bool> DefinirBd(string bd)
        {
            string cmd = "Bd=" + bd;
            bool ret = await DefinirServeurOuBd(cmd);
            return ret;
        }

        public async Task<bool> ValiderUtilisateur(string trig_usr, string passwd_usr)
        {
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.definir_serveur_ou_bd, trig_usr + "/" + passwd_usr, "");
            return json_val == "OK";
        }

        public async Task<DataSet> LireEnsembleDeTables(string sql)
        {
            DataSet mes_tables = null;
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.lire_ensemble_de_tables, sql, "");
            // 20190414            AZEnsembleDeTables et = JsonConvert.DeserializeObject<AZEnsembleDeTables>(json_val);
            TraducJson tj = new TraducJson();
            AZEnsembleDeTables et = tj.DeserialiserObjet(json_val);
            if (et.tables != null)
            {
                mes_tables = et.CreerDataSet();
            }
            return mes_tables;
        }

        public async Task<DataTable> LireTable(string sql)
        {
            DataTable ma_table = null;
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.lire_ensemble_de_tables, sql, "");
            // 20190414 AZEnsembleDeTables et = JsonConvert.DeserializeObject<AZEnsembleDeTables>(json_val);
            TraducJson tj = new TraducJson();
            AZEnsembleDeTables et = tj.DeserialiserObjet(json_val);
            if (et.tables != null)
            {
                DataSet ds = et.CreerDataSet();
                ma_table = ds.Tables[0];
            }
            return ma_table;
        }

        public async Task<string> LireChaine(string sql)
        {
            string json_val = await LireUneValeur(sql);
            return json_val;
        }
        public async Task<string> LireChaineOuVide(string sql)
        {
            string json_val = await LireUneValeurOuVide(sql);
            return json_val;
        }

        public async Task<int?> LireEntier(string sql)
        {
            int? ret = null;
            string json_val = await LireUneValeur(sql);
            if (json_val.Length > 0)
            {
                ret = Convert.ToInt32(json_val);
            }
            return ret;
        }

        public async Task<DateTime?> LireDate(string sql)
        {
            DateTime? ret = null;
            string json_val = await LireUneValeur(sql);
            if (json_val.Length > 0)
            {
                ret = Convert.ToDateTime(json_val);
            }
            return ret;
        }

        public async Task<byte[]> LireBlob(string sql)
        {
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.lire_blob, sql, "");
            byte[] ret = Convert.FromBase64String(json_val);
            return ret;
        }

        public async Task<string> EcrireTable(string sql, DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt.Copy());
            AZEnsembleDeTables t = new AZEnsembleDeTables(ds);
            // 20190414           string str_table = JsonConvert.SerializeObject(t);
            TraducJson tj = new TraducJson();
            string str_table = tj.SerialiserObjet(t);
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.ecrire_table, sql, str_table);
            return json_val;
        }
        public async Task<string> EcrireEnsembleDeTables(string sql, DataSet ds)
        {
            AZEnsembleDeTables t = new AZEnsembleDeTables(ds);
            // 20190414 string str_tables = JsonConvert.SerializeObject(t);
            TraducJson tj = new TraducJson();
            string str_tables = tj.SerialiserObjet(t);
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.ecrire_table, sql, str_tables);
            return json_val;
        }

        public async Task<bool> ExecSql(string sql)
        {
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.exec_sql, sql, "");
            return true;
        }
        public async Task<bool> EcrireBlob(string sql, byte[] blob)
        {
            string str_blob = Convert.ToBase64String(blob);
            string json_val = await AccesDeBase(TYPE_ACCES_SERVICE_WEB.ecrire_blob, sql, str_blob);
            return true;
        }
    }
}
