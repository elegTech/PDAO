using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using P_DAO.BusnessLogics;
using P_DAO.DomainEntities;

namespace P_DAO.Serialization
{
    class Serializer
    {
        public static Product LoadProduct(string xmlFilePath)
        {
            if (String.IsNullOrWhiteSpace(xmlFilePath))
                return null;

            if (!File.Exists(xmlFilePath))
                return null;

            XmlDocument xml = Utilities.LoadXMLFile(xmlFilePath);

            if (null != xml)
                return new Product(xml);

            return null;
        }

        public static Product SaveProduct(string xmlFilePath)
        {
            if (String.IsNullOrWhiteSpace(xmlFilePath))
                return null;

            XmlDocument xml = Utilities.LoadXMLFile(xmlFilePath);

            if (null != xml)
                return new Product(xml);

            return null;
        }

    }
}
