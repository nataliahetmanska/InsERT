using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsERT.Moria.Asortymenty;
using InsERT.Moria.Asortymenty.Typy_wyliczeniowe;
using InsERT.Moria.Dokumenty.Logistyka;
using InsERT.Moria.Klienci;
using InsERT.Moria.ModelDanych;
using InsERT.Moria.Sfera;
using InsERT.Moria.SrodkiTrwale;
using InsERT.Moria.Waluty;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.IO;
using CSV_program;
using Xceed.Wpf.Toolkit;

namespace CSV_program
{

    public partial class CSV_tools : XlsxExporter
    {
        static XNamespace nsTowary = "http://schemas.insert.com.pl/2018/hop/towary";


        static string nsTowaryParam = "http://schemas.insert.com.pl/2015/hop/towary";
        static XNamespace nsHop = "http://schemas.insert.com.pl/2013/hop";

        private readonly SferaLauncher _launcher;

        private string _filePath;
        private int _headerRowsNo;
        private string[] _separators;
        private string _encoding;


        // konstruktor

        public CSV_tools(string filePath, int headerRowsNo, string[] separators, string encoding, string server, string dbName, string userStrefa, string passwordStrefa, string userDB, string passwordDB)
        {
            this._filePath = filePath;
            this._headerRowsNo = headerRowsNo;
            this._separators = separators;
            this._encoding = encoding;


            //_launcher = new SferaLauncher("MAJA-DELL\\INSERTNEXO", "Nexo_Demo_1", "Szef", "robocze", "sa", "insert");
            _launcher = new SferaLauncher(server, dbName, userStrefa, passwordStrefa, userDB, passwordDB);

        }

        //Methods defined


        //get/set separator
        public string[] Separators
        {
            get => this._separators;

            set => this._separators = value;

        }

        //get/set number of headers' rows
        public int HeaderRowsNo
        {
            get => this._headerRowsNo;

            set => this._headerRowsNo = value;
        }

        //get/set path to the file
        public string FilePath
        {
            get => this._filePath;
            set => this._filePath = value;

        }

        //get/set encoding (utf8/windows-1250/...)
        public string Encdng
        {
            get => this._encoding;
            set => this._encoding = value;
        }

