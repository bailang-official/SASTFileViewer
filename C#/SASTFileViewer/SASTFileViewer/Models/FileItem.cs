using Microsoft.VisualBasic.FileIO; //处理文件和目录操作的库
using System; //提供基础数据类型（string...）
using System.Collections.Generic; //允许List列表
using System.ComponentModel; //提供INotifyPropertyChanged接口，允许界面自动更新
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace SASTFileViewer.Models
{
    public class FileItem
    {
        public string Name { get; set; } = string.Empty; //存储文件名称
        public string FullPath { get; set; } = string.Empty; //存储精确位置（例：C:\Users\Admin）
        public bool IsFolder { get; set; } //标记是文件夹还是文件，图标
        //public string? Path { get; set; } 
        public string FileType { get; set; } = string.Empty; //存储文件类型（关联FileIcon属性）
        public string FileSize { get; set; } = ""; //存储文件大小
        public string LastModified { get; set; } = "";

        public string FileIcon => //=>推导符号 = { get; }
            IsFolder ? "Folder" : //IsFolder为真的话直接返回"Folder"
            (FileType ?? "").ToLower() switch //FileType为空则赋值为""; ToLower后缀名小写化
            {
                ".jpg" or ".png" or ".jpeg" => "Pictures", //FileIcon——> "Pictures"
                ".mp3" or ".wav" => "Audio",
                ".mp4" or ".mkv" => "Video",
                ".zip" or ".7z" or ".rar" => "Library",
                ".exe" => "Setting",
                _ => "Document" //_弃元，保证有默认图标，类switch-default
            };
    };
    
}


// 直接返回图标枚举，让 XAML 零逻辑调用
