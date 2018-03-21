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
using System.Windows.Forms;

using P_DAO.BusnessLogics;
using P_DAO.UIController;


namespace P_DAO.DomainEntities
{
    class ProductParameter
    {
        public ProductParameter(string name, double minValue, double maxValue)
        {
            this.name = name;
            this.minValue = minValue;
            this.maxValue = maxValue;

            childProductName = string.Empty;
            childProductParameterName = string.Empty;

            mCompatibilityRatio = double.NaN;
        }

        // 对于包含子产品的父产品, 其输入输出参数均由子产品的参数来表示;
        public ProductParameter(string name, string childProdName, string childProdParameterName)
        {
            this.name = name;
            this.childProductName = childProdName;
            this.childProductParameterName = childProdParameterName;

            minValue = double.NaN;
            maxValue = double.NaN;
            mCompatibilityRatio = double.NaN;
        }

        public string name;
        public double minValue;
        public double maxValue;

        // 对任意父产品而言, 其输入/输出参数均为某子产品的
        // 输入/输出参数, 需记录相应的产品名和参数名;
        public string childProductName; 
        public string childProductParameterName;

        // 该参数与相邻产品参数的兼容比值;
        // 以两个参数取值区间的交集区间占各参数取值区间的比;
        public double mCompatibilityRatio;

    }

    class ProductDependency
    {
        public Product sourceProduct;
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

