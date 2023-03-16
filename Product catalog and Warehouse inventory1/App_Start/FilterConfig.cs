using System.Web;
using System.Web.Mvc;

namespace Product_catalog_and_Warehouse_inventory1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
