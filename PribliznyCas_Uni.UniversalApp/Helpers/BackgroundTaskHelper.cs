using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PribliznyCas_Uni.Helpers
{
    public static class BackgroundTaskHelper
    {
        public const string MaintenanceTaskName         = "Approximate time refresh task";
        public const string MaintenanceTaskEntryPoint   = "PribliznyCas_Uni.Background.BackgroundSchedulerTask";
        public const uint   MaintenanceTaskInterval     = 60;

        public static bool CheckBackgroundTaskActive()
        {
            return BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == MaintenanceTaskName);
        }

        public static void ActivateTimeTrigger()
        {
            //
            // A friendly task name.
            //
            string name = MaintenanceTaskName;

            //
            // Must be the same entry point that is specified in the manifest.
            //
            string taskEntryPoint = MaintenanceTaskEntryPoint;

            //
            // A system trigger that goes off every 15 minutes as long as the device is plugged in to AC power.
            //  
            TimeTrigger trigger = new TimeTrigger(MaintenanceTaskInterval, false);

            //
            // Build the background task.
            //
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            //
            // Register the background task, and get back a BackgroundTaskRegistration object representing the registered task.
            //
            BackgroundTaskRegistration task = builder.Register();
        }

        public static void DeactivateTimeTrigger()
        {
            string name = MaintenanceTaskName;
            string taskEntryPoint = MaintenanceTaskEntryPoint;
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == name)
                {
                    task.Value.Unregister(false);
                }
            }
        }
    }
}
