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
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Data;
using System.Windows.Forms;


namespace P_DAO.BusnessLogics
{
    class Utilities
    {
        // Load XML a file.
        public static XDocument LoadXMLFile(string xmlPath)
        {
            if (String.IsNullOrWhiteSpace(xmlPath))
                return null;

            if (!File.Exists(xmlPath))
                return null;

            XDocument xmlDoc = null;
            
            try
            {
                xmlDoc = XDocument.Load(xmlPath);
            }
            catch
            {
                MessageBox.Show("该文件无法加载或已被加载！");
                return null;   
            }
            
            return xmlDoc;
        }

        public static DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                string strTest = ex.Message;
                return null;
            }
            finally
            {
                if (null != reader)
                    reader.Close();
                if (null != stream)
                    stream.Close();
            }
        }








    }
}
