/*
 * 提供公共工具方法;
 * 
 * 
 * 作者: 樊红日;
 * 日期: 2018.1.3;
 * 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace P_DAO.BusnessLogics
{
    class Utilities
    {
        // Load XML a file.
        public static XmlDocument LoadXMLFile(string xmlPath)
        {
            if (String.IsNullOrWhiteSpace(xmlPath))
                return null;

            if (!File.Exists(xmlPath))
                return null;

            XmlDocument xmlDoc = new XmlDocument();
            
            try
            {
                xmlDoc.Load(xmlPath);
            }
            catch
            {
                return null;   
            }

            return xmlDoc;
        }
        







    }
}
