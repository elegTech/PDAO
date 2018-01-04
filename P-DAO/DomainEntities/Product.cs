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

using P_DAO.BusnessLogics;

using P_DAO.UIController;


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

        public XmlDocument ProductData
        {
            get { return mProductXML; }
            set { mProductXML = value; }
        }



        #region Constructors

        // Default constructor.
        public Product()
        {
            this.mProductName = "New Product";

            mProductXML = new XmlDocument();

            mProductXML.LoadXml("<Product>" + mProductName + "</Product>");
        }

        // Construct an instance from a XML file.
        public Product(XmlDocument productXmlDoc)
        {
            mProductXML = productXmlDoc;
        }
                
        #endregion


        #region Private logics
        


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
