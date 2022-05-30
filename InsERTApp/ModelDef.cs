using System;
using System.Collections.Generic;
using System.Linq;

namespace CSV_program
{
    /// <summary>
    /// Klasa ModelDef zawiera informacje o ścieżce do tabeli oraz jej charakterystykach - obiekt TableDef
    /// </summary>
    class ModelDef
    {
        private Dictionary<string, TableDef> pathToTable;

        public ModelDef() //konstruktor
        {
            this.pathToTable = new Dictionary<string, TableDef>();
        }

        public ModelDef(Dictionary<string, TableDef> pathToTable) //konstruktor
        {
            this.pathToTable = pathToTable;
        }

        public TableDef getTableDefByName(String name)
        {
            return pathToTable.Values.AsEnumerable().Where(table => table.Name() == name).First();
        }

        public List<ColumnDef> findColumns(String query, TableDef table)
        {
            // Lista na kolumny
            List<ColumnDef> columns = new List<ColumnDef>();

            // Pobranie wszystkich kolumn zadanej tabeli oraz kolumn będących w relacji (tabele kluczy obcych) i dodanie do listy
            table.GetAllTablesReferencedByForeignKeys(true)
                .ForEach(tableDef => tableDef.ColumnHeader().ColumnNameToColumnDef().Values.ToList()
                .ForEach(column => columns.Add(column)));

            // Lista na wyniki
            List<Tuple<ColumnDef, double>> queryResult = new List<Tuple<ColumnDef, double>>();

            string queryLower = query.ToLower().Replace(" ", "");

            foreach (ColumnDef column in columns)
            {
                string columnLower = column.ColumnName().ToLower();
                List<double> similarities = new List<double>();

                // Określenie podobieństw
                foreach (string subQuery in queryLower.Split('.'))
                {
                    double similarity = 0.0;

                    // jeżeli zgodność 100% 2.0
                    if (subQuery == columnLower)
                    {
                        similarity = 2.0;
                        // jeżeli część słowa się zawiera <1.0, 2.0) 
                    }
                    else if (columnLower.Contains(subQuery))
                    {
                        similarity = 1.0 + ((double)subQuery.Length / (double)columnLower.Length);
                    }
                    // Podobieństwo Levenshteina <0, 1)
                    else
                    {
                        similarity = Utils.Similarity(subQuery, columnLower);
                    }

                    similarities.Add(similarity);
                }

                if (similarities.Count == 0)
                    continue;

                // Zwiększenie wagi ostatniego elementu
                similarities.Add(similarities.First());

                double meanSimilarity = similarities.Sum() / similarities.Count;

                queryResult.Add(Tuple.Create(column, meanSimilarity));
            }

            return queryResult.OrderByDescending(t => t.Item2).Select(t => t.Item1).ToList();
        }

        public Dictionary<string, TableDef> PathToTable() //geter
        {
            return this.pathToTable;
        }

    }
}