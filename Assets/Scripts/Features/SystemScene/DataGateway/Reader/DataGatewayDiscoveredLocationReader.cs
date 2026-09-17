using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayDiscoveredLocationReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayDiscoveredLocationReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<DiscoveredLocationData> ReadAll()
        {
            return _reader.ReadSave("discovered_locations_sql.xml", "select_all")
                .Select(row => new DiscoveredLocationData(
                    Convert.ToString(row["location_id"]),
                    Convert.ToInt64(row["first_discovered_utc_seconds"])))
                .ToList();
        }
    }
}
