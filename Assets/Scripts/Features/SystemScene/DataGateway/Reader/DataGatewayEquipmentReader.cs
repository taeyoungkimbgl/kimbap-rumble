using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayEquipmentReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayEquipmentReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<EquipmentData> ReadAll()
        {
            return _reader.ReadMaster("equipment_sql.xml", "select_all").Select(Map).ToList();
        }

        public IReadOnlyList<EquipmentData> ReadInitialOwned()
        {
            return _reader.ReadMaster("equipment_sql.xml", "select_initial_owned").Select(Map).ToList();
        }

        public EquipmentData ReadById(string equipmentId)
        {
            var parameters = new Dictionary<string, object> { ["@equipment_id"] = equipmentId };
            return _reader.ReadMaster("equipment_sql.xml", "select_by_id", parameters).Select(Map).Single();
        }

        static EquipmentData Map(IReadOnlyDictionary<string, object> row)
        {
            return new EquipmentData(
                Convert.ToString(row["equipment_id"]),
                Convert.ToString(row["slot_type"]),
                Convert.ToString(row["name_key"]),
                Convert.ToString(row["home_image_path"]),
                Convert.ToString(row["photo_image_path"]),
                Convert.ToString(row["list_icon_image_path"]),
                Convert.ToString(row["detail_icon_image_path"]),
                Convert.ToInt32(row["is_initial_owned"]) == 1,
                Convert.ToInt32(row["sort_order"]));
        }
    }
}
