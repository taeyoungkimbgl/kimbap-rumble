using System;
using System.Linq;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayCoordinator
    {
        const int MaximumAlbumPhotos = 30;

        readonly DataGatewayModel _model;
        readonly DataGatewayWriter _writer;
        readonly DataGatewayEquipmentReader _equipmentReader;
        readonly DataGatewaySaveMetadataWriter _saveMetadataWriter;
        readonly DataGatewayOwnedEquipmentWriter _ownedEquipmentWriter;
        readonly DataGatewayEquippedItemWriter _equippedItemWriter;
        readonly DataGatewayActiveRumbleWriter _activeRumbleWriter;
        readonly DataGatewayAlbumPhotoWriter _albumPhotoWriter;
        readonly DataGatewayDiscoveredLocationWriter _discoveredLocationWriter;

        public DataGatewayCoordinator(
            DataGatewayModel model,
            DataGatewayWriter writer,
            DataGatewayEquipmentReader equipmentReader,
            DataGatewaySaveMetadataWriter saveMetadataWriter,
            DataGatewayOwnedEquipmentWriter ownedEquipmentWriter,
            DataGatewayEquippedItemWriter equippedItemWriter,
            DataGatewayActiveRumbleWriter activeRumbleWriter,
            DataGatewayAlbumPhotoWriter albumPhotoWriter,
            DataGatewayDiscoveredLocationWriter discoveredLocationWriter)
        {
            _model = model;
            _writer = writer;
            _equipmentReader = equipmentReader;
            _saveMetadataWriter = saveMetadataWriter;
            _ownedEquipmentWriter = ownedEquipmentWriter;
            _equippedItemWriter = equippedItemWriter;
            _activeRumbleWriter = activeRumbleWriter;
            _albumPhotoWriter = albumPhotoWriter;
            _discoveredLocationWriter = discoveredLocationWriter;
        }

        public void CreateSaveDatabase(MasterMetadataData masterMetadata, long utcSeconds)
        {
            var initialEquipment = _equipmentReader.ReadInitialOwned();

            Execute(transaction =>
            {
                _writer.ExecuteScript("v001_create_save.sql", transaction);
                for (var version = 2;
                     version <= DataGatewayModel.SupportedSaveSchemaVersion;
                     version++)
                {
                    _writer.ExecuteMigration(version, transaction);
                }
                _saveMetadataWriter.Insert(new SaveMetadataData(
                    DataGatewayModel.SupportedSaveSchemaVersion,
                    masterMetadata.ContentVersion,
                    utcSeconds,
                    utcSeconds,
                    utcSeconds), transaction);

                foreach (var equipment in initialEquipment)
                {
                    _ownedEquipmentWriter.Insert(
                        new OwnedEquipmentData(equipment.EquipmentId, utcSeconds), transaction);
                }

                foreach (var slot in new[] { "weapon", "shield", "accessory" })
                {
                    var initial = initialEquipment.Single(value => value.SlotType == slot);
                    _equippedItemWriter.Upsert(
                        new EquippedItemData(slot, initial.EquipmentId), transaction);
                }
            });
        }

        public void Migrate(int currentVersion)
        {
            Execute(transaction =>
            {
                for (var version = currentVersion + 1;
                     version <= DataGatewayModel.SupportedSaveSchemaVersion;
                     version++)
                {
                    _writer.ExecuteMigration(version, transaction);
                }
            });
        }

        public void SaveEquippedItem(EquippedItemData item, long logicalUtcSeconds)
        {
            Execute(transaction =>
            {
                _equippedItemWriter.Upsert(item, transaction);
                _saveMetadataWriter.UpdateTime(
                    logicalUtcSeconds,
                    logicalUtcSeconds,
                    transaction);
            });
        }

        public void SaveActiveRumble(ActiveRumbleData rumble, long logicalUtcSeconds)
        {
            Execute(transaction =>
            {
                _activeRumbleWriter.Upsert(rumble, transaction);
                _saveMetadataWriter.UpdateTime(
                    logicalUtcSeconds,
                    logicalUtcSeconds,
                    transaction);
            });
        }

        public long SaveRumbleResult(RumbleResultSaveData result)
        {
            var photoId = 0L;
            Execute(transaction =>
            {
                if (result.RewardEquipment != null)
                {
                    _ownedEquipmentWriter.Insert(result.RewardEquipment, transaction);
                }
                _albumPhotoWriter.DeleteExpired(result.Photo.AcquiredUtcSeconds, transaction);
                _albumPhotoWriter.TrimToMaximum(MaximumAlbumPhotos - 1, transaction);
                photoId = _albumPhotoWriter.Insert(result.Photo, transaction);
                _discoveredLocationWriter.Insert(result.DiscoveredLocation, transaction);
                _activeRumbleWriter.Delete(transaction);
                _saveMetadataWriter.UpdateTime(
                    result.LogicalUtcSeconds,
                    result.LogicalUtcSeconds,
                    transaction);
            });
            return photoId;
        }

        public void DeleteAlbumPhoto(long photoId, long logicalUtcSeconds)
        {
            Execute(transaction =>
            {
                _albumPhotoWriter.Delete(photoId, transaction);
                _saveMetadataWriter.UpdateTime(
                    logicalUtcSeconds,
                    logicalUtcSeconds,
                    transaction);
            });
        }

        public void SaveLogicalUtc(long logicalUtcSeconds, long updatedUtcSeconds)
        {
            Execute(transaction => _saveMetadataWriter.UpdateTime(
                logicalUtcSeconds,
                updatedUtcSeconds,
                transaction));
        }

        public void SaveBaseContentVersion(int contentVersion)
        {
            Execute(transaction => _saveMetadataWriter.UpdateBaseContentVersion(
                contentVersion,
                transaction));
        }

        void Execute(Action<SqliteTransaction> operation)
        {
            using var transaction = _model.SaveConnection.BeginTransaction();
            try
            {
                operation(transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
