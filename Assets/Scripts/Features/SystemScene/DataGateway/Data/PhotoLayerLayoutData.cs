namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class PhotoLayerLayoutData
    {
        public string PhotoVariantId { get; }
        public string LayerType { get; }
        public double PositionX { get; }
        public double PositionY { get; }
        public double RotationDegrees { get; }
        public double ScaleX { get; }
        public double ScaleY { get; }
        public int SortingOrder { get; }

        public PhotoLayerLayoutData(
            string photoVariantId,
            string layerType,
            double positionX,
            double positionY,
            double rotationDegrees,
            double scaleX,
            double scaleY,
            int sortingOrder)
        {
            PhotoVariantId = photoVariantId;
            LayerType = layerType;
            PositionX = positionX;
            PositionY = positionY;
            RotationDegrees = rotationDegrees;
            ScaleX = scaleX;
            ScaleY = scaleY;
            SortingOrder = sortingOrder;
        }
    }
}
