namespace CSV_program
{
    /// <summary>
    /// Klasa TableDef zawiera informacje o tabeli
    /// </summary>
    class TableDef
    {
        private string scheme;
        private string name;
        private string path;
        private HeaderDef columnHeader;


        public TableDef(string scheme, string name, string path, HeaderDef columnHeader)
        {
            this.scheme = scheme;
            this.name = name;
            this.path = path;
            this.columnHeader = columnHeader;

        }

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
        public override string ToString()
        {
            return name;
        }
    }

}