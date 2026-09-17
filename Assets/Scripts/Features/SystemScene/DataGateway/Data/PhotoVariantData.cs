namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class PhotoVariantData
    {
        public string PhotoVariantId { get; }
        public string LocationId { get; }
        public int FragmentIndex { get; }
        public string BackgroundImagePath { get; }
        public string CharacterImagePath { get; }
        public int SortOrder { get; }

        public PhotoVariantData(
            string photoVariantId,
            string locationId,
            int fragmentIndex,
            string backgroundImagePath,
            string characterImagePath,
            int sortOrder)
        {
            PhotoVariantId = photoVariantId;
            LocationId = locationId;
            FragmentIndex = fragmentIndex;
            BackgroundImagePath = backgroundImagePath;
            CharacterImagePath = characterImagePath;
            SortOrder = sortOrder;
        }
    }
}
