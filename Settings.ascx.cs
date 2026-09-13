/*
 *  This is a free module from DnnModule.com, you can use it as your wish.
 *
 * Version history
 *  11/05/2008 v2.0 First release as free module
 *
 *  6/13/2009  v2.2 Add two options:"Recursive" and "Child tab prefix"
 *
 * 9/17/2009  v2.5 add six options to build elegant menu style.
  *  12/10/2018 v4.0 upgrade to DNN 9.2 platform
  *  09/13/2026 v5.0 40Fingers upgrade to DNN 10.2 platform (replaced removed ModuleController instance methods GetModuleSettings/UpdateModuleSetting with PortalModuleBase.Settings and ModuleController.Instance; compatible with DNN 9.13.x and 10.2.x)
  *  09/13/2026 v5.1 40Fingers security: honour Page.IsValid on save, validate ListTemplate path, sanitize ColumnPerRow and whitelist LinkTarget before persisting
  *  09/13/2026 v5.0.1 40Fingers repackaged as a compiled DNN module (Visual Studio 2026 / .NET Framework 4.7.2 project); code-behind now compiled into CrossChildPageList.dll
 */

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Microsoft.VisualBasic;

using DotNetNuke;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common;



namespace Cross.Modules.ChildPageList
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class Settings : PortalModuleBase
    {
        // Control declarations - required in SDK-style projects (no designer file generated)
        protected DropDownList ddlParentTab;
        protected DropDownList ddlListTemplate;
        protected DropDownList ddlLinkTarget;
        protected CheckBox     chkIncludeSelf;
        protected CheckBox     chkIncludeInvisible;
        protected CheckBox     chkRecursive;
        protected CheckBox     chkDisplayIcon;
        protected TextBox      txtColumnCount;

        #region Private Members
        private string Parent_Tab = "ParentTab";
        private string Include_Self = "IncludeSelf";
        private string Include_Invisible = "IncludeInvisible";
        private string List_Template = "ListTemplate";
        private string Column_Per_Row = "ColumnPerRow";
        private string List_Template_Path = "Template";
        private string Link_Target = "LinkTarget";

        private string Recursive = "Recursive";


        private string DisplayIcon = "DisplayIcon";


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {

                    //  ddlParentTab.DataSource = DotNetNuke.Common.Globals.GetPortalTabs(PortalSettings.DesktopTabs, true, false);
                    ddlParentTab.DataSource = DotNetNuke.Entities.Tabs.TabController.GetPortalTabs(PortalId, DotNetNuke.Common.Utilities.Null.NullInteger, true, true);
                    ddlParentTab.DataBind();

                    //Get parent tab
                    if (this.Settings[Parent_Tab] != null && Convert.ToString(this.Settings[Parent_Tab]) != "")
                    {
                        string parentTab = Convert.ToString(this.Settings[Parent_Tab]);
                        if (ddlParentTab.Items.FindByValue(parentTab) != null)
                        {
                            ddlParentTab.SelectedValue = parentTab;
                        }
                    }
                    else
                    {
                        ddlParentTab.SelectedValue = TabId.ToString();
                    }

                    //Check include self
                    if (this.Settings[Include_Self] != null && Convert.ToString(this.Settings[Include_Self]) != "")
                    {

                        chkIncludeSelf.Checked = Convert.ToBoolean(Convert.ToString(this.Settings[Include_Self]));
                    }

                    //check include invisible
                    if (this.Settings[Include_Invisible] != null && Convert.ToString(this.Settings[Include_Invisible]) != "")
                    {

                        chkIncludeInvisible.Checked = Convert.ToBoolean(Convert.ToString(this.Settings[Include_Invisible]));
                    }

                    //check recursive option
                    if (this.Settings[Recursive] != null && Convert.ToString(this.Settings[Recursive]) != "")
                    {
                        chkRecursive.Checked = Convert.ToBoolean(Convert.ToString(this.Settings[Recursive]));
                    }



                    //check displayicon option
                    if (this.Settings[DisplayIcon] != null && Convert.ToString(this.Settings[DisplayIcon]) != "")
                    {
                        chkDisplayIcon.Checked = Convert.ToBoolean(Convert.ToString(this.Settings[DisplayIcon]));
                    }




                    //Check columns per row
                    if (this.Settings[Column_Per_Row] != null && Convert.ToString(this.Settings[Column_Per_Row]) != "")
                    {
                        txtColumnCount.Text = Convert.ToString(this.Settings[Column_Per_Row]);
                    }

                    //Check list template
                    string listTemplatePath = "default/List_Standard.ascx";
                    if (this.Settings[List_Template] != null && Convert.ToString(this.Settings[List_Template]) != "")
                    {
                        listTemplatePath = Convert.ToString(this.Settings[List_Template]);
                    }

                    //  ddlListTemplate = FillSubDirectoryTemplate(ddlListTemplate, Request.MapPath(this.ModulePath + List_Template_Path), listTemplatePath, "*.ascx");
                    ddlListTemplate = FillSubDirectoryTemplate(ddlListTemplate, Request.MapPath(this.ControlPath + List_Template_Path), listTemplatePath, "*.ascx");

                    //Check link target
                    if (this.Settings[Link_Target] != null && Convert.ToString(this.Settings[Link_Target]) != "")
                    {
                        string linkTarget = Convert.ToString(this.Settings[Link_Target]);
                        if (ddlLinkTarget.Items.FindByValue(linkTarget) != null)
                        {
                            ddlLinkTarget.SelectedValue = linkTarget;
                        }
                    }

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #region Base Method Implementations



        private System.Web.UI.WebControls.DropDownList FillSubDirectoryTemplate(System.Web.UI.WebControls.DropDownList ddlTemplate, String path, string selectedValue, string fileExtension)
        {
            string[] folders;
            string[] files;
            string seperator = "/";

            ListItem item;
            if (Directory.Exists(path))
            {
                folders = Directory.GetDirectories(path);
                foreach (string folder in folders)
                {
                    files = Directory.GetFiles(folder, fileExtension);
                    foreach (string file in files)
                    {
                        string folderName = Strings.Mid(folder, Strings.InStrRev(folder, "\\", -1, CompareMethod.Text) + 1);

                        item = new ListItem();
                        item.Text = folderName + seperator + Path.GetFileNameWithoutExtension(file);
                        item.Value = folderName + seperator + Path.GetFileName(file);
                        if (item.Value == selectedValue)
                        {
                            item.Selected = true;
                        }
                        ddlTemplate.Items.Add(item);


                    }
                }

            }
            return ddlTemplate;
        }

        #endregion


        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            // Honour server-side validators (e.g. the integer check on ColumnPerRow) before
            // persisting anything. Without this the validators are advisory only.
            if (!Page.IsValid)
            {
                return;
            }

            IModuleController ctlModule = ModuleController.Instance;

            ctlModule.UpdateModuleSetting(ModuleId, Parent_Tab, ddlParentTab.SelectedValue);
            ctlModule.UpdateModuleSetting(ModuleId, Include_Self, chkIncludeSelf.Checked.ToString());
            ctlModule.UpdateModuleSetting(ModuleId, Include_Invisible, chkIncludeInvisible.Checked.ToString());

            ctlModule.UpdateModuleSetting(ModuleId, Recursive, chkRecursive.Checked.ToString());

            ctlModule.UpdateModuleSetting(ModuleId, DisplayIcon, chkDisplayIcon.Checked.ToString());

            // ColumnPerRow must be a non-negative integer; fall back to "1" on anything unexpected.
            int columnPerRow;
            if (!int.TryParse(txtColumnCount.Text, out columnPerRow) || columnPerRow < 1)
            {
                columnPerRow = 1;
            }
            ctlModule.UpdateModuleSetting(ModuleId, Column_Per_Row, columnPerRow.ToString());

            // Only store a template value that is a safe, in-folder *.ascx path (defends the
            // LoadControl in View.ascx.cs against path-traversal to arbitrary user controls).
            string listTemplate = ddlListTemplate.SelectedValue;
            if (IsValidTemplatePath(listTemplate))
            {
                ctlModule.UpdateModuleSetting(ModuleId, List_Template, listTemplate);
            }

            // LinkTarget is a fixed choice; never persist an arbitrary posted value.
            string linkTarget = ddlLinkTarget.SelectedValue == "_blank" ? "_blank" : "_self";
            ctlModule.UpdateModuleSetting(ModuleId, Link_Target, linkTarget);

            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId), true);
        }

        /// <summary>
        /// Validates that a configured template path is a safe, relative *.ascx path that
        /// stays inside the module's Template folder. Rejects rooted paths, backslashes and
        /// any ".." traversal segment.
        /// </summary>
        private static bool IsValidTemplatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (path.IndexOf("..", StringComparison.Ordinal) >= 0)
            {
                return false;
            }

            if (path.IndexOf('\\') >= 0 || path.StartsWith("/") || path.StartsWith("~"))
            {
                return false;
            }

            return path.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase);
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// cmdCancel_Click runs when the cancel button is clicked
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void cmdReturn_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId), true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
