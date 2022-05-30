using System;
using System.Collections.Generic;
using System.Linq;

namespace CSV_program
{
    /// <summary>
    /// klasa ColumnDef zawiera pola dotyczące kolumn - atrybutów 
    /// </summary>
    class ColumnDef
    {
        private string columnName; // pole nazwa kolumny
        private DataType type; // pole typ kolumny
        private bool nullable; // pole przyjmujące wartość boolowską wskazującą czy wartością w kolumnie może być nullem
        private int maxLength; // pole zawierające max długość jeśli typem jest nvarchar
        public TableDef tableDef = null; // pole typu TableDef (potrzebne do podobieństwa nazw)
        public ColumnDef references = null;
        public List<ColumnDef> referencedIn = new List<ColumnDef>();

        // konstruktor
        public ColumnDef(string columnName, DataType type, bool nullable, int maxLength)
        {
            this.type = type;
            this.columnName = columnName;
            this.nullable = nullable;
            this.maxLength = maxLength;
        }

        //gettery
        public string ColumnName()
        {
            return this.columnName;
        }

        public DataType Type()
        {
            return this.type;
        }

        public bool Nullable()
        {
            return this.nullable;
        }

        public int MaxLength()
        {
            return this.maxLength;
        }

        // nadpisanie potrzebne do wykrywania nazw nazwatabeli . nazwakolumny
        public override string ToString()
        {
            if (this.tableDef == null)
                return columnName;

            return tableDef.ToString() + "." + columnName;
        }
    }

}