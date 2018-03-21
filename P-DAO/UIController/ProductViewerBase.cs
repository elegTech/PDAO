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
        protected ProductViewerBase(DocumentPanel uIViewer, Product product)  
        {
            this.mUIViewer = uIViewer;
            this.mUIGrid = new Grid();
            this.mProduct = product;

            this.mProductInfoViewer = new GridControl();
            this.mProductInfoViewer.AutoGenerateColumns = AutoGenerateColumnsMode.AddNew;
        }


        protected DocumentPanel mUIViewer;

        protected Grid mUIGrid;

        protected GridControl mProductInfoViewer;

        protected Product mProduct;

        public DocumentPanel UIViewer
        {
            get { return mUIViewer; }
        }

        public Product Product
        {
            get { return mProduct; }
        }

    }
}
