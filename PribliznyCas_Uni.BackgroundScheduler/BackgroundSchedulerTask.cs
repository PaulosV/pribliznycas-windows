using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using TimeApprox.PRC;

namespace PribliznyCas_Uni.Background
{
    public sealed class BackgroundSchedulerTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the background task details
            string taskName = taskInstance.Task.Name;

            Debug.WriteLine("Background " + taskName + " starting...");

            ApproxTileUpdater.ScheduleLiveTileUpdates();
            ApproxTileUpdater.UpdateLiveTiles();
            
            Debug.WriteLine("Background " + taskName + " completed!");
        }
    }
}
