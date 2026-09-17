using System;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayMasterMetadataReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayMasterMetadataReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public MasterMetadataData Read()
        {
            var row = _reader.ReadMaster("master_metadata_sql.xml", "select_all").Single();
            return new MasterMetadataData(
                Convert.ToString(row["pack_id"]),
                Convert.ToInt32(row["schema_version"]),
                Convert.ToInt32(row["content_version"]));
        }
    }
}
