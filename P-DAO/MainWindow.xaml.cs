using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using P_DAO.DomainEntities;

namespace P_DAO
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DAOProject designProject;


        public MainWindow()
        {
            InitializeComponent();
                        
            productStructureTree.ItemsSource = Stuff.GetStuff();

            designProject = new DAOProject(this); 
        }

        private void biOpen_ItemClick_1(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            designProject.OpenProuduct();
        }

        private void biNew_ItemClick_1(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            designProject.CreateNewProuduct();
        }
    }
}
