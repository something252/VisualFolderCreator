using System;
using System.Collections.Generic;
using System.Drawing;
using WinAPI;

namespace IconHelper
{
    public partial class IconManager
    {
        public class IconProperties : IDisposable
        {
            #region Variables
            /// <summary>
            /// Store all icons information,
            /// Shell32.SHFILEINFO
            /// </summary>
            public Dictionary<IconReader.IconSize, Shell32.SHFILEINFO> IconsInfo;
            /// <summary>
            /// Store all icons ListImage index,
            /// </summary>
            public Dictionary<IconReader.IconSize, int> IconsIndex;
            /// <summary>
            /// Store all icons image
            /// </summary>
            public Dictionary<IconReader.IconSize, Icon> Icons;
            /// <summary>
            /// Store any optional data you want
            /// </summary>
            private object _tag;
            /// <summary>
            /// Store any optional data you want
            /// </summary>
            public object Tag
            {
                get { return _tag; }
                set { _tag = value; }
            }
            #endregion
            #region Constructors
            /// <summary>
            /// Initalize Class
            /// </summary>
            public IconProperties()
            {
                IconsInfo = new Dictionary<IconReader.IconSize, Shell32.SHFILEINFO>();
                IconsIndex = new Dictionary<IconReader.IconSize, int>();
                Icons = new Dictionary<IconReader.IconSize, Icon>();
                _tag = null;
            }
            #endregion
            #region IsValid Methods
            /// <summary>
            /// Check if class contain a icon
            /// </summary>
            public bool IsValid(IconReader.IconSize size)
            {
                return Icons.ContainsKey(size);
            }
            /// <summary>
            /// Check if class contain a icon
            /// And if that icon is not null
            /// </summary>
            /// <param name="size">Icon Size to check</param>
            public bool IsValidEx(IconReader.IconSize size)
            {
                if (!Icons.ContainsKey(size)) return false;
                return Icons[size] != null;
            }
            /// <summary>
            /// Check if class contain any icon 
            /// </summary>
            public bool IsValid()
            {
                return Icons.ContainsKey(IconReader.IconSize.Small) || Icons.ContainsKey(IconReader.IconSize.Large) || Icons.ContainsKey(IconReader.IconSize.ExtraLarge) || Icons.ContainsKey(IconReader.IconSize.Jumbo);
            }

            public IconProperties FirstValid()
            {
                return this;
            }
            #endregion
            #region Remove Methods
            /// <summary>
            /// Remove a especified icon size from class,
            /// Supports multi sizes flags
            /// </summary>
            /// <param name="iconSize">Icon Size to remove, support multi size flags</param>
            /// <returns>A dictionary with removed icons (size and thier index on ListImage)</returns>
            public Dictionary<IconReader.IconSize, int> Remove(IconReader.IconSize iconSize)
            {
                Dictionary<IconReader.IconSize, int> removedIcons = new Dictionary<IconReader.IconSize, int>();
                if ((iconSize & IconReader.IconSize.Small) == IconReader.IconSize.Small)
                {
                    if (Icons.ContainsKey(IconReader.IconSize.Small))
                    {
                        if (IconsIndex[IconReader.IconSize.Small] >= 0)
                            removedIcons.Add(IconReader.IconSize.Small, IconsIndex[IconReader.IconSize.Small]);
                        Icons.Remove(IconReader.IconSize.Small);
                        IconsInfo.Remove(IconReader.IconSize.Small);
                        IconsIndex.Remove(IconReader.IconSize.Small);
                    }
                }
                if ((iconSize & IconReader.IconSize.Large) == IconReader.IconSize.Large)
                {
                    if (Icons.ContainsKey(IconReader.IconSize.Large))
                    {
                        if (IconsIndex[IconReader.IconSize.Large] >= 0)
                            removedIcons.Add(IconReader.IconSize.Large, IconsIndex[IconReader.IconSize.Large]);
                        Icons.Remove(IconReader.IconSize.Large);
                        IconsInfo.Remove(IconReader.IconSize.Large);
                        IconsIndex.Remove(IconReader.IconSize.Large);
                    }
                }
                if ((iconSize & IconReader.IconSize.ExtraLarge) == IconReader.IconSize.ExtraLarge)
                {
                    if (Icons.ContainsKey(IconReader.IconSize.ExtraLarge))
                    {
                        if (IconsIndex[IconReader.IconSize.ExtraLarge] >= 0)
                            removedIcons.Add(IconReader.IconSize.ExtraLarge, IconsIndex[IconReader.IconSize.ExtraLarge]);
                        Icons.Remove(IconReader.IconSize.ExtraLarge);
                        IconsInfo.Remove(IconReader.IconSize.ExtraLarge);
                        IconsIndex.Remove(IconReader.IconSize.ExtraLarge);
                    }
                }
                if ((iconSize & IconReader.IconSize.Jumbo) == IconReader.IconSize.Jumbo)
                {
                    if (Icons.ContainsKey(IconReader.IconSize.Jumbo))
                    {
                        if (IconsIndex[IconReader.IconSize.Jumbo] >= 0)
                            removedIcons.Add(IconReader.IconSize.Jumbo, IconsIndex[IconReader.IconSize.Jumbo]);
                        Icons.Remove(IconReader.IconSize.Jumbo);
                        IconsInfo.Remove(IconReader.IconSize.Jumbo);
                        IconsIndex.Remove(IconReader.IconSize.Jumbo);
                    }
                }
                return removedIcons;
            }
            #endregion
            #region Dispose
            /// <summary>
            /// Free all resources used by that class.
            /// </summary>
            public void Dispose()
            {
                // Dispose Icon
                foreach (KeyValuePair<IconReader.IconSize, Icon> icon in Icons)
                {
                    icon.Value.Dispose();
                }
                // Clear dictionray
                Icons.Clear();
                IconsIndex.Clear();
                IconsInfo.Clear();
                Icons = null;
                IconsIndex = null;
                IconsInfo = null;
            }
            #endregion
        }
    }
}
