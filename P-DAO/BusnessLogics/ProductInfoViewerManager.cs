/*
 * 产品视图管理器, 用于管理不同层次产品的信息视图, 并负责与前端物理UI的信息交互;
 * 产品包含若干子产品，每个子产品也对应一个产品视图(prodcut UI viewer);
 * 当产品结构树中选中其他的产品时，则显示对应的产品视图;
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
using System.Windows.Forms;

using DevExpress.Xpf.Docking;

using P_DAO.DomainEntities;
using P_DAO.UIController;

namespace P_DAO.BusnessLogics
{
    class ProductInfoViewerManager
    {
        DocumentGroup mProductInfoUIViewers;

        Dictionary<Product, ProductInformationViewer> mProduct2ViewerDictionary;

        private Product rootProduct;

        public Product RootProduct
        {
            get { return rootProduct; }
            set { rootProduct = value; }
        }



        public ProductInfoViewerManager(DocumentGroup prodDocGroup)
        {
            mProductInfoUIViewers = prodDocGroup;
            mProduct2ViewerDictionary = new Dictionary<Product, ProductInformationViewer>();
        }


        //public Product GetProductViewer(string name)
        //{
        //    if (String.IsNullOrWhiteSpace(name))
        //        return null;

        //    try
        //    {
        //        KeyValuePair<Product, DocumentPanel> result = mProduct2ViewerDictionary.First(
        //                                                        pair => pair.Key.Name.Equals(name));
        //        return result.Key;
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        MessageBox.Show("该产品未创建：" + name);
        //    }

        //    return null;
        //}


        /// <summary>
        /// Create a new product UI viewer.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public void CreateProductInformationViewer(Product product)
        {
            if (null == product)
                return;
            if (mProduct2ViewerDictionary.ContainsKey(product))
                return;

            DocumentPanel panel = new DocumentPanel();
            panel.Visibility = System.Windows.Visibility.Hidden;
            panel.Caption = product.Name;
            panel.Name = product.Name;
            mProductInfoUIViewers.Add(panel);
            panel.Visibility = System.Windows.Visibility.Visible;
            int index = mProductInfoUIViewers.Items.IndexOf(panel);
            mProductInfoUIViewers.SelectedTabIndex = index;

            ProductInformationViewer productInfoViewer = new ProductInformationViewer(panel);

            mProduct2ViewerDictionary.Add(product, productInfoViewer);
        }

        public void ShowProductInfoViewer(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return;

            Product product = rootProduct.GetChildProduct(productName, rootProduct);

            if (null != product && mProduct2ViewerDictionary.Keys.Contains(product))
            {
                mProduct2ViewerDictionary[product].UIViewer.Visibility = System.Windows.Visibility.Visible;

                int index = mProductInfoUIViewers.Items.IndexOf(mProduct2ViewerDictionary[product].UIViewer);
                mProductInfoUIViewers.SelectedTabIndex = index;
            }
            else
            {
                CreateProductInformationViewer(product);
            }
        }

    }
}
