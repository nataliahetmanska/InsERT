using System;
using System.Collections.Generic;

namespace CSV_program
{
    /// <summary>
    /// klasa HeaderDef zawiera nazwę kolumny i wszystkie informacje o niej
    /// </summary>
    class HeaderDef
    {
        private Dictionary<string, ColumnDef> columnNameToColumnDef;

        public HeaderDef(Dictionary<string, ColumnDef> columnNameToColumnDef) //konstruktor
        {
            this.columnNameToColumnDef = columnNameToColumnDef;
        }

        public Dictionary<string, ColumnDef> ColumnNameToColumnDef() //geter
        {
            return this.columnNameToColumnDef;
        }
    }
}