using System;
using System.Collections.Generic;
using Tuple = System.Tuple;

namespace CSV_program
{

    public class CSVSferaMapItem
    {
        public static string CATALOG_PODMIOT = "Podmiot";
        public static string CATALOG_ASORTYMENT = "Asortymenty";
        public static string CATALOG_SRODEKTRWALY = "SrodekTrwaly";

        public static char MAP_SEPARATOR = '.';

        private string _strSferaModelNSAttrib;
        private Dictionary<string, Tuple<string, string>> _dictNsName;

        private int _csvColumnIdx; //idx kolumy w csv

        public CSVSferaMapItem(string strSferaModelNSAttrib, int csvColumnIdx)
        {
            StrSferaModelNSAttrib = strSferaModelNSAttrib;
            CSVColumnIdx = csvColumnIdx;
            _dictNsName = new Dictionary<string, Tuple<string, string>>();
            _dictNsName.Add(CATALOG_ASORTYMENT, Tuple.Create("http://schemas.insert.com.pl/2018/hop/towary", "http://schemas.insert.com.pl/2015/hop/towary"));
            _dictNsName.Add(CATALOG_PODMIOT, Tuple.Create("http://schemas.insert.com.pl/2018/hop/klienci", "http://schemas.insert.com.pl/2018/hop/klienci"));
            _dictNsName.Add(CATALOG_SRODEKTRWALY, Tuple.Create("http://schemas.insert.com.pl/2018/hop/srodkitrwale", "http://schemas.insert.com.pl/2018/hop/srodkitrwale"));

        }

        public Tuple<string, string> getXmlNamespaceName()
        {
            Tuple<string, string> nameSpaces;
            if (_dictNsName.TryGetValue(getCatalogName(), out nameSpaces))
            {
                return nameSpaces;
            }
            return null;

        }
        public string StrSferaModelNSAttrib
        {

            get => this._strSferaModelNSAttrib;

            set => this._strSferaModelNSAttrib = value;

        }

        public int CSVColumnIdx
        {

            get => this._csvColumnIdx;

            set => this._csvColumnIdx = value;

        }

        public string getCatalogName()
        {
            return StrSferaModelNSAttrib.Split(MAP_SEPARATOR)[0];

        }

        public string getBaseAttribName()
        {
            return StrSferaModelNSAttrib.Split(MAP_SEPARATOR)[1];

        }
        // Pomysł na kolejne metody do implementacji: isBaseAttrib(), isCollectionAttrib(), getCollectionInstanceNumber()
    }
    
}