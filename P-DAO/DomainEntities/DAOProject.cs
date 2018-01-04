using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using DevExpress.Xpf.Docking;


using P_DAO;
using P_DAO.UIController;
using P_DAO.BusnessLogics;

using ProdInfoViewerList = System.Collections.Generic.List<P_DAO.UIController.ProductInformationViewer>; 

namespace P_DAO.DomainEntities
{
    class DAOProject
    {
        private string mProjectName = "No Project";
        private Product mProduct;

        // 主UI框架
        public MainWindow mMainUI;
        public ProductStructureViewer mLogicProductStructureViewer;
        public ProductInfoViewerManager mProdInfoViewerMgr;
        

        public Product DesignProduct
        {
            get { return mProduct; }
            set 
            { 
                mProduct = value;
                if (null != mProduct)
                    mProjectName = mProduct.Name;
            }
        }

        public DAOProject(MainWindow uiWindow)
        {
            mMainUI = uiWindow;
            InitializeComponents();


        }

        /// <summary>
        /// 将物理UI与UI控制器相关联
        /// </summary>
        private void InitializeComponents()
        {
            mLogicProductStructureViewer = new ProductStructureViewer(mMainUI.productStructureTree);

            // Use the root product name to initilize the first product information viewer UI.
            string documentGroupName = "ProductGroup";
            DependencyObject documentGroup = LogicalTreeHelper.FindLogicalNode(mMainUI, documentGroupName);
            if (documentGroup is DocumentGroup)
            {
                DocumentGroup group = documentGroup as DocumentGroup;
                DocumentPanel panel = new DocumentPanel();
                panel.Caption = mProduct.Name;
                group.Add(panel);
                mProdInfoViewerMgr = new ProductInfoViewerManager(group);
            }
            



        }


    }
}
