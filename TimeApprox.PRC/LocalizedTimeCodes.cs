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
        static ResourceLoader resourceLoader = null;

        public static string GetTimeName(string resourceName)
        {
            if (resourceLoader == null)
            {
                resourceLoader = ResourceLoader.GetForViewIndependentUse("TimeApprox.PRC/TimeResources");
            }
            return resourceLoader.GetString(resourceName);
        }
    }
}
