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
        public ProductParameter(string name, double minValue, double maxValue)
        {
            this.name = name;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public string name;
        public double minValue;
        public double maxValue;
    }

    struct ProductDependency
    {
        public ProductParameter sourceParameter;

        public Product targetProduct;
        public ProductParameter targetParameter;
    }


    class Product
    {
        #region members
                
        private string mProductName;
        private List<ProductParameter> mParameterList;
        private List<ProductDependency> mDependencyList;

        private string mID;
        private string mParentID;

        private XElement mProductXML;

        private Product mParentProduct;

        private List<Product> mChildProductList;

        // 为注册全局Product的容器;
        private static List<Product> productList = new List<Product>();

        private DataTable mDataTable;

        #endregion

        public string ID
        {
            get { return mID; }
        }

        public string ParentID
        {
            get { return mParentID; }
        }

        public string Name
        {
            get { return mProductName; }
        }

        public Product ParentProduct
        {
            get { return mParentProduct; }
        }

        public DataTable ProductData
        {
            get { return mDataTable; }
        }

        public List<ProductParameter> ParameterList
        {
            get { return mParameterList; }
        }


        #region Constructors

        // ID=1 说明该产品为顶层产品;
        public Product()
        {
            mParameterList = new List<ProductParameter>();
            mDependencyList = new List<ProductDependency>();
            mChildProductList = new List<Product>();
            mProductXML = new XElement("Product",
                    new XAttribute("Name", "NewProduct"));         
                
            //mProductXML.LoadXml("<Product Name=\"NewProduct\"></Product>");

            this.mID = "1";
            this.mParentID = "0";

            mDataTable = GenerateData();
            mProductName = mProductXML.Attribute("Name").Value;
        }
        

        // 由XML元素节点生成一个Product.
        public Product(XElement productXmlElement, Product parent)
        {
            mParameterList = new List<ProductParameter>();

            List<XAttribute> attrList = productXmlElement.Attributes().ToList();

            foreach (XAttribute attr in attrList)
            {
                if(attr.Name.LocalName.Contains("Input") || 
                   attr.Name.LocalName.Contains("Output"))
                {
                    string[] values = attr.Value.Split(',');
                    ProductParameter par = new ProductParameter(attr.Name.LocalName, 
                                                Double.Parse(values[0]), Double.Parse(values[1]));
                    mParameterList.Add(par);
                }                    
            }

            mID = productXmlElement.Attribute("ID").Value;
            mParentID = productXmlElement.Attribute("ParentID").Value;

            mParentProduct = parent;
            mDependencyList = new List<ProductDependency>();
            mProductXML = productXmlElement;
            mDataTable = GenerateData();
            mChildProductList = new List<Product>();
            mProductName = mProductXML.Attribute("Name").Value;
            GenerateProductTree(this);

            productList.Add(this);
        }
                
        #endregion


        #region Private logics

        private void AddChildProduct(Product child)
        {
            if (null == child)
                return;

            mChildProductList.Add(child);
        }


        private void GenerateProductTree(Product product)
        {
            if (null == product)
                return;

            var childProductList = product.mProductXML.Elements("Product").ToList();
            foreach (XElement childProdElmt in childProductList)  //循环遍历当前元素的子元素集合
            {
                Product childProd = new Product(childProdElmt, product);  // 定义一个Product对象
                product.AddChildProduct(childProd);
                GenerateProductTree(childProd);   // 调用本方法进行递归
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



        #endregion

        #region Public Logics


        // 处理依赖于该Product参数的其他其他参数, 创建依赖关系;
        // 该方法需在文档根节点产品被创建后调用;
        public void GenerateProductDependency()
        {
            // 为空或无子节点时退出;
            if (null == this.mProductXML || false == this.mProductXML.HasElements)
                return;

            var dependencyList = this.mProductXML.Elements("Dependency").ToList();
            foreach (XElement dependencyElmt in dependencyList)
            {
                ProductDependency prodDep = new ProductDependency();
                prodDep.targetProduct = this.ParentProduct.mChildProductList.Find(prod => string.Equals(prod.ID, 
                                                            dependencyElmt.Attribute("ID").Value,
                                                            StringComparison.CurrentCultureIgnoreCase));

                prodDep.targetParameter = prodDep.targetProduct.ParameterList.Find(par => string.Equals(par.name,
                                                                                    dependencyElmt.Attribute("TargetPar").Value,
                                                                                    StringComparison.CurrentCultureIgnoreCase));
                prodDep.sourceParameter = this.ParameterList.Find(par => string.Equals(par.name,
                                                                                    dependencyElmt.Attribute("SourcePar").Value,
                                                                                    StringComparison.CurrentCultureIgnoreCase));
                this.mDependencyList.Add(prodDep);                
            }

            foreach (Product childProduct in this.mChildProductList)  //循环遍历当前Product元素的子Product元素集合
            {
                childProduct.GenerateProductDependency();   // 调用本方法进行递归
            }
        }



        /// <summary>
        /// Save product information as a xml file.
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            if (null == filePath)
                return;

            XmlWriter writer = XmlWriter.Create(filePath);
            mProductXML.Save(writer);
            writer.Close();
        }


        // 获取具有指定名称的产品
        public static Product GetProduct(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return null;

            return productList.Find(prod => string.Equals(productName, prod.Name, 
                                        StringComparison.CurrentCultureIgnoreCase));
        }


        public DataTable GetSubProductInfo()
        {
            if (null == mProductXML.Element("Product"))
                return null;

            // 记录每个子产品输入输出参数的个数，取最大值;
            int columnNumMax = 0;
            foreach (Product prod in mChildProductList)
            {
                if (prod.mParameterList.Count > columnNumMax)
                    columnNumMax = prod.mParameterList.Count;
            }

            if (columnNumMax == 0)
                return null;

            // 构建容纳子产品数据列表表格
            DataTable subProductInfoTable = new DataTable();

            DataColumn column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Name";
            column.AutoIncrement = false;
            column.Caption = "Product Name";
            column.ReadOnly = true;
            column.Unique = false;
            subProductInfoTable.Columns.Add(column);

            for (int i = 0; i < columnNumMax; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Parameter" + i.ToString();
                column.AutoIncrement = false;
                column.Caption = "Product" + i.ToString();
                column.ReadOnly = true;
                column.Unique = false;
                subProductInfoTable.Columns.Add(column);
            }


            return subProductInfoTable;
        }

        #endregion

    }
}
