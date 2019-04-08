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

        private void biOpen_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            designProject.OpenProuduct();
        }

        private void biNew_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) 
        {
            return;

            designProject.CreateNewProuduct();
        }


        private void biOverlap_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) 
        {
            designProject.GenerateProuductParameterOverlap();
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

        // 显示产品结构视图;
        private void biStructure_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            object prodStructureViewer = this.FindName("ProductStructureViewer");
            LayoutPanel panel = null ;
            if (null != prodStructureViewer)
            {
                panel = prodStructureViewer as LayoutPanel;
                if (panel.IsClosed || panel.IsHidden)
                {
                    object docLayoutMgr = this.FindName("viewerLayoutManager");
                    if (null != docLayoutMgr)
                    {
                        DockLayoutManager group = docLayoutMgr as DockLayoutManager;
                        group.DockController.Restore(panel);


                    }
                }
            }
        }

        private void biAnalysis_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            object docLayoutMgr = this.FindName("viewerLayoutManager");
            if (null != docLayoutMgr)
            {
                DockLayoutManager group = docLayoutMgr as DockLayoutManager;
                BaseLayoutItem viewer = null;
                if (group.HiddenItems.Count != 0)
                {
                    viewer = group.HiddenItems.First(panel => panel is DocumentPanel);
                    group.DockController.Restore(viewer);
                    if (null != viewer)
                    {
                        group.DockController.Restore(viewer);
                        return;
                    }
                }
                if (group.ClosedPanels.Count != 0)
                {
                    viewer = group.ClosedPanels.First(panel => panel is DocumentPanel);
                    if (null != viewer)
                    {
                        group.DockController.Restore(viewer);
                        return;
                    }
                }
            }
        }

        private void biResult_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

            Window1 win = new Window1();

            win.Width = SystemParameters.PrimaryScreenWidth;
            win.Height = SystemParameters.PrimaryScreenHeight;

            //win.size

            win.Show();

            return;


            bool isRestored = false;
            object obj = this.FindName("ResultPanel");
            DocumentPanel group = null;
            if (null != obj)
            {
                group = obj as DocumentPanel;
                            
                if (group.IsClosed || group.IsHidden)
                {
                    object docLayoutMgr = this.FindName("topLayoutManager");
                    if (null != docLayoutMgr)
                    {
                        DockLayoutManager groupMgr = docLayoutMgr as DockLayoutManager;
                        isRestored = groupMgr.DockController.Restore(group);
                    }
                }
            }
        }

        private void ShowViewer(string viewerName)
        {
            if (string.IsNullOrWhiteSpace(viewerName))
                return;

            



        }

    }
}
