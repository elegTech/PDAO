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
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data;


using P_DAO.BusnessLogics;
using P_DAO.UIController;


namespace P_DAO.DomainEntities
{
    struct ProductParameter
    {     
        string name;
        double minValue;
        double maxValue;
    }

    struct ProductDependency
    {
        Product sourceProduct;
        ProductParameter sourceParameter;

        Product targetProduct;
        ProductParameter targetParameter;
    }


    class Product
    {
        #region members
        
        private string mProductName;
        private List<ProductParameter> parameterList;
        private List<ProductDependency> dependencyList;



        private XElement mProductXML;

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
            parameterList = new List<ProductParameter>();
            dependencyList = new List<ProductDependency>();
            mChildProductList = new List<Product>();
            mProductXML = new XElement("Product",
                    new XAttribute("Name", "NewProduct"));         
                
            //mProductXML.LoadXml("<Product Name=\"NewProduct\"></Product>");


            mDataTable = GenerateData();
            mProductName = mProductXML.Attribute("Name").Value;
        }
        

        // Construct an instance from a XML file.
        public Product(XElement productXmlElement)
        {
            parameterList = new List<ProductParameter>();
            dependencyList = new List<ProductDependency>();
            mProductXML = productXmlElement;
            mDataTable = GenerateData();
            mChildProductList = new List<Product>();
            mProductName = mProductXML.Attribute("Name").Value;
            GenerateProductTree(mProductXML, this);
            
        }
                
        #endregion


        #region Private logics


        private void AddChildProduct(Product child)
        {
            if (null == child)
                return;

            mChildProductList.Add(child);
        }


        private void GenerateProductTree(XElement xmlNode, Product parentProduct)
        {
            if (null == xmlNode || null == parentProduct)
                return;

            // 若该节点不是Product节点;
            if (string.Equals(xmlNode.Name.LocalName, "Product", StringComparison.CurrentCultureIgnoreCase))
                return;

            foreach (XElement node in xmlNode.Elements("Product"))//循环遍历当前元素的子元素集合
            {
                Product child = new Product(node);// 定义一个Product对象
                parentProduct.AddChildProduct(child);
                GenerateProductTree(node, child);// 调用本方法进行递归
            }
        }


        private DataTable GenerateData()
        {
            if (null == mProductXML)
                return null;

            //DataSet ds = Utilities.ConvertXMLToDataSet(mProductXML.InnerXml);

            DataSet ds = Utilities.ConvertXMLToDataSet(mProductXML.ToString());            
            if (null != ds && ds.Tables.Count > 0)
                return ds.Tables[0];

            return null;
        }



        // 计算子Product的参数取值区间的匹配度
        private void ProcessInterface4ChildProduct()
        {
            if (null == mProductXML)
                return;

            var dependencyList = mProductXML.Elements("Dependency").ToList();
            if (null == dependencyList || dependencyList.Count == 0)
                return;

            XmlNode sourceNode = null;
            XmlNode targetNode = null;
            XmlAttribute sourceID;
            XmlAttribute targetID;
            foreach (XmlNode node in dependencyList)
            {
                sourceID = (XmlAttribute)node.Attributes.GetNamedItem("SourceID");
                targetID = (XmlAttribute)node.Attributes.GetNamedItem("TargetID");

                sourceNode = mProductXML.GetElementById(sourceID.Value);
                targetNode = mProductXML.GetElementById(targetID.Value);



            }


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
