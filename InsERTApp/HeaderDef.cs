using System;
using System.Collections.Generic;

namespace CSV_program
{
    /// <summary>
    /// klasa HeaderDef zawiera nazwę kolumny i wszystkie informacje o niej
    /// </summary>
    class HeaderDef
    {
        // string - nazwa kolumny, ColumnDef - obiekt zawierający wszystkie potrzebne o niej informacje
        private Dictionary<string, ColumnDef> columnNameToColumnDef;

        public HeaderDef(Dictionary<string, ColumnDef> columnNameToColumnDef) //konstruktor
        {
            this.columnNameToColumnDef = columnNameToColumnDef;
        }

        public Dictionary<string, ColumnDef> ColumnNameToColumnDef() //getter
        {
            return this.columnNameToColumnDef;
        }
    }
}