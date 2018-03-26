using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;

using P_DAO.DomainEntities;

namespace P_DAO.UIController
{
    class ProductViewerBase
    {
        public ProductViewerBase(DocumentPanel uIViewer, Product product)  
        {
            this.mUIViewer = uIViewer;
            this.mUIGrid = new Grid();
            this.mProduct = product;

            this.mProductInfoContainer = new GridControl();
            this.mProductInfoContainer.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;
        }


        protected DocumentPanel mUIViewer;

        protected Grid mUIGrid;

        protected GridControl mProductInfoContainer;

        protected Product mProduct;

        protected delegate void FocusedCellChangedEventHandler();

        protected event FocusedCellChangedEventHandler FocusedCellChangedEvent;


        public DocumentPanel UIViewer
        {
            get { return mUIViewer; }
        }

        public GridControl InfoContainer
        {
            get { return mProductInfoContainer; }
        }

        public void InvokeFocusedCellChangedEvent()
        {
            FocusedCellChangedEvent();
        }

        public Product Product
        {

            get { return mProduct; }
        }

    }
}
