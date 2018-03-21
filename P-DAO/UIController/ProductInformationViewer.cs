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
using DevExpress.Xpf.Editors;

using P_DAO.DomainEntities;

namespace P_DAO.UIController
{
    class ProductInformationViewer : ProductViewerBase
    {
        // 物理UI,是显示该产品信息Viewer的最顶层父窗口;
        //private DocumentPanel mUIViewer;       
            
        //private Grid mUIGrid;

        // 以默认的表格视图方式显示当前产品的子产品信息;
        // 也可以卡片、或树形视图方式显示;
        //private GridControl mProductInfoViewer;

        //private TreeListControl mUITreeListControl;

        //private Product mProduct;

        // 由于每个依赖的参数显示单元格Cell是在代码中手动设置其控件的背景色,
        // 需要记录前一个获取焦点的Cell位置; 待下次选择其他Cell时将前一被选中的Cell恢复原状;
        private LightweightCellEditor mPreSelectedCell;

        // 记录前一个获取焦点的单元格的UIElement;
        // 获取焦点的Cell需要手动更改其背景颜色,
        // 当该Cell失去焦点时同样要手动恢复原有背景颜色;
        private LightweightCellEditor mPreFocusedCell;

        private string mPreProductName;

        private string mPreParameterName;

        //private int focusedRowHandle;

        //private GridColumn focusedColumn;

        //private bool mIsUpdated;


        //public DocumentPanel UIViewer
        //{
        //    get { return mUIViewer; }
        //}


        // 仅作为编码示例
        //public ProductInformationViewer(DocumentPanel uiViewer)
        //{
        //    mUIViewer = uiViewer;

        //    mUIGrid = new Grid();

        //    mUITreeListControl = new TreeListControl();

        //    mUITreeListControl.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;

        //    mUITreeListControl.EnableSmartColumnsGeneration = true;

        //    mUITreeListControl.ItemsSource = Stuff.GetStuff();

        //    mUITreeListControl.View = new TreeListView();

        //    mUITreeListControl.View.KeyFieldName = "ID";

        //    mUITreeListControl.View.ParentFieldName = "ParentID";
            
        //    mUITreeListControl.View.AllowPerPixelScrolling = true;

        //    mUITreeListControl.View.ShowTotalSummary = true;

        //    mUITreeListControl.View.AllowEditing = false;

        //    mUITreeListControl.View.AutoWidth = true;

        //    mUITreeListControl.RefreshData();

        //    mUIGrid.Children.Add(mUITreeListControl);


        //    mUIViewer.Content = mUIGrid;
        //}


        public ProductInformationViewer(DocumentPanel uiViewer, Product product)
            : base(uiViewer, product)
        {
            //mUIViewer = uiViewer;

            //mProduct = product;

            //mUIGrid = new Grid();
            
            //mProductInfoViewer = new GridControl();
            //mProductInfoViewer.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;
            
            // 设置数据来源;
            mProductInfoViewer.ItemsSource = product.GetSubProductInfo();

            // 设置表格视图显示属性;
            TableView view = (TableView)mProductInfoViewer.View;
            // 隐藏列排序设置面板, 用户可通过右键菜单显示;
            view.ShowGroupPanel = false;

            view.AllowEditing = false;

            view.CellStyle = (Style)Application.Current.Resources["FocusedCellStyle1"];

            view.RowStyle = (Style)Application.Current.Resources["SelectedRowStyle1"];

            //view.Style = (Style)Application.Current.Resources["ViewStyle"];

            view.ShowFocusedRectangle = true;

            view.UseIndicatorForSelection = true;

            mUIGrid.Children.Add(mProductInfoViewer);

            mUIViewer.Content = mUIGrid;

            view.UseLightweightTemplates = UseLightweightTemplates.All;

            mProductInfoViewer.SelectionMode = MultiSelectMode.Cell;

            view.NavigationStyle = GridViewNavigationStyle.Cell;

            mProductInfoViewer.CurrentColumnChanged += CurrentColumnChanged;
            mProductInfoViewer.CurrentItemChanged += CurrentItemChanged;

            //view.FocusedRowHandleChanged +=view_FocusedRowHandleChanged;
        }

        void view_FocusedRowHandleChanged(object sender, FocusedRowHandleChangedEventArgs e)
        {


        }

        void CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            FocusedCellChanged();
            return;
        }

        void CurrentColumnChanged(object sender, CurrentColumnChangedEventArgs e)
        {
            FocusedCellChanged();
            return;            
        }

        public void SelectDependentProduct(string productName, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(parameterName))
                return;
            
            Product dependentProduct = null;
            string dependentParameterName = string.Empty;

            Product focusedProduct = Product.GetProduct(productName);
            focusedProduct.FindDependentParameter(mProduct, parameterName, ref dependentProduct, ref dependentParameterName);
            
            LightweightCellEditor focuesdCellElmt;
            int focusedRowHandle;
            TableView view = (TableView)mProductInfoViewer.View;

