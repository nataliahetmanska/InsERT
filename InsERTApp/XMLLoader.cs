using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSV_program
{
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
                int maxLength = -1;
                if (columnNode.Attribute("MaxLength") != null)
                    maxLength = int.Parse(columnNode.Attribute("MaxLength").Value);

                columnNameToColumnDef.Add(columnName, new ColumnDef(columnName, rawType, nullable, maxLength));
            }

            return new HeaderDef(columnNameToColumnDef);
        }

        private static Tuple<string, string> GetTablePathAndColumnNameFromAssociation(XElement association, XNamespace ns, string type)
        {
            return association.Descendants(ns + "ReferentialConstraint")
                        .Descendants(ns + type)
                        .Select(reference => {
                            string tablePath = association.Descendants(ns + "End")
                                .Where(e => e.Attribute("Role").Value == reference.Attribute("Role").Value)
                                .Select(e => e.Attribute("Type").Value).First();
                            string columnName = reference.Descendants(ns + "PropertyRef")
                                .Select(p => p.Attribute("Name").Value).First();

                            return Tuple.Create(tablePath, columnName);
                        }).First();
        }

        private static void loadRelations(ModelDef model, XDocument doc, XNamespace ns)
        {
            doc.Descendants(ns + "Association")
                .ToList().ForEach(association => {
                    Tuple<string, string> dependentData = GetTablePathAndColumnNameFromAssociation(association, ns, "Dependent");
                    Tuple<string, string> principalData = GetTablePathAndColumnNameFromAssociation(association, ns, "Principal");

                    ColumnDef dependentColumnDef = model.PathToTable()[dependentData.Item1]
                        .ColumnHeader().ColumnNameToColumnDef()[dependentData.Item2];
                    ColumnDef principalColumnDef = model.PathToTable()[principalData.Item1]
                        .ColumnHeader().ColumnNameToColumnDef()[principalData.Item2];

                    principalColumnDef.referencedIn.Add(dependentColumnDef);
                    dependentColumnDef.references = principalColumnDef;
                });
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

            loadRelations(model, doc, ns);

            return model;
        }
    }
}