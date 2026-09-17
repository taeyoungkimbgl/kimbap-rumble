using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayServiceImpl : IDataGatewayService
    {
        readonly DataGatewayModel _model;
        readonly DataGatewaySqlReader _sqlReader;
        readonly DataGatewayReader _reader;
        readonly DataGatewayMasterMetadataReader _masterMetadataReader;
        readonly DataGatewayEquipmentReader _equipmentReader;
        readonly DataGatewayLocationReader _locationReader;
        readonly DataGatewayPhotoVariantReader _photoVariantReader;
        readonly DataGatewayPhotoLayerLayoutReader _photoLayerLayoutReader;
        readonly DataGatewaySaveMetadataReader _saveMetadataReader;
        readonly DataGatewayOwnedEquipmentReader _ownedEquipmentReader;
        readonly DataGatewayEquippedItemReader _equippedItemReader;
        readonly DataGatewayActiveRumbleReader _activeRumbleReader;
        readonly DataGatewayAlbumPhotoReader _albumPhotoReader;
        readonly DataGatewayDiscoveredLocationReader _discoveredLocationReader;
        readonly DataGatewayCoordinator _coordinator;

        public DataGatewayServiceImpl(
            DataGatewayModel model,
            DataGatewaySqlReader sqlReader,
            DataGatewayReader reader,
            DataGatewayMasterMetadataReader masterMetadataReader,
            DataGatewayEquipmentReader equipmentReader,
            DataGatewayLocationReader locationReader,
            DataGatewayPhotoVariantReader photoVariantReader,
            DataGatewayPhotoLayerLayoutReader photoLayerLayoutReader,
            DataGatewaySaveMetadataReader saveMetadataReader,
            DataGatewayOwnedEquipmentReader ownedEquipmentReader,
            DataGatewayEquippedItemReader equippedItemReader,
            DataGatewayActiveRumbleReader activeRumbleReader,
            DataGatewayAlbumPhotoReader albumPhotoReader,
            DataGatewayDiscoveredLocationReader discoveredLocationReader,
            DataGatewayCoordinator coordinator)
        {
            _model = model;
            _sqlReader = sqlReader;
            _reader = reader;
            _masterMetadataReader = masterMetadataReader;
            _equipmentReader = equipmentReader;
            _locationReader = locationReader;
            _photoVariantReader = photoVariantReader;
            _photoLayerLayoutReader = photoLayerLayoutReader;
            _saveMetadataReader = saveMetadataReader;
            _ownedEquipmentReader = ownedEquipmentReader;
            _equippedItemReader = equippedItemReader;
            _activeRumbleReader = activeRumbleReader;
            _albumPhotoReader = albumPhotoReader;
            _discoveredLocationReader = discoveredLocationReader;
            _coordinator = coordinator;
        }

        public void Initialize(string streamingAssetsPath, string persistentDataPath)
        {
            _model.SetPaths(streamingAssetsPath, persistentDataPath);
            MasterMetadataData masterMetadata;
            try
            {
                _model.OpenMasterConnection();
                ExecuteConnectionSql(_model.MasterConnection, "enable_foreign_keys");
                masterMetadata = ValidateMaster();
            }
            catch (Exception exception)
            {
                throw new InvalidDataException(
                    $"master.db initialization failed: {_model.MasterDatabasePath}", exception);
            }

            var saveExists = File.Exists(_model.SaveDatabasePath);
            if (!saveExists)
            {
                OpenSaveConnection();
                _coordinator.CreateSaveDatabase(masterMetadata, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                ValidateSaveReferences();
            }
            else
            {
                InitializeExistingSave(masterMetadata);
            }

            _model.EnsureInitialized();
        }

        public MasterMetadataData ReadMasterMetadata() => _masterMetadataReader.Read();
        public IReadOnlyList<EquipmentData> ReadEquipment() => _equipmentReader.ReadAll();
        public EquipmentData ReadEquipment(string equipmentId) => _equipmentReader.ReadById(equipmentId);
        public IReadOnlyList<LocationData> ReadLocations() => _locationReader.ReadAll();
        public LocationData ReadLocation(string locationId) => _locationReader.ReadById(locationId);
        public IReadOnlyList<PhotoVariantData> ReadPhotoVariants() => _photoVariantReader.ReadAll();
        public IReadOnlyList<PhotoVariantData> ReadPhotoVariants(string locationId) => _photoVariantReader.ReadByLocationId(locationId);
        public IReadOnlyList<PhotoLayerLayoutData> ReadPhotoLayerLayouts() => _photoLayerLayoutReader.ReadAll();
        public IReadOnlyList<PhotoLayerLayoutData> ReadPhotoLayerLayouts(string photoVariantId) => _photoLayerLayoutReader.ReadByPhotoVariantId(photoVariantId);
        public SaveMetadataData ReadSaveMetadata() => _saveMetadataReader.Read();
        public IReadOnlyList<OwnedEquipmentData> ReadOwnedEquipment() => _ownedEquipmentReader.ReadAll();
        public IReadOnlyList<EquippedItemData> ReadEquippedItems() => _equippedItemReader.ReadAll();
        public ActiveRumbleData ReadActiveRumble() => _activeRumbleReader.Read();
        public IReadOnlyList<AlbumPhotoData> ReadAlbumPhotos() => _albumPhotoReader.ReadAll();
        public IReadOnlyList<DiscoveredLocationData> ReadDiscoveredLocations() => _discoveredLocationReader.ReadAll();
        public void SaveEquippedItem(EquippedItemData item, long logicalUtcSeconds) => _coordinator.SaveEquippedItem(item, logicalUtcSeconds);
        public void SaveActiveRumble(ActiveRumbleData rumble, long logicalUtcSeconds) => _coordinator.SaveActiveRumble(rumble, logicalUtcSeconds);
        public long SaveRumbleResult(RumbleResultSaveData result) => _coordinator.SaveRumbleResult(result);
        public void DeleteAlbumPhoto(long photoId, long logicalUtcSeconds) => _coordinator.DeleteAlbumPhoto(photoId, logicalUtcSeconds);
        public void SaveLogicalUtc(long logicalUtcSeconds, long updatedUtcSeconds) => _coordinator.SaveLogicalUtc(logicalUtcSeconds, updatedUtcSeconds);

        public byte[] ReadImageBytes(string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
            {
                throw new InvalidDataException($"Image path must be relative: {relativePath}");
            }

            var root = Path.GetFullPath(_model.BaseContentPath) + Path.DirectorySeparatorChar;
            var filePath = Path.GetFullPath(Path.Combine(root, relativePath));
            if (!filePath.StartsWith(root, StringComparison.Ordinal))
            {
                throw new InvalidDataException($"Image path escapes Base Content Pack: {relativePath}");
            }
            return File.ReadAllBytes(filePath);
        }

        public void Dispose()
        {
            _model.Dispose();
        }

        void InitializeExistingSave(MasterMetadataData masterMetadata)
        {
            SaveMetadataData validatedMetadata;
            try
            {
                OpenSaveConnection();
                var saveMetadata = _saveMetadataReader.Read();
                if (saveMetadata.SchemaVersion > DataGatewayModel.SupportedSaveSchemaVersion)
                {
                    throw new NotSupportedException(
                        $"Save schema is newer than this application: {saveMetadata.SchemaVersion}");
                }
                if (saveMetadata.SchemaVersion < DataGatewayModel.SupportedSaveSchemaVersion)
                {
                    _coordinator.Migrate(saveMetadata.SchemaVersion);
                    saveMetadata = _saveMetadataReader.Read();
                }
                if (saveMetadata.SchemaVersion != DataGatewayModel.SupportedSaveSchemaVersion)
                {
                    throw new InvalidDataException(
                        $"Save migration did not reach schema version {DataGatewayModel.SupportedSaveSchemaVersion}.");
                }
                ValidateSaveReferences();
                validatedMetadata = saveMetadata;
            }
            catch (NotSupportedException)
            {
                _model.CloseSaveConnection();
                throw;
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError($"save.db could not be read and will be recreated: {exception}");
                _model.CloseSaveConnection();
                File.Delete(_model.SaveDatabasePath);
                OpenSaveConnection();
                _coordinator.CreateSaveDatabase(masterMetadata, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                ValidateSaveReferences();
                validatedMetadata = _saveMetadataReader.Read();
            }

            if (validatedMetadata.BaseContentVersion != masterMetadata.ContentVersion)
            {
                _coordinator.SaveBaseContentVersion(masterMetadata.ContentVersion);
            }
        }

        MasterMetadataData ValidateMaster()
        {
            var masterMetadata = _masterMetadataReader.Read();
            if (masterMetadata.PackId != "base")
            {
                throw new InvalidDataException($"Unsupported master pack ID: {masterMetadata.PackId}");
            }
            if (masterMetadata.SchemaVersion != DataGatewayModel.SupportedMasterSchemaVersion)
            {
                throw new NotSupportedException(
                    $"Unsupported master schema version: {masterMetadata.SchemaVersion}");
            }

            _equipmentReader.ReadAll();
            _locationReader.ReadAll();
            _photoVariantReader.ReadAll();
            _photoLayerLayoutReader.ReadAll();
            if (_reader.ReadMaster("connection_sql.xml", "foreign_key_check").Count != 0)
            {
                throw new InvalidDataException("master.db contains invalid foreign key references.");
            }
            return masterMetadata;
        }

        void ValidateSaveReferences()
        {
            var equipmentIds = new HashSet<string>(_equipmentReader.ReadAll().Select(value => value.EquipmentId));
            var locationIds = new HashSet<string>(_locationReader.ReadAll().Select(value => value.LocationId));
            var photoVariantIds = new HashSet<string>(_photoVariantReader.ReadAll().Select(value => value.PhotoVariantId));

            foreach (var value in _ownedEquipmentReader.ReadAll())
            {
                RequireReference(equipmentIds, value.EquipmentId, "owned_equipment.equipment_id");
            }
            foreach (var value in _equippedItemReader.ReadAll())
            {
                RequireReference(equipmentIds, value.EquipmentId, "equipped_items.equipment_id");
            }
            foreach (var value in _albumPhotoReader.ReadAll())
            {
                RequireReference(equipmentIds, value.WeaponId, "album_photos.weapon_id");
                RequireReference(equipmentIds, value.ShieldId, "album_photos.shield_id");
                RequireReference(equipmentIds, value.AccessoryId, "album_photos.accessory_id");
                RequireReference(locationIds, value.LocationId, "album_photos.location_id");
                RequireReference(photoVariantIds, value.PhotoVariantId, "album_photos.photo_variant_id");
            }
            foreach (var value in _discoveredLocationReader.ReadAll())
            {
                RequireReference(locationIds, value.LocationId, "discovered_locations.location_id");
            }

            var rumble = _activeRumbleReader.Read();
            if (rumble == null)
            {
                return;
            }
            RequireReference(equipmentIds, rumble.WeaponId, "active_rumble.weapon_id");
            RequireReference(equipmentIds, rumble.ShieldId, "active_rumble.shield_id");
            RequireReference(equipmentIds, rumble.AccessoryId, "active_rumble.accessory_id");
            RequireReference(locationIds, rumble.LocationId, "active_rumble.location_id");
            RequireReference(photoVariantIds, rumble.PhotoVariantId, "active_rumble.photo_variant_id");
            if (rumble.RewardEquipmentId != null)
            {
                RequireReference(equipmentIds, rumble.RewardEquipmentId, "active_rumble.reward_equipment_id");
            }
        }

        static void RequireReference(ISet<string> ids, string value, string column)
        {
            if (!ids.Contains(value))
            {
                throw new InvalidDataException($"Unknown master ID in {column}: {value}");
            }
        }

        void ExecuteConnectionSql(SqliteConnection connection, string sqlId)
        {
            using var command = connection.CreateCommand();
            command.CommandText = _sqlReader.ReadXml("connection_sql.xml", "update", sqlId);
            command.ExecuteNonQuery();
        }

        void OpenSaveConnection()
        {
            _model.OpenSaveConnection();
            ExecuteConnectionSql(_model.SaveConnection, "enable_foreign_keys");
        }
    }
}
