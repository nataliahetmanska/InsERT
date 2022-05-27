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
        private string columnName;
        private string type;
        private bool nullable;
        private int? maxLength;
        public TableDef tableDef = null;

        public ColumnDef(string columnName, string type, bool nullable, int? maxLength)
        {
            this.type = type;
            this.columnName = columnName;
            this.nullable = nullable;
            this.maxLength = maxLength;
        }

        public string ColumnName()
        {
            return this.columnName;
        }

        public string Type()
        {
            return this.type;
        }

        public bool Nullable()
        {
            return this.nullable;
        }

        public int? MaxLength()
        {
            return this.maxLength;
        }
        public override string ToString()
        {
            if (this.tableDef == null)
                return columnName;

            return tableDef.ToString() + "." + columnName;
        }
    }

}