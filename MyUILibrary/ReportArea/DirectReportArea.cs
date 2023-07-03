using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelEntites;
using ProxyLibrary;


using MyUILibraryInterfaces.DataTreeArea;
using MyUILibrary.DataTreeArea;
using MyUILibraryInterfaces.EntityArea;
using MyUILibrary.EntitySelectArea;

namespace MyUILibrary.EntityArea
{
    class DirectReportArea : I_DirectReportArea
    {



        public bool SecurityNoAccess { set; get; }
        public bool SecurityReadonly { set; get; }
        public bool SecurityEdit { set; get; }

        public I_GeneralEntityDataSelectArea GeneralEntityDataSelectArea { set; get; }
        //public object MainView
        //{
        //    set; get;
        //}
        EntityDirectReportDTO EntityDirectReport { set; get; }
        public DirectReportAreaInitializer AreaInitializer { set; get; }
        public object View { set; get; }

        public DirectReportArea(DirectReportAreaInitializer areaInitializer)
        {
            //DirectReportArea: 93d179c8164c
            //بهتره اکسترنال ویو ست بشه .. یعنی یک فرم و کنترل که مثل مرورگر باشد
            AreaInitializer = areaInitializer;
            EntityDirectReport = AgentUICoreMediator.GetAgentUICoreMediator.ReportManager.GetEntityDirectReport(AgentUICoreMediator.GetAgentUICoreMediator.GetRequester(), areaInitializer.ReportID);
            if (EntityDirectReport == null)
            {
                AgentUICoreMediator.GetAgentUICoreMediator.UIManager.ShowInfo("دسترسی به گزارش به شناسه" + " " + areaInitializer.ReportID + " " + "امکانپذیر نمی باشد", "", Temp.InfoColor.Red);
                return;
            }


            EntityDataSelectAreaInitializer selectAreaInitializer = new EntityDataSelectAreaInitializer();
            selectAreaInitializer.EntityID = EntityDirectReport.TableDrivedEntityID;
            selectAreaInitializer.EntityListViewID = EntityDirectReport.EntityListViewID;
            selectAreaInitializer.EntitySearchID = EntityDirectReport.EntitySearchID;
            selectAreaInitializer.DataItem = new List<DP_BaseData>() { areaInitializer.DataInstance };
            if (selectAreaInitializer.DataItem != null)
                selectAreaInitializer.LockDataSelector = true;
            GeneralEntityDataSelectArea = new GeneralEntityDataSelectArea();
            GeneralEntityDataSelectArea.DataItemChanged += EntitySelectArea_DataItemSelected;

            GeneralEntityDataSelectArea.SetAreaInitializer(selectAreaInitializer);
            View = GeneralEntityDataSelectArea.View;
        }

        private void EntitySelectArea_DataItemSelected(object sender, List<DP_FormDataRepository> e)
        {
            var url = EntityDirectReport.URL;
            if (e != null && e.Any())
            {
                var dataItem = e.First();
                foreach (var item in dataItem.KeyProperties)
                {
                    var paramCol = EntityDirectReport.EntityDirectlReportParameters.FirstOrDefault(x => x.ColumnID == item.ColumnID);
                    if (paramCol != null)
                    {
                        var param = paramCol.ParameterName + "=" + item.Value;
                        if (url.Contains("?"))
                            url += "&" + param;
                        else
                            url += "?" + param;
                    }
                }
                System.Diagnostics.Process.Start(url);
            }
        }


        //DP_DataView MainDataInstance { set; get; }







    }
}
