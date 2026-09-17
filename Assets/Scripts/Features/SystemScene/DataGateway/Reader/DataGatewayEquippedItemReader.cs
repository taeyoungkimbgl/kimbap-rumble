using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayEquippedItemReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayEquippedItemReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<EquippedItemData> ReadAll()
        {
            return _reader.ReadSave("equipped_items_sql.xml", "select_all")
                .Select(row => new EquippedItemData(
                    Convert.ToString(row["slot_type"]),
                    Convert.ToString(row["equipment_id"])))
                .ToList();
        }
    }
}
