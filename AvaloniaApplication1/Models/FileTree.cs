﻿using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using AvaloniaApplication1.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class FileTree : ReactiveObject
    {
        #region FIELDS
        private string _path;
        private string _name;
        private long? _size;
        private DateTimeOffset? _modified;
        private ObservableCollection<FileTree>? _children;
        private bool _hasChildren = true;
        private bool _isExpanded;
        private string _version;
        private string? _hashSum;
        private bool _isChecked;
        #endregion

        #region PROPERTIES
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isChecked, value);
                if (HasChildren)
                {
                    Task.Run(() =>
                    {
                        foreach (var child in Children)
                        {
                            child.IsChecked = value;
                        }
                    });
                }
            }
        }
        public string? HashSum
        {
            get => _hashSum;
            private set => this.RaiseAndSetIfChanged(ref _hashSum, value);
        }
        public string Version
        {
            get => _version;
            set => this.RaiseAndSetIfChanged(ref _version, value);
        }
        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public bool HasChildren
        {
            get => _hasChildren;
            set => this.RaiseAndSetIfChanged(ref _hasChildren, value);
        }

        public bool IsDirectory { get; }
        public FileTree? Parent { get; set; }
        public ObservableCollection<FileTree> Children => _children ??= LoadChildren();
        #endregion


        public FileTree(string path, bool isDirectory, FileTree? parent = null, bool isRoot = false)
        {
            _path = path;
            _name = isRoot ? path : System.IO.Path.GetFileName(Path);
            _isExpanded = isRoot;
            IsDirectory = isDirectory;
            HasChildren = isDirectory;
            _isChecked = false;
            Parent = parent;
            try
            {
                if (!IsDirectory)
                {
                    _version = string.IsNullOrEmpty(FileVersionInfo.GetVersionInfo(Path).ProductVersion!)
                        ? "-"
                        : FileVersionInfo.GetVersionInfo(Path).ProductVersion!;

                    var info = new FileInfo(path);
                }
                else
                {
                    _version = "-";
                }
            }
            catch
            {

            }

        }

        private ObservableCollection<FileTree> LoadChildren()
        {
            if (!IsDirectory)
            {
                return null;
            }
            var options = new EnumerationOptions()
            {
                IgnoreInaccessible = true,
                AttributesToSkip = default
            };
            var result = new ObservableCollection<FileTree>();

            foreach (var d in Directory.EnumerateDirectories(Path, "*", options))
            {
                result.Add(new FileTree(d, true, this));
            }

            foreach (var f in Directory.EnumerateFiles(Path, "*", options))
            {
                result.Add(new FileTree(f, false, this));
            }

            if (result.Count == 0)
                HasChildren = false;

            return result;
        }
    }
}