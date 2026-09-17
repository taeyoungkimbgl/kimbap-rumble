using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayPhotoVariantWriter
    {
        readonly DataGatewayWriter _writer;

        public DataGatewayPhotoVariantWriter(DataGatewayWriter writer)
        {
            _writer = writer;
        }

        public void Upsert(PhotoVariantData data, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "photo_variants_sql.xml", "insert", "upsert", new Dictionary<string, object>
            {
                ["@photo_variant_id"] = data.PhotoVariantId,
                ["@location_id"] = data.LocationId,
                ["@fragment_index"] = data.FragmentIndex,
                ["@background_image_path"] = data.BackgroundImagePath,
                ["@character_image_path"] = data.CharacterImagePath,
                ["@sort_order"] = data.SortOrder
            }, transaction);
        }

        public void Delete(string photoVariantId, SqliteConnection connection, SqliteTransaction transaction)
        {
            _writer.Write(connection, "photo_variants_sql.xml", "delete", "delete_by_id", new Dictionary<string, object>
            {
                ["@photo_variant_id"] = photoVariantId
            }, transaction);
        }
    }
}
