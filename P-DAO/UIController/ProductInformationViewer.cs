//---------------------------------------------------------------------------
// 产品视图类，用于表示当前被选中产品的子产品信息; 
// 每个子产品使用表格的一行表示;
// 
// 作者: 樊红日;
// 日期: 2018.1.3;
// 
//---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;


using P_DAO.DomainEntities;

namespace P_DAO.UIController
{
    class ProductInformationViewer
    {
        // 物理UI,是显示该产品信息Viewer的最顶层父窗口;
        private DocumentPanel mUIViewer;

        
        private Grid mUIGrid;

        // 以默认的表格视图方式显示当前产品的子产品信息;
        // 也可以卡片、或树形视图方式显示;
        private GridControl mProductInfoViewer;

        private TreeListControl mUITreeListControl;

        private Product mProduct;


        // 仅作为编码示例
        public ProductInformationViewer(DocumentPanel uiViewer)
        {
            mUIViewer = uiViewer;

            mUIGrid = new Grid();

            mUITreeListControl = new TreeListControl();

            mUITreeListControl.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;

            mUITreeListControl.EnableSmartColumnsGeneration = true;

            mUITreeListControl.ItemsSource = Stuff.GetStuff();

            mUITreeListControl.View = new TreeListView();

            mUITreeListControl.View.KeyFieldName = "ID";

            mUITreeListControl.View.ParentFieldName = "ParentID";
            
            mUITreeListControl.View.AllowPerPixelScrolling = true;

            mUITreeListControl.View.ShowTotalSummary = true;

            mUITreeListControl.View.AllowEditing = false;

            mUITreeListControl.View.AutoWidth = true;

            mUITreeListControl.RefreshData();

            mUIGrid.Children.Add(mUITreeListControl);


            mUIViewer.Content = mUIGrid;
        }


        public ProductInformationViewer(DocumentPanel uiViewer, Product product)
        {
            mUIViewer = uiViewer;

            mProduct = product;

            mUIGrid = new Grid();
            
            mProductInfoViewer = new GridControl();
            mProductInfoViewer.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;
            
            // 设置数据来源;
            mProductInfoViewer.ItemsSource = Stuff.GetStuff();
            GridViewBase view = (GridViewBase)mProductInfoViewer.View;
            // 隐藏列排序设置面板, 用户可通过右键菜单显示;
            view.ShowGroupPanel = false;


            mUIGrid.Children.Add(mProductInfoViewer);

            mUIViewer.Content = mUIGrid;


        }


        public DocumentPanel UIViewer
        {
            get { return mUIViewer; }
        }


    }
}
