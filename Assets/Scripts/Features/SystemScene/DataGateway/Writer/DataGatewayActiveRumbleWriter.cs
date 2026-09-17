using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayActiveRumbleWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayActiveRumbleWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(ActiveRumbleData data, SqliteTransaction transaction)
        {
            _writer.WriteSave("active_rumble_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@started_utc_seconds"] = data.StartedUtcSeconds,
                ["@completes_utc_seconds"] = data.CompletesUtcSeconds,
                ["@weapon_id"] = data.WeaponId,
                ["@shield_id"] = data.ShieldId,
                ["@accessory_id"] = data.AccessoryId,
                ["@location_id"] = data.LocationId,
                ["@photo_variant_id"] = data.PhotoVariantId,
                ["@reward_equipment_id"] = data.RewardEquipmentId
            }, transaction);
        }

        public void Delete(SqliteTransaction transaction)
        {
            _writer.WriteSave("active_rumble_sql.xml", "delete", "delete", new Dictionary<string, object>(), transaction);
        }
    }
}
