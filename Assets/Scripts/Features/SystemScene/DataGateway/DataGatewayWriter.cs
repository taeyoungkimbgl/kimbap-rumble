using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayWriter
    {
        readonly DataGatewayModel _model;
        readonly DataGatewaySqlReader _sqlReader;

        public DataGatewayWriter(DataGatewayModel model, DataGatewaySqlReader sqlReader)
        {
            _model = model;
            _sqlReader = sqlReader;
        }

        public int WriteSave(
            string fileName,
            string operation,
            string sqlId,
            IReadOnlyDictionary<string, object> parameters,
            SqliteTransaction transaction)
        {
            return Write(_model.SaveConnection, fileName, operation, sqlId, parameters, transaction);
        }

        public int Write(
            SqliteConnection connection,
            string fileName,
            string operation,
            string sqlId,
            IReadOnlyDictionary<string, object> parameters,
            SqliteTransaction transaction)
        {
            using var command = connection.CreateCommand();
            command.CommandText = _sqlReader.ReadXml(fileName, operation, sqlId);
            command.Transaction = transaction;
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? System.DBNull.Value);
            }
            return command.ExecuteNonQuery();
        }

        public void ExecuteScript(string fileName, SqliteTransaction transaction)
        {
            using var command = _model.SaveConnection.CreateCommand();
            command.CommandText = _sqlReader.ReadFile(fileName);
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        public void ExecuteMigration(int version, SqliteTransaction transaction)
        {
            ExecuteScript(_sqlReader.FindMigrationFileName(version), transaction);
        }
    }
}
