/*
 * 产品视图管理器, 用于管理不同层次产品的信息视图, 并负责与前端物理UI的信息交互;
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

using DevExpress.Xpf.Docking;

using P_DAO.DomainEntities;

namespace P_DAO.BusnessLogics
{
    class ProductInfoViewerManager
    {
        DocumentGroup mProductInfoUIViewers;

        public ProductInfoViewerManager(DocumentGroup prodDocGroup)
        {
            mProductInfoUIViewers = prodDocGroup;
        }

        public void CreateProductInformationViewer(Product product)
        {
            if (null == product)
                return;

            DocumentPanel panel = new DocumentPanel();
            panel.Visibility = System.Windows.Visibility.Hidden;
            panel.Caption = product.Name;
            mProductInfoUIViewers.Add(panel);
            panel.Visibility = System.Windows.Visibility.Visible;
        }

    }
}
