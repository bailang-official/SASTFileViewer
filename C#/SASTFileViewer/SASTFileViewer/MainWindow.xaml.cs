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
            

            string path = AppDomain.CurrentDomain.BaseDirectory; //选择程序的运行目录

            ViewModel.LoadFiles(path); //读取文件夹

            this.InitializeComponent(); //加载界面
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GoBack();
        }
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
        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // 只要用户打字，就实时过滤
            ViewModel.FilterFiles(sender.Text);
        }

    }
}
