using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayEquipmentWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayEquipmentWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(EquipmentData data, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "equipment_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@equipment_id"] = data.EquipmentId,
                ["@slot_type"] = data.SlotType,
                ["@name_key"] = data.NameKey,
                ["@home_image_path"] = data.HomeImagePath,
                ["@photo_image_path"] = data.PhotoImagePath,
                ["@list_icon_image_path"] = data.ListIconImagePath,
                ["@detail_icon_image_path"] = data.DetailIconImagePath,
                ["@is_initial_owned"] = data.IsInitialOwned ? 1 : 0,
                ["@sort_order"] = data.SortOrder
            }, transaction);
        }

        public void Delete(string equipmentId, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "equipment_sql.xml", "delete", "delete_by_id", new Dictionary<string, object>
            {
                ["@equipment_id"] = equipmentId
            }, transaction);
        }
    }
}
