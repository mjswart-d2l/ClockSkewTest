using System.Data.SqlClient;
using System.Data;

namespace LogMessage;
public class LogMessageData {

    private string m_connectionString;
    public LogMessageData(string connectionString) {
        m_connectionString = connectionString;
    }
    
    public void CreateLogTable() {
        string sql = @"
            DROP TABLE IF EXISTS dbo.MESSAGE_LOG; 
            CREATE TABLE dbo.MESSAGE_LOG
            (
                LogId INT IDENTITY NOT NULL 
                    PRIMARY KEY,
                LogMessage NVARCHAR(MAX) NOT NULL,
                LastUpdate DATETIME2
                    DEFAULT (GETDATE())
            )";
        using (SqlConnection connection = GetOpenConnection()) {
            SqlCommand cmd = new SqlCommand(sql,connection);
            cmd.ExecuteNonQuery();
        }
        
    }

    public int AddLogMessage(string message) {
        string sql = @"
            INSERT dbo.MESSAGE_LOG(LogMessage)
            OUTPUT inserted.LogId
            VALUES (@Message);";
        using (SqlConnection connection = GetOpenConnection()) {
            SqlCommand cmd = new SqlCommand(sql,connection);
            cmd.Parameters.Add("Message", SqlDbType.NVarChar).Value = message;
            return (int)cmd.ExecuteScalar();
        }
    }
    public int AddLogMessageWithDate(string message, DateTime lastUpdate ) {
        string sql = @"
            INSERT dbo.MESSAGE_LOG(LogMessage, LastUpdate)
            OUTPUT inserted.LogId
            VALUES (@Message, @LastUpdate);";
        using (SqlConnection connection = GetOpenConnection()) {
            SqlCommand cmd = new SqlCommand(sql,connection);
            cmd.Parameters.Add("Message", SqlDbType.NVarChar).Value = message;
            cmd.Parameters.Add("LastUpdate", SqlDbType.DateTime).Value = lastUpdate;
            return (int)cmd.ExecuteScalar();
        }
    }

    public void UpdateLogMessage(int logId, string message) {
        string sql = @"
            UPDATE dbo.MESSAGE_LOG
            SET LogMessage = @Message,
                LastUpdate = GETDATE()
            WHERE LogId = @LogId;";
        using (SqlConnection connection = GetOpenConnection()) {
            SqlCommand cmd = new SqlCommand(sql,connection);
            cmd.Parameters.Add("Message", SqlDbType.NVarChar).Value = message;
            cmd.Parameters.Add("LogId", SqlDbType.Int).Value = logId;
            cmd.ExecuteNonQuery();
        }
    }

     public LogMessageDto? GetLogMessage(int logId ) {
        string sql = @"
            SELECT LogId, LogMessage, LastUpdate
            FROM dbo.MESSAGE_LOG
            WHERE LogId = @LogId";
        using (SqlConnection connection = GetOpenConnection()) {
            SqlCommand cmd = new SqlCommand(sql,connection);
            cmd.Parameters.Add("LogId", SqlDbType.Int).Value = logId;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                return new LogMessageDto(
                    (int)reader[0], 
                    (string)reader[1], 
                    (DateTime)reader[2]);                
            }
            else {
                return null;
            }
        }
    }

    private SqlConnection GetOpenConnection() {
        SqlConnection result = new SqlConnection(m_connectionString);
        result.Open();
        return result;
    }
}
