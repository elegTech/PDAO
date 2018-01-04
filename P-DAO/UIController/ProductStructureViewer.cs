using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Grid;
using System.Xml;

using P_DAO.DomainEntities;


namespace P_DAO.UIController
{
    class ProductStructureViewer
    {
        #region members

        private TreeListControl mProductUIViewer;

        private XmlDocument mProductData;



        #endregion


        public TreeListControl UIViewer
        {
            get { return mProductUIViewer; }
            set { mProductUIViewer = value;}
        }

        public XmlDocument DataSource
        {
            get { return mProductData; }
            set { mProductData = value; }
        }



        #region Constructors

        public ProductStructureViewer(TreeListControl uiViewer)
        {
            mProductUIViewer = uiViewer;
        }

        #endregion


        #region
        public void Refresh(Product product)
        {
            if (null == product)
            {
                mProductUIViewer.ItemsSource = null;
                return;
            }

            mProductUIViewer.ItemsSource = product.ProductData;
            mProductUIViewer.RefreshData();
        }


        #endregion



    }
}