        private double THRESHOLD = 1e-5;
        private static double ONE = 1.000000000000;


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
                if (attr.Name.LocalName.Contains("Input") ||
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
            if (null == mParentProduct)
            {
                foreach (ProductParameter prodPar in mParameterList)
                {
                    prodPar.mCompatibilityRatio = ONE;
                }
            }
            
            mDependencyList = new List<ProductDependency>();
            mProductXML = productXmlElement;

            mChildProductList = new List<Product>();
            mProductName = mProductXML.Attribute("Name").Value;
            productList.Add(this);
            
            mDataTable = GenerateData();
            GenerateProductTree(this);

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
                //GenerateProductTree(childProd);   // 调用本方法进行递归
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

                // 依赖的源产品为当前节点产品或某一子产品
                if (dependencyElmt.Attribute("SourceProduct").Value == this.ID)
                    prodDep.sourceProduct = this;
                else
                    prodDep.sourceProduct = mChildProductList.Find(prod => string.Equals(prod.ID,
                                                       dependencyElmt.Attribute("SourceProduct").Value,
                                                       StringComparison.CurrentCultureIgnoreCase));
                
                // 依赖的目标产品为当前节点产品或某一子产品
                if (dependencyElmt.Attribute("TargetProduct").Value == this.ID)
                    prodDep.targetProduct = this;
                else
                    prodDep.targetProduct = mChildProductList.Find(prod => string.Equals(prod.ID,
                                                            dependencyElmt.Attribute("TargetProduct").Value,
                                                            StringComparison.CurrentCultureIgnoreCase));
               
                prodDep.targetParameter = prodDep.targetProduct.ParameterList.Find(par => string.Equals(par.name,
                                                                                    dependencyElmt.Attribute("TargetPar").Value,
                                                                                    StringComparison.CurrentCultureIgnoreCase));
                prodDep.sourceParameter = prodDep.sourceProduct.ParameterList.Find(par => string.Equals(par.name,
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

        public DataTable GetSubProductCompatibilityInfo()
        {
            foreach (Product child in mChildProductList)
            {
                child.CalculateInterfaceCompatibility();
            }

            DataTable subProductCompatibilityInfoTable = GenerateEmptyTable();

            // 将当前产品参数的兼容性数值放入表中;
            DataRow row = subProductCompatibilityInfoTable.NewRow();
            row["Name"] = Name;

            foreach (ProductParameter par in mParameterList)
            {
                row[par.name] = par.mCompatibilityRatio.ToString("0.00%");
            }

            subProductCompatibilityInfoTable.Rows.Add(row);


            foreach (Product child in mChildProductList)
            {
                row = subProductCompatibilityInfoTable.NewRow();
                row["Name"] = child.Name;

                foreach (ProductParameter par in child.ParameterList)
                {
                    row[par.name] = par.mCompatibilityRatio.ToString("0.00%");
                }

                subProductCompatibilityInfoTable.Rows.Add(row);
            }

            return subProductCompatibilityInfoTable;

        }



        private DataTable GenerateEmptyTable()
        {
            // 记录每个子产品输入输出参数的个数，取最大值;
            int inputParamNumMax = mParameterList.FindAll(par => par.name.StartsWith("Input_",
                    StringComparison.CurrentCultureIgnoreCase)).Count;

            int outputParamNumMax = mParameterList.Count - inputParamNumMax;

            foreach (Product prod in mChildProductList)
            {
                int inputParNum = prod.mParameterList.FindAll(par => par.name.StartsWith("Input_",
                    StringComparison.CurrentCultureIgnoreCase)).Count;

                int onputParNum = prod.mParameterList.Count - inputParNum;

                if (inputParNum > inputParamNumMax)
                    inputParamNumMax = inputParNum;

                if (onputParNum > outputParamNumMax)
                    outputParamNumMax = onputParNum;
            }

            // 构建容纳子产品数据列表表格
            DataTable productInfoTable = new DataTable();

            DataColumn column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Name";
            column.AutoIncrement = false;
            column.Caption = "Product Name";
            column.ReadOnly = true;
            column.Unique = false;
            productInfoTable.Columns.Add(column);

            for (int i = 1; i <= inputParamNumMax; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Input_" + i.ToString();
                column.AutoIncrement = false;
                column.Caption = "Input_" + i.ToString();
                column.ReadOnly = true;
                column.Unique = false;
                productInfoTable.Columns.Add(column);
            }

            for (int i = 1; i <= outputParamNumMax; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Output_" + i.ToString();
                column.AutoIncrement = false;
                column.Caption = "Output_" + i.ToString();
                column.ReadOnly = true;
                column.Unique = false;
                productInfoTable.Columns.Add(column);
            }

            return productInfoTable;
        }



        public DataTable GetSubProductInfo()
        {
            // 构建容纳子产品数据列表表格
            DataTable subProductInfoTable = GenerateEmptyTable();
            
            // 将当前产品信息放入表中;
            DataRow row = subProductInfoTable.NewRow();
            row["Name"] = Name;

            foreach (ProductParameter par in mParameterList)
            {
                row[par.name] = par.minValue.ToString() + "," + par.maxValue.ToString();
            }

            subProductInfoTable.Rows.Add(row);


            foreach (Product child in mChildProductList)
            {
                row = subProductInfoTable.NewRow();
                row["Name"] = child.Name;

                foreach (ProductParameter par in child.ParameterList)
                {
                    row[par.name] = par.minValue.ToString() + "," + par.maxValue.ToString();
                }

                subProductInfoTable.Rows.Add(row);
            }

            return subProductInfoTable;
        }


        /// <summary>
        /// 输出contextProduct
        /// </summary>
        /// <param name="contextProduct">该参数为当前本产品或父产品 </param>
        /// <param name="sourceParameterName"></param>
        /// <param name="targetProduct"></param>
        /// <param name="targetParameterName"></param>
        public void FindDependentParameter(Product contextProduct, string sourceParameterName, ref Product targetProduct, ref string targetParameterName)
        {
            if (string.IsNullOrWhiteSpace(sourceParameterName))
                return;

            ProductDependency productDep = null;

            // 在父产品或当前产品的依赖列表中找依赖产品及参数;
            // Product tempProduct = (null != this.ParentProduct ? this.ParentProduct : this);

            Product tempProduct;
            if (contextProduct == mParentProduct)
                tempProduct = mParentProduct;
            else 
                tempProduct = this;

            productDep = tempProduct.mDependencyList.Find(dep => dep.sourceProduct == this &&
                                                    dep.sourceParameter.name.Equals(sourceParameterName,
                                                    StringComparison.CurrentCultureIgnoreCase));

            if (null == productDep)
            {
                productDep = tempProduct.mDependencyList.Find(dep => dep.targetProduct == this &&
                                    dep.targetParameter.name.Equals(sourceParameterName,
                                    StringComparison.CurrentCultureIgnoreCase));
            }

            if (null == productDep)
            {
                // MessageBox.Show("该产品：" + this.Name + "的参数：" + "sourceParameterName" + "没有" +"依赖项，请检查XML文件中依赖信息是否完整！");               
                return;
            }

            if (productDep.sourceProduct == this)
            {
                targetProduct = productDep.targetProduct;
                targetParameterName = productDep.targetParameter.name;
            }
            else
            {
                targetProduct = productDep.sourceProduct;
                targetParameterName = productDep.sourceParameter.name;
            }
            
        }

        // 根据接口参数取值的区间, 定量计算
        public void CalculateInterfaceCompatibility()
        {
            Product tempDependentProduct = null;
            string tempDependentParameterName = "";
            ProductParameter tempDependentParameter = null;
            foreach(ProductParameter productPar in mParameterList)
            {
                // 若兼容比值已计算则略过;
                if (!double.IsNaN(productPar.mCompatibilityRatio))
                    continue;

                FindDependentParameter(mParentProduct, productPar.name, ref tempDependentProduct, ref tempDependentParameterName);
                tempDependentParameter = tempDependentProduct.GetParameter(tempDependentParameterName);
                    
                // 若当前Product的依赖产品为其父产品, 则参数的兼容比值直接从父Product的依赖参数中获取,无需计算;
                if (tempDependentProduct == mParentProduct)
                {
                    if (!double.IsNaN(tempDependentParameter.mCompatibilityRatio))
                    {
                        productPar.mCompatibilityRatio = tempDependentParameter.mCompatibilityRatio;
                        continue;
                    }
                }

                // 说明两区间无交集;
                if (productPar.maxValue < tempDependentParameter.minValue ||
                    tempDependentParameter.maxValue < productPar.minValue)
                {
                    productPar.mCompatibilityRatio = 0.0;
                }
                // 两区间交集仅为一个孤立的数字;
                else if ((Math.Abs(productPar.maxValue - tempDependentParameter.minValue) < THRESHOLD) ||
                         (Math.Abs(tempDependentParameter.maxValue - productPar.minValue) < THRESHOLD))
                {
                    productPar.mCompatibilityRatio = 0.0;
                }
                // 两区间存在交集，此时求出该交集区间
                else 
                {
                    List<double> list = new List<double>();
                    list.Add(tempDependentParameter.minValue);
                    list.Add(tempDependentParameter.maxValue);
                    list.Add(productPar.minValue);
                    list.Add(productPar.maxValue);
                    list.Sort();

                    // 在交集存在的前提下，中间两个数值为交集区间;
                    double floorValueOfIntersection = list[1];
                    double ceilValueOfIntersection = list[2];

                    productPar.mCompatibilityRatio = Math.Abs(ceilValueOfIntersection - floorValueOfIntersection) / Math.Abs(productPar.maxValue - productPar.minValue);
                    tempDependentParameter.mCompatibilityRatio = Math.Abs(ceilValueOfIntersection - floorValueOfIntersection) / Math.Abs(tempDependentParameter.maxValue - tempDependentParameter.minValue);
                }
            }
        }

    

        public ProductParameter GetParameter(string name)
        {
            if (null == mParameterList || mParameterList.Count == 0)
                return null;

            return mParameterList.Find(par => string.Equals(par.name, name, StringComparison.CurrentCulture));
        }

        
        #endregion

    }
}
