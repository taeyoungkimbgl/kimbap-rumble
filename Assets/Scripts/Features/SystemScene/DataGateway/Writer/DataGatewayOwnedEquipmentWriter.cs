using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayOwnedEquipmentWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayOwnedEquipmentWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Insert(OwnedEquipmentData data, SqliteTransaction transaction)
        {
            _writer.WriteSave("owned_equipment_sql.xml", "insert", "insert_or_ignore", new Dictionary<string, object>
            {
                ["@equipment_id"] = data.EquipmentId,
                ["@acquired_utc_seconds"] = data.AcquiredUtcSeconds
            }, transaction);
        }
    }
}
