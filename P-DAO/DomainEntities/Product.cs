//---------------------------------------------------------------------------
// 产品类，用于表示一个独立的产品设计任务，任何时候仅能进行一个产品设计活动;
// 产品包含若干子产品，逐层包含形成树状产品结构 
// 
// 作者: 樊红日;
// 日期: 2018.1.3;
// 
//---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data;


using P_DAO.BusnessLogics;

using P_DAO.UIController;


namespace P_DAO.DomainEntities
{
    class Product
    {
        #region members
        
        private string mProductName;
        private string mParMinValue;
        private string mParMaxValue;

        private XmlDocument mProductXML;

        private List<Product> mChildProductList;

        private DataTable mDataTable;

        #endregion


        public string Name
        {
            get { return mProductName; }
            set { mProductName = value; }
        }

        public DataTable ProductData
        {
            get { return mDataTable; }
        }



        #region Constructors

        // Default constructor.
        public Product()
        {
            //mSubProductList = new List<Product>();
            mProductXML = new XmlDocument();
            mProductXML.LoadXml("<Product Name=\"NewProduct\"></Product>");
            mDataTable = GenerateData();
            mProductName = mProductXML.DocumentElement.GetAttribute("Name");
        }
        

        // Construct an instance from a XML file.
        public Product(XmlDocument productXmlDoc)
        {
            //mSubProductList = new List<Product>();
            mProductXML = productXmlDoc;
            mDataTable = GenerateData();
            mChildProductList = new List<Product>();
            mProductName = mProductXML.DocumentElement.GetAttribute("Name");
            GenerateProductTree(mProductXML.DocumentElement, this);
            
        }
                
        #endregion


        #region Private logics


        private void AddChildProduct(Product child)
        {
            if (null == child)
                return;

            mChildProductList.Add(child);
        }


        private void GenerateProductTree(XmlNode xmlNode, Product parentProduct)
        {
            if (null == xmlNode || null == parentProduct)
                return;

            foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(node.OuterXml);
                Product child = new Product(doc);// 定义一个Product对象
                parentProduct.AddChildProduct(child);
                GenerateProductTree(node, child);// 调用本方法进行递归
            }
        }


        private DataTable GenerateData()
        {
            if (null == mProductXML)
                return null;

            DataSet ds = Utilities.ConvertXMLToDataSet(mProductXML.InnerXml);
            if (null != ds && ds.Tables.Count > 0)
                return ds.Tables[0];

            return null;
        }


        #endregion

        #region Public Logics

        /// <summary>
        /// Save product information as a xml file.
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            if (null == filePath)
                return;

            mProductXML.Save(XmlWriter.Create(filePath));
        }


        // 获取具有制定产品名的
        public Product GetChildProduct(string productName, Product product)
        {
            if (string.IsNullOrWhiteSpace(productName) || null == product)
                return null;

            if (string.Equals(product.Name, productName, StringComparison.CurrentCultureIgnoreCase))
                return product;

            Product resultProduct = null;
            foreach (Product childProduct in mChildProductList)
            {
                resultProduct = GetChildProduct(productName, childProduct);
                if (null != resultProduct)
                    break;
            }
            return resultProduct;
        }



        #endregion

    }
}
