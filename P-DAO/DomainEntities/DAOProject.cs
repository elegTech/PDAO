using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using DevExpress.Xpf.Docking;


using P_DAO;
using P_DAO.UIController;
using P_DAO.BusnessLogics;
using P_DAO.Serialization;

using ProdInfoViewerList = System.Collections.Generic.List<P_DAO.UIController.ProductInformationViewer>;
using DevExpress.Xpf.Grid;
using System.Data; 

namespace P_DAO.DomainEntities
{
    class DAOProject
    {

        public delegate void ProductCloseEventHandler(Product product);
        public delegate void ProductCreateEventHandler(Product product);
        public delegate void ProductLoadEventHandler(Product product);
        public delegate void ProductActivateEventHandler(string productName);

        public event ProductCloseEventHandler CloseEvent;
        public event ProductCreateEventHandler ProductCreateAfterEvent;
        public event ProductLoadEventHandler ProductLoadAfterEvent;
        public event ProductActivateEventHandler ProductActivateEvent;



        private string mProjectName = "No Project";
        
        // 表示最高层产品
        private Product mRootProduct;


        private string productFilePath; 

        // 主UI框架
        public MainWindow mMainUI;
        public ProductStructureViewer mLogicProductStructureViewer;
        public ProductInfoViewerManager mProdInfoViewerMgr;
        

        public Product DesignProduct
        {
            get { return mRootProduct; }
            set 
            {
                mRootProduct = value;
                if (null != mRootProduct)
                    mProjectName = mRootProduct.Name;
            }
        }


        public DAOProject(MainWindow uiWindow)
        {
            mMainUI = uiWindow;
            InitializeComponents();


        }

        /// <summary>
        /// 将物理UI与UI控制器(逻辑UI)相关联
        /// </summary>
        private void InitializeComponents()
        {
            if (null == mMainUI)
                return;

            // 初始化产品树形结构视图
            mLogicProductStructureViewer = new ProductStructureViewer(mMainUI.productStructureTree);
            

            // 初始化时根目录产品为当前活动产品，因此
            // 使用根目录产品名称为产品信息视图窗口名赋值.

            string documentGroupName = "ProductGroup";
            DocumentGroup documentGroup = (DocumentGroup)mMainUI.FindName(documentGroupName);
            mProdInfoViewerMgr = new ProductInfoViewerManager(documentGroup);

            CloseEvent += mLogicProductStructureViewer.Refresh;
            ProductCreateAfterEvent += mLogicProductStructureViewer.Refresh;
            ProductLoadAfterEvent += mLogicProductStructureViewer.Refresh;

            ProductCreateAfterEvent += mProdInfoViewerMgr.CreateProductInformationViewer;
            ProductLoadAfterEvent += mProdInfoViewerMgr.CreateProductInformationViewer;
            ProductActivateEvent += mProdInfoViewerMgr.ShowProductInfoViewer;
        }


        public bool OpenProuduct()
        {
            if (null != mRootProduct)
                mRootProduct.Save(productFilePath);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "XML文件|*.xml";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                productFilePath = openFileDialog.FileName;
                mRootProduct = Serializer.LoadProduct(productFilePath);
            }

            if (null != mRootProduct)
            {
                mProdInfoViewerMgr.RootProduct = mRootProduct;
                ProductCreateAfterEvent(mRootProduct);
                return true;
            }

            return false;
        }


        public void CreateNewProuduct()
        {
            // Save the current product design information.
            if (null != mRootProduct)
            {
                mRootProduct.Save(productFilePath);                
            }

            mRootProduct = new Product();
            mProdInfoViewerMgr.RootProduct = mRootProduct;
            ProductLoadAfterEvent(mRootProduct);
        }


        public void ActivateProduct()
        {
            TreeListControl ctrl = (TreeListControl)mMainUI.FindName("productStructureTree");
            TreeListNode node = ctrl.GetSelectedNodes()[0];
            DataRowView rowView = (DataRowView)node.Content;
            DataRow data = rowView.Row;

            ProductActivateEvent((string)data["Name"]);        
        }

    }
}
