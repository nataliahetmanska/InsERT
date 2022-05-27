using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSV_program
{
    using System.Xml.Linq;

    class XMLLoader
    {
        private static HashSet<String> loadedTypes = new HashSet<String>();


        private static HeaderDef GetHeaderDef(string tableName, XDocument doc, XNamespace ns)
        {
            List<XElement> columnNodes = doc.Descendants(ns + "EntityType")
                .Where(e => e.Attribute("Name").Value == tableName).First()
                .Descendants(ns + "Property").ToList();

            Dictionary<string, ColumnDef> columnNameToColumnDef = new Dictionary<string, ColumnDef>();

            foreach (XElement columnNode in columnNodes)
            {
                string columnName = columnNode.Attribute("Name").Value;
                bool nullable = Boolean.Parse(columnNode.Attribute("Nullable").Value);
                string rawType = columnNode.Attribute("Type").Value;
                loadedTypes.Add(rawType);
                int? maxLength = null;
                if (columnNode.Attribute("MaxLength") != null)
                    maxLength = int.Parse(columnNode.Attribute("MaxLength").Value);

                columnNameToColumnDef.Add(columnName, new ColumnDef(columnName, rawType, nullable, maxLength));
            }

            return new HeaderDef(columnNameToColumnDef);
        }

        public static ModelDef LoadModelFromXml(string source)
        {
            XDocument doc = XDocument.Load(source);
            XNamespace ns = doc.Root.Name.Namespace;

            var entitySets = doc.Descendants(ns + "EntityContainer")
                .Descendants(ns + "EntitySet").ToList();

            ModelDef model = new ModelDef();

            foreach (XElement node in entitySets)
            {
                string name = node.Attribute("Name").Value;
                string schema = node.Attribute("Schema")?.Value;
                string path = node.Attribute("EntityType").Value;
                HeaderDef header = GetHeaderDef(name, doc, ns);
                TableDef table = new TableDef(schema, name, path, header);

                header.ColumnNameToColumnDef().Values.ToList().ForEach(column => column.tableDef = table);

                model.PathToTable().Add(path, table);
            }
            /*
            foreach (String type in loadedTypes)
            {
                Console.WriteLine(type);
            }
            */
            return model;
        }
    }
}