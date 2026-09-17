namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class AlbumPhotoData
    {
        public long PhotoId { get; }
        public long AcquiredUtcSeconds { get; }
        public long ExpiresUtcSeconds { get; }
        public string WeaponId { get; }
        public string ShieldId { get; }
        public string AccessoryId { get; }
        public string LocationId { get; }
        public string PhotoVariantId { get; }

        public AlbumPhotoData(
            long photoId,
            long acquiredUtcSeconds,
            long expiresUtcSeconds,
            string weaponId,
            string shieldId,
            string accessoryId,
            string locationId,
            string photoVariantId)
        {
            PhotoId = photoId;
            AcquiredUtcSeconds = acquiredUtcSeconds;
            ExpiresUtcSeconds = expiresUtcSeconds;
            WeaponId = weaponId;
            ShieldId = shieldId;
            AccessoryId = accessoryId;
            LocationId = locationId;
            PhotoVariantId = photoVariantId;
        }
    }
}
