using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Scripts.Features.SystemScene.DataGateway
{
    public sealed class DataGatewayReader
    {
        readonly DataGatewayModel _model;
        readonly DataGatewaySqlReader _sqlReader;

        public DataGatewayReader(DataGatewayModel model, DataGatewaySqlReader sqlReader)
        {
            _model = model;
            _sqlReader = sqlReader;
        }

        public List<Dictionary<string, object>> ReadMaster(
            string fileName,
            string sqlId,
            IReadOnlyDictionary<string, object> parameters = null)
        {
            return Read(_model.MasterConnection, fileName, sqlId, parameters);
        }

        public List<Dictionary<string, object>> ReadSave(
            string fileName,
            string sqlId,
            IReadOnlyDictionary<string, object> parameters = null)
        {
            return Read(_model.SaveConnection, fileName, sqlId, parameters);
        }

        public long ReadSaveScalarLong(
            string fileName,
            string sqlId,
            SqliteTransaction transaction)
        {
            using var command = _model.SaveConnection.CreateCommand();
            command.CommandText = _sqlReader.ReadXml(fileName, "select", sqlId);
            command.Transaction = transaction;
            return (long)command.ExecuteScalar();
        }

        static void Bind(SqliteCommand command, IReadOnlyDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? System.DBNull.Value);
            }
        }

        List<Dictionary<string, object>> Read(
            SqliteConnection connection,
            string fileName,
            string sqlId,
            IReadOnlyDictionary<string, object> parameters)
        {
            using var command = connection.CreateCommand();
            command.CommandText = _sqlReader.ReadXml(fileName, "select", sqlId);
            Bind(command, parameters);

            var rows = new List<Dictionary<string, object>>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>(reader.FieldCount);
                for (var index = 0; index < reader.FieldCount; index++)
                {
                    row.Add(reader.GetName(index), reader.GetValue(index));
                }
                rows.Add(row);
            }
            return rows;
        }
    }
}
