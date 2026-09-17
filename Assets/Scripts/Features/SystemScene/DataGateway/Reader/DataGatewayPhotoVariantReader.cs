using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayPhotoVariantReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayPhotoVariantReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<PhotoVariantData> ReadAll()
        {
            return _reader.ReadMaster("photo_variants_sql.xml", "select_all").Select(Map).ToList();
        }

        public IReadOnlyList<PhotoVariantData> ReadByLocationId(string locationId)
        {
            var parameters = new Dictionary<string, object> { ["@location_id"] = locationId };
            return _reader.ReadMaster("photo_variants_sql.xml", "select_by_location_id", parameters).Select(Map).ToList();
        }

        static PhotoVariantData Map(IReadOnlyDictionary<string, object> row)
        {
            return new PhotoVariantData(
                Convert.ToString(row["photo_variant_id"]),
                Convert.ToString(row["location_id"]),
                Convert.ToInt32(row["fragment_index"]),
                Convert.ToString(row["background_image_path"]),
                Convert.ToString(row["character_image_path"]),
                Convert.ToInt32(row["sort_order"]));
        }
    }
}
