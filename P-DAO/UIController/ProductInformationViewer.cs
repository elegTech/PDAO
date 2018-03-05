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
using System.Windows;

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
            mProductInfoViewer.ItemsSource = product.GetSubProductInfo();

            // 设置表格视图显示属性;
            TableView view = (TableView)mProductInfoViewer.View;
            // 隐藏列排序设置面板, 用户可通过右键菜单显示;
            view.ShowGroupPanel = false;

            view.AllowEditing = false;

            view.CellStyle = (Style)Application.Current.Resources["SelectionStateCellStyle"];
            
            view.RowStyle = (Style)Application.Current.Resources["SelectedRowStyle"];

            view.UseIndicatorForSelection = true;

            mUIGrid.Children.Add(mProductInfoViewer);

            mUIViewer.Content = mUIGrid;

            view.UseLightweightTemplates = UseLightweightTemplates.All;

            DataControlBase baseViewer = (DataControlBase) mProductInfoViewer;

            mProductInfoViewer.SelectionMode = MultiSelectMode.Cell;

            view.NavigationStyle = GridViewNavigationStyle.Cell;

            mProductInfoViewer.CurrentColumnChanged += CurrentColumnChanged;
            mProductInfoViewer.CurrentItemChanged += CurrentItemChanged;

            //mProductInfoViewer.SelectItem(1);
            //mProductInfoViewer.SelectItem(0);

            view.SelectCell(0, mProductInfoViewer.Columns[0]);
           // view.SelectCell(0, mProductInfoViewer.Columns[1]);
           // view.SelectCell(0, mProductInfoViewer.Columns[2]);


        }

        void CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            GridColumn focusedColumn = (GridColumn)e.Source.CurrentColumn;

            string culumnCaption = focusedColumn.HeaderCaption.ToString();

            if (culumnCaption == "Name")
                return;

            string productName = mProductInfoViewer.GetFocusedRowCellDisplayText("Name");

            SelectNeighborProduct(productName, culumnCaption);
        }

        void CurrentColumnChanged(object sender, CurrentColumnChangedEventArgs e)
        { 
            GridColumn focusedColumn = (GridColumn)e.NewColumn;

            string culumnCaption = focusedColumn.HeaderCaption.ToString();

            if (culumnCaption == "Name")
                return;

            string productName = mProductInfoViewer.GetFocusedRowCellDisplayText("Name");

            SelectNeighborProduct(productName, culumnCaption);
            
        }


        public void SelectNeighborProduct(string productName, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(parameterName))
                return;

            Product neighborProduct = null;
            string neighborPar = string.Empty;

            Product focusedProduct = Product.GetProduct(productName);
            focusedProduct.FindNeighborParameter(parameterName, ref neighborProduct, ref neighborPar);

            TableView view = (TableView)mProductInfoViewer.View;

            int rowHandle = mProductInfoViewer.FindRowByValue("Name", focusedProduct.Name);
            view.SelectCell(rowHandle, mProductInfoViewer.Columns[neighborPar]);

        }


        public DocumentPanel UIViewer
        {
            get { return mUIViewer; }
        }


    }
}
