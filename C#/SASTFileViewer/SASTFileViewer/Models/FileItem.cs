using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using:

namespace SASTFileViewer.Models
{
    public class FileItem
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsFolder { get; set; }
        public string path { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string FileSize { get; set; } = "";
        public string LastModified { get; set; } = "";

        // 显式写全命名空间，防止编译器类型推断失败
        // 返回字符串类型，避开所有枚举转换陷阱
        public string FileIcon =>
            IsFolder ? "Folder" :
            (FileType ?? "").ToLower() switch
            {
                ".jpg" or ".png" or ".jpeg" => "Pictures",
                ".mp3" or ".wav" => "Audio",
                ".mp4" or ".mkv" => "Video",
                ".zip" or ".7z" or ".rar" => "Library",
                ".exe" => "Setting",
                _ => "Document"
            };
    };
    
}


// 直接返回图标枚举，让 XAML 零逻辑调用
