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
    
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Breadcrumbs { get; set; } = new(); //储存面包屑每一级的名称

        private List<FileItem> _allFiles = new();
        public ObservableCollection<FileItem> Files { get; set; } = new();
        private string _currentPath = "";
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));//路径改变，刷新界面
            }
        }
        
        /*
        public MainViewModel() 
        {
            Files.Add(new FileItem { Name = "测试文件 1" });
        }
        */

        public async Task LoadFiles(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            CurrentPath = folderPath; //更新路径
            Files.Clear(); // 清空旧的测试数据

            //DirectoryInfo dir = new DirectoryInfo(folderPath);

            var items = await Task.Run(() =>
            {
                var tempList = new List<FileItem>();
                DirectoryInfo dir = new DirectoryInfo(folderPath);

                try
                {
                    // 1. 获取所有文件夹
                    foreach (var directory in dir.GetDirectories())
                    {
                        tempList.Add(new FileItem
                        {
                            Name = directory.Name,
                            FullPath = directory.FullName,
                            IsFolder = true,
                            FileType = "文件夹"
                        });
                    }

                    // 2. 获取所有文件
                    foreach (var file in dir.GetFiles())
                    {
                        tempList.Add(new FileItem
                        {
                            Name = file.Name,
                            FullPath = file.FullName,
                            IsFolder = false,
                            FileType = file.Extension,
                            FileSize = (file.Length / 1024.0).ToString("F2") + " KB",
                            LastModified = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                        });
                    }
                }
                catch (UnauthorizedAccessException) { /* 处理权限不足的文件夹 */ }

                return tempList;
            });

            // 回到 UI 线程更新集合
            foreach (var item in items)
            {
                Files.Add(item);
            }

            // 更新搜索备份记录
            _allFiles = Files.ToList();

            Breadcrumbs.Clear();
            var parts = folderPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

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
            if (string.IsNullOrEmpty(CurrentPath)) return;

            var parent = Directory.GetParent(CurrentPath);
            if(parent != null)
            {
                await LoadFiles(parent.FullName);
            }
        }

        public void FilterFiles(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // 如果搜索框空了，显示所有文件
                Files.Clear();
                foreach (var f in _allFiles) Files.Add(f);
            }
            else
            {
                // 只保留名字里包含搜索词的文件（忽略大小写）
                var filtered = _allFiles.Where(f => f.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
                Files.Clear();
                foreach (var f in filtered) Files.Add(f);
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
    }
}