        public List<string> getHeaders()
        {
            List<string[]> csvHeaders = new List<string[]>();

            using (var fileStream = System.IO.File.OpenRead(this.FilePath))
            using (var streamReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(fileStream, Encoding.GetEncoding(this.Encdng)))
            {
                streamReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                streamReader.Delimiters = this.Separators;
                int lineNo = 0;
                string previousField = null;
                string[] currentFields = null;
                //read the lines associated with the header
                while (streamReader.EndOfData == false && lineNo < this.HeaderRowsNo)
                {
                    currentFields = streamReader.ReadFields();

                    for (int fieldIdx = 0; fieldIdx < currentFields.Length; fieldIdx++)

                    {
                        if (currentFields[fieldIdx] == "")
                        {
                            if (previousField != null)
                                currentFields[fieldIdx] = previousField;
                        }
                        else
                        {
                            previousField = currentFields[fieldIdx];
                        }
                    }

                    csvHeaders.Add(currentFields); //the parts table is added as a header line in csvHeaders 
                    lineNo++;
                }

                int noHeaderInFirstRow = csvHeaders.First().Length;

                List<string> headersResult = new List<string>();
                for (int i = 0; i < noHeaderInFirstRow; i++) //go by columns and for each column take the rows of that column and put . between
                {
                    string columnHeader = "";
                    foreach (string[] header in csvHeaders)
                    {
                        if (header[i] != "")
                        {
                            if (columnHeader != "")
                            {
                                columnHeader = columnHeader + "." + header[i]; //create a name for parent.child header
                            }
                            else
                            {
                                columnHeader = header[i];
                            }
                        }
                    }
                    headersResult.Add(columnHeader);
                }
                fileStream.Close();
                return headersResult;

            }
        }
        //EXTRA: opcjonalny parametr removeMinus, który definiuje czy wyjściowe dane maja zawierać "-" jako pustą wartość czy jawnie pustą wartość
        public List<string[]> getData(bool removeMinus = false)
        {
            List<string[]> csvData = new List<string[]>();

            using (var fileStream = System.IO.File.OpenRead(this.FilePath))
            using (var streamReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(fileStream, Encoding.GetEncoding(this.Encdng)))
            //using (var streamReader = new System.IO.StreamReader(fileStream, Encoding.GetEncoding(this.Encdng), true, BufferSize))
            {
                streamReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                streamReader.Delimiters = this.Separators;



                string[] previousFields = null;
                string[] currentFields = null;
                int lineNo = 0;
                while (streamReader.EndOfData == false)
                {
                    //skip header rows
                    if (lineNo < this.HeaderRowsNo)
                    {
                        streamReader.ReadLine();
                        lineNo++;
                        continue;
                    }

                    currentFields = streamReader.ReadFields();

                    for (int fieldIdx = 0; fieldIdx < currentFields.Length; fieldIdx++)
                    {

                        if (currentFields[fieldIdx] == "" && previousFields != null)
                        {
                            currentFields[fieldIdx] = previousFields[fieldIdx];

                        }

                        if (removeMinus == true && currentFields[fieldIdx].Equals("-"))
                        {
                            currentFields[fieldIdx] = "";
                        }

                    }
                    csvData.Add(currentFields);
                    previousFields = currentFields;
                    lineNo++;
                }
            }
            return csvData;
        }
        protected Uchwyt UruchomSfere()
        {
            return _launcher.UruchomSfere();
        }
        //brak obsługi relacji
        //dozwolone jest zeby csvSferaMaps zawierała mapowania tylko dla jednego typu kartoteki (asortyment/klient/srodkitrwale) -> graficzne okienko z dropdown list w interfejsie 
        public string ExportExecute(bool exportToXML, bool exportToSfera, List<CSVSferaMapItem> csvSferaMaps, bool removeMinus = true)
        {
            string outputStrXML = "";
            csvSferaMaps.Sort((x, y) => x.StrSferaModelNSAttrib.CompareTo(y.StrSferaModelNSAttrib)); //sortuję listę według namespaca, to pomoże rozwiązać problem tworzenia kolekcji będących w relacji z kartoteką (w przyszłości)

            //kontrola parametrów
            if ((!exportToXML && !exportToSfera) || (csvSferaMaps.Count == 0))
            {
                return "";
            }

            if (exportToXML == true)
            {
                //export csv to xml

                CSVSferaMapItem firstMap = csvSferaMaps.First();
                var nsValues = firstMap.getXmlNamespaceName();

                ElementHelper xmlItems = new ElementHelper(nsValues.Item1, firstMap.getCatalogName(), null);
                xmlItems.GetXElement().Add(new XAttribute(XNamespace.Xmlns + "w", nsHop));

                foreach (var row in getData(removeMinus))
                {
                    var xmlItem = xmlItems.AddElement(firstMap.getCatalogName());
                    foreach (var map in csvSferaMaps)
                    {
                        xmlItem.AddElementValue(map.getBaseAttribName(), row[map.CSVColumnIdx]);
                    }
                }

                outputStrXML = "Wygenerowano plik: " + ZapiszDoPliku(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + ".xml", xmlItems.GetXElement()) + "\n";
            }
            string saveToSferaStr = "";
            if (exportToSfera == true)
            {
                //save csv to sfera
                using (var sfera = UruchomSfere())
                {
                    CSVSferaMapItem firstElement = csvSferaMaps.First();

                    if (firstElement.getCatalogName().Equals(CSVSferaMapItem.CATALOG_ASORTYMENT))
                    {

                        IAsortymenty asortymenty = sfera.PodajObiektTypu<IAsortymenty>();
                        ISzablonyAsortymentu szablony = sfera.PodajObiektTypu<ISzablonyAsortymentu>();
                        foreach (var row in getData(removeMinus))
                        {
                            using (IAsortyment towar = asortymenty.Utworz()) //wywołuje na menadzerze metodę Utwórz()
                            {

                                towar.WypelnijNaPodstawieSzablonu(szablony.DaneDomyslne.Towar);
                                //na obecny moment bez relacji!!!
                                foreach (CSVSferaMapItem map in csvSferaMaps)
                                {
                                    towar.Dane.GetType().GetProperty(map.getBaseAttribName()).SetValue(towar.Dane, row[map.CSVColumnIdx]); //https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperty?view=net-6.0
                                }

                                if (!towar.Zapisz())
                                    towar.WypiszBledy();
                            }
                        }
                    }
                    else if (firstElement.getCatalogName().Equals(CSVSferaMapItem.CATALOG_PODMIOT))
                    {
                        IPodmioty podmioty = sfera.PodajObiektTypu<IPodmioty>();

                        foreach (var row in getData(removeMinus))
                        {
                            using (IPodmiot podmiot = podmioty.UtworzFirme())
                            {
                                podmiot.AutoSymbol();
                                //na obecny moment bez relacji!!!
                                foreach (CSVSferaMapItem map in csvSferaMaps)
                                {
                                    podmiot.Dane.GetType().GetProperty(map.getBaseAttribName()).SetValue(podmiot.Dane, row[map.CSVColumnIdx]); //https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperty?view=net-6.0
                                }

                                if (!podmiot.Zapisz())
                                    podmiot.WypiszBledy();
                            }

                        }


                    }
                    else if (firstElement.getCatalogName().Equals(CSVSferaMapItem.CATALOG_SRODEKTRWALY))
                    {
                        ISrodkiTrwale srodkiTrwale = sfera.PodajObiektTypu<InsERT.Moria.SrodkiTrwale.ISrodkiTrwale>();
                        IGrupySrodkowTrwalychDane grupy = sfera.PodajObiektTypu<IGrupySrodkowTrwalychDane>();

                        IQueryable<RodzajSrodkaTrwalego> rodzaje = sfera.PodajObiektTypu<IRodzajeSrodkowTrwalych>().Dane.Wszystkie();

                        foreach (var row in getData(removeMinus))
                        {
                            using (ISrodekTrwaly srodekTrwaly = srodkiTrwale.Utworz())
                            {
                                //możliwe że trzeba dodać wypełnienie domyślnymi wartosciami???
                                int nr = srodkiTrwale.Dane.Wszystkie().Max(s => s.Id) + 1;
                                srodekTrwaly.Dane.NrInwentarzowy = string.Format("{0:00000}", nr);

                                foreach (CSVSferaMapItem map in csvSferaMaps)
                                {
                                    //logika dla wymaganego pola kst
                                    if (map.getBaseAttribName().Equals("KST"))
                                    {
                                        //odszukaj kst
                                        var strKST = row[map.CSVColumnIdx];
                                        srodekTrwaly.UstawJakoSrodekTrwalyKST2010(rodzaje.Where(r => r.Symbol == strKST).Single()); //https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperty?view=net-6.0
                                        //srodekTrwaly.UstawJakoSrodekTrwalyKST2016(rodzaje.Where(r => r.Symbol == row[map.CSVColumnIdx]).Single());
                                    }
                                    else
                                    {
                                        srodekTrwaly.Dane.GetType().GetProperty(map.getBaseAttribName()).SetValue(srodekTrwaly.Dane, row[map.CSVColumnIdx]); //https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperty?view=net-6.0

                                    }

                                }

                                if (!srodekTrwaly.Zapisz())
                                    srodekTrwaly.WypiszBledy();
                            }
                        }

                    }
                    saveToSferaStr = "Zapisano obiekty do sfery";

                }
            }
            return outputStrXML + "\n" + saveToSferaStr;
        }
    }
}