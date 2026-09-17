using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayLocationReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayLocationReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<LocationData> ReadAll()
        {
            return _reader.ReadMaster("locations_sql.xml", "select_all").Select(Map).ToList();
        }

        public LocationData ReadById(string locationId)
        {
            var parameters = new Dictionary<string, object> { ["@location_id"] = locationId };
            return _reader.ReadMaster("locations_sql.xml", "select_by_id", parameters).Select(Map).Single();
        }

        static LocationData Map(IReadOnlyDictionary<string, object> row)
        {
            return new LocationData(
                Convert.ToString(row["location_id"]),
                Convert.ToString(row["internal_name"]),
                Convert.ToInt32(row["sort_order"]));
        }
    }
}
