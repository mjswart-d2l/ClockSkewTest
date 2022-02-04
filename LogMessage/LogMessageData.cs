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
            IF NOT EXISTS (
                SELECT *
                FROM sys.tables
                WHERE object_id = OBJECT_ID('dbo.MESSAGE_LOG')
            )
            BEGIN
                CREATE TABLE dbo.MESSAGE_LOG
                (
                    LogId INT IDENTITY NOT NULL 
                        PRIMARY KEY,
                    LogMessage NVARCHAR(MAX) NOT NULL,
                    LastUpdatedTime DATETIME2
                        DEFAULT (SYSDATETIME())
                )
            END";
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

    public void UpdateLogMessage(int logId, string message) {
        string sql = @"
            UPDATE dbo.MESSAGE_LOG
            SET Message = @Message,
                LastUpdatedTime = SYSDATETIME()
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
            SELECT LogId, LogMessage, LastUpdatedTime
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
