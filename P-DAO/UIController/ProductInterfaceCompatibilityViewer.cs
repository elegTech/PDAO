using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;

using P_DAO.DomainEntities;

namespace P_DAO.UIController
{
    class ProductInterfaceCompatibilityViewer : ProductViewerBase
    {
        public ProductInterfaceCompatibilityViewer(DocumentPanel uiViewer, Product product)
            : base(uiViewer, product)
        {
            mProductInfoViewer.ItemsSource = product.GetSubProductCompatibilityInfo();

            // 设置表格视图显示属性;
            TableView view = (TableView)mProductInfoViewer.View;
            // 隐藏列排序设置面板, 用户可通过右键菜单显示;
            view.ShowGroupPanel = false;

            view.AllowEditing = false;

            //view.CellStyle = (Style)Application.Current.Resources["FocusedCellStyle1"];

            //view.RowStyle = (Style)Application.Current.Resources["SelectedRowStyle1"];
            
            view.ShowFocusedRectangle = true;

            view.UseIndicatorForSelection = true;

            mUIGrid.Children.Add(mProductInfoViewer);

            mUIViewer.Content = mUIGrid;

            //view.UseLightweightTemplates = UseLightweightTemplates.All;

            //mProductInfoViewer.SelectionMode = MultiSelectMode.Cell;

            //view.NavigationStyle = GridViewNavigationStyle.Cell;

            //mProductInfoViewer.CurrentColumnChanged += CurrentColumnChanged;
            //mProductInfoViewer.CurrentItemChanged += CurrentItemChanged;



        
        }


    }
}
