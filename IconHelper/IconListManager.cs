using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinAPI;

namespace IconHelper
{
    /// <summary>
    /// Provide an CACHE system for add and retrive icons,
    /// And create a ImageList with all retrived icons.
    /// </summary>
    public partial class IconManager : IDisposable
    {
        #region Constants
        /// <summary>
        /// Use when you want acess to OpenFolder Icon
        /// </summary>
        public const string FolderOpen = ":FOLDEROPEN:";
        /// <summary>
        /// Use when you want acess to ClosedFolder Icon
        /// </summary>
        public const string FolderClosed = ":FOLDERCLOSED:";
        /// <summary>
        /// Equivalent to: IconReader.IconSize.Small | IconReader.IconSize.Large
        /// </summary>
        public const IconReader.IconSize IconSizeBoth = IconReader.IconSize.Small | IconReader.IconSize.Large;
        /// <summary>
        /// Equivalent to: IconReader.IconSize.Small | IconReader.IconSize.Large | IconReader.IconSize.ExtraLarge | IconReader.IconSize.Jumbo
        /// </summary>
        public const IconReader.IconSize IconSizeAll = IconReader.IconSize.Small | IconReader.IconSize.Large | IconReader.IconSize.ExtraLarge | IconReader.IconSize.Jumbo;
        /// <summary>
        /// Equivalent to: IconReader.IconSize.Small | IconReader.IconSize.Large
        /// IconReader.IconSize.ExtraLarge AND IconReader.IconSize.Jumbo will be only added if the current OS support that sizes.
        /// NOTE: Use that instead 'IconManager.IconSizeAll'
        /// </summary>
        public readonly IconReader.IconSize IconsSizeAllSupported = IconReader.IconSize.Small | IconReader.IconSize.Large;
        #endregion
        #region Variables
        /// <summary>
        /// ImageList Dictionary with associated icons,
        /// use and assign like: IImageList[IconReader.Large]
        /// </summary>
        /// <example>IImageList[IconReader.Large]</example>
        public Dictionary<IconReader.IconSize, ImageList> IImageList;
        public Dictionary<string, IconProperties> IconList;
        #endregion
        #region Constructors
        /// <summary>
        /// Initalize class, and create Small and Large ImageList
        /// </summary>
        public IconManager()
            : this(true, true)
        { }
        /// <summary>
        /// Initalize class, ExtraLarge and Jumbo ImageLists will not be created!
        /// </summary>
        /// <param name="createSmallIconList">Create or not the Small ImageList.</param>
        /// <param name="createLargeIconList">Create or not the Large ImageList.</param>
        public IconManager(bool createSmallIconList, bool createLargeIconList)
            : this(createSmallIconList, createLargeIconList, false, false, false)
        { }
        /// <summary>
        /// Initalize class
        /// NOTE: By using that constructor will not optimize to OS
        /// </summary>
        /// <param name="createSmallIconList">Create or not the Small ImageList.</param>
        /// <param name="createLargeIconList">Create or not the Large ImageList.</param>
        /// <param name="createExtraLargeIconList">Create or not the ExtraLarge ImageList.</param>
        /// <param name="createJumboIconList">Create or not the Jumbo ImageList.</param>
        public IconManager(bool createSmallIconList, bool createLargeIconList, bool createExtraLargeIconList, bool createJumboIconList)
            : this(createSmallIconList, createLargeIconList, createExtraLargeIconList, createJumboIconList, false)
        { }
        /// <summary>
        /// Initalize class
        /// </summary>
        /// <param name="createSmallIconList">Create or not the Small ImageList.</param>
        /// <param name="createLargeIconList">Create or not the Large ImageList.</param>
        /// <param name="createExtraLargeIconList">Create or not the ExtraLarge ImageList.</param>
        /// <param name="createJumboIconList">Create or not the Jumbo ImageList.</param>
        /// <param name="optimizeToOS">Since XP or above support ExtraLarge And Vista or abdove support Jumbo icon sizes
        /// Disable unless sizes and ImageList on OS that not support that features.</param>
        public IconManager(bool createSmallIconList, bool createLargeIconList, bool createExtraLargeIconList, bool createJumboIconList, bool optimizeToOS)
        {
            IconList = new Dictionary<string, IconProperties>(4);
            IImageList = new Dictionary<IconReader.IconSize, ImageList>();

            // Add extra flags to supported sizes for the current OS
            if (Utils.IsXpOrAbove())
                IconsSizeAllSupported |= IconReader.IconSize.ExtraLarge;
            if (Utils.IsVistaOrAbove())
                IconsSizeAllSupported |= IconReader.IconSize.Jumbo;

            if (optimizeToOS)
            {
                // Check if a some features work on current OS, if not disable it by force
                if (!Utils.IsXpOrAbove())       // XP+ Support ExtraLarge
                    createExtraLargeIconList = false;
                if (!Utils.IsVistaOrAbove())    // Vista+ Support Jumbo
                    createJumboIconList = false;
            }

            // Create a imagelist with all colected icons and information
            if (createSmallIconList)
            {
                IImageList[IconReader.IconSize.Small] = new ImageList();
                IImageList[IconReader.IconSize.Small].TransparentColor = Color.Transparent;
                IImageList[IconReader.IconSize.Small].ColorDepth = ColorDepth.Depth32Bit;
                IImageList[IconReader.IconSize.Small].ImageSize = new Size(16, 16);
            }
            else
                IImageList[IconReader.IconSize.Small] = null;

            if (createLargeIconList)
            {
                IImageList[IconReader.IconSize.Large] = new ImageList();
                IImageList[IconReader.IconSize.Large].TransparentColor = Color.Transparent;
                IImageList[IconReader.IconSize.Large].ColorDepth = ColorDepth.Depth32Bit;
                IImageList[IconReader.IconSize.Large].ImageSize = new Size(32, 32);
            }
            else
                IImageList[IconReader.IconSize.Large] = null;

            if (createExtraLargeIconList)
            {
                IImageList[IconReader.IconSize.ExtraLarge] = new ImageList();
                IImageList[IconReader.IconSize.ExtraLarge].TransparentColor = Color.Transparent;
                IImageList[IconReader.IconSize.ExtraLarge].ColorDepth = ColorDepth.Depth32Bit;
                IImageList[IconReader.IconSize.ExtraLarge].ImageSize = new Size(48, 48);
            }
            else
                IImageList[IconReader.IconSize.Small] = null;

            if (createJumboIconList)
            {
                IImageList[IconReader.IconSize.Jumbo] = new ImageList();
                IImageList[IconReader.IconSize.Jumbo].TransparentColor = Color.Transparent;
                IImageList[IconReader.IconSize.Jumbo].ColorDepth = ColorDepth.Depth32Bit;
                IImageList[IconReader.IconSize.Jumbo].ImageSize = new Size(256, 256);
            }
            else
                IImageList[IconReader.IconSize.Jumbo] = null;
        }
        #endregion
        #region Get Methods
        /// <summary>
        /// Retrive the icon index from the ImageList
        /// </summary>
        /// <param name="extension">The extension, 
        /// such as ex: ".mp3".</param>
        /// <param name="iconSize">The icon size.</param>
        /// <returns>Returns icon index from ImageList, if not exist will trown an exception</returns>
        public int GetIconIndex(string extension, IconReader.IconSize iconSize)
        {
            return GetIconIndex(extension, iconSize, true);
        }
        /// <summary>
        /// Retrive the icon index from the ImageList
        /// </summary>
        /// <param name="extension">The extension, 
        /// such as ex: ".mp3".</param>
        /// <param name="iconSize">The icon size.</param>
        /// <returns>Returns -1 if extension not exist on list, otherwise returns >= 0</returns>
        public int GetIconIndex(string extension, IconReader.IconSize iconSize, bool check)
        {
            if (check)
                if (!IsValid(extension)) return -1;
            return IconList[extension].IconsIndex[iconSize];
        }
        #endregion
        #region Is Methods
        /// <summary>
        /// Check if an extension exist on IconList
        /// </summary>
        public bool IsValid(string extension)
        {
            return IconList.ContainsKey(extension);
        }

