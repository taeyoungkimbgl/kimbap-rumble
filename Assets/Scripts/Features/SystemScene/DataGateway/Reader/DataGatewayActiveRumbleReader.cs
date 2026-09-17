using System;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayActiveRumbleReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayActiveRumbleReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public ActiveRumbleData Read()
        {
            var row = _reader.ReadSave("active_rumble_sql.xml", "select_all").SingleOrDefault();
            if (row == null)
            {
                return null;
            }

            return new ActiveRumbleData(
                Convert.ToInt64(row["started_utc_seconds"]),
                Convert.ToInt64(row["completes_utc_seconds"]),
                Convert.ToString(row["weapon_id"]),
                Convert.ToString(row["shield_id"]),
                Convert.ToString(row["accessory_id"]),
                Convert.ToString(row["location_id"]),
                Convert.ToString(row["photo_variant_id"]),
                row["reward_equipment_id"] == DBNull.Value ? null : Convert.ToString(row["reward_equipment_id"]));
        }
    }
}
