using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayAlbumPhotoWriter
    {
        readonly DataGatewayWriter _writer;
        readonly DataGatewayReader _reader;

        public DataGatewayAlbumPhotoWriter(DataGatewayWriter writer, DataGatewayReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public long Insert(AlbumPhotoData data, SqliteTransaction transaction)
        {
            _writer.WriteSave("album_photos_sql.xml", "insert", "insert", new Dictionary<string, object>
            {
                ["@acquired_utc_seconds"] = data.AcquiredUtcSeconds,
                ["@expires_utc_seconds"] = data.ExpiresUtcSeconds,
                ["@weapon_id"] = data.WeaponId,
                ["@shield_id"] = data.ShieldId,
                ["@accessory_id"] = data.AccessoryId,
                ["@location_id"] = data.LocationId,
                ["@photo_variant_id"] = data.PhotoVariantId
            }, transaction);
            return _reader.ReadSaveScalarLong("connection_sql.xml", "last_insert_id", transaction);
        }

        public void Delete(long photoId, SqliteTransaction transaction)
        {
            _writer.WriteSave("album_photos_sql.xml", "delete", "delete_by_id", new Dictionary<string, object>
            {
                ["@photo_id"] = photoId
            }, transaction);
        }

        public void DeleteExpired(long logicalUtcSeconds, SqliteTransaction transaction)
        {
            _writer.WriteSave("album_photos_sql.xml", "delete", "delete_expired", new Dictionary<string, object>
            {
                ["@logical_utc_seconds"] = logicalUtcSeconds
            }, transaction);
        }

        public void TrimToMaximum(int maximumCount, SqliteTransaction transaction)
        {
            _writer.WriteSave("album_photos_sql.xml", "delete", "trim_to_maximum", new Dictionary<string, object>
            {
                ["@maximum_count"] = maximumCount
            }, transaction);
        }
    }
}