        /// <summary>
        /// Check if an extension exist on IconList and return thier value, 
        /// otherwise returns new IconProperties();
        /// </summary>
        /// <returns>IconProperties of founded path, otherwise return new IconProperties()</returns>
        public IconProperties IsValidEx(string extension)
        {
            return !IconList.ContainsKey(extension) ? new IconProperties() : IconList[extension];
        }
        /*public bool IsCreated(string extension, IconReader.IconSize iconSize)
        {
            return false;
        }*/
        #endregion
        #region Add Methods
        /*public IconProperties Add(string extension, IconProperties iconProp, bool check)
        {
            if (check)
                if (IsValid(extension)) return IconList[extension];
            IconProperties _fileinfo = new IconProperties();
            _fileinfo.IconsInfo.Small = fileInfo;
            _fileinfo.IconsIndex.Small = ImageListSmall.Images.Count;
            _fileinfo.Icons.Small = icon;
            IconList.Add(extension, _fileinfo);
            ImageListSmall.Images.Add(extension, icon);
            return _fileinfo;
        }*/
        /// <summary>
        /// Internal add a icon to class
        /// </summary>
        /// <param name="path">Icon Path on filesystem, or extension</param>
        /// <param name="iconSize">Icon Size</param>
        /// <param name="iconProp">Icon Properties to assign to list</param>
        private void Add(string path, IconReader.IconSize iconSize, IconProperties iconProp)
        {
            iconProp.IconsIndex[iconSize] = -1;
            iconProp.IconsInfo[iconSize] = new Shell32.SHFILEINFO();
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            iconProp.Icons[iconSize] = IconReader.GetFileIcon(path, iconSize, false, ref shfi);

            iconProp.IconsInfo[iconSize] = shfi;
            if (IImageList[iconSize] != null)
            {
                iconProp.IconsIndex[iconSize] = IImageList[iconSize].Images.Count;
                IImageList[iconSize].Images.Add(path, iconProp.Icons[iconSize]);
            }
        }
        /// <summary>
        /// Internal add a icon to class
        /// FOLDERS ONLY!!!!!!
        /// </summary>
        /// <param name="path">Icon Path on filesystem, or extension</param>
        /// <param name="iconSize">Icon Size</param>
        /// <param name="iconProp">Icon Properties to assign to list</param>
        /// <param name="folder">Folder type (open or closed)</param>
        private void Add(string path, IconReader.IconSize iconSize, IconProperties iconProp, IconReader.FolderType folder)
        {
            iconProp.IconsIndex[iconSize] = -1;
            iconProp.IconsInfo[iconSize] = new Shell32.SHFILEINFO();
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            iconProp.Icons[iconSize] = IconReader.GetFolderIcon(iconSize, folder, ref shfi);

            iconProp.IconsInfo[iconSize] = shfi;
            if (IImageList[iconSize] != null)
            {
                iconProp.IconsIndex[iconSize] = IImageList[iconSize].Images.Count;
                IImageList[iconSize].Images.Add(path, iconProp.Icons[iconSize]);
            }
        }
        /// <summary>
        /// Add an extension to List
        /// </summary>
        /// <param name="path">The extension, 
        /// such as ex: ".mp3" or full path "C:\\mymusic.mp3".</param>
        /// <param name="iconSize">The icon size, support multi size flags</param>
        /// <returns>Returns Icon and thier information.</returns>
        public IconProperties AddEx(string path, IconReader.IconSize iconSize)
        {
            IconProperties iconProp = IsValidEx(path);
            if ((iconSize & IconReader.IconSize.Small) == IconReader.IconSize.Small)
            {
                if (!iconProp.IsValidEx(IconReader.IconSize.Small))
                    Add(path, IconReader.IconSize.Small, iconProp);
            }
            if ((iconSize & IconReader.IconSize.Large) == IconReader.IconSize.Large)
            {
                if (!iconProp.IsValidEx(IconReader.IconSize.Large))
                    Add(path, IconReader.IconSize.Large, iconProp);
            }
            if ((iconSize & IconReader.IconSize.ExtraLarge) == IconReader.IconSize.ExtraLarge)
            {
                if (!iconProp.IsValidEx(IconReader.IconSize.ExtraLarge))
                    Add(path, IconReader.IconSize.ExtraLarge, iconProp);
            }
            if ((iconSize & IconReader.IconSize.Jumbo) == IconReader.IconSize.Jumbo)
            {
                if (!iconProp.IsValidEx(IconReader.IconSize.Jumbo))
                    Add(path, IconReader.IconSize.Jumbo, iconProp);
            }
            if (!IsValid(path))
                IconList.Add(path, iconProp);
            return iconProp;
        }
        /// <summary>
        /// Util function to Add Computer drivers icons and information to list
        /// </summary>
        /// <param name="iconSize">The icon size, support multi size flags</param>
        public void AddComputerDrives(IconReader.IconSize iconSize)
        {
            foreach (string drive in Directory.GetLogicalDrives())
            {
                AddEx(drive, iconSize);
            }
        }
        /// <summary>
        /// Util function to Add Folders icons and information to list
        /// </summary>
        /// <param name="iconSize">The icon size, support multi size flags</param>
        public void AddFolder(IconReader.IconSize iconSize)
        {
            IconProperties iconPropOpen = new IconProperties();
            IconProperties iconPropClosed = new IconProperties();
            if ((iconSize & IconReader.IconSize.Small) == IconReader.IconSize.Small)
            {
                Add(FolderOpen, IconReader.IconSize.Small, iconPropOpen, IconReader.FolderType.Open);
                Add(FolderClosed, IconReader.IconSize.Small, iconPropClosed, IconReader.FolderType.Closed);
            }
            if ((iconSize & IconReader.IconSize.Large) == IconReader.IconSize.Large)
            {
                Add(FolderOpen, IconReader.IconSize.Large, iconPropOpen, IconReader.FolderType.Open);
                Add(FolderClosed, IconReader.IconSize.Large, iconPropClosed, IconReader.FolderType.Closed);
            }
            if ((iconSize & IconReader.IconSize.ExtraLarge) == IconReader.IconSize.ExtraLarge)
            {
                Add(FolderOpen, IconReader.IconSize.ExtraLarge, iconPropOpen, IconReader.FolderType.Open);
                Add(FolderClosed, IconReader.IconSize.ExtraLarge, iconPropClosed, IconReader.FolderType.Closed);
            }
            if ((iconSize & IconReader.IconSize.Jumbo) == IconReader.IconSize.Jumbo)
            {
                Add(FolderOpen, IconReader.IconSize.Jumbo, iconPropOpen, IconReader.FolderType.Open);
                Add(FolderClosed, IconReader.IconSize.Jumbo, iconPropClosed, IconReader.FolderType.Closed);
            }
            IconList.Add(FolderOpen, iconPropOpen);
            IconList.Add(FolderClosed, iconPropClosed);
        }
        #endregion
        #region Remove Methods
        /// <summary>
        /// Remove whole icons information for a especified path or extension
        /// </summary>
        /// <param name="path">Icon path or extension</param>
        /// <param name="removeIconFromList">Did you want remove icon from ImageList? true or false</param>
        /// <example>Remove(".mp3", false);</example>
        public bool Remove(string path, bool removeIconFromList)
        {
            if (!IsValid(path)) return false;
            if (removeIconFromList)
                foreach (KeyValuePair<IconReader.IconSize, int> list in IconList[path].IconsIndex)
                    IImageList[list.Key].Images.RemoveAt(list.Value);
            IconList[path].Dispose();
            IconList.Remove(path);
            return true;
        }
        /// <summary>
        /// Remove icons information for a especified icon size
        /// </summary>
        /// <param name="path">Icon path or extension</param>
        /// <param name="iconSize">The icon size, support multi size flags</param>
        /// <param name="removeIconFromList">Did you want remove icon from ImageList? true or false</param>
        /// <example>Remove(".txt", IconReader.IconSize.Jumbo | IconReader.IconSize.ExtraLarge, true);</example>
        public bool Remove(string path, IconReader.IconSize iconSize, bool removeIconFromList)
        {
            if (!IsValid(path)) return false;
            Dictionary<IconReader.IconSize, int> removedIcons = IconList[path].Remove(iconSize);
            if (removeIconFromList)
                foreach (KeyValuePair<IconReader.IconSize, int> item in removedIcons)
                    IImageList[item.Key].Images.RemoveAt(item.Value);
            if (!IconList[path].IsValid())
            {
                IconList[path].Dispose();
                IconList.Remove(path);
            }
            return true;
        }
        #endregion
        #region Dispose
        /// <summary>
        /// Free all resources used by that class.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<IconReader.IconSize, ImageList> list in IImageList)
            {
                if (list.Value == null) continue;
                list.Value.Dispose();
            }
            IconList.Clear();
        }
        #endregion
    }
}