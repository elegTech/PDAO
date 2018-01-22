using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;

using P_DAO.DomainEntities;

namespace P_DAO.UIController
{
    class ProductInformationViewer
    {
        // 物理UI
        private DocumentPanel mUIViewer;

        private Grid mUIGrid;

        private TreeListControl mUITreeListControl;

        
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

        public DocumentPanel UIViewer
        {
            get { return mUIViewer; }
        }


    }
}
