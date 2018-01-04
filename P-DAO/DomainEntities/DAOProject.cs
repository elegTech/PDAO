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

namespace P_DAO.DomainEntities
{
    class DAOProject
    {

        public delegate void ProductCloseEventHandler(Product product);
        public delegate void ProductCreateEventHandler(Product product);
        public delegate void ProductLoadEventHandler(Product product);

        public event ProductCloseEventHandler CloseEvent;
        public event ProductCreateEventHandler CreateEvent;
        public event ProductLoadEventHandler LoadEvent;



        private string mProjectName = "No Project";
        private Product mProduct;

        private string productFilePath; 

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
        /// 将物理UI与UI控制器(逻辑UI)相关联
        /// </summary>
        private void InitializeComponents()
        {
            if (null == mMainUI || null == mProduct)
                return;

            // 初始化产品树形结构视图
            mLogicProductStructureViewer = new ProductStructureViewer(mMainUI.productStructureTree);

            // 初始化时根目录产品为当前活动产品，因此
            // 使用根目录产品名称为产品信息视图窗口名赋值.
            string documentGroupName = "ProductGroup";
            object documentGroup = mMainUI.FindName(documentGroupName);
            if (documentGroup is DocumentGroup)
            {
                DocumentGroup group = documentGroup as DocumentGroup;
                DocumentPanel panel = new DocumentPanel();
                panel.Caption = mProduct.Name;
                group.Add(panel);
                mProdInfoViewerMgr = new ProductInfoViewerManager(group);
            }

            CloseEvent += mLogicProductStructureViewer.Refresh;
            CreateEvent += mLogicProductStructureViewer.Refresh;
            LoadEvent += mLogicProductStructureViewer.Refresh;
        }

        public bool OpenProuduct()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "XML文件|*.xml";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                productFilePath = openFileDialog.FileName;
                mProduct = Serializer.LoadProduct(productFilePath);
            }

            if (null != mProduct)
                return true;

            return false;
        }


        public void CreateNewProuduct()
        {
            // Save the current product design information.
            if (null != mProduct)
            {
                mProduct.Save(productFilePath);                
            }

            mProduct = new Product();
            mLogicProductStructureViewer.DataSource = mProduct.ProductData;
            mLogicProductStructureViewer.Refresh(mProduct);
        }


    }
}
