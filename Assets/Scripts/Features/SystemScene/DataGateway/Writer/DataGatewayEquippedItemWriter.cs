using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayEquippedItemWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayEquippedItemWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(EquippedItemData data, SqliteTransaction transaction)
        {
            _writer.WriteSave("equipped_items_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@slot_type"] = data.SlotType,
                ["@equipment_id"] = data.EquipmentId
            }, transaction);
        }
    }
}
