
using MyModelManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProject_WPF
{
    /// <summary>
    /// Interaction logic for frmFormulaItemSelector.xaml
    /// </summary>
    public partial class frmPropertySelector : UserControl
    {
        public event EventHandler<MyPropertyInfo> PropertySelected;
        BizTableDrivedEntity bizTableDrivedEntity = new BizTableDrivedEntity();
        public frmPropertySelector(int entityID)
        {
            InitializeComponent();
            dtgFormulaItems.MouseDoubleClick += DtgFormulaItems_MouseDoubleClick;
            var entity = bizTableDrivedEntity.GetPermissionedEntity(MyProjectManager.GetMyProjectManager.GetRequester(), entityID);
            var properties = FormulaInstanceInternalHelper.GetProperties(MyProjectManager.GetMyProjectManager.GetRequester(),entity, null, true).Select(x => x.Value).ToList();
            dtgFormulaItems.ItemsSource = properties;
        }

        private void DtgFormulaItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dtgFormulaItems.SelectedItem != null)
            {
                if (PropertySelected != null)
                    PropertySelected(this, dtgFormulaItems.SelectedItem as MyPropertyInfo);
            }
        }
    }
}
