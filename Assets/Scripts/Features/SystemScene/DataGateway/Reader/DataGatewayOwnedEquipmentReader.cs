using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayOwnedEquipmentReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayOwnedEquipmentReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<OwnedEquipmentData> ReadAll()
        {
            return _reader.ReadSave("owned_equipment_sql.xml", "select_all")
                .Select(row => new OwnedEquipmentData(
                    Convert.ToString(row["equipment_id"]),
                    Convert.ToInt64(row["acquired_utc_seconds"])))
                .ToList();
        }
    }
}
