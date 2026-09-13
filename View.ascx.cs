/*
 *  This is a free module from DnnModule.com, you can use it as your wish.
 *  Any suggestions , please visit our forum athttp://DnnModule.com or mail us at xiaoqi98@msn.com
 *
 * Version history
 *  11/05/2005 v2.0 First release as free module
 *  12/10/2018 v4.0 upgrade to DNN 9.2 platform
 *  09/13/2026 v5.0 40Fingers upgrade to DNN 10.2 platform (replaced removed ModuleController.GetModuleSettings with PortalModuleBase.Settings; compatible with DNN 9.13.x and 10.2.x)
 *  09/13/2026 v5.1 40Fingers security: validate the ListTemplate setting before LoadControl to block path traversal to arbitrary user controls
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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;

namespace Cross.Modules.ChildPageList
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class View:PortalModuleBase,IActionable
    {
        // Control declarations - required in SDK-style projects (no designer file generated)
        protected PlaceHolder phTemplate;

        #region Private Members


        private string List_Template = "ListTemplate";


        #endregion



        #region Private Methods

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
                PortalModuleBase objListTemplate;


                //Check list template
                string listTemplatePath = "default/List_Standard.ascx";

                object listTemplateSetting = this.Settings[List_Template];
                string candidate = Convert.ToString(listTemplateSetting);
                // Only accept a safe, in-folder template path. This blocks path traversal
                // (e.g. "../../SomeModule/Control.ascx") that would otherwise let LoadControl
                // instantiate an arbitrary user control outside the module's Template folder.
                if (IsValidTemplatePath(candidate))
                {
                    listTemplatePath = candidate;
                }

                objListTemplate = (PortalModuleBase)this.LoadControl("Template/" + listTemplatePath);
                objListTemplate.ModuleConfiguration = this.ModuleConfiguration;
                phTemplate.Controls.Add(objListTemplate);

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }


        #endregion





        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {

                ModuleActionCollection Actions = new ModuleActionCollection();
                Actions.Add(this.GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, this.LocalResourceFile), ModuleActionType.AddContent, "", "", this.EditUrl(), false, SecurityAccessLevel.Edit, true, false);
                Actions.Add(this.GetNextActionID(), Localization.GetString("OnlineHelp.Text", this.LocalResourceFile), ModuleActionType.OnlineHelp, "", "", "http://DnnModule.com", false, SecurityAccessLevel.Edit, true,true);
                return Actions;
            }
        }

        #endregion
    }
}
