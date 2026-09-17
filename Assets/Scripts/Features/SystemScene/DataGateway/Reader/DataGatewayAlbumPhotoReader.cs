using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayAlbumPhotoReader
    {
        readonly DataGatewayReader _reader;

        public DataGatewayAlbumPhotoReader(DataGatewayReader reader)
        {
            _reader = reader;
        }

        public IReadOnlyList<AlbumPhotoData> ReadAll()
        {
            return _reader.ReadSave("album_photos_sql.xml", "select_all")
                .Select(row => new AlbumPhotoData(
                    Convert.ToInt64(row["photo_id"]),
                    Convert.ToInt64(row["acquired_utc_seconds"]),
                    Convert.ToInt64(row["expires_utc_seconds"]),
                    Convert.ToString(row["weapon_id"]),
                    Convert.ToString(row["shield_id"]),
                    Convert.ToString(row["accessory_id"]),
                    Convert.ToString(row["location_id"]),
                    Convert.ToString(row["photo_variant_id"])))
                .ToList();
        }
    }
}
