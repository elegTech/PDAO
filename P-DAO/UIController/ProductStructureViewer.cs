﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Grid;
using System.Xml;
using System.Data;

using P_DAO.DomainEntities;


namespace P_DAO.UIController
{
    class ProductStructureViewer
    {
        #region members

        private TreeListControl mProductUIViewer;

        private DataTable mProductData;

        #endregion


        public TreeListControl UIViewer
        {
            get { return mProductUIViewer; }
            set { mProductUIViewer = value;}
        }

        public DataTable ProductData
        {
            get { return mProductData; }
            set { mProductData = value; }
        }



        #region Constructors

        public ProductStructureViewer(TreeListControl uiViewer)
        {
            mProductUIViewer = uiViewer;
        }

        #endregion


        #region
        public void Refresh(Product product)
        {
            if (null == product)
            {
                mProductUIViewer.ItemsSource = null;
                return;
            }
            mProductData = product.ProductData;

            // 在产品结构树中仅仅显示产品名, 其他信息略去;
            DataTable productStructureTree = mProductData.DefaultView.ToTable(false, new string[] { "Name", "ID", "ParentID" });

            mProductUIViewer.ItemsSource = productStructureTree;
            mProductUIViewer.RefreshData();
        }

        public string SelectedNodeName()
        {
            TreeListNode node = mProductUIViewer.GetSelectedNodes()[0];
            DataRowView rowView = (DataRowView)node.Content;
            DataRow data = rowView.Row;

            return ((string)data["Name"]);       
        }


        #endregion



    }
}
