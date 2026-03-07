using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SASTFileViewer.Models;
using System.IO;
using System.ComponentModel;


namespace SASTFileViewer
{
    
    public class MainViewModel : INotifyPropertyChanged //数据改变则UI更新
    {
        public ObservableCollection<string> Breadcrumbs { get; set; } = new(); //储存面包屑每一级的名称

        private List<FileItem> _allFiles = new(); //备份仓库，存储当前目录下的所有文件和文件夹，供搜索过滤使用
        public ObservableCollection<FileItem> Files { get; set; } = new(); //关联FileItem.cs，储存当前目录下的文件和文件夹，用于界面展示
        private string _currentPath = "";
        public string CurrentPath
        {
            get => _currentPath; //获取当前路径
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));//路径改变，刷新界面
            }
        }
        
        public async Task LoadFiles(string folderPath) //加载指定路径下的文件和文件夹信息，并更新界面显示
        {
            if (!Directory.Exists(folderPath)) return; //路径不存在则不加载

            CurrentPath = folderPath; //更新路径显示
            Files.Clear(); //清空当前显示的文件列表

            var items = await Task.Run(() => //在后台线程获取文件信息，避免界面卡顿
            {
                var tempList = new List<FileItem>(); //创建临时列表存储文件信息
                DirectoryInfo dir = new DirectoryInfo(folderPath); //获取目录信息对象

                try
                {
                    // 1. 获取所有文件夹
                    foreach (var directory in dir.GetDirectories())
                    {
                        tempList.Add(new FileItem //找到子文件夹后创建文件信息对象并添加到列表
                        {
                            Name = directory.Name,
                            FullPath = directory.FullName,
                            IsFolder = true, //标记为文件夹，在FileItem.cs中根据这个属性返回对应图标
                            FileType = "文件夹"
                        });
                    }

                    // 2. 获取当前目录的所有文件
                    foreach (var file in dir.GetFiles()) 
                    {
                        tempList.Add(new FileItem  
                        {
                            Name = file.Name,
                            FullPath = file.FullName,
                            IsFolder = false,
                            FileType = file.Extension, //获取文件扩展名作为类型，FileItem.cs中根据这个属性返回对应图标
                            FileSize = (file.Length / 1024.0).ToString("F2") + " KB", //将文件大小转换为KB并格式化为两位小数
                            LastModified = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                        });
                    }
                }
                catch (UnauthorizedAccessException) { /* 处理权限不足的文件夹 */ } //跳过无法读取的核心文件夹

                return tempList; //返回获取到的文件信息列表，Task.Run结束后回到UI线程继续执行
            });

            // 回到 UI 线程更新集合
            foreach (var item in items)
            {
                Files.Add(item);
            }

            // 更新搜索备份记录，以便搜索功能使用
            _allFiles = Files.ToList();

            Breadcrumbs.Clear(); 
            var parts = folderPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries); //将路径按目录分割成数组，去掉空项

            // 处理磁盘驱动器（如 C:）
            if (folderPath.Contains(':'))
            {
                Breadcrumbs.Add(parts[0] + Path.DirectorySeparatorChar);
                for (int i = 1; i < parts.Length; i++) Breadcrumbs.Add(parts[i]);
            }
            else
            {
                foreach (var part in parts) Breadcrumbs.Add(part);
            }
        }
        public async Task GoBack()
        {
            if (string.IsNullOrEmpty(CurrentPath)) return; //没有路径则不执行返回操作

            var parent = Directory.GetParent(CurrentPath); //获取当前路径的父目录信息对象
            if (parent != null)
            {
                await LoadFiles(parent.FullName);
            }
        }

        public void FilterFiles(string query) //根据搜索框输入的查询词过滤文件列表，更新界面显示（query为搜索词）
        {
            if (string.IsNullOrWhiteSpace(query)) // 如果搜索框为空或仅包含空白字符，则显示所有文件
            {
                Files.Clear();
                foreach (var f in _allFiles) Files.Add(f); // 直接将备份列表中的所有文件重新添加到显示列表中
            }
            else
            {
                // 只保留名字里包含搜索词的文件（忽略大小写）
                var filtered = _allFiles.Where(f => f.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList(); //OrdinalIgnoreCase表示不区分大小写的字符串比较
                Files.Clear();
                foreach (var f in filtered) Files.Add(f);
            }
        }
        // 实现 INotifyPropertyChanged 接口，通知界面属性值改变以刷新显示
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
    }
}
