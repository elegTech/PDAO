using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Grid;


namespace P_DAO.UIController
{
    class ProductStructureViewer
    {
#region members

        private TreeListControl mProductUIViewer;
        
#endregion


        public TreeListControl UIViewer
        {
            get { return mProductUIViewer; }
            set { mProductUIViewer = value;}
        }





        #region Constructors

        public ProductStructureViewer(TreeListControl uiViewer)
        {
            mProductUIViewer = uiViewer;
        }

        #endregion





    }
}
