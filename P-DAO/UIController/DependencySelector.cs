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
    class DependencySelector
    {
        private ProductViewerBase mProductViewer;


        // 由于每个依赖的参数显示单元格Cell是在代码中手动设置其控件的背景色,
        // 需要记录前一个获取焦点的Cell位置; 待下次选择其他Cell时将前一被选中的Cell恢复原状;
        private LightweightCellEditor mPreSelectedCell;

        // 记录前一个获取焦点的单元格的UIElement;
        // 获取焦点的Cell需要手动更改其背景颜色,
        // 当该Cell失去焦点时同样要手动恢复原有背景颜色;
        private LightweightCellEditor mPreFocusedCell;

        private string mPreProductName;

        private string mPreParameterName;


        public DependencySelector(ProductViewerBase viewer)
        {
            this.mProductViewer = viewer;
        }

        public void ChangeFocusedCell()
        {
            GridColumn focusedColumn = (GridColumn)mProductViewer.InfoContainer.CurrentColumn;

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
            string productName = mProductViewer.InfoContainer.GetFocusedRowCellDisplayText("Name");

            //mProductInfoViewer.UnselectAll();

            SelectDependentProduct(productName, culumnCaption);       
        
        }


        public void SelectDependentProduct(string productName, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(parameterName))
                return;

            Product dependentProduct = null;
            string dependentParameterName = string.Empty;

            Product focusedProduct = Product.GetProduct(productName);
            focusedProduct.FindDependentParameter(mProductViewer.Product, parameterName, ref dependentProduct, ref dependentParameterName);

            LightweightCellEditor focuesdCellElmt;
            int focusedRowHandle;
            TableView view = (TableView)mProductViewer.InfoContainer.View;

            // 若在当前上下文中没有找到依赖产品参数,说明该产品已经是叶子产品,仅需高亮该获取焦点的Cell即可;
            if (null == dependentProduct)
            {
                if (null != mPreFocusedCell)
                {
                    RestoreAppreance(mPreFocusedCell);
                }

                focusedRowHandle = mProductViewer.InfoContainer.FindRowByValue("Name", productName);
                focuesdCellElmt = (LightweightCellEditor)view.GetCellElementByRowHandleAndColumn(focusedRowHandle, mProductViewer.InfoContainer.Columns.First(col => col.HeaderCaption.ToString().Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)));
                focusedRowHandle = mProductViewer.InfoContainer.FindRowByValue("Name", productName);

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


            focusedRowHandle = mProductViewer.InfoContainer.FindRowByValue("Name", productName);

            neighborRowHandle = mProductViewer.InfoContainer.FindRowByValue("Name", dependentProduct.Name);

            dependentCellElmt = (LightweightCellEditor)view.GetCellElementByRowHandleAndColumn(neighborRowHandle, mProductViewer.InfoContainer.Columns.First(col => col.HeaderCaption.ToString().Equals(dependentParameterName, StringComparison.CurrentCultureIgnoreCase)));

            focuesdCellElmt = (LightweightCellEditor)view.GetCellElementByRowHandleAndColumn(focusedRowHandle, mProductViewer.InfoContainer.Columns.First(col => col.HeaderCaption.ToString().Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)));


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

                InplaceBaseEdit cellEditor = (InplaceBaseEdit)focuesdCellElmt.Content;
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


        // 将相应Cell的外观恢复初始默认状态;
        private void RestoreAppreance(LightweightCellEditor gridCell)
        {
            if (null == gridCell)
                return;

            gridCell.Background = null;
            gridCell.Foreground = Brushes.Black;

            InplaceBaseEdit cellEditor = (InplaceBaseEdit)gridCell.Content;
            cellEditor.FontWeight = FontWeights.Normal;
        }





    }
}
