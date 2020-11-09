
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //Test frm = new Test();
            //frm.ShowDialog();
            //   MyProjectManager.GetMyProjectManager.ShowDialog(frm,"");
            //  var frm = new frmNewFormulaDefinition("", 54278);
            //var frm = new frmFormula(6023, 54278);

            //MyProjectManager.GetMyProjectManager.ShowDialog(frm, "",Enum_WindowSize.Maximized);
            MyProjectManager.GetMyProjectManager.StartApp();


        }

       
    }
}
