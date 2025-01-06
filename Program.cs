using Quartz;
using System.Collections.Specialized;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();
app.UseHttpsRedirection();
app.MapGet("/hello", () =>
{
    return 1;
});

/* Schema needs to be created beforehand.
 using SQLLiteConnection connection = new("Data Source = quartz.db");
 connection.Open();
 using var command = connection.CreateCommand();
 command.CommandText =
 ***
 DROP TABLE IF EXISTS QRTZ_FIRE_TRIGGERS;
 DROP TABLE IF EXISTS QRTZ_PAUSED_TRIGGER_GRPS;
 DROP TABLE IF EXISTS QRTZ_SCHEDULER_STATE;
 DROP TABLE IF EXISTS QRTZ_LOCKS;
 DROP TABLE IF EXISTS QRTZ_SIMPROP_TRIGGERS;
 DROP TABLE IF EXISTS QRTZ_SIMPLE_TRIGGERS;
 DROP TABLE IF EXISTS QRTZ_CRON_TRIGGERS;
 DROP TABLE IF EXISTS QRTZ_BLOB_TRIGGERS;
 DROP TABLE IF EXISTS QRTZ_TRIGGERS;
 DROP TABLE IF EXISTS QRTZ_JOB_DETAILS;
 DROP TABLE IF EXISTS QRTZ_JOB_CALENDARS;
 
 CREATE TABLE QRTZ_JOB_DETAILS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        JOB_NAME NVARCHAR(150) NOT NULL,
        JOB_GROUP NVARCHAR(150) NOT NULL,
        DESCRIPTION NVARCHAR(250) NULL,
        JOB_CLASS_NAME NVARCHAR(250) NOT NULL,
        IS_DURABLE BIT NOT NULL,
        IS_NONCONCURRENT BIT NOT NULL,
        IS_UPDATE_DATA BIT NOT NULL,
        REQUESTS_RECOVERY BIT NOT NULL,
        JOB_DATA BLOB NULL,
        PRIMARY KEY (SCHED_NAME, JOB_NAME, JOB_GROUP)
    );

CREATE TABLE QRTZ_TRIGGERS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        TRIGGER_NAME NVARCHAR(150) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        JOB_NAME NVARCHAR(150) NOT NULL,
        JOB_GROUP NVARCHAR(150) NOT NULL,
        DESCRIPTION NVARCHAR(250) NULL,
        NEXT_FIRE_TIME BIGINT NULL,
        PREV_FIRE_TIME BIGINT NULL,
        PRIORITY INTEGER NULL,
        TRIGGER_STATE NVARCHAR(16) NOT NULL,
        TRIGGER_TYPE NVARCHAR(8) NOT NULL,
        START_TIME BIGINT NOT NULL,
        END_TIME BIGINT NULL,
        CALENDAR_NAME NVARCHAR(200) NULL,
        MISFIRE_INSTR INTEGER NULL,
        JOB_DATA BLOB NULL,
        PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
        FOREIGN KEY (SCHED_NAME, JOB_NAME, JOB_GROUP)
            REFERENCES QRTZ_JOB_DETAILS(SCHED_NAME,JOB_NAME,JOB_GROUP)
    );

CREATE TABLE SIMPLE_TRIGGERS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        TRIGGER_NAME NVARCHAR(150) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        REPEAT_COUNT BIGINT NOT NULL,
        REPEAT_INTERVAL BIGINT NOT NULL,
        TIMES_TRIGGERED BIGINT NOT NULL,
        PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
        FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
            REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP) ON DELETE CASCADE
    );

CREATE TRIGGER DELETE_SIMPLE_TRIGGER DELETE ON QRTZ_TRIGGERS
BEGIN
    DELETE FROM QRTZ_SIMPLE_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP
END
;

CREATE TABLE QRTZ_SIMPROP_TRIGGERS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        TRIGGER_NAME NVARCHAR(150) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        STR_PROP_1 NVARCHAR(512) NULL,
        STR_PROP_2 NVARCHAR(512) NULL,
        STR_PROP_3 NVARCHAR(512) NULL,
        INT_PROP_1 INT NULL,
        INT_PROP_2 INT NULL,
        LONG_PROP_1 BIGINT NULL,
        LONG_PROP_2 BIGINT NULL,
        DEC_PROP_1 NUMERIC NULL,
        DEC_PROP_2 NUMERIC NULL,
        BOOL_PROP_1 BIT NULL,
        BOOL_PROP_2 BIT NULL,
        TIME_ZONE_ID NVARCHAR(80) NULL,
        PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
        FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
            REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP) ON DELETE CASCADE
    );

CREATE TRIGGER DELETE_SIMPROP_TRIGGER DELETE ON QRTZ_TRIGGERS
BEGIN
    DELETE FROM QRTZ_SIMPROP_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP
END

CREATE TABLE QRTZ_CRON_TRIGGERS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        TRIGGER_NAME NVARCHAR(150) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        CRON_EXPRESSION NVARCHAR(250) NOT NULL,
        TIME_ZONE_ID NVARCHAR(80),
        PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
        FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
            REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP) ON DELETE CASCADE
    );

CREATE TRIGGER DELETE_CRON_TRIGGER DELETE ON QRTZ_TRIGGERS
BEGIN
    DELETE FROM QRTZ_CRON_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP
END

CREATE TABLE QRTZ_BLOB_TRIGGERS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        TRIGGER_NAME NVARCHAR(150) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        BLOB_DATA BLOB NULL,
        PRIMARY KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP),
        FOREIGN KEY (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP)
            REFERENCES QRTZ_TRIGGERS(SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP) ON DELETE CASCADE
    );

CREATE TRIGGER DELETE_BLOB_TRIGGER DELETE ON QRTZ_TRIGGERS
BEGIN
    DELETE FROM QRTZ_BLOB_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP
END

CREATE TABLE QRTZ_CALENDARS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        CALENDAR_NAME NVARCHAR(200) NOT NULL,
        CALENDAR BLOB NOT NULL
        PRIMARY KEY (SCHED_NAME, CALENDAR_NAME)
    );

CREATE TABLE QRTZ_PAUSED_TRIGGER_GRPS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        PRIMARY KEY (SCHED_NAME, TRIGGER_GROUP)
    );

CREATE TABLE QRTZ_FIRED_TRIGGERS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        ENTRY_ID NVARCHAR(140) NOT NULL,
        TRIGGER_NAME NVARCHAR(150) NOT NULL,
        TRIGGER_GROUP NVARCHAR(150) NOT NULL,
        INSTANCE_NAME NVARCHAR(200) NOT NULL,
        FIRED_TIME BIGINT NOT NULL,
        SCHED_TIME BIGINT NOT NULL,
        PRIORITY INTEGER NOT NULL,
        STATE NVARCHAR(16) NOT NULL,
        JOB_NAME NVARCHAR(150) NULL,
        JOB_GROUP NVARCHAR(150) NULL,
        IS_NONCONCURRENT BIT NULL,
        REQUESTS_RECOVERY BIT NULL,
        PRIMARY KEY (SCHED_NAME, ENTRY_ID)
    );

CREATE TABLE QRTZ_SCHEDULER_STATE
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        INSTANCE_NAME NVARCHAR(200) NOT NULL,
        LAST_CHECKIN_TIM BIGINT NOT NULL,
        CHECKIN_INTERVAL BIGINT NOT NULL,
        PRIMARY KEY (SCHED_NAME, INSTANCE_NAME)
    );

CREATE TABLE QRTZ_LOCKS
    (
        SCHED_NAME NVARCHAR(120) NOT NULL,
        LOCK_NAME NVARCHAR(40) NOT NULL,
        PRIMARY KEY (SCHED_NAME, LOCK_NAME)
    );
 ***;
 command.ExcecuteNonQuery();
 */

/* Table prefix. */
NameValueCollection properties = new()
{
    ["quartz.jobStore.tablePrefix"] = "QRTZ_"
};
/* Core logic for the scheduler.
 * Able to configure additional options with fluid syntax.
 * Other serializers exist, but currently using SystemTextJsonSerializer.
 * For connection string, if using PostGres, SQLServer, etc; better to use config file.
 */
IScheduler scheduler = await SchedulerBuilder
    .Create(properties)
    .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
    .WithMisfireThreshold(TimeSpan.FromSeconds(10))
    .UsePersistentStore(x =>
    {
        x.UseProperties = true;
        x.UseSQLite("Data Source=quartz.db");
        x.UseSystemTextJsonSerializer();
        x.PerformSchemaValidation = true;
    })
    .BuildScheduler();

await scheduler.Start(); //is async

/* Test key and trigger 
*  once job defined, can create different triggers.
*/
var jobKey = new JobKey("hello-world", "test-jobs"); //name, group
var jobTrigger = TriggerBuilder.Create()
    .WithIdentity("trigger-name", "test-triggers")
    .ForJob(jobKey)
    .StartAt(DateTimeOffset.Now.AddSeconds(5))
    .Build();

var jobDetail = JobBuilder //linked to jobkey by identity
    .Create<OurTestJob>()
    .WithIdentity(jobKey)
    .Build();

await scheduler.ScheduleJob(jobDetail, jobTrigger); //is async

/* Run the app
 * If removed will work of the minimal quartz api.
 */
app.Run();

sealed class OurTestJob : IJob
{
    public Task Execute(IJobExecutionContext context) //can run async
    {
        Console.WriteLine("Hello, world!");
        return Task.CompletedTask;
    }
}
