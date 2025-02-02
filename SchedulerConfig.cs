using Quartz.Impl;
using Quartz;

namespace QuartzSchedulerTraining
{
    public class SchedulerConfig
    {
        public static async Task ConfigureAndStart()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();

            // Create job
            var jobDetail = JobBuilder.Create<ApiFetchJob>()
                .WithIdentity("apiFetchJob", "group1")
                .Build();

            // Create trigger that runs every minute
            var trigger = TriggerBuilder.Create()
                .WithIdentity("apiFetchTrigger", "group1")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();

            // Schedule the job
            await scheduler.ScheduleJob(jobDetail, trigger);
            await scheduler.Start();
        }
    }
}