            // 若在当前上下文中没有找到依赖产品参数,说明该产品已经是叶子产品,仅需高亮该获取焦点的Cell即可;
            if (null == dependentProduct)
            {
                if (null != mPreFocusedCell)
                {
                    RestoreAppreance(mPreFocusedCell);
                }

                focusedRowHandle = mProductInfoViewer.FindRowByValue("Name", productName);
                focuesdCellElmt = (LightweightCellEditor)view.GetCellElementByRowHandleAndColumn(focusedRowHandle, mProductInfoViewer.Columns.First(col => col.HeaderCaption.ToString().Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)));
                focusedRowHandle = mProductInfoViewer.FindRowByValue("Name", productName);

                focuesdCellElmt.Background = Brushes.Orange;
                InplaceBaseEdit cellEditor = (InplaceBaseEdit)focuesdCellElmt.Content;
                cellEditor.FontWeight = FontWeights.Bold;

                mPreFocusedCell = focuesdCellElmt;
                return;
            }

            LightweightCellEditor dependentCellElmt;
            int neighborRowHandle;

            // 若当前找到的相关产品参数已经被访问过,则退出当前处理过程;
            if (!string.IsNullOrEmpty(mPreProductName) && 
                mPreProductName.Equals(dependentProduct.Name) && 
                mPreParameterName.Equals(dependentParameterName))
                return;


            focusedRowHandle = mProductInfoViewer.FindRowByValue("Name", productName);

            neighborRowHandle = mProductInfoViewer.FindRowByValue("Name", dependentProduct.Name);

            dependentCellElmt = (LightweightCellEditor)view.GetCellElementByRowHandleAndColumn(neighborRowHandle, mProductInfoViewer.Columns.First(col => col.HeaderCaption.ToString().Equals(dependentParameterName, StringComparison.CurrentCultureIgnoreCase)));

            focuesdCellElmt = (LightweightCellEditor)view.GetCellElementByRowHandleAndColumn(focusedRowHandle, mProductInfoViewer.Columns.First(col => col.HeaderCaption.ToString().Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)));
            

            // 如果当前需选择的Cell和前一个选择的mPreSelectedCell相同, 则退出;
            if (mPreSelectedCell == dependentCellElmt)
                return;           
            
            // 通过view.SelectCell方式不能奏效，因此在代码中直接对控件进行修改;
            // 将上一被选中的Cell的外观复原;
            //InplaceBaseEdit cellEditor;
            if (null != mPreFocusedCell)
            {
                //mPreFocusedCell.Background = null;
                //mPreFocusedCell.Foreground = Brushes.Black;

                //InplaceBaseEdit cellEditor = (InplaceBaseEdit)mPreFocusedCell.Content;
                //cellEditor.FontWeight = FontWeights.Normal;
                RestoreAppreance(mPreFocusedCell);
            }

            if (null != mPreSelectedCell)
            {
                //mPreSelectedCell.Background = null;
                //mPreSelectedCell.Foreground = Brushes.Black;

                RestoreAppreance(mPreSelectedCell);

            }

            if (null != focuesdCellElmt)
            {
                focuesdCellElmt.Background = Brushes.Orange;

                InplaceBaseEdit cellEditor = (InplaceBaseEdit) focuesdCellElmt.Content;
                cellEditor.FontWeight = FontWeights.Bold;
            }

            if (null != dependentCellElmt)
            {
                dependentCellElmt.Background = Brushes.Red;
            }

            //InplaceBaseEdit cellEditor = (InplaceBaseEdit) cellElmt.Content;
            //cellEditor.FontWeight = FontWeights.Bold;

            mPreFocusedCell = focuesdCellElmt;
            mPreSelectedCell = dependentCellElmt;
            mPreProductName = dependentProduct.Name;
            mPreParameterName = dependentParameterName;
        }

        private void RestoreAppreance(LightweightCellEditor gridCell)
        {
            if (null == gridCell)
                return;

            gridCell.Background = null;
            gridCell.Foreground = Brushes.Black;

            InplaceBaseEdit cellEditor = (InplaceBaseEdit)gridCell.Content;
            cellEditor.FontWeight = FontWeights.Normal;
        }


        private void FocusedCellChanged()
        {
            GridColumn focusedColumn = (GridColumn)mProductInfoViewer.CurrentColumn;

            string culumnCaption = focusedColumn.HeaderCaption.ToString();

            // 若选中的是位于产品名字栏的Cell，则退出;
            if (culumnCaption == "Name")
            {

                if (!string.IsNullOrEmpty(mPreProductName))
                {
                    mPreProductName = "";
                }

                if (!string.IsNullOrEmpty(mPreParameterName))
                {
                    mPreParameterName = "";
                }
                if (null != mPreFocusedCell)
                {
                    RestoreAppreance(mPreFocusedCell);
                    mPreFocusedCell = null;
                }

                if (null != mPreSelectedCell)
                {
                    RestoreAppreance(mPreSelectedCell);
                    mPreSelectedCell = null;
                }
                return;
            }
            string productName = mProductInfoViewer.GetFocusedRowCellDisplayText("Name");

            //mProductInfoViewer.UnselectAll();

            SelectDependentProduct(productName, culumnCaption);
        
        }


    }
}
