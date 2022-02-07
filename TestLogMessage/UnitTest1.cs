
using NUnit.Framework;
using System.Data.SqlClient;
using LogMessage;
using System;

namespace TestLogMessage;

public class Tests
{
    private LogMessageData m_data = null!;
    private DatabaseNowProvider m_nowProvider = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
        b.DataSource = "127.0.0.1";
        b.InitialCatalog = "tempdb";
        b.IntegratedSecurity = true;
        m_data = new LogMessageData( b.ToString() );
        m_data.CreateLogTable();
        m_nowProvider = new DatabaseNowProvider(b.ToString());
    }

    [Test]
    public void UpdateMessage_DateIsUpdated_1() {
        // Store message (arrange)
        string message = Guid.NewGuid().ToString();
        int logId = m_data.AddLogMessage( message );
        LogMessageDto? dto = m_data.GetLogMessage( logId );
        DateTime createdDate = dto.LastUpdate;

        // Update message (act)
        string newMessage = Guid.NewGuid().ToString();
        m_data.UpdateLogMessage( logId, newMessage );
        
        // Check Date (assert)
        dto = m_data.GetLogMessage( logId );
        DateTime updatedDate = dto.LastUpdate;
        
        // The following assertion may fail! 
        // updatedDate and createdDate are Equal if the computer is fast enough
        Assert.Greater( updatedDate, createdDate ); 
    }

    [Test]
    public void UpdateMessage_DateIsUpdated_2() {
        // Store message (arrange)
        string message = Guid.NewGuid().ToString();
        DateTime now = DateTime.Now;
        int logId = m_data.AddLogMessageWithDate( message, now );

        // Update message (act)
        string newMessage = Guid.NewGuid().ToString();
        m_data.UpdateLogMessage( logId, newMessage );
        
        // Check Date (assert)
        LogMessageDto? dto = m_data.GetLogMessage( logId );
        
        // This next assertion can fail if the database is in a different time zone        
        Assert.GreaterOrEqual( dto.LastUpdate, now );
    }

    [Test]
    public void UpdateMessage_DateIsUpdated_3() {
        // Store message (arrange)
        string message = Guid.NewGuid().ToString();
        DateTime now = DateTime.Now;
        int logId = m_data.AddLogMessageWithDate( message, now );

        // Update message (act)
        string newMessage = Guid.NewGuid().ToString();
        m_data.UpdateLogMessage( logId, newMessage );
        
        // Check Date (assert)
        LogMessageDto? dto = m_data.GetLogMessage( logId );
        
        // This next assertion can fail if the clocks on the database server is off by a few seconds
        Assert.GreaterOrEqual( dto.LastUpdate, now );
    }

    [Test]    
    public void StoreDate_ReadItBack() {
         // Store message with Date
        string message = Guid.NewGuid().ToString();
        DateTime now = DateTime.Now;
        int logId = m_data.AddLogMessageWithDate( message, now );
        
        // Read date
        LogMessageDto? dto = m_data.GetLogMessage( logId );

        // The following assertion may fail! 
        // SQL Server's DATETIME has a different precision than .Net's DateTime
        Assert.AreEqual( now, dto.LastUpdate );
    }

    [Test]    
    public void StoreDateInTheFuture() {
        string message = Guid.NewGuid().ToString();
        DateTime inAMonth = DateTime.Now + TimeSpan.FromDays( 30 );        
        // CovertTime may fail because "a month from now" may be an invalid DateTime (with daylight savings)
        inAMonth = TimeZoneInfo.ConvertTime( inAMonth, TimeZoneInfo.Local );
        m_data.AddLogMessageWithDate( message, inAMonth );
        Assert.Pass();
    }
}