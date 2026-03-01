using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SASTFileViewer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SASTFileViewer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();
        public MainWindow()
        {
            this.InitializeComponent(); //加载界面

            string path = AppDomain.CurrentDomain.BaseDirectory; //选择程序的运行目录

            _ = ViewModel.LoadFiles(path); //读取文件夹

            
        }


        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.GoBack();
        }
        /*
        private async void FileListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 获取被点击的对象并转换为 FileItem
            if (e.ClickedItem is SASTFileViewer.Models.FileItem clickedItem)
            {
                if(clickedItem.IsFolder)
                {
                    ViewModel.LoadFiles(clickedItem.FullPath);
                }
                else
                {
                    var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(clickedItem.FullPath);
                    await Windows.System.Launcher.LaunchFileAsync(file);
                }
                    // 如果是文件夹，就进入
                    ViewModel.LoadFiles(clickedItem.FullPath);
            }
        }
        */
        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // 只要用户打字，就实时过滤
            ViewModel.FilterFiles(sender.Text);
        }
        // 核心：处理 XAML 报错 CS1061 的对应方法
        private async void FileListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            // 这里的 FileListView 必须对应 XAML 中的 x:Name
            if (FileListView.SelectedItem is SASTFileViewer.Models.FileItem selectedItem)
            {
                if (selectedItem.IsFolder)
                {
                    // 逻辑原理：双击的是文件夹，则异步进入
                    await ViewModel.LoadFiles(selectedItem.FullPath);
                }
                else
                {
                    // 逻辑原理：双击的是文件，则调用系统默认程序
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(selectedItem.FullPath)
                        {
                            UseShellExecute = true
                        });
                    }
                    catch { /* 容错：防止遇到无法打开的文件格式导致程序闪退 */ }
                }
            }
        }
        private async void PathBreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            // 逻辑原理：点击第 n 个节点，就截取前 n 个路径片段重新组合成路径
            var items = ViewModel.Breadcrumbs;
            string targetPath = "";

            for (int i = 0; i <= args.Index; i++)
            {
                // 组合路径，自动处理斜杠
                targetPath = Path.Combine(targetPath, items[i]);
            }

            if (Directory.Exists(targetPath))
            {
                await ViewModel.LoadFiles(targetPath);
            }
        }

    }
}
