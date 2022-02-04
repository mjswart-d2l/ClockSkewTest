
using NUnit.Framework;
using System.Data.SqlClient;
using LogMessage;
using System;

namespace TestLogMessage;

public class Tests
{
    private LogMessageData m_data = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
        b.DataSource = "127.0.0.1";
        b.InitialCatalog = "tempdb";
        b.IntegratedSecurity = true;
        m_data = new LogMessageData( b.ToString() );
        m_data.CreateLogTable();        
    }

    [Test]
    public void LogAndRetrieveMessage()
    {
        string message = Guid.NewGuid().ToString();
        DateTime now = DateTime.Now;
        
        int logId = m_data!.AddLogMessage( message );
        LogMessageDto? dto = m_data.GetLogMessage( logId );

        Assert.IsNotNull( dto, "read a message" );
        Assert.AreEqual( message, dto!.Message, "comparing Message" );
        Assert.AreEqual( logId, dto.LogId, "comparing LogId" );
        Assert.LessOrEqual( now, dto.LastUpdatedTime, "comparing CreatedDate" );
    }
}