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
        public readonly static string IDENTIFIERATTRNAME = "ID";
        public readonly static string PARENTIDENTIFIERATTRNAME = "ParentID";
        public readonly static string INPUTATTRNAME = "Input";
        public readonly static string OUTPUTATTRNAME = "Output";
        public readonly static string ATTRNUMSEPARATOR = "_";


        public readonly static string DEPNODENAME = "Dependency";


        public readonly static string SOURCEPRODATTRNAME = "SourceProduct";
        public readonly static string TARGETPRODATTRNAME = "TargetProduct";
        public readonly static string SOURCEPARATTRNAME = "SourcePar";
        public readonly static string TARGETPARATTRNAME = "TargetPar";

        // 数字保留的精度
        public static string PRECISION = "n2";
        public readonly static string CULTURE = "zh-CN";


        //合法的XML文档的根Product节点应该有ID,ParentID,Input和output等属性
        public static bool IsLegal(XDocument xml)
        {
            if (null == xml)
                return false;

            // 若无ID属性
            if (null == xml.Root.Attribute(Utilities.IDENTIFIERATTRNAME))
                return false;

            // 若无指向父节点ID的属性
            if (null == xml.Root.Attribute(Utilities.PARENTIDENTIFIERATTRNAME))
                return false;

            bool hasInputAtt = false;
            bool hasOutputAtt = false;

            List<XAttribute> attrList = xml.Root.Attributes().ToList();
            foreach (XAttribute attr in attrList)
            {
                if (attr.Name.LocalName.Contains("Input"))
                    hasInputAtt = true;

                else if (attr.Name.LocalName.Contains("Output"))
                    hasOutputAtt = true;
            }


            // 若既无Input, 又无Output属性,则返回false
            return (hasInputAtt && hasOutputAtt) ? true : false;
        }


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
