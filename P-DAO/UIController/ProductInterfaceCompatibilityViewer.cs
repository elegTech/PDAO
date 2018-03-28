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
        private DependencySelector mDependencySelector;



        public ProductInterfaceCompatibilityViewer(DocumentPanel uiViewer, Product product)
            : base(uiViewer, product)
        {
            mProductInfoContainer.ItemsSource = product.GetSubProductCompatibilityInfo();

            mProductInfoContainer.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;

            // 设置表格视图显示属性;
            TableView view = (TableView)mProductInfoContainer.View;
            // 隐藏列排序设置面板, 用户可通过右键菜单显示;
            view.ShowGroupPanel = false;

            view.AllowConditionalFormattingMenu = true;

            view.AllowEditing = false;

            view.CellStyle = (Style)Application.Current.Resources["FocusedCellStyle1"];

            //view.RowStyle = (Style)Application.Current.Resources["SelectedRowStyle1"];
            
            view.ShowFocusedRectangle = true;

            view.UseIndicatorForSelection = true;

            mUIGrid.Children.Add(mProductInfoContainer);

            mUIViewer.Content = mUIGrid;

            view.UseLightweightTemplates = UseLightweightTemplates.All;

            mProductInfoContainer.SelectionMode = MultiSelectMode.Cell;

            view.NavigationStyle = GridViewNavigationStyle.Cell;

            mProductInfoContainer.CurrentColumnChanged += CurrentColumnChanged;
            mProductInfoContainer.CurrentItemChanged += CurrentItemChanged;

            mDependencySelector = new DependencySelector(this);

            FocusedCellChangedEvent += mDependencySelector.ChangeFocusedCell;        
        }

        public void CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            InvokeFocusedCellChangedEvent();
            //FocusedCellChangedEvent();
            //ChangeFocusedCell();
            return;
        }

        public void CurrentColumnChanged(object sender, CurrentColumnChangedEventArgs e)
        {
            InvokeFocusedCellChangedEvent();
            //FocusedCellChangedEvent();
            //ChangeFocusedCell();
            return;
        }


    }
}
