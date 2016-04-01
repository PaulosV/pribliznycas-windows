using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace TimeApprox.PRC
{
    public static class LocalizedTimeCodes
    {
        static DateTime lastResourceLoad = DateTime.MinValue;
        static ResourceLoader resourceLoader = null;

        public static string GetTimeName(string resourceName)
        {
            if (resourceLoader == null || DateTime.Today != lastResourceLoad.Date)
            {
                string resourceString = DateTime.Today.Month == 4 &&
                                        (DateTime.Today.Day == 1 || DateTime.Today.Day == 2) ?
                                        "TimeApprox.PRC/TimeResourcesAprilFools" : "TimeApprox.PRC/TimeResources";
                resourceLoader = ResourceLoader.GetForViewIndependentUse(resourceString);

                lastResourceLoad = DateTime.Today;
            }
            return resourceLoader.GetString(resourceName);
        }
    }
}
