/*
 * 产品类，用于表示一个独立的产品设计任务，任何时候仅能进行一个产品设计活动;
 *  
 * 
 * 作者: 樊红日;
 * 日期: 2018.1.3;
 * 
 * */
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

        private List<Product> mSubProductList;

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
            mProductXML.Load(XmlReader.Create(@"C:\CodeProjects\P-DAO\Resources\testXML.xml"));
            mDataTable = GenerateData();
        }

        // Construct an instance from a XML file.
        public Product(XmlDocument productXmlDoc)
        {
            //mSubProductList = new List<Product>();
            mProductXML = productXmlDoc;
            mDataTable = GenerateData();
        }
                
        #endregion


        #region Private logics

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





        #endregion

    }
}
