using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayModel : IDisposable
    {
        public const int SupportedMasterSchemaVersion = 1;
        public const int SupportedSaveSchemaVersion = 1;

        public string BaseContentPath { get; private set; }
        public string MasterDatabasePath { get; private set; }
        public string SqlCatalogPath { get; private set; }
        public string SaveDatabasePath { get; private set; }
        public SqliteConnection MasterConnection { get; private set; }
        public SqliteConnection SaveConnection { get; private set; }

        public void SetPaths(string streamingAssetsPath, string persistentDataPath)
        {
            BaseContentPath = Path.Combine(streamingAssetsPath, "Base");
            MasterDatabasePath = Path.Combine(BaseContentPath, "master.db");
            SqlCatalogPath = Path.Combine(streamingAssetsPath, "file", "sql");
            SaveDatabasePath = Path.Combine(persistentDataPath, "Database", "save.db");
        }

        public void OpenMasterConnection()
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = MasterDatabasePath,
                ReadOnly = true
            };
            MasterConnection = new SqliteConnection(builder.ToString());
            MasterConnection.Open();
        }

        public void OpenSaveConnection()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveDatabasePath));
            SaveConnection = new SqliteConnection($"URI=file:{SaveDatabasePath}");
            SaveConnection.Open();
        }

        public void CloseSaveConnection()
        {
            if (SaveConnection == null)
            {
                return;
            }

            SaveConnection.Dispose();
            SaveConnection = null;
        }

        public void EnsureInitialized()
        {
            if (MasterConnection == null || MasterConnection.State != ConnectionState.Open ||
                SaveConnection == null || SaveConnection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("DataGateway has not been initialized.");
            }
        }

        public void Dispose()
        {
            CloseSaveConnection();
            MasterConnection?.Dispose();
            MasterConnection = null;
        }
    }
}
