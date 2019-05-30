/*
using System;
using System.Collections.Generic;
using System.Text;

namespace AZ
{
    public class TraducJson
    {
        public TraducJson()
        {

        }
        private string WriteStartObject()
        {
            return "{";
        }
        private string WriteEndObject()
        {
            return "}";
        }
        private string WritePropertyName(string nom_prop)
        {
            return "\"" + nom_prop + "\":";
        }
        private string WritePropertyName(int num_prop)
        {
            return num_prop.ToString() + ":";
        }
        private string WriteValue(string val)
        {
            return val;
        }
        private string WriteStartArray()
        {
            return "[";
        }
        private string WriteEndArray()
        {
            return "]";
        }
        public string TranscrireEnJsonUneTable(AZ.AZEnsembleDeTables t)
        {
            string ret = "";
            ret += WriteStartObject();
            ret += WritePropertyName("t");
            ret += WriteStartArray();
            //		foreach (AZTable t in et.tables)
            //            {
            for (int num_table = 0;num_table < t.tables.Count;num_table++)
		    {
                ret += WriteStartObject();
                ret += WritePropertyName("c");
                ret += WriteStartArray();
			    int nb_col = t.tables[num_table].cols.Count;
                for (int i = 0;i < nb_col;i++)
                {
				    string nom_col = t.tables[num_table].cols[i].nom_col;
				    AZ.TypeCol type_col = t.tables[num_table].cols[i].type_col;
                    if (i > 0)
				    {
                        ret += ",";
                    }
                    ret += WriteStartObject();
                    ret += WritePropertyName(nom_col);
				    int type_col_int = 0;
                    switch (type_col)
				    {
					    case TypeCol.ChampBool:
						    type_col_int = 4;
                            break;
					    case TypeCol.ChampDate:
						    type_col_int = 3;
                            break;
					    case TypeCol.ChampInt:
						    type_col_int = 1;
                            break;
					    case TypeCol.ChampString:
						    type_col_int = 2;
                            break;
                        default:
						    throw new Exception("type_col_inconnu:" + type_col);
                            break;
                    }
                    ret += WriteValue(((int)type_col_int).ToString());
                    ret += WriteEndObject();
                }
                ret += WriteEndArray();
                ret += ",";
                ret += WritePropertyName("l");
                ret += WriteStartArray();
			    int nb_lig = t.tables[num_table].ligs.Count;
                for (int i = 0;i < nb_lig;i++)
                {
                    if (i > 0)
				    {
                        ret += ",";
                    }
                    ret += WriteStartObject();
                    ret += WritePropertyName("s");
				    string etat =t.tables[num_table].ligs[i].champs[0].val_champ;
				    int etat_int = 1;  // pas modifi�
                    ret += WriteValue(etat_int.ToString());
                    ret += ",";
                    ret += WritePropertyName("v");
                    ret += WriteStartArray();
				    bool debut = true;
                    int nb_champs = t.tables[num_table].ligs[i].champs.Count;
                    for (int j = 0;j < nb_champs;j++)
				    {
                        int num_champ = t.tables[num_table].ligs[i].champs[j].num_champ;
                        string val_col = t.tables[num_table].ligs[i].champs[j].val_champ;
                        //					print "val_col=".$val_col;
                        if (val_col.Length > 0)
                        {
                            //						print "passe";
                            if (debut)
						    {
							    debut = false;
                            }
                            else
                            {
                                ret += ",";
                            }
                            ret += WriteStartObject();
                            ret += WritePropertyName(num_champ);
						    TypeCol type_col = t.tables[num_table].cols[num_champ].type_col;
                            //						print "(j=$j,type_col=".$type_col.")";
                            switch (type_col)
						    {
							    case TypeCol.ChampBool:
								    ret += WriteValue(val_col);
                                    break;
							    case TypeCol.ChampInt:
								    ret += WriteValue(val_col);
                                    break;
							    case TypeCol.ChampDate:
								    string suffixe = " 00:00:00.000";
                                    if (val_col.EndsWith(suffixe))
                                    {
//									$val_col=substr($val_col,0,strlen($val_col)-strlen($suffixe));
									    val_col = val_col.Substring(0, 4) + val_col.Substring(5, 2) + val_col.Substring(8, 2);
                                        ret += WriteValue(val_col);
                                    }
                                    else
                                    {
                                        ret += "\"" + WriteValue(val_col) + "\"";
                                    }
                                    break;
		    					case TypeCol.ChampString:
								    string val_format = val_col.Replace("\\", "\\\\");
                                    ret += "\"" + WriteValue(val_format) + "\"";
                                    break;
                                default:
	    							ret += "type inconnu:" + type_col.ToString();
                                    break;
                            }
                            ret += WriteEndObject();
                        }
                    }
                    ret += WriteEndArray();
                    ret += WriteEndObject();
                }
                ret += WriteEndArray();
                ret += WriteEndObject();
            }
            ret += WriteEndArray();
            ret += WriteEndObject();
            return ret;
        }
    }
}
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace AZ
{
    public class TraducJson
    {
        public TraducJson()
        {

        }
        private string WriteStartObject()
        {
            return "{";
        }
        private string WriteEndObject()
        {
            return "}";
        }
        private string WritePropertyName(string nom_prop)
        {
            return "\"" + nom_prop + "\":";
        }
        private string WritePropertyName(int num_prop)
        {
            return num_prop.ToString() + ":";
        }
        private string WriteValue(string val)
        {
            return val.Replace("\\", "\\\\").Replace("\"","\\\"");
        }
        private string WriteStartArray()
        {
            return "[";
        }
        private string WriteEndArray()
        {
            return "]";
        }
        public string SerialiserObjet(string val)
        {
            return "\"" + val.Replace("\\","\\\\").Replace("\"","\\" + "\"") + "\"";
        }
        public string SerialiserObjet(int val)
        {
            return val.ToString();
        }
        public string SerialiserObjet(DateTime val)
        {
            return val.ToString();
        }
        public string SerialiserObjet(AZ.AZEnsembleDeTables t)
        {
            string ret = "";
            ret += WriteStartObject();
            ret += WritePropertyName("t");
            ret += WriteStartArray();
            //		foreach (AZTable t in et.tables)
            //            {
            for (int num_table = 0; num_table < t.tables.Count; num_table++)
            {
                ret += WriteStartObject();
                ret += WritePropertyName("c");
                ret += WriteStartArray();
                int nb_col = t.tables[num_table].cols.Count;
                for (int i = 0; i < nb_col; i++)
                {
                    string nom_col = t.tables[num_table].cols[i].nom_col;
                    AZ.TypeCol type_col = t.tables[num_table].cols[i].type_col;
                    if (type_col != TypeCol.ChampBlob)
                    {
                        if (i > 0)
                        {
                            ret += ",";
                        }
                        ret += WriteStartObject();
                        ret += WritePropertyName(nom_col);
                        int type_col_int = 0;
                        switch (type_col)
                        {
                            case TypeCol.ChampBool:
                            case TypeCol.ChampDate:
                            case TypeCol.ChampInt:
                            case TypeCol.ChampString:
                                type_col_int = (int)type_col;
                                ret += WriteValue(((int)type_col_int).ToString());
                                ret += WriteEndObject();
                                break;
                            default:
                                throw new Exception("type_col_inconnu:" + type_col);
//                                break;
                        }
                    }
                }
                ret += WriteEndArray();
                ret += ",";
                ret += WritePropertyName("l");
                ret += WriteStartArray();
                int nb_lig = t.tables[num_table].ligs.Count;
                for (int i = 0; i < nb_lig; i++)
                {
                    if (i > 0)
                    {
                        ret += ",";
                    }
                    ret += WriteStartObject();
                    ret += WritePropertyName("s");
                    string etat = t.tables[num_table].ligs[i].champs[0].val_champ;
                    int etat_int = 1;  // pas modifi�
                    ret += WriteValue(etat_int.ToString());
                    ret += ",";
                    ret += WritePropertyName("v");
                    ret += WriteStartArray();
                    bool debut = true;
                    int nb_champs = t.tables[num_table].ligs[i].champs.Count;
                    for (int j = 0; j < nb_champs; j++)
                    {
                        int num_champ = t.tables[num_table].ligs[i].champs[j].num_champ;
                        string val_col = t.tables[num_table].ligs[i].champs[j].val_champ;
                        TypeCol type_col = t.tables[num_table].cols[num_champ].type_col;
                        //					print "val_col=".$val_col;
                        if (val_col.Length > 0 && type_col != TypeCol.ChampBlob)
                        {
                            //						print "passe";
                            if (debut)
                            {
                                debut = false;
                            }
                            else
                            {
                                ret += ",";
                            }
                            ret += WriteStartObject();
                            //                            ret += WritePropertyName(num_champ.ToString());
                            ret += WritePropertyName(num_champ);
                            //						print "(j=$j,type_col=".$type_col.")";
                            switch (type_col)
                            {
                                case TypeCol.ChampBool:
                                    ret += WriteValue(val_col);
                                    break;
                                case TypeCol.ChampInt:
                                    ret += WriteValue(val_col);
                                    break;
                                case TypeCol.ChampDate:
                                    string suffixe = " 00:00:00.000";
                                    if(val_col.Length == 8)
                                    {
                                        ret += WriteValue(val_col);
                                    }
                                    else if (val_col.EndsWith(suffixe))
                                    {
                                        //									$val_col=substr($val_col,0,strlen($val_col)-strlen($suffixe));
                                        val_col = val_col.Substring(0, 4) + val_col.Substring(5, 2) + val_col.Substring(8, 2);
                                        ret += WriteValue(val_col);
                                    }
                                    else
                                    {
                                        ret += "\"" + WriteValue(val_col) + "\"";
                                    }
                                    break;
                                case TypeCol.ChampString:
//                                    string val_format = val_col.Replace("\\", "\\\\");
                                    ret += "\"" + WriteValue(val_col) + "\"";
                                    break;
                                default:
                                    ret += "type inconnu:" + type_col.ToString();
                                    break;
                            }
                            ret += WriteEndObject();
                        }
                    }
                    ret += WriteEndArray();
                    ret += WriteEndObject();
                }
                ret += WriteEndArray();
                ret += WriteEndObject();
            }
            ret += WriteEndArray();
            ret += WriteEndObject();
            return ret;
        }
        private const char StartObject = '{';
        private const char EndObject = '}';
        private const char StartArray = '[';
        private const char EndArray = ']';

        private bool LireNomPropriete(char[] tab_val, ref int num_char, out string nom_propriete)
        {
            bool ret = false;
            nom_propriete = "";
            if (tab_val[num_char] == '\"')
            {
                num_char++;
                bool fini = false;
                while (fini == false)
                {
                    if (num_char >= tab_val.Length)
                        fini = true;
                    else if (tab_val[num_char] == '\"')
                    {
                        num_char++;
                        fini = true;
                        ret = tab_val[num_char] == ':';
                    }
                    else
                    {
                        nom_propriete += tab_val[num_char++].ToString();
                    }
                }
            }
            return ret;
        }
        private bool LireEntier(char[] tab_val, ref int num_char, out int val)
        {
            bool ret = false;
            string val_champ = "";
            bool fini = false;
            bool chiffre_trouve = false;
            val = 0;
            while (fini == false)
            {
                if (num_char >= tab_val.Length)
                    fini = true;
                else if ((tab_val[num_char]<'0' || tab_val[num_char]>'9') && tab_val[num_char] != '-')
                {
                    fini = true;
                    if(chiffre_trouve)
                        num_char--;
                }
                else
                {
                    chiffre_trouve = true;
                    val_champ += tab_val[num_char].ToString();
                    num_char++;
                }
            }
            ret = chiffre_trouve;
            if (ret)
            {
                ret = int.TryParse(val_champ, out val);
            }
            return ret;
        }
        private bool LireDouble(char[] tab_val, ref int num_char, out double val)
        {
            bool ret = false;
            string val_champ = "";
            bool fini = false;
            bool chiffre_trouve = false;
            val = 0.0;
            while (fini == false)
            {
                if (num_char >= tab_val.Length)
                    fini = true;
                else if (tab_val[num_char] == '}')
                {
                    fini = true;
                    num_char--;
                }
                else
                {
                    chiffre_trouve = true;
                    val_champ += tab_val[num_char].ToString();
                    num_char++;
                }
            }
            ret = chiffre_trouve;
            if (ret)
            {
                ret = Double.TryParse(val_champ, out val);
            }
            return ret;
        }
        private bool LireChaine(char[] tab_val, ref int num_char, out string val)
        {
            bool ret = false;
            val = "";
            if (tab_val[num_char] == '\"')
            {
                num_char++;
                bool fini = false;
                bool echappement = false;
                while (fini == false)
                {
                    if (num_char >= tab_val.Length)
                        fini = true;
                    else if (tab_val[num_char] == '\\')
                    {
                        echappement = true;
                        num_char++;
                    }
                    else if (tab_val[num_char] == '\"' && echappement == false)
                    {
                        fini = true;
                        ret = true;
                    }
                    else
                    {
                        echappement = false;
                        val += tab_val[num_char++].ToString();
                    }
                }
            }
            return ret;
        }
        public AZEnsembleDeTables DeserialiserObjet(string value)
        {
            int num_char = 0;
            AZEnsembleDeTables et = new AZEnsembleDeTables();
            try
            {
                char[] tab_val = value.ToCharArray();
                if (tab_val[num_char] != StartObject)
                {
                    throw new Exception("Json.DeserialiserObjet: pas de début d'ensemble de tables");
                }
                num_char++;
                //            if (tab_val[num_char] != '\"' || tab_val[num_char + 1] != 't' || tab_val[num_char+2]!='\"')
                string nom_propriete = "";
                if (!LireNomPropriete(tab_val, ref num_char, out nom_propriete))
                {
                    throw new Exception("Json.DeserialiserObjet: pas de propriété 't'");
                }
                else if (nom_propriete != "t")
                {
                    throw new Exception("Json.DeserialiserObjet: pas de propriété 't' (2)");
                }
                bool lect_tabs = true;
                num_char++;
                if (num_char >= tab_val.Length)
                    lect_tabs = false;
                else if (tab_val[num_char] != StartArray)
                {
                    throw new Exception("Json.DeserialiserObjet: pas de début de tableau de tables");
                }
                et.tables = new List<AZTable>();
                while (lect_tabs)
                {
                    num_char++;
                    if (tab_val[num_char] == EndArray)
                    {
                        // c'est sans doute la fin des tables
                        num_char--;
                        lect_tabs = false;
                    }
                    else if (et.tables.Count > 0)
                    {
                        if (tab_val[num_char] != ',')
                        {
                            throw new Exception("Json.DeserialiserObjet: pas de séparateur de tables");
                        }
                        else
                        {
                            num_char++;
                        }
                    }
                    if (lect_tabs == true)
                    {
                        if (tab_val[num_char] != StartObject)
                        {
                            throw new Exception("Json.DeserialiserObjet: pas de début de table");
                        }
                        else
                        {
                            AZTable t = new AZTable();
                            t.cols = new List<AZColonne>();
                            t.ligs = new List<AZLigne>();
                            num_char++;
                            //                    if (tab_val[num_char] != '\"' || tab_val[num_char + 1] != 'c' || tab_val[num_char + 2] != '\"')
                            if (!LireNomPropriete(tab_val, ref num_char, out nom_propriete))
                            {
                                throw new Exception("Json.DeserialiserObjet: pas de propriété 'c'");
                            }
                            else if (nom_propriete != "c")
                            {
                                throw new Exception("Json.DeserialiserObjet: pas de propriété 'c' (2)");
                            }
                            else
                            {
                                num_char++;
                                if (tab_val[num_char] != StartArray)
                                {
                                    throw new Exception("Json.DeserialiserObjet: pas de début de tableau de colonnes");
                                }
                                else
                                {
                                    bool lect_col = true;
                                    while (lect_col)
                                    {
                                        num_char++;
                                        if (tab_val[num_char] == EndArray)
                                        {
                                            lect_col = false;
                                            num_char--;
                                        }
                                        else if (t.cols.Count > 0)
                                        {
                                            if (tab_val[num_char] != ',')
                                            {
                                                throw new Exception("Json.DeserialiserObjet: pas de séparateur de colonnes");
                                            }
                                            else
                                            {
                                                num_char++;
                                            }
                                        }
                                        if (lect_col == true)
                                        {
                                            if (tab_val[num_char] != StartObject)
                                            {
                                                throw new Exception("Json.DeserialiserObjet: pas de début de colonne");
                                            }
                                            else
                                            {
                                                num_char++;
                                                string nom_col = "";
                                                if (!LireNomPropriete(tab_val, ref num_char, out nom_col))
                                                {
                                                    throw new Exception("Json.DeserialiserObjet: pas de nom de colonne");
                                                }
                                                else
                                                {
                                                    num_char++;
                                                    //                                        if (tab_val[num_char] != JsonToken.Integer)
                                                    int type_col = 0;
                                                    if (!LireEntier(tab_val, ref num_char, out type_col))
                                                    {
                                                        throw new Exception("Json.DeserialiserObjet: pas de type de colonne");
                                                    }
                                                    else
                                                    {
                                                        TypeCol tp = (TypeCol)type_col;
                                                        switch(tp)
                                                        {
                                                            case TypeCol.ChampBlob:
                                                            case TypeCol.ChampBool:
                                                            case TypeCol.ChampChar:
                                                            case TypeCol.ChampDate:
                                                            case TypeCol.ChampDouble:
                                                            case TypeCol.ChampGuid:
                                                            case TypeCol.ChampInt:
                                                            case TypeCol.ChampString:
                                                                AZColonne c = new AZColonne();
                                                                c.nom_col = nom_col;
                                                                c.type_col = (TypeCol)type_col;
                                                                t.cols.Add(c);
                                                                num_char++;
                                                                if (tab_val[num_char] != EndObject)
                                                                {
                                                                    throw new Exception("Json.DeserialiserObjet: pas de fin de colonne");
                                                                }
                                                                break;
                                                            default:
                                                                throw new Exception("Json.DeserialiserObjet: type de colonne invalide");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    num_char++;
                                    if (tab_val[num_char] != EndArray)
                                    {
                                        throw new Exception("Json.DeserialiserObjet: pas de fin de colonnes");
                                    }
                                }
                            }
                            num_char++; // pour la virgule qui sépare les colonnes et les lignes
                            num_char++;
                            //                    if (reader.TokenType != JsonToken.PropertyName)
                            if (!LireNomPropriete(tab_val, ref num_char, out nom_propriete))
                            {
                                throw new Exception("Json.DeserialiserObjet: pas de propriété l");
                            }
                            else if (nom_propriete != "l")
                            {
                                throw new Exception("Json.DeserialiserObjet: pas de propriété l (2)");
                            }
                            else
                            {
                                num_char++;
                                if (tab_val[num_char] != StartArray)
                                {
                                    throw new Exception("Json.DeserialiserObjet: pas de début de tableau de lignes");
                                }
                                else
                                {
                                    bool lect_lig = true;
                                    while (lect_lig)
                                    {
                                        num_char++;
                                        if (tab_val[num_char] == EndArray)
                                        {
                                            lect_lig = false;
                                            num_char--;
                                        }
                                        else if (t.ligs.Count > 0)
                                        {
                                            if (tab_val[num_char] != ',')
                                            {
                                                throw new Exception("Json.DeserialiserObjet: pas de séparateur de lignes");
                                            }
                                            else
                                            {
                                                num_char++;
                                            }
                                        }
                                        if (lect_lig == true)
                                        {
                                            if (tab_val[num_char] != StartObject)
                                            {
                                                throw new Exception("Json.DeserialiserObjet: pas de début de ligne");
                                            }
                                            else
                                            {
                                                num_char++;
                                                //                                    if (reader.TokenType != JsonToken.PropertyName)
                                                if (!LireNomPropriete(tab_val, ref num_char, out nom_propriete))
                                                {
                                                    throw new Exception("Json.DeserialiserObjet: pas de propriété de statut de ligne");
                                                }
                                                else
                                                {
                                                    num_char++;
                                                    //                                        if (reader.TokenType != JsonToken.Integer)
                                                    int statut_lig = 0;
                                                    if (!LireEntier(tab_val, ref num_char, out statut_lig))
                                                    {
                                                        throw new Exception("Json.DeserialiserObjet: pas de statut de ligne");
                                                    }
                                                    else
                                                    {
                                                        if (statut_lig < 1 || statut_lig > 3)
                                                        {
                                                            throw new Exception("Json.DeserialiserObjet: statut de ligne invalide");
                                                        }
                                                        else
                                                        {
                                                            num_char++; // pour la virgule
                                                            num_char++;
                                                            //                                                if (reader.TokenType != JsonToken.PropertyName)
                                                            if (!LireNomPropriete(tab_val, ref num_char, out nom_propriete))
                                                            {
                                                                throw new Exception("Json.DeserialiserObjet: pas de propriété de champs de ligne");
                                                            }
                                                            else
                                                            {
                                                                num_char++;
                                                                if (tab_val[num_char] != StartArray)
                                                                {
                                                                    throw new Exception("Json.DeserialiserObjet: pas de début de tableau de champs");
                                                                }
                                                                else
                                                                {
                                                                    AZLigne l = new AZLigne();
                                                                    l.statut_lig = (StatutLigne)statut_lig;
                                                                    l.champs = new List<AZChampHttp>();
                                                                    bool lect_champs = true;
                                                                    while (lect_champs)
                                                                    {
                                                                        num_char++;
                                                                        if (tab_val[num_char] == EndArray)
                                                                        {
                                                                            lect_champs = false;
                                                                            num_char--;
                                                                        }
                                                                        else if (l.champs.Count > 0)
                                                                        {
                                                                            if (tab_val[num_char] != ',')
                                                                            {
                                                                                throw new Exception("Json.DeserialiserObjet: pas de séparateur de champs de lignes");
                                                                            }
                                                                            else
                                                                            {
                                                                                num_char++;
                                                                            }
                                                                        }
                                                                        if (lect_champs == true)
                                                                        {
                                                                            if (tab_val[num_char] != StartObject)
                                                                            {
                                                                                throw new Exception("Json.DeserialiserObjet: pas de début de champ");
                                                                            }
                                                                            else
                                                                            {
                                                                                num_char++;
                                                                                //                                                                if (reader.TokenType != JsonToken.PropertyName)
                                                                                int num_champ = 0;
                                                                                if (!LireEntier(tab_val, ref num_char, out num_champ))
                                                                                {
                                                                                    throw new Exception("Json.DeserialiserObjet: pas de numéro de champ");
                                                                                }
                                                                                else
                                                                                {
                                                                                    num_char++; // pour les :
                                                                                    num_char++;
                                                                                    string val_champ = "";
                                                                                    switch (t.cols[num_champ].type_col)
                                                                                    {
                                                                                        case TypeCol.ChampBool:
                                                                                            //                                                                            if (reader.TokenType != JsonToken.Integer)
                                                                                            int val_champ_bool = 0;
                                                                                            if (!LireEntier(tab_val, ref num_char, out val_champ_bool))
                                                                                            {
                                                                                                throw new Exception("Json.DeserialiserObjet: pas de valeur de champ booleen");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                val_champ = val_champ_bool.ToString();
                                                                                            }
                                                                                            break;
                                                                                        case TypeCol.ChampInt:
                                                                                            int val_champ_int = 0;
                                                                                            // if (reader.TokenType != JsonToken.Integer)
                                                                                            if (!LireEntier(tab_val, ref num_char, out val_champ_int))
                                                                                            {
                                                                                                throw new Exception("Json.DeserialiserObjet: pas de valeur de champ int");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                val_champ = val_champ_int.ToString();
                                                                                            }
                                                                                            break;
                                                                                        case TypeCol.ChampDate:
                                                                                            int val_champ_date = 0;
                                                                                            string val_date = "";
                                                                                            //                                                                            if (reader.TokenType == JsonToken.Integer)
                                                                                            if (LireEntier(tab_val, ref num_char, out val_champ_date))
                                                                                            {
                                                                                                // format date simple
                                                                                                val_champ = val_champ_date.ToString();
                                                                                            }
                                                                                            //                                                                            else if (reader.TokenType == JsonToken.String)
                                                                                            else if (LireChaine(tab_val, ref num_char, out val_date))
                                                                                            {
                                                                                                // format date heure
                                                                                                val_champ = val_date;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                throw new Exception("Json.DeserialiserObjet: pas de valeur de champ date");
                                                                                            }
                                                                                            break;
                                                                                        case TypeCol.ChampDouble:
                                                                                            double val_double = 0.0;
                                                                                            //                                                                            if (reader.TokenType != JsonToken.Float)
                                                                                            if (!LireDouble(tab_val, ref num_char, out val_double))
                                                                                            {
                                                                                                throw new Exception("Json.DeserialiserObjet: pas de valeur de champ double");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                val_champ = val_double.ToString();
                                                                                            }
                                                                                            break;
                                                                                        case TypeCol.ChampChar:
                                                                                        case TypeCol.ChampGuid:
                                                                                        case TypeCol.ChampString:
                                                                                            string val_string = "";
                                                                                            //                                                                            if (reader.TokenType != JsonToken.String)
                                                                                            if (!LireChaine(tab_val, ref num_char, out val_string))
                                                                                            {
                                                                                                throw new Exception("Json.DeserialiserObjet: pas de valeur de champ guid ou string");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                val_champ = val_string;
                                                                                            }
                                                                                            break;
                                                                                    }
                                                                                    if (val_champ.Length == 0)
                                                                                    {
                                                                                        throw new Exception("Json.DeserialiserObjet: pas de valeur de champ");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        AZChampHttp c = new AZChampHttp(num_champ, val_champ);
                                                                                        l.champs.Add(c);
                                                                                        num_char++;
                                                                                        if (tab_val[num_char] != EndObject)
                                                                                        {
                                                                                            throw new Exception("Json.DeserialiserObjet: pas de fin de champ");
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    t.ligs.Add(l);
                                                                    num_char++;
                                                                    if (tab_val[num_char] != EndArray)
                                                                    {
                                                                        throw new Exception("Json.DeserialiserObjet: pas de fin de champs");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                num_char++;
                                                if (tab_val[num_char] != EndObject)
                                                {
                                                    throw new Exception("Json.DeserialiserObjet: pas de fin de ligne");
                                                }
                                            }
                                        }
                                    }
                                    num_char++;
                                    if (tab_val[num_char] != EndArray)
                                    {
                                        throw new Exception("Json.DeserialiserObjet: pas de fin de lignes");
                                    }
                                }
                            }
                            et.tables.Add(t);
                            num_char++;
                            if (tab_val[num_char] != EndObject)
                            {
                                throw new Exception("Json.DeserialiserObjet: pas de fin de table");
                            }
                        }
                    }
                }
                num_char++;
                if (tab_val[num_char] != EndArray)
                {
                    throw new Exception("Json.DeserialiserObjet: mauvaise fin de tableau de tables");
                }
                num_char++;
                if (tab_val[num_char] != EndObject)
                {
                    throw new Exception("Json.DeserialiserObjet: mauvaise fin d'ensemble de tables");
                }
                num_char++;
                if (num_char < tab_val.Length)
                {
                    throw new Exception("Json.DeserialiserObjet: mauvaise fin de données");
                }
            }
            catch(Exception ex)
            {
                Exception ex_bis = new Exception("num_char=" + num_char.ToString(), ex);
            }
            return et;
        }
    }
}
