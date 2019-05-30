using System;
using System.Collections.Generic;
using System.Data;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace AZ
{
    /*
    public class AZChamp
    {
        public string n { get; set; }
    }
    */
    public class AZChampHttp
    {
        public int num_champ { get; set; }
        public string val_champ { get; set; }

        public AZChampHttp(int num, string val)
        {
            num_champ = num;
            val_champ = val;
        }
    }
    /*
    public class B : AZChamp
    {
        public bool? b { get; set; }

        public B(string nom, bool? val)
        {
            n = nom;
            b = val;
        }
    }

    public class D : AZChamp
    {
        public DateTime t { get; set; }

        public D(string nom, DateTime val)
        {
            n = nom;
            t = val;
        }
    }

    public class F : AZChamp
    {
        public double? d { get; set; }

        public F(string nom, double? val)
        {
            n = nom;
            d = val;
        }
    }

    public class I : AZChamp
    {
        public int? i { get; set; }

        public I(string nom, int? val)
        {
            n = nom;
            i = val;
        }
    }

    public class S : AZChamp
    {
        public string s { get; set; }

        public S(string nom, string val)
        {
            n = nom;
            s = val;
        }
    }
    */
    public enum StatutLigne { inconnu = 0, pas_modifie = 1, modifie = 2, ajoute = 3, supprime = 4 }
    public class AZLigne
    {
        public StatutLigne statut_lig { get; set; }
        public List<AZChampHttp> champs { get; set; }
    }

    public enum TypeCol { inconnu = 0, ChampInt = 1, ChampString = 2, ChampDate = 3, ChampBool = 4, ChampDouble = 5, ChampGuid = 6, ChampChar = 7, ChampBlob = 8 }
    public class AZColonne
    {
        public TypeCol type_col { get; set; }
        public string nom_col { get; set; }
    }

    public class AZTable
    {
        public string nom { get; set; }
        public List<AZColonne> cols { get; set; }
        public List<AZLigne> ligs { get; set; }
    }

//    [JsonConverter(typeof(AZEnsembleDeTablesConverter))]
    public class AZEnsembleDeTables
    {
        public List<AZTable> tables { get; set; }

        public AZEnsembleDeTables()
        {

        }
        public AZEnsembleDeTables(DataSet ds)
        {
            this.tables = new List<AZTable>();
            foreach (DataTable dt in ds.Tables)
            {
                AZTable t = new AZTable();
                t.cols = new List<AZColonne>();
                foreach (DataColumn c in dt.Columns)
                {
                    AZColonne ac = new AZColonne();
                    ac.nom_col = c.ColumnName;
                    string type_col = c.DataType.ToString();
                    switch (type_col)
                    {
                        case "System.Boolean":
                            ac.type_col = TypeCol.ChampBool;
                            break;
                        case "System.Byte[]":
                            ac.type_col = TypeCol.ChampBlob;
                            break;
                        case "System.Char":
                            ac.type_col = TypeCol.ChampChar;
                            break;
                        case "System.DateTime":
                            ac.type_col = TypeCol.ChampDate;
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            ac.type_col = TypeCol.ChampDouble;
                            break;
                        case "System.Guid":
                            ac.type_col = TypeCol.ChampGuid;
                            break;
                        case "System.Int32":
                            ac.type_col = TypeCol.ChampInt;
                            break;
                        case "System.String":
                            ac.type_col = TypeCol.ChampString;
                            break;
                        default:
                            ac.type_col = TypeCol.inconnu;
                            throw new Exception("Erreur: type de donnée inconnu: " + type_col);
//                            break;
                    }
                    t.cols.Add(ac);
                }
                t.ligs = new List<AZLigne>();
                foreach (DataRow dr in dt.Rows)
                {
                    AZLigne ligne = new AZLigne();
                    ligne.statut_lig = StatutLigne.pas_modifie;
                    ligne.champs = new List<AZChampHttp>();
                    string etat = dt.Columns.Contains("etat") ? dr["etat"].ToString() : "?";
                    switch (dr.RowState)
                    {
                        case DataRowState.Added:
                            etat = "I";
                            break;
                        case DataRowState.Deleted:
                            etat = "D";
                            dr.RejectChanges();
                            break;
                        case DataRowState.Modified:
                            if(etat=="?")
                                etat = "U";
                            break;
                        case DataRowState.Unchanged:
                            etat = "?";
                            break;
                    }
                    dr["etat"] = etat;
                    for (int i = 0; i < t.cols.Count; i++)
                    {
                        string str_val = dr[i].ToString();
                        int lg = str_val.Trim().Length;
                        if (lg > 0)
                        {
                            string nom = t.cols[i].nom_col;
                            switch (t.cols[i].type_col)
                            {
                                case TypeCol.ChampBool:
                                    bool val = Convert.ToBoolean(str_val);
                                    str_val = val ? "1" : "0";
                                    break;
                                case TypeCol.ChampChar:
                                    break;
                                case TypeCol.ChampDate:
                                    DateTime d = Convert.ToDateTime(str_val);
                                    string annee = d.Year.ToString();
                                    string mois = d.Month < 10 ? "0" + d.Month.ToString() : d.Month.ToString();
                                    string jour = d.Day < 10 ? "0" + d.Day.ToString() : d.Day.ToString();
                                    str_val = annee + mois + jour;
                                    if (d.Hour > 0 || d.Minute > 0 || d.Second > 0 || d.Millisecond > 0)
                                    {
                                        string heure = d.Hour < 10 ? "0" + d.Hour.ToString() : d.Hour.ToString();
                                        string minute = d.Minute < 10 ? "0" + d.Minute.ToString() : d.Minute.ToString();
                                        string seconde = d.Second < 10 ? "0" + d.Second.ToString() : d.Second.ToString();
                                        string milliseconde = d.Millisecond < 10 ? "00" + d.Millisecond.ToString() : d.Millisecond < 100 ? "0" + d.Millisecond.ToString() : d.Millisecond.ToString();
                                        str_val += heure + minute + seconde + milliseconde;
                                    }
                                    break;
                                case TypeCol.ChampDouble:
                                    double f = Convert.ToDouble(str_val);
                                    str_val = f.ToString();
                                    break;
                                case TypeCol.ChampGuid:
                                    break;
                                case TypeCol.ChampInt:
                                    int j = Convert.ToInt32(str_val);
                                    str_val = j.ToString();
                                    break;
                                case TypeCol.ChampString:
                                    break;
                                default:
                                    break;
                            }
                            AZChampHttp c = new AZChampHttp(i, str_val);
                            ligne.champs.Add(c);
                        }
                    }
                    t.ligs.Add(ligne);
                }
                this.tables.Add(t);
            }
        }

        public DataSet CreerDataSet()
        {
            DataSet ds = new DataSet();
            foreach (AZTable t in tables)
            {
                DataTable dt = new DataTable(t.nom);
                foreach (AZColonne c in t.cols)
                {
                    DataColumn dc = new DataColumn(c.nom_col);
                    bool faire = true;
                    switch (c.type_col)
                    {
                        case TypeCol.ChampBool:
                            dc.DataType = typeof(bool);
                            break;
                        case TypeCol.ChampChar:
                            dc.DataType = typeof(char);
                            break;
                        case TypeCol.ChampDate:
                            dc.DataType = typeof(DateTime);
                            break;
                        case TypeCol.ChampDouble:
                            dc.DataType = typeof(double);
                            break;
                        case TypeCol.ChampGuid:
                            dc.DataType = typeof(Guid);
                            break;
                        case TypeCol.ChampInt:
                            dc.DataType = typeof(int);
                            break;
                        case TypeCol.ChampString:
                            dc.DataType = typeof(string);
                            break;
                        default:
                            faire = false;
                            break;
                    }
                    if (faire)
                    {
                        dt.Columns.Add(dc);
                    }
                }
                foreach (AZLigne l in t.ligs)
                {
                    DataRow dr = dt.NewRow();
                    dr.BeginEdit();
                    for (int i = 0; i < t.cols.Count; i++)
                    {
                        foreach (AZChampHttp c in l.champs)
                        {
                            if (c.num_champ == i)
                            {
                                switch (t.cols[i].type_col)
                                {
                                    case TypeCol.ChampBool:
                                        bool val = c.val_champ != "0";
                                        dr[i] = val;
                                        break;
                                    case TypeCol.ChampChar:
                                        dr[i] = c.val_champ;
                                        break;
                                    case TypeCol.ChampDate:
                                        DateTime d = DateTime.MinValue;
                                        int annee = Convert.ToInt32(c.val_champ.Substring(0, 4));
                                        int mois = Convert.ToInt32(c.val_champ.Substring(4, 2));
                                        int jour = Convert.ToInt32(c.val_champ.Substring(6, 2));
                                        if (c.val_champ.Length == 8)
                                        {
                                            // format date simple
                                            d = new DateTime(annee, mois, jour);
                                        }
                                        else
                                        {
                                            // format date heure
                                            int heure = Convert.ToInt32(c.val_champ.Substring(8, 2));
                                            int minute = Convert.ToInt32(c.val_champ.Substring(10, 2));
                                            int seconde = Convert.ToInt32(c.val_champ.Substring(12, 2));
                                            int milliseconde = Convert.ToInt32(c.val_champ.Substring(14));
                                            d = new DateTime(annee, mois, jour, heure, minute, seconde, milliseconde);
                                        }
                                        dr[i] = d;
                                        break;
                                    case TypeCol.ChampDouble:
                                        dr[i] = Convert.ToDouble(c.val_champ);
                                        break;
                                    case TypeCol.ChampGuid:
                                        dr[i] = new Guid(c.val_champ);
                                        break;
                                    case TypeCol.ChampInt:
                                        dr[i] = Convert.ToInt32(c.val_champ);
                                        break;
                                    case TypeCol.ChampString:
                                        dr[i] = c.val_champ;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    dr.EndEdit();
                    dt.Rows.Add(dr);
                    dr.AcceptChanges();
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }
    }
    /*
    public class AZEnsembleDeTablesConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            AZEnsembleDeTables et = new AZEnsembleDeTables();
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonReaderException("pas de début d'ensemble de tables");
            }
            reader.Read();
            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonReaderException("pas de propriété 't'");
            }
            bool lect_tabs = true;
            reader.Read();
            if (reader.TokenType == JsonToken.Null)
                lect_tabs = true;
            else if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonReaderException("pas de début de tableau de tables");
            }
            et.tables = new List<AZTable>();
            while (lect_tabs)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.EndArray)
                {
                    lect_tabs = false;
                    reader.Read();
                }
                else if (reader.TokenType != JsonToken.StartObject)
                {
                    throw new JsonReaderException("pas de début de table");
                }
                else
                {
                    AZTable t = new AZTable();
                    t.cols = new List<AZColonne>();
                    t.ligs = new List<AZLigne>();
                    reader.Read();
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        throw new JsonReaderException("pas de propriété c");
                    }
                    else
                    {
                        reader.Read();
                        if (reader.TokenType != JsonToken.StartArray)
                        {
                            throw new JsonReaderException("pas de début de tableau de colonnes");
                        }
                        else
                        {
                            bool lect_col = true;
                            while (lect_col)
                            {
                                reader.Read();
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    lect_col = false;
                                }
                                else if (reader.TokenType != JsonToken.StartObject)
                                {
                                    throw new JsonReaderException("pas de début de colonne");
                                }
                                else
                                {
                                    reader.Read();
                                    if (reader.TokenType != JsonToken.PropertyName)
                                    {
                                        throw new JsonReaderException("pas de nom de colonne");
                                    }
                                    else
                                    {
                                        string nom_col = (string)reader.Value;
                                        reader.Read();
                                        if (reader.TokenType != JsonToken.Integer)
                                        {
                                            throw new JsonReaderException("pas de type de colonne");
                                        }
                                        else
                                        {
                                            int type_col = Convert.ToInt32(reader.Value);
                                            if (type_col < 1 || type_col > 8)
                                            {
                                                throw new JsonReaderException("type de colonne invalide");
                                            }
                                            else if (type_col < 8)
                                            {
                                                AZColonne c = new AZColonne();
                                                c.nom_col = nom_col;
                                                c.type_col = (TypeCol)type_col;
                                                t.cols.Add(c);
                                            }
                                            reader.Read();
                                            if (reader.TokenType != JsonToken.EndObject)
                                            {
                                                throw new JsonReaderException("pas de fin de colonne");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    reader.Read();
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        throw new JsonReaderException("pas de propriété l");
                    }
                    else
                    {
                        reader.Read();
                        if (reader.TokenType != JsonToken.StartArray)
                        {
                            throw new JsonReaderException("pas de début de tableau de lignes");
                        }
                        else
                        {
                            bool lect_lig = true;
                            while (lect_lig)
                            {
                                reader.Read();
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    lect_lig = false;
                                }
                                else if (reader.TokenType != JsonToken.StartObject)
                                {
                                    throw new JsonReaderException("pas de début de ligne");
                                }
                                else
                                {
                                    reader.Read();
                                    if (reader.TokenType != JsonToken.PropertyName)
                                    {
                                        throw new JsonReaderException("pas de propriété de statut de ligne");
                                    }
                                    else
                                    {
                                        reader.Read();
                                        if (reader.TokenType != JsonToken.Integer)
                                        {
                                            throw new JsonReaderException("pas de statut de ligne");
                                        }
                                        else
                                        {
                                            int statut_lig = Convert.ToInt32(reader.Value);
                                            if (statut_lig < 1 || statut_lig > 3)
                                            {
                                                throw new JsonReaderException("statut de ligne invalide");
                                            }
                                            else
                                            {
                                                reader.Read();
                                                if (reader.TokenType != JsonToken.PropertyName)
                                                {
                                                    throw new JsonReaderException("pas de propriété de champs de ligne");
                                                }
                                                else
                                                {
                                                    reader.Read();
                                                    if (reader.TokenType != JsonToken.StartArray)
                                                    {
                                                        throw new JsonReaderException("pas de début de tableau de champs");
                                                    }
                                                    else
                                                    {
                                                        AZLigne l = new AZLigne();
                                                        l.statut_lig = (StatutLigne)statut_lig;
                                                        l.champs = new List<AZChampHttp>();
                                                        bool lect_champs = true;
                                                        while (lect_champs)
                                                        {
                                                            reader.Read();
                                                            if (reader.TokenType == JsonToken.EndArray)
                                                            {
                                                                lect_champs = false;
                                                            }
                                                            else if (reader.TokenType != JsonToken.StartObject)
                                                            {
                                                                throw new JsonReaderException("pas de début de champ");
                                                            }
                                                            else
                                                            {
                                                                reader.Read();
                                                                if (reader.TokenType != JsonToken.PropertyName)
                                                                {
                                                                    throw new JsonReaderException("pas de numéro de champ");
                                                                }
                                                                else
                                                                {
                                                                    int num_champ = Convert.ToInt32(reader.Value);
                                                                    reader.Read();
                                                                    string val_champ = "";
                                                                    switch (t.cols[num_champ].type_col)
                                                                    {
                                                                        case TypeCol.ChampBool:
                                                                            if (reader.TokenType != JsonToken.Integer)
                                                                            {
                                                                                throw new JsonReaderException("pas de valeur de champ booleen");
                                                                            }
                                                                            else
                                                                            {
                                                                                val_champ = reader.Value.ToString();
                                                                            }
                                                                            break;
                                                                        case TypeCol.ChampInt:
                                                                            if (reader.TokenType != JsonToken.Integer)
                                                                            {
                                                                                throw new JsonReaderException("pas de valeur de champ int");
                                                                            }
                                                                            else
                                                                            {
                                                                                val_champ = reader.Value.ToString();
                                                                            }
                                                                            break;
                                                                        case TypeCol.ChampDate:
                                                                            if (reader.TokenType == JsonToken.Integer)
                                                                            {
                                                                                // format date simple
                                                                                val_champ = reader.Value.ToString();
                                                                            }
                                                                            else if (reader.TokenType == JsonToken.String)
                                                                            {
                                                                                // format date heure
                                                                                val_champ = reader.Value.ToString();
                                                                            }
                                                                            else
                                                                            {
                                                                                throw new JsonReaderException("pas de valeur de champ date");
                                                                            }
                                                                            break;
                                                                        case TypeCol.ChampDouble:
                                                                            if (reader.TokenType != JsonToken.Float)
                                                                            {
                                                                                throw new JsonReaderException("pas de valeur de champ double");
                                                                            }
                                                                            else
                                                                            {
                                                                                val_champ = reader.Value.ToString();
                                                                            }
                                                                            break;
                                                                        case TypeCol.ChampChar:
                                                                        case TypeCol.ChampGuid:
                                                                        case TypeCol.ChampString:
                                                                            if (reader.TokenType != JsonToken.String)
                                                                            {
                                                                                throw new JsonReaderException("pas de valeur de champ guid ou string");
                                                                            }
                                                                            else
                                                                            {
                                                                                val_champ = reader.Value.ToString();
                                                                            }
                                                                            break;
                                                                    }
                                                                    if (t.cols[num_champ].type_col != TypeCol.ChampBlob)
                                                                    {
                                                                        if (val_champ.Length == 0)
                                                                        {
                                                                            throw new JsonReaderException("pas de valeur de champ");
                                                                        }
                                                                        else
                                                                        {
                                                                            AZChampHttp c = new AZChampHttp(num_champ, val_champ);
                                                                            l.champs.Add(c);
                                                                            reader.Read();
                                                                            if (reader.TokenType != JsonToken.EndObject)
                                                                            {
                                                                                throw new JsonReaderException("pas de fin de champ");
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        reader.Read();
                                                                        if (reader.TokenType != JsonToken.EndObject)
                                                                        {
                                                                            throw new JsonReaderException("pas de fin de champ");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        t.ligs.Add(l);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    reader.Read();
                                    if (reader.TokenType != JsonToken.EndObject)
                                    {
                                        throw new JsonReaderException("pas de fin de ligne");
                                    }
                                }
                            }
                        }
                    }
                    et.tables.Add(t);
                    reader.Read();
                    if (reader.TokenType != JsonToken.EndObject)
                    {
                        throw new JsonReaderException("pas de fin de table");
                    }
                }
            }
            if (reader.TokenType != JsonToken.EndObject)
            {
                throw new JsonReaderException("mauvaise fin d'ensemble de tables");
            }
            reader.Read();
            if (reader.TokenType != JsonToken.None)
            {
                throw new JsonReaderException("mauvaise fin de données");
            }
            return et;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var et = (AZEnsembleDeTables)value;
            writer.WriteStartObject();
            writer.WritePropertyName("t");
            writer.WriteStartArray();
            foreach (AZTable t in et.tables)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("c");
                writer.WriteStartArray();
                foreach (AZColonne c in t.cols)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(c.nom_col);
                    writer.WriteValue(c.type_col);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WritePropertyName("l");
                writer.WriteStartArray();
                foreach (AZLigne l in t.ligs)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("s");
                    writer.WriteValue(l.statut_lig);
                    writer.WritePropertyName("v");
                    writer.WriteStartArray();
                    foreach (AZChampHttp c in l.champs)
                    {
                        if (t.cols[c.num_champ].type_col != TypeCol.ChampBlob)
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName(c.num_champ.ToString());
                            switch (t.cols[c.num_champ].type_col)
                            {
                                case TypeCol.ChampBool:
                                    int val_bool = c.val_champ == "0" ? 0 : 1;
                                    writer.WriteValue(val_bool);
                                    break;
                                case TypeCol.ChampChar:
                                    writer.WriteValue(c.val_champ);
                                    break;
                                case TypeCol.ChampDate:
                                    if (c.val_champ.Length == 8)
                                    {
                                        // format date simple
                                        int annee = Convert.ToInt32(c.val_champ.Substring(0, 4));
                                        int mois = Convert.ToInt32(c.val_champ.Substring(4, 2));
                                        int jour = Convert.ToInt32(c.val_champ.Substring(6, 2));
                                        int date_complete = annee * 10000 + mois * 100 + jour;
                                        writer.WriteValue(date_complete);
                                    }
                                    else
                                    {
                                        // format date + heure
                                        writer.WriteValue(c.val_champ);
                                    }
                                    break;
                                case TypeCol.ChampDouble:
                                    double val_double = Convert.ToDouble(c.val_champ);
                                    writer.WriteValue(val_double);
                                    break;
                                case TypeCol.ChampGuid:
                                    writer.WriteValue(c.val_champ);
                                    break;
                                case TypeCol.ChampInt:
                                    int val_int = Convert.ToInt32(c.val_champ);
                                    writer.WriteValue(val_int);
                                    break;
                                case TypeCol.ChampString:
                                    writer.WriteValue(c.val_champ);
                                    break;
                                default:
                                    writer.WriteValue(c.val_champ);
                                    break;
                            }
                            writer.WriteEndObject();
                        }
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
    */
}