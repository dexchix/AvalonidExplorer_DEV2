using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication1.Helper;
using AvaloniaApplication1.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.IO;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public FileSystemWatcher FileSystemWatcher { get; set; }
        private static IconConverter? s_iconConverter;
        private static FileTree _fileTree = new FileTree();

        #region PROPERTIES
        public static FileTree OpenedFolder { get => _fileTree; }

        public FileTree FileTree { get { return _fileTree; } set { this.RaiseAndSetIfChanged(ref _fileTree, value); } }

        public static IMultiValueConverter FileIconConverter
        {
            get
            {
                if (s_iconConverter is null)
                {
                    var assetLoader = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();

                    using (var fileStream = assetLoader.Open(new Uri("avares://AvaloniaApplication1/Assets/file.png")))
                    using (var folderStream = assetLoader.Open(new Uri("avares://AvaloniaApplication1/Assets/folder.png")))
                    using (var folderOpenStream = assetLoader.Open(new Uri("avares://AvaloniaApplication1/Assets/folder.png")))
                    {
                        s_iconConverter = new IconConverter(new Bitmap(fileStream), new Bitmap(folderStream), new Bitmap(folderOpenStream));
                    }
                }

                return s_iconConverter;
            }
        }
        #endregion

        #region COMMANDS
        public void GoToFolderCommand(FileTreeNodeModel selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                FileTree.GoToFolder(selectedFile);
            }
        }
        public void GoBackFolderCommand()
        {
            if (FileTree.OpenedFolder != null && FileTree.OpenedFolder.Parent != null)
            {
                FileTree.GoBackFolder();
            }
        }
        public void CancelCommand(Window window)
        {
            window.Close();
        }
        public void OkCommand(Window window)
        {
            FileTree.CheckGCWatcher();
        }

        #endregion
    }
}
