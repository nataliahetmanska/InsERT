using System.Collections.Generic;

namespace CSV_program
{
    /// <summary>
    /// Klasa TableDef zawiera informacje o tabeli
    /// </summary>
    class TableDef
    {
        private string scheme; //nazwa schematu
        private string name; // nazwa tabeli
        private string path; // nazwa ścieżki
        private HeaderDef columnHeader; // obiekt typu HeaderDef zawierający informacje o kolumnach

        // konstruktor
        public TableDef(string scheme, string name, string path, HeaderDef columnHeader)
        {
            this.scheme = scheme;
            this.name = name;
            this.path = path;
            this.columnHeader = columnHeader;
        }

        public List<TableDef> GetAllTablesReferencedByForeignKeys(bool includeSelf = false)
        {
            List<TableDef> referencedTables = new List<TableDef>();

            if (includeSelf)
                referencedTables.Add(this);

            foreach (ColumnDef column in columnHeader.ColumnNameToColumnDef().Values)
                if (column.references != null)
                    referencedTables.Add(column.references.tableDef);

            return referencedTables;
        }

        // gettery
        public string Scheme()
        {
            return this.scheme;
        }

        public string Name()
        {
            return this.name;
        }

        public HeaderDef ColumnHeader()
        {
            return this.columnHeader;
        }

        public string Path()
        {
            return this.path;
        }

        // nadpisanie toString aby w rezultacie otrzymać name
        public override string ToString()
        {
            return name;
        }
    }
}