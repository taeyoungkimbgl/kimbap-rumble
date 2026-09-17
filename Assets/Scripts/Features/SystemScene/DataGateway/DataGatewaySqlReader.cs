using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewaySqlReader
    {
        readonly DataGatewayModel _model;

        public DataGatewaySqlReader(DataGatewayModel model)
        {
            _model = model;
        }

        public string ReadXml(string fileName, string operation, string id)
        {
            var document = XDocument.Load(Path.Combine(_model.SqlCatalogPath, fileName));
            var element = document.Root.Elements(operation)
                .Single(value => string.Equals((string)value.Attribute("id"), id, StringComparison.Ordinal));
            return element.Value.Trim();
        }

        public string ReadFile(string fileName)
        {
            return File.ReadAllText(Path.Combine(_model.SqlCatalogPath, fileName));
        }

        public string FindMigrationFileName(int version)
        {
            return Directory.GetFiles(_model.SqlCatalogPath, $"v{version:000}_*.sql")
                .Select(Path.GetFileName)
                .Single();
        }
    }
}
