using System.Data.SqlClient;
using System;

namespace TestLogMessage;

public class DatabaseNowProvider
{
    string m_connectionString;
    public DatabaseNowProvider(string connectionString) {
        m_connectionString = connectionString;            
    }

    public DateTime Now() {
        string sql = @"SELECT GETDATE();";
        
        
        using (SqlConnection conn = new SqlConnection(m_connectionString)) {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql,conn);
            return (DateTime)cmd.ExecuteScalar();
        }
    }
}
