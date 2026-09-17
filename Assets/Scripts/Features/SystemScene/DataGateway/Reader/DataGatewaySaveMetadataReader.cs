using System;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewaySaveMetadataReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewaySaveMetadataReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public SaveMetadataData Read()
        {
            var row = _reader.ReadSave("save_metadata_sql.xml", "select_all").Single();
            return new SaveMetadataData(
                Convert.ToInt32(row["schema_version"]),
                Convert.ToInt32(row["base_content_version"]),
                Convert.ToInt64(row["created_utc_seconds"]),
                Convert.ToInt64(row["updated_utc_seconds"]),
                Convert.ToInt64(row["logical_utc_seconds"]));
        }
    }
}
