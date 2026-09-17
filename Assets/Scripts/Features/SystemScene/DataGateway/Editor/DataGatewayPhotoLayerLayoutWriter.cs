using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayPhotoLayerLayoutWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayPhotoLayerLayoutWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(PhotoLayerLayoutData data, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "photo_layer_layouts_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@photo_variant_id"] = data.PhotoVariantId,
                ["@layer_type"] = data.LayerType,
                ["@position_x"] = data.PositionX,
                ["@position_y"] = data.PositionY,
                ["@rotation_degrees"] = data.RotationDegrees,
                ["@scale_x"] = data.ScaleX,
                ["@scale_y"] = data.ScaleY,
                ["@sorting_order"] = data.SortingOrder
            }, transaction);
        }

        public void Delete(string photoVariantId, string layerType, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "photo_layer_layouts_sql.xml", "delete", "delete_by_id", new Dictionary<string, object>
            {
                ["@photo_variant_id"] = photoVariantId,
                ["@layer_type"] = layerType
            }, transaction);
        }
    }
}
