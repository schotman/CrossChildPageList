/*
 *  This is a free module from DnnModule.com, you can use it as your wish.
 *  Any suggestions , please visit our forum at DnnModule.com or mail us at xiaoqi97#msn.com
 *
 * Version history
 *  11/05/2008 v2.0 First release as free module
 *
 *  6/13/2009  v2.2 Add two options:"Recursive" and "Child tab prefix"
 *   9/17/2009  v2.5 add six options to build elegant menu style.
 *
 * 2/1/2010 v2.6 remove viewstate to improve load speed.
 * 2010/11/22 v3.0  display child page from menu root. remove wrap from list item.
  *  12/10/2018 v4.0 upgrade to DNN 9.2 platform
  *  09/13/2026 v5.0 40Fingers upgrade to DNN 10.2 platform (replaced removed ModuleController.GetModuleSettings and TabController instance APIs new TabController()/GetTab(int)/GetTabsByParentId with PortalModuleBase.Settings, TabController.Instance, GetTab(tabId,portalId) and static GetTabsByParent; compatible with DNN 9.13.x and 10.2.x)
  *  09/13/2026 v5.2 40Fingers fix "Include Hidden Tab": pass includeInvisible as the includeHidden flag to GetPortalTabs so hidden root-level tabs are no longer stripped before the visibility filter
  *  09/13/2026 v5.3 40Fingers restore page-permission filtering: exclude pages the current user is not authorized to view via TabPermissionController.CanViewPage
  *  09/13/2026 v5.0.1 40Fingers repackaged as a compiled DNN module (Visual Studio 2026 / .NET Framework 4.7.2 project); code-behind now compiled into CrossChildPageList.dll
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;

namespace Cross.Modules.ChildPageList
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The List class displays the content
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class List : PortalModuleBase
    {
        // Control declarations - required in SDK-style projects (no designer file generated)
        protected DataList dlSubTabList;

        #region Private Members
        private string Parent_Tab = "ParentTab";
        private string Include_Self = "IncludeSelf";
        private string Include_Invisible = "IncludeInvisible";

        private string Column_Per_Row = "ColumnPerRow";

        private string Link_Target = "LinkTarget";
        private string Recursive = "Recursive";


        private string DisplayIcon = "DisplayIcon";


        #endregion


        #region public members

        public int ColumnPerRow
        {
            get
            {
                //check recursive
                bool isRecursive = false;
                if (this.Settings[Recursive] != null && Convert.ToString(this.Settings[Recursive]) != "")
                {
                    isRecursive = Convert.ToBoolean(this.Settings[Recursive]);
                }
                if (isRecursive)
                {
                    return 1;
                }
                else
                {
                    if (this.Settings[Column_Per_Row] != null && Convert.ToString(this.Settings[Column_Per_Row]) != "")
                    {
                        return Convert.ToInt32(this.Settings[Column_Per_Row]);
                    }
                    else
                    {
                        return 1;
                    }
                }

            }
        }

        public string LinkTarget
        {
            get
            {
                if (this.Settings[Link_Target] != null && Convert.ToString(this.Settings[Link_Target]) != "")
                {
                    return Convert.ToString(this.Settings[Link_Target]);
                }
                else
                {
                    return "_self";
                }
            }
        }

        #endregion

        #region Event Handlers

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void Page_Load(System.Object sender, System.EventArgs e)
        {
            try
            {
                LocalResourceFile = Localization.GetResourceFile(this, "List.ascx");

                int parentTab = TabId;//Default value

                if (this.Settings[Parent_Tab] != null && Convert.ToString(this.Settings[Parent_Tab]) != "")
                {
                    parentTab = Convert.ToInt32(this.Settings[Parent_Tab]);
                }


                TabInfo objParent = TabController.Instance.GetTab(parentTab, PortalId);


                if ((parentTab != DotNetNuke.Common.Utilities.Null.NullInteger) && (objParent == null || !objParent.HasChildren))
                {
                    return;
                }

                //Check include self
                bool includeSelf = false;
                if (this.Settings[Include_Self] != null && Convert.ToString(this.Settings[Include_Self]) != "")
                {
                    includeSelf = Convert.ToBoolean(this.Settings[Include_Self]);
                }

                //check include invisible
                bool includeInvisible = false;
                if (this.Settings[Include_Invisible] != null && Convert.ToString(this.Settings[Include_Invisible]) != "")
                {
                    includeInvisible = Convert.ToBoolean(this.Settings[Include_Invisible]);
                }

                //check recursive
                bool isRecursive = false;
                if (this.Settings[Recursive] != null && Convert.ToString(this.Settings[Recursive]) != "")
                {
                    isRecursive = Convert.ToBoolean(this.Settings[Recursive]);
                }

                //check display icon
                bool isDisplayIcon = false;
                if (this.Settings[DisplayIcon] != null && Convert.ToString(this.Settings[DisplayIcon]) != "")
                {
                    isDisplayIcon = Convert.ToBoolean(this.Settings[DisplayIcon]);
                }

                ArrayList arrTab = new ArrayList();

                if (isRecursive)
                {
                    arrTab = GetChildPageList(parentTab,
                        parentTab == DotNetNuke.Common.Utilities.Null.NullInteger ? -1 : objParent.Level,
                        includeSelf, includeInvisible, isDisplayIcon);
                }
                else
                {
                    ArrayList tabList = new ArrayList();
                    if (parentTab == DotNetNuke.Common.Utilities.Null.NullInteger)//默认为根菜单
                    {
                        //foreach (TabInfo tabInfo in ctlTab.GetTabs(PortalId))
                        //{
                        // Pass includeInvisible as the includeHidden flag so hidden root tabs
                        // are not stripped here before the IsVisible/includeInvisible filter below.
                        foreach (TabInfo tabInfo in TabController.GetPortalTabs(PortalId, -1, false, includeInvisible))
                        {
                            if (tabInfo.Level == 0 && tabInfo.ParentId == -1)//找到第一层tab
                            {
                                tabList.Add(tabInfo);
                            }
                        }
                    }
                    else
                    {
                        foreach (TabInfo tabInfo in TabController.GetTabsByParent(parentTab, PortalId))
                        {
                            tabList.Add(tabInfo);
                        }
                    }
                    foreach (TabInfo tabInfo in tabList)
                    {
                        // Skip pages the current user is not permitted to view (honours role/deny permissions).
                        if ((!tabInfo.IsDeleted) && (!tabInfo.DisableLink) && TabPermissionController.CanViewPage(tabInfo))
                        {
                            if (tabInfo.IsVisible || includeInvisible)
                            {
                                //不等于本页面或者等于本页面，但模块设置中显示本页面
                                if (tabInfo.TabID != TabId || includeSelf)
                                {
                                    TabInfo childTab = tabInfo.Clone();
                                    string iconUrl = "";
                                    if (isDisplayIcon)
                                    {
                                        if (string.IsNullOrEmpty(childTab.IconFile))
                                        {
                                            iconUrl = string.Format("<img src='{0}' border='0' width='16px' height='16px'/>", Page.ResolveUrl("~/images/icon_unknown_16px.gif"));
                                        }
                                        else
                                        {
                                            iconUrl = string.Format("<img src='{0}' border='0' width='16px' height='16px'/>", Page.ResolveUrl(childTab.IconFile));
                                        }
                                    }
                                    childTab.TabName = iconUrl + tabInfo.TabName;
                                    arrTab.Add(childTab);
                                }
                            }

                        }
                    }
                }
                if (arrTab.Count > 0)
                {
                    dlSubTabList.DataSource = arrTab;
                    dlSubTabList.DataBind();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private ArrayList GetChildPageList(int parentTabId, int parentTabLevel, bool includeSelf, bool includeInvisible, bool isDisplayIcon)
        {
            ArrayList arrTab = new ArrayList();

            ArrayList tabList = new ArrayList();
            if (parentTabId == DotNetNuke.Common.Utilities.Null.NullInteger)//parenttabid为根菜单
            {
                // foreach (TabInfo tabInfo in ctlTab.GetTabs(PortalId))
                // Pass includeInvisible as the includeHidden flag so hidden root tabs are not
                // stripped here before the IsVisible/includeInvisible filter is applied.
                foreach (TabInfo tabInfo in TabController.GetPortalTabs(PortalId, -1, false, includeInvisible))
                {
                    if (tabInfo.Level == 0 && tabInfo.ParentId == -1)//找到第一层tab list
                    {
                        tabList.Add(tabInfo);
                    }
                }
            }
            else
            {
                foreach (TabInfo tabInfo in TabController.GetTabsByParent(parentTabId, PortalId))//否则找到当前的子菜单 list
                {
                    tabList.Add(tabInfo);
                }
            }

            TabInfo objParent = new TabInfo();
            if (parentTabId != DotNetNuke.Common.Utilities.Null.NullInteger)
            {
                objParent = TabController.Instance.GetTab(parentTabId, PortalId);
                if (objParent == null || !objParent.HasChildren)//递归到没有下级菜单，则返回
                {
                    return arrTab;
                }
            }

            foreach (TabInfo tabInfo in tabList)
            {
                // Skip pages the current user is not permitted to view (honours role/deny permissions).
                if ((!tabInfo.IsDeleted) && (!tabInfo.DisableLink) && TabPermissionController.CanViewPage(tabInfo))
                {
                    if (tabInfo.IsVisible || includeInvisible)
                    {
                        //不等于本页面或者等于本页面，但模块设置中显示本页面
                        if (tabInfo.TabID != TabId || includeSelf)
                        {
                            TabInfo childTab = tabInfo.Clone();
                            //检查tab level的层次差距，大于2层以上才加childTabPrefix
                            int levelDiff = tabInfo.Level - parentTabLevel;
                            string prefix = "";
                            for (int i = 2; i < levelDiff; i++)
                            {
                                prefix += string.Format("<img src='{0}' border='0'/>", Page.ResolveUrl("~/desktopmodules/CrossChildPageList/images/line.gif"));
                            }
                            if (levelDiff > 1)
                            {
                                prefix += string.Format("<img src='{0}' border='0'/>", Page.ResolveUrl("~/desktopmodules/CrossChildPageList/images/node.gif"));
                            }

                            string iconUrl = "";
                            if (isDisplayIcon)
                            {
                                if (string.IsNullOrEmpty(childTab.IconFile))
                                {
                                    iconUrl = string.Format("<img src='{0}' border='0' width='16px' height='16px'/>", Page.ResolveUrl("~/images/icon_unknown_16px.gif"));
                                }
                                else
                                {
                                    iconUrl = string.Format("<img src='{0}' border='0' width='16px' height='16px'/>", Page.ResolveUrl(childTab.IconFile));
                                }
                            }
                            childTab.TabName = prefix + iconUrl + tabInfo.TabName;
                            arrTab.Add(childTab);
                            //递归获取下一层次的array tab
                            ArrayList subTabArr = GetChildPageList(tabInfo.TabID, parentTabLevel, includeSelf, includeInvisible, isDisplayIcon);
                            foreach (object subTabItem in subTabArr)
                            {
                                arrTab.Add(subTabItem);
                            }

                        }
                    }

                }

            }

            return arrTab;
        }

        #endregion

    }
}
