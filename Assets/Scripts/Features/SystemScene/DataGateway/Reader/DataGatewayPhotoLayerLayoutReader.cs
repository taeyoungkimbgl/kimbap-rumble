using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayPhotoLayerLayoutReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayPhotoLayerLayoutReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<PhotoLayerLayoutData> ReadAll()
        {
            return _reader.ReadMaster("photo_layer_layouts_sql.xml", "select_all").Select(Map).ToList();
        }

        public IReadOnlyList<PhotoLayerLayoutData> ReadByPhotoVariantId(string photoVariantId)
        {
            var parameters = new Dictionary<string, object> { ["@photo_variant_id"] = photoVariantId };
            return _reader.ReadMaster("photo_layer_layouts_sql.xml", "select_by_photo_variant_id", parameters).Select(Map).ToList();
        }

        static PhotoLayerLayoutData Map(IReadOnlyDictionary<string, object> row)
        {
            return new PhotoLayerLayoutData(
                Convert.ToString(row["photo_variant_id"]),
                Convert.ToString(row["layer_type"]),
                Convert.ToDouble(row["position_x"]),
                Convert.ToDouble(row["position_y"]),
                Convert.ToDouble(row["rotation_degrees"]),
                Convert.ToDouble(row["scale_x"]),
                Convert.ToDouble(row["scale_y"]),
                Convert.ToInt32(row["sorting_order"]));
        }
    }
}
