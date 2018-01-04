/*
 * 注册系统各类菜单事件处理函数;
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

using DevExpress.Xpf.Bars;

using P_DAO.DomainEntities;
using P_DAO;



namespace P_DAO.BusnessLogics
{
    class EventManager
    {
        public DAOProject mProject;
        public MainWindow mMainUI;

        public void RegisterOpenButton()
        {
            BarButtonItem openBtn = (BarButtonItem) mMainUI.FindName("biOpen");
        }



    }
}
