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
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Tuple = System.Tuple;

namespace CSV_program
{

    public class CSV_tools : XlsxExporter
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


        // zwraca liste nagłówków lub null w przypadku błędu (messege box shows up)
        public List<string> getHeaders()
        {
            List<string[]> csvHeaders = new List<string[]>();
            List<string> headersResult = new List<string>();
            try
            {

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


                }
            }
            catch (Microsoft.VisualBasic.FileIO.MalformedLineException e)
            {
                System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Niespójność w ilości kolumn nagłówków w wierszach. Nie można utworzyć nagłówków. Popraw plik csv.", "Błąd");



            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Nie można otworzyć pliku lub brak do niego dostępu. " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");


            }

            return headersResult;


        }
        //EXTRA: opcjonalny parametr removeMinus, który definiuje czy wyjściowe dane maja zawierać "-" jako pustą wartość czy jawnie pustą wartość
        public List<string[]> getData(bool removeMinus = false)
        {
            List<string[]> csvData = new List<string[]>();
            int lineNo = 0;
            try
            {


                using (var fileStream = System.IO.File.OpenRead(this.FilePath))
                using (var streamReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(fileStream, Encoding.GetEncoding(this.Encdng)))

                {
                    streamReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                    streamReader.Delimiters = this.Separators;



                    string[] previousFields = null;
                    string[] currentFields = null;

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
                    return csvData;
                }
            }

            catch (Microsoft.VisualBasic.FileIO.MalformedLineException e)
            {
                System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Niespójność w ilości kolumn w wierszu (" + lineNo.ToString() + "). Popraw plik csv. Żadne dane nie zostały przetworzone.", "Błąd");
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Nie można otworzyć pliku lub brak do niego dostępu. Brak danych do przetworzenia. " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
            }
            return new List<string[]>();

        }
        protected Uchwyt UruchomSfere()
        {
            return _launcher.UruchomSfere();
        }

        //dozwolone jest zeby csvSferaMaps zawierała mapowania tylko dla jednego typu kartoteki (asortyment/klient/srodkitrwale) -> graficzne okienko z dropdown list w interfejsie 
        public void ExportExecute(bool exportToXML, bool exportToSfera, List<CSVSferaMapItem> csvSferaMaps, bool removeMinus = true)
        {
            string outputStrXML = "";
            // csvSferaMaps.Sort((x, y) => x.StrSferaModelNSAttrib.CompareTo(y.StrSferaModelNSAttrib)); //sortuję listę według namespaca, to pomoże rozwiązać problem tworzenia kolekcji będących w relacji z kartoteką (w przyszłości)

            //kontrola parametrów
            if ((!exportToXML && !exportToSfera) || (csvSferaMaps.Count == 0))
            {
                System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Nie określono akcji (eksport do xml lub zapis do sfery) lub lista mapowań jest pusta.", "Błąd");
                return;
            }

            //blok tworzący xmla (razem z prostymi relacjami - czyli tylko z jedną kartoteką (jeden poziom))

            if (exportToXML == true)
            {

                List<CSVSferaMapItem> csvSferaMapsEntityTypeOnly = csvSferaMaps.FindAll(
                                                                                delegate (CSVSferaMapItem mapItem)
                                                                                {
                                                                                    return mapItem.MapType == CSVSferaMapItem.MAP_TYPE_ENTITY; //lista mapowań tylko typu Entity
                                                                                });
                List<CSVSferaMapItem> csvSferaMapsRelTypeOnly = csvSferaMaps.FindAll(
                                                                                delegate (CSVSferaMapItem mapItem)
                                                                                {
                                                                                    return mapItem.MapType == CSVSferaMapItem.MAP_TYPE_REL; //lista mapowań tylko typu Relacje
                                                                                });

                CSVSferaMapItem firstMap = csvSferaMaps.First(); //bierzemy pierwsze mapowanie z listy
                var nsValues = firstMap.getXmlNamespaceName();

                //korzystamy z api insertowego do budowy xmla (szczegóły w przykładzie: Excel2hop
                ElementHelper xmlItems = new ElementHelper(nsValues.Item1, firstMap.getCatalogName(), null);
                xmlItems.GetXElement().Add(new XAttribute(XNamespace.Xmlns + "w", nsHop));



                //główna pętla iterująca wiersz po wierszu czyli np Asortyment po Asortymencie (po jednym wierszu z csv)

                foreach (var row in getData(removeMinus))
                {
                    var xmlItem = xmlItems.AddElement(firstMap.getCatalogName()); //tworzę nagłówek xmla (obiekt głównej kartoteki)


                    //najpierw zajmuję się mapowaniami typu Entity
                    if (csvSferaMapsEntityTypeOnly.Count > 0)
                        foreach (var map in csvSferaMapsEntityTypeOnly)
                        {
                            xmlItem.AddElementValue(map.getBaseAttribName(), row[map.CSVColumnIdx]); //tutaj sie dodają pola np Symbol, Nazwa, Opis, etc z ich wartościami 

                        }


                    //teraz zajmuję się mapowaniami typu Relacje
                    string contextRelName = "";
                    int contextRelInstance = -1;


                    ElementHelper rel = null;
                    if (csvSferaMapsRelTypeOnly.Count > 0)
                        foreach (var map in csvSferaMapsRelTypeOnly)
                        {
                            if (map.getRelName().Equals(contextRelName))
                            {
                                if (map.getRelInstanceNumber() == contextRelInstance)
                                {
                                    //ta sama kartoteka, ta sama relacja , ta sama instancja relacji, czyli np Asortyment.Dostawcy.1 --->  dodaję tylko atrybut (np. Symbol) z wartością                              
                                    rel.AddElementValue(map.getBaseAttribName(), row[map.CSVColumnIdx]);
                                }
                                else
                                {
                                    //nowa instancja realacji, czyli np (stare) Asortyment.Dostawca.1 i (obecny) Asortyment.Dostawca.2 --->  dodaję atrybut w nowej relacji
                                    rel = xmlItem.AddElement(contextRelName); // tutaj w obrębie Asortymentu tworzymy blok np Dostawca (relacja)
                                    contextRelInstance = map.getRelInstanceNumber();
                                    rel.AddElementValue(map.getBaseAttribName(), row[map.CSVColumnIdx]); //dodaję atrybuty dostawcy (czyli atrybity które posiada kartoteka z którą mamy relację)
                                }
                            }
                            else
                            {
                                //nowa relacja, czyli np Asortyment.Dostawca i Asortyment.Odbiorca ---> tworzę atrybut w nowej relacji
                                contextRelName = map.getRelName();
                                rel = xmlItem.AddElement(contextRelName);// budowa bloku głównej kartoteki np <Asortyment> ... </Asortyment>
                                contextRelInstance = map.getRelInstanceNumber();
                                rel.AddElementValue(map.getBaseAttribName(), row[map.CSVColumnIdx]);
                            }
                        }



                }
                //zapisuję do pliku
                try
                {

                    outputStrXML = "Wygenerowano plik xml: " + ZapiszDoPliku(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + ".xml", xmlItems.GetXElement()) + "\n";
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Nie można zapisać exportu do pliku xml " + FilePath + " Sprawdź czy instnieje katalog docelowy lub jego uprawnienia. " + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
                }
            }

            //WAŻNE ####################################################################################################################################################################################################################################################################

            //blok zapisujący do sfery
            string saveToSferaStr = "";
            if (exportToSfera == true)
            {
                //save csv to sfera
                using (var sfera = UruchomSfere())
                {
                    CSVSferaMapItem firstElement = csvSferaMaps.First();
                    int lineNo = 0;
                    foreach (var row in getData(removeMinus))
                    {
                        //metoda która zapisuje obiekt do sfery z mapowaniami typu Entity i Relacje- do edycji przy tworzeniu relacji środków trwałych i podmiotów!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        try
                        {
                            processCSVCells(sfera, csvSferaMaps, row, lineNo);
                        }
                        catch (Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Błąd w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Błąd w przetwarzaniu linii pliku csv " + lineNo.ToString() + ". Przerwano przetwarzanie.\n" + e.Message + " " + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
                            return;
                        }
                        lineNo++;

                    }




                    /*if (firstElement.getCatalogName().Equals(CSVSferaMapItem.CATALOG_ASORTYMENT))
                    {
                        
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// stare kody: jak się tworzy Podmioty i Środki trwałe 
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
                    }*/
                    saveToSferaStr = "Zapisano obiekty do sfery";

                }
            }

        }


        public void processCSVCells(Uchwyt sfera, List<CSVSferaMapItem> csvSferaMaps, string[] csvValues, int csvLineNo)
        {


            CSVSferaMapItem firstElement = csvSferaMaps.First(); //odczytuję pierwszy element z listy mapowań 

            if (firstElement.getCatalogName().Equals(CSVSferaMapItem.CATALOG_ASORTYMENT)) //z firstElement czytam nazwę głównej kartoteki i wchodzę do odpowiedniej sekcji która go obsługuje
            {
                // OBSŁUGA ASORTYMENTU

                //mapowanie "Symbol" jest wymagane
                //szukam czy jest mapowanie względem kluczowego pola (Symbol), po któym w piersszej kolejności szukam czy istnieje już asortyment w kartotece
                //takie pole to na sztywno Symbol dla asortymentu (jest unique)




                CSVSferaMapItem mapSymbol = csvSferaMaps.Find(map => map.MapType == CSVSferaMapItem.MAP_TYPE_ENTITY && map.getBaseAttribName().Equals("Symbol")); //szukam mapowania "Symbol" czyli np Asortyment.Symbol 

                if (mapSymbol == null)
                {

                    throw new NullReferenceException("Brak mapowania dla Symbol dla kartoteki " + CSVSferaMapItem.CATALOG_ASORTYMENT + " Popraw mapowanie.");


                }



                string mapSymbolValue = csvValues[mapSymbol.CSVColumnIdx]; //pobieram wartość z csv dla tego mapowania (dla atrybutu Symbol)
                if (mapSymbolValue.Equals(""))
                {

                    System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Kartoteka: " + firstElement.getCatalogName() +
                        ". Brak wartości dla wymaganego atrybutu Symbol w pliku csv w linii " + csvLineNo + ".  Obiekt nie został został zapisany w Sferze.", "Ostrzeżenie");
                    return;

                }
                IAsortymenty asortymenty = sfera.PodajObiektTypu<IAsortymenty>();
                Asortyment encjaTowar = asortymenty.Dane.Wszystkie().Where(a => a.Symbol.Equals(mapSymbolValue)).FirstOrDefault(); //szukam w kartotece asortymetu w sferze który też zawiera wartość taką samą jak Symbol w csv
                IAsortyment towar;


                if (encjaTowar == null)
                {
                    //nie ma towaru o takim symbolu w kartotece wiec go tworzymy
                    ISzablonyAsortymentu szablony = sfera.PodajObiektTypu<ISzablonyAsortymentu>();
                    towar = asortymenty.Utworz();
                    towar.WypelnijNaPodstawieSzablonu(szablony.DaneDomyslne.Towar); //tak było w innym przykładzie - tworzymy i wypełniamy szablonem
                }
                else
                {
                    towar = asortymenty.Znajdz(encjaTowar); //znaleźliśmy towar
                }

                //mamy obiekt Asortyment w towar


                //wyodrębnione mapowania typu entity (lista)- wypełniam je dla towaru
                List<CSVSferaMapItem> csvSferaMapsEntityTypeOnly = csvSferaMaps.FindAll(
                                                            delegate (CSVSferaMapItem mapItem)
                                                            {
                                                                return mapItem.MapType == CSVSferaMapItem.MAP_TYPE_ENTITY; //lista mapowań tylko typu Entity
                                                            });
                //wyodrebnione mapowania typu relacja
                List<CSVSferaMapItem> csvSferaMapsRelTypeOnly = csvSferaMaps.FindAll(
                                                                                delegate (CSVSferaMapItem mapItem)
                                                                                {
                                                                                    return mapItem.MapType == CSVSferaMapItem.MAP_TYPE_REL; //lista mapowań tylko typu Relacje
                                                                                });
                csvSferaMapsRelTypeOnly.Sort((x, y) => x.StrSferaModelNSAttrib.CompareTo(y.StrSferaModelNSAttrib));
                //sortuję listę typu Relacje po stringu mapowania dzięki temu elementy są pokudładane wg relacji i nr instancji w obrębie relacji, przykład:
                //przed:
                // Asortyment.Dostawca.1.Symbol
                // Asortyment.Dostawca.2.Nazwa
                // Asortyment.Dostawca.3.NIP
                // Asortyment.Dostawca.2.Symbol
                // Asortyment.Dostawca.1.Nazwa
                // Asortyment.Odbiorca.1.Symbol
                // Asortyment.Dostawca.2.NIP
                // Asortyment.Dostawca.3.Symbol
                // Asortyment.Dostawca.3.Nazwa
                // Asortyment.Dostawca.1.NIP

                // po:
                // Asortyment.Dostawca.1.Nazwa
                // Asortyment.Dostawca.1.NIP
                // Asortyment.Dostawca.1.Symbol

                // Asortyment.Dostawca.2.Nazwa
                // Asortyment.Dostawca.2.NIP
                // Asortyment.Dostawca.2.Symbol

                // Asortyment.Dostawca.3.Nazwa
                // Asortyment.Dostawca.3.NIP
                // Asortyment.Dostawca.3.Symbol

                // Asortyment.Odbiorca.1.Symbol



                //w pierwszej kolejności wypełnianie atrybutów tylko typy entity  (te proste)
                if (csvSferaMapsEntityTypeOnly.Count > 0)
                    foreach (CSVSferaMapItem map in csvSferaMapsEntityTypeOnly)
                    {
                        try
                        {

                            ValueOnAttribEntity<Asortyment>(towar.Dane, csvValues[map.CSVColumnIdx], map);


                        }
                        catch (Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Ustawienie atrybutu na obiekcie w Sferze się nie powiodło. Obiekt nie został zapisany w Sferze. Kontynuję." + e.Message + " "
                                + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
                        }
                    }

                if (!towar.Zapisz())
                    towar.WypiszBledy();

                //a teraz relacje :/ <3 :)))
                foreach (List<CSVSferaMapItem> listOfMapsSameRelInst in NextRelationInstanceMaps(csvSferaMapsRelTypeOnly))   //NextRelationInstanceMaps(csvSferaMapsRelTypeOnly)) ---> enumerator ; zwraca po sekcjach (opisane niżej)
                {
                    //listOfMapsSameRelInst zawiera listę mapowań jednej relacji i tej samej instancji
                    //np:

                    // Asortyment.Dostawca.3.Nazwa
                    // Asortyment.Dostawca.3.NIP
                    // Asortyment.Dostawca.3.Symbol


                    //obsługuję relacje: Dostawca (n) , Odbiorca (n), Producent (tylko jeden)
                    if (listOfMapsSameRelInst.First().getRelName().Equals(CSVSferaMapItem.CATALOG_ODBIORCA))
                    {
                        //po polu kluczowym znajdz w katalogu podmiotów, jak nie znajdujesz to utworz nowy
                        //wypełnij jego pola
                        //i dodaj obiekt relacyjny do towaru 


                        CSVSferaMapItem mapNazwaSkrocona = listOfMapsSameRelInst.Find(map => map.getBaseAttribName().Equals("NazwaSkrocona"));
                        if (mapNazwaSkrocona == null)
                        {
                            throw new NullReferenceException("Kartoteka: " + firstElement.getCatalogName() + " Relacja: "
                            + CSVSferaMapItem.CATALOG_ODBIORCA + ". Brak mapowania kluczowego atrybutu NazwaSkrocona dla " +
                            CSVSferaMapItem.CATALOG_ODBIORCA + ". Przerywam przetwarzanie. Popraw mapowanie.");

                        }

                        string mapNazwaSkroconaValue = csvValues[mapNazwaSkrocona.CSVColumnIdx];
                        if (mapNazwaSkroconaValue.Equals(""))
                        {
                            System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Kartoteka: " + firstElement.getCatalogName() + " Relacja: "
                            + CSVSferaMapItem.CATALOG_ODBIORCA + "Brak wartości dla kluczowego atrybutu NazwaSkrocona w pliku cvs w linii " + csvLineNo.ToString() + ". Relacja nie została utworzona.", "Ostrzeżenie");
                            continue;
                        }





                        IPodmioty podmioty = sfera.PodajObiektTypu<IPodmioty>();

                        Podmiot encjaOdbiorca = podmioty.Dane.Wszystkie().Where(a => a.NazwaSkrocona.Equals(mapNazwaSkroconaValue)).FirstOrDefault();
                        IPodmiot odbiorca;


                        if (encjaOdbiorca == null)
                        {
                            //nie ma odbiorcy o takiej nazwieskroconej w kartotece wiec go tworzymy
                            odbiorca = podmioty.UtworzFirme();
                        }
                        else
                        {
                            odbiorca = podmioty.Znajdz(encjaOdbiorca);
                        }


                        odbiorca.AutoSymbol();
                        foreach (CSVSferaMapItem mapOdbiorcy in listOfMapsSameRelInst)
                        {
                            try
                            {
                                ValueOnAttribEntity<Podmiot>(odbiorca.Dane, csvValues[mapOdbiorcy.CSVColumnIdx], mapOdbiorcy);
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Ustawienie atrybutu na obiekcie relacji Odbiorca dla Asortymentu dla linii " + csvLineNo.ToString() + " w Sferze się nie powiodło. Obiekt nie został zapisany w Sferze. Kontynuję." + e.Message + " "
                                    + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
                            }
                        }

                        if (!odbiorca.Zapisz()) //zapisuję odbiorcę (ten obiekt dołączany)
                            odbiorca.WypiszBledy();


                        encjaOdbiorca = podmioty.Dane.Wszystkie().Where(a => a.NazwaSkrocona.Equals(mapNazwaSkroconaValue)).FirstOrDefault(); //jeszcze raz szukam po nazwie skroconej naszego odbiorcy (inaczej się nie dało xd,
                                                                                                                                              //NazwaSkrocona jest wymagana dla relacji Asortyment, jakikolwiek obiekt typu Podmiot, a nie IPodmiot


                        DaneAsortymentuDlaPodmiotu odbiorcaTowaru = towar.Odbiorcy.Dodaj(encjaOdbiorca); //dołączenie relacji (dołączenie do głównej kartoteki Asortyment relacyjnego obiektu - czyli tu Odbiorę, kartoteki Podmiot
                        //na końcu zapisz towar
                        if (!towar.Zapisz())
                            towar.WypiszBledy();




                    }
                    else if (listOfMapsSameRelInst.First().getRelName().Equals(CSVSferaMapItem.CATALOG_PRODUCENT))
                    {

                        CSVSferaMapItem mapNazwaSkrocona = listOfMapsSameRelInst.Find(map => map.getBaseAttribName().Equals("NazwaSkrocona"));
                        if (mapNazwaSkrocona == null)
                        {
                            throw new NullReferenceException("Kartoteka: " + firstElement.getCatalogName() + " Relacja: "
                            + CSVSferaMapItem.CATALOG_PRODUCENT + ". Brak mapowania kluczowego atrybutu NazwaSkrocona dla " +
                            CSVSferaMapItem.CATALOG_PRODUCENT + ". Przerywam przetwarzanie. Popraw mapowanie.");

                        }

                        string mapNazwaSkroconaValue = csvValues[mapNazwaSkrocona.CSVColumnIdx];
                        if (mapNazwaSkroconaValue.Equals(""))
                        {
                            System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Kartoteka: " + firstElement.getCatalogName() + " Relacja: "
                            + CSVSferaMapItem.CATALOG_PRODUCENT + "Brak wartości dla kluczowego atrybutu NazwaSkrocona w pliku csv w linii " + csvLineNo.ToString() + ". Relacja nie została utworzona.", "Ostrzeżenie");
                            continue;


                        }

                        IPodmioty podmioty = sfera.PodajObiektTypu<IPodmioty>();

                        Podmiot encjaProducent = podmioty.Dane.Wszystkie().Where(a => a.NazwaSkrocona.Equals(mapNazwaSkroconaValue)).FirstOrDefault();
                        IPodmiot producent;


                        if (encjaProducent == null)
                        {
                            //nie ma odbiorcy o takiej nazwieskroconej w kartotece wiec go tworzymy
                            producent = podmioty.UtworzFirme();

                        }
                        else
                        {
                            producent = podmioty.Znajdz(encjaProducent);
                        }


                        producent.AutoSymbol();
                        foreach (CSVSferaMapItem mapProducent in listOfMapsSameRelInst)
                        {
                            try
                            {

                                ValueOnAttribEntity<Podmiot>(producent.Dane, csvValues[mapProducent.CSVColumnIdx], mapProducent);
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Ustawienie atrybutu na obiekcie relacji Producent dla Asortymentu dla linii " + csvLineNo.ToString() + " w Sferze się nie powiodło. Obiekt nie został zapisany w Sferze. Kontynuję." + e.Message + " "
                                    + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
                            }
                        }

                        if (!producent.Zapisz())
                            producent.WypiszBledy();
                        //jeszcze raz go otszukuję w kartotece żeby mieć obiekt klasy Podmiot (a nie IPodmiot)
                        encjaProducent = podmioty.Dane.Wszystkie().Where(a => a.NazwaSkrocona.Equals(mapNazwaSkroconaValue)).FirstOrDefault();

                        DaneAsortymentuDlaPodmiotu producentTowaru = towar.Producent.Ustaw(encjaProducent);
                        //na końcu zapisz towar
                        if (!towar.Zapisz())
                            towar.WypiszBledy();


                    }
                    else if (listOfMapsSameRelInst.First().getRelName().Equals(CSVSferaMapItem.CATALOG_DOSTAWCA))
                    {
                        CSVSferaMapItem mapNazwaSkrocona = listOfMapsSameRelInst.Find(map => map.getBaseAttribName().Equals("NazwaSkrocona"));
                        if (mapNazwaSkrocona == null)
                        {
                            throw new NullReferenceException("Kartoteka: " + firstElement.getCatalogName() + " Relacja: "
                                + CSVSferaMapItem.CATALOG_DOSTAWCA + ". Brak mapowania kluczowego atrybutu NazwaSkrocona dla " +
                                CSVSferaMapItem.CATALOG_DOSTAWCA + ". Przerywam przetwarzanie. Popraw mapowanie.");


                        }

                        string mapNazwaSkroconaValue = csvValues[mapNazwaSkrocona.CSVColumnIdx];
                        if (mapNazwaSkroconaValue.Equals(""))
                        {
                            System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Kartoteka: " + firstElement.getCatalogName() + " Relacja: "
                           + CSVSferaMapItem.CATALOG_DOSTAWCA + "Brak wartości dla kluczowego atrybutu NazwaSkrocona w pliku csv w linii " + csvLineNo.ToString() + ". Relacja nie została utworzona.", "Ostrzeżenie");
                            continue;
                        }
                        IPodmioty podmioty = sfera.PodajObiektTypu<IPodmioty>();

                        Podmiot encjaDostawca = podmioty.Dane.Wszystkie().Where(a => a.NazwaSkrocona.Equals(mapNazwaSkroconaValue)).FirstOrDefault();
                        IPodmiot dostawca;


                        if (encjaDostawca == null)
                        {
                            //nie ma odbiorcy o takiej nazwieskroconej w kartotece wiec go tworzymy 
                            dostawca = podmioty.UtworzFirme();
                        }
                        else
                        {
                            dostawca = podmioty.Znajdz(encjaDostawca);
                        }


                        dostawca.AutoSymbol();
                        foreach (CSVSferaMapItem mapDostawcy in listOfMapsSameRelInst)
                        {
                            try
                            {


                                ValueOnAttribEntity<Podmiot>(dostawca.Dane, csvValues[mapDostawcy.CSVColumnIdx], mapDostawcy);
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Ostrzeżenie w lokalizacji: " + this.GetType().Name + " " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + "Ustawienie atrybutu na obiekcie relacji Dostawca dla Asortymentu dla linii " + csvLineNo.ToString() + " w Sferze się nie powiodło. Obiekt nie został zapisany w Sferze. Kontynuję." + e.Message + " "
                                    + (e.InnerException != null ? e.InnerException.Message : ""), "Błąd");
                            }
                        }

                        if (!dostawca.Zapisz())
                            dostawca.WypiszBledy();

                        //dołączenie odbiorcy do asortymentu - relacja
                        encjaDostawca = podmioty.Dane.Wszystkie().Where(a => a.NazwaSkrocona.Equals(mapNazwaSkroconaValue)).FirstOrDefault();
                        DaneAsortymentuDlaPodmiotu dostawcaTowaru = towar.Dostawcy.Dodaj(encjaDostawca);

                        //na końcu zapisz towar
                        if (!towar.Zapisz())
                            towar.WypiszBledy();




                    }
                }
            }
        }
        protected void ValueOnAttribEntity<T>(T dane, string valueStr, CSVSferaMapItem mapItem)
        {
            if (dane.GetType().GetProperty(mapItem.getBaseAttribName()).PropertyType.Name.Equals("String"))
            {
                dane.GetType().GetProperty(mapItem.getBaseAttribName()).SetValue(dane, valueStr);
            }
            else if (dane.GetType().GetProperty(mapItem.getBaseAttribName()).PropertyType.Name.Equals("Decimal"))
            {
                dane.GetType().GetProperty(mapItem.getBaseAttribName()).SetValue(dane, Convert.ToDecimal(valueStr));
            }
            else if (dane.GetType().GetProperty(mapItem.getBaseAttribName()).PropertyType.Name.Equals("Nullable`1")) //integer
            {
                dane.GetType().GetProperty(mapItem.getBaseAttribName()).SetValue(dane, Convert.ToInt32(valueStr));
            }
            else if (dane.GetType().GetProperty(mapItem.getBaseAttribName()).PropertyType.Name.Equals("Byte")) //tinyint
            {
                dane.GetType().GetProperty(mapItem.getBaseAttribName()).SetValue(dane, Convert.ToByte(valueStr));
            }
            else if (dane.GetType().GetProperty(mapItem.getBaseAttribName()).PropertyType.Name.Equals("Boolean"))
            {
                if (valueStr.Equals("0") || valueStr.Equals("nie") || valueStr.Equals("no") || valueStr.Equals("false"))
                    dane.GetType().GetProperty(mapItem.getBaseAttribName()).SetValue(dane, false);
                else if (valueStr.Equals("1") || valueStr.Equals("tak") || valueStr.Equals("yes") || valueStr.Equals("true"))
                    dane.GetType().GetProperty(mapItem.getBaseAttribName()).SetValue(dane, true);
            }
        }
        public System.Collections.Generic.IEnumerable<List<CSVSferaMapItem>> NextRelationInstanceMaps(List<CSVSferaMapItem> csvSferaMaps)
        {
            List<CSVSferaMapItem> results = new List<CSVSferaMapItem>();
            string strRegex = @"^\w+\.\w+\.\d+\.";
            Regex re = new Regex(strRegex);
            Match match = re.Match(csvSferaMaps.First().StrSferaModelNSAttrib);
            string currentMatchStr = "";
            if (match.Success)
            {
                currentMatchStr = match.Value;
            }



            foreach (var map in csvSferaMaps)
            {


                //bierzemy "String.String.Number."


                Regex regexp = new Regex(strRegex);

                Match mt = regexp.Match(map.StrSferaModelNSAttrib);

                if (mt.Success)
                {
                    if (currentMatchStr.Equals(mt.Value))
                    {
                        results.Add(map);
                    }
                    else
                    {
                        yield return results;
                        results = new List<CSVSferaMapItem>(); //zwracamy nową lisę na kolejny degment
                        results.Add(map);
                        currentMatchStr = mt.Value;
                    }
                }
            }

            if (results.Count() > 0)
                yield return results;

        }




    }

    /*
    Uwagi:
    - Pola w mapowaniu rozdzielane są kropką
    - Kropka nie może występować w nazwach 
    - Mapowanie jest case sensitive 
    - Rodzaje mapowań zaczynają się od: Podmiot (czyli Klient) , SrodekTrwaly, Asortyment
                                        Ogólny schemat: kartoteka.atrybut , kartoteka.kolekcja.instancja.atrybut
                                        Przykład: "Podmiot.NIP" lub "Podmiot.Symbol" lub "Podmiot.Kontakty.1.Typ" lub "Podmiot.Kontakty.1.Wartosc" lub "Podmiot.Kontakty.2.Typ" ...
    - Klasa wiąże atrybut z indeksem kolumny w csv
    - idx kolumy w csv zaczyna się od 0 
     */

    public class CSVSferaMapItem
    {
        public static string CATALOG_PODMIOT = "Podmiot";
        public static string CATALOG_ASORTYMENT = "Asortymenty";
        public static string CATALOG_SRODEKTRWALY = "SrodekTrwaly";
        public static string CATALOG_DOSTAWCA = "Dostawca";
        public static string CATALOG_ODBIORCA = "Odbiorca";
        public static string CATALOG_PRODUCENT = "Producent";
        public static int MAP_TYPE_ENTITY = 1; //mapowanie typu encja, czyli takie typowe mapowanie indeks kolumny z csv + atrybut modelu
        public static int MAP_TYPE_REL = 2; //mapowanie typu relacja


        public static char MAP_SEPARATOR = '.';

        private string _strSferaModelNSAttrib;
        private Dictionary<string, Tuple<string, string>> _dictNsName;
        private int _mapType; // pole przybierające jedno z dwóch wyżej zdefiniowanych typów mapowania


        private int _csvColumnIdx; //idx kolumy w csv

        public CSVSferaMapItem(string strSferaModelNSAttrib, int csvColumnIdx, int mapType)
        {
            StrSferaModelNSAttrib = strSferaModelNSAttrib;
            CSVColumnIdx = csvColumnIdx;
            _dictNsName = new Dictionary<string, Tuple<string, string>>();
            _dictNsName.Add(CATALOG_ASORTYMENT, Tuple.Create("http://schemas.insert.com.pl/2018/hop/towary", "http://schemas.insert.com.pl/2015/hop/towary"));
            _dictNsName.Add(CATALOG_PODMIOT, Tuple.Create("http://schemas.insert.com.pl/2018/hop/klienci", "http://schemas.insert.com.pl/2018/hop/klienci"));
            _dictNsName.Add(CATALOG_SRODEKTRWALY, Tuple.Create("http://schemas.insert.com.pl/2018/hop/srodkitrwale", "http://schemas.insert.com.pl/2018/hop/srodkitrwale"));
            MapType = mapType;
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

        public int MapType
        {

            get => this._mapType;

            set => this._mapType = value;

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
            //zwroc ostatni elelment po splicie
            string[] attribsMap = StrSferaModelNSAttrib.Split(MAP_SEPARATOR);

            return attribsMap[attribsMap.Length - 1];

        }
        // Pomysł na kolejne metody do implementacji: isBaseAttrib(), isRelAttrib(), getRelInstanceNumber()


        //metody do mapowania
        public string getRelName() //zwraca nazwę relacji
        {

            if (MapType.Equals(MAP_TYPE_REL))
            {
                return StrSferaModelNSAttrib.Split(MAP_SEPARATOR)[1]; // np "Podmiot.Kontakty.1.Wartosc" zwraca "Kontakty"
            }
            else
            {
                return ""; // pusty string jak mapowanie nie jest typu Relacje
            }


        }

        public int getRelInstanceNumber() //zwraca 
        {

            if (MapType.Equals(MAP_TYPE_REL))
            {
                return Int32.Parse(StrSferaModelNSAttrib.Split(MAP_SEPARATOR)[2]); // np "Podmiot.Kontakty.1.Wartosc" zwraca "1"
            }
            else
            {
                return -1; // -1 jako błąd
            }


        }







    }

    public class SferaLauncher
    {
        private string _serwer;
        private string _baza;
        private string _login;
        private string _haslo;
        private string _uzytkownikSerwera;
        private string _hasloSerwera;

        public SferaLauncher(string serwer, string baza, string login, string haslo, string uzytkownikSerwera, string hasloSerwera)
        {
            _serwer = serwer ?? throw new ArgumentNullException(nameof(serwer));
            _baza = baza ?? throw new ArgumentNullException(nameof(baza));
            _login = login ?? throw new ArgumentNullException(nameof(login));
            _haslo = haslo ?? throw new ArgumentNullException(nameof(haslo));
            _uzytkownikSerwera = uzytkownikSerwera ?? throw new ArgumentNullException(nameof(uzytkownikSerwera));
            _hasloSerwera = hasloSerwera ?? throw new ArgumentNullException(nameof(hasloSerwera));
        }

        public Uchwyt UruchomSfere()
        {
            DanePolaczenia danePolaczenia = DanePolaczenia.Jawne(_serwer, _baza, uzytkownikSerwera: _uzytkownikSerwera, hasloUzytkownikaSerwera: _hasloSerwera);
            MenedzerPolaczen mp = new MenedzerPolaczen();
            Uchwyt sfera = mp.Polacz(danePolaczenia, InsERT.Mox.Product.ProductId.Subiekt);
            if (!sfera.ZalogujOperatora(_login, _haslo))
                throw new ArgumentException("Nieprawidłowa nazwa lub hasło użytkownika.");
            return sfera;
        }
    }




}