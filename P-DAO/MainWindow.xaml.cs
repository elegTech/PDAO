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
using System.Data;

using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Grid;
using P_DAO.DomainEntities;
using DevExpress.Xpf.Docking;

namespace P_DAO
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DAOProject designProject;


        public MainWindow()
        {
            InitializeComponent();
                        
            //productStructureTree.ItemsSource = Stuff.GetStuff();

            designProject = new DAOProject(this); 
        }

        private void biOpen_ItemClick_1(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            designProject.OpenProuduct();
        }

        private void biNew_ItemClick_1(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            designProject.CreateNewProuduct();
        }

        // Logics for a row has been double clicked.
        private void structureView_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {

            //DocumentGroup documentGroup = (DocumentGroup)this.FindName("ProductGroup");

            //documentGroup.SelectedTabIndex = 1;



            if ( MouseButton.Left!= e.ChangedButton)
                return;

            designProject.ActivateProduct();
        }



    }
}
