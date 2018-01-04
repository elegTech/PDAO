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
using P_DAO.BusnessLogics;
using System.IO;
using System.Xml;


namespace P_DAO.DomainEntities
{
    class Product
    {

        #region members
        
        // Product Name
        private string mProductName;

        private XmlDocument mProductXML;

        #endregion


        public string Name
        {
            get { return mProductName; }
            set { mProductName = value; }
        }



        #region Constructors

        // Default constructor.
        public Product()
        {
            this.mProductName = "New Product";
        }

        // Construct an instance from a XML file.
        public Product(string xmlPath)
        {
            LoadProduct(xmlPath);
        }
                
        #endregion
        

        #region Public Logics
        public bool LoadProduct(string xmlFilePath)
        {            
            if (String.IsNullOrWhiteSpace(xmlFilePath))
                return false;

            if (!File.Exists(xmlFilePath))
                return false;

            mProductXML = Utilities.LoadXMLFile(xmlFilePath);
            if (null == mProductXML)
                return false;

            mProductName = mProductXML.DocumentElement.Name;
            return true;
        }


        #endregion

    }
}
