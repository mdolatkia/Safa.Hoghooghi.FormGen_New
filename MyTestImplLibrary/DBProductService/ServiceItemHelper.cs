using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTestImplLibrary.DBProductService
{
    public class ServiceItemHelper
    {
        public static double GetHoursSpent(DateTime start, DateTime end)
        {
            return end.Subtract(start).TotalHours;
        }
    }
}
