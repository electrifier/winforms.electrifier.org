﻿/*
** 
**  electrifier
** 
**  Copyright 2017-19 Thorsten Jung, www.electrifier.org
**  
**  Licensed under the Apache License, Version 2.0 (the "License");
**  you may not use this file except in compliance with the License.
**  You may obtain a copy of the License at
**  
**      http://www.apache.org/licenses/LICENSE-2.0
**  
**  Unless required by applicable law or agreed to in writing, software
**  distributed under the License is distributed on an "AS IS" BASIS,
**  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
**  See the License for the specific language governing permissions and
**  limitations under the License.
**
*/


using electrifier.Core.Components;
using electrifier.Core.Components.DockContents;
using electrifier.Core.Forms;
using RibbonLib.Controls.Events;
using RibbonLib.Interop;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Vanara.PInvoke;
using WeifenLuo.WinFormsUI.Docking;

namespace RibbonLib.Controls
{
    /// <summary>
    /// 
    /// This partial class file contains the creation of the Ribbon.
    /// </summary>
    public partial class RibbonItems
    {
        #region Fields ====================================================================================================

        private ElClipboardAbilities clipboardAbilities = ElClipboardAbilities.CanCut | ElClipboardAbilities.CanCopy;

        private Shell32.FOLDERVIEWMODE shellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_AUTO;

        #endregion Fields =================================================================================================

        #region Properties =====================================================================================================

        public ElApplicationWindow ApplicationWindow { get; }

        private IDockContent activeDockContent = null;
        public IDockContent ActiveDockContent
        {
            get => this.activeDockContent;
            set
            {
                // Update the clipboard buttons accordingly if activated DockContent is an IElClipboardConsumer
                this.ClipboardAbilities = (value is IElClipboardConsumer clipboardConsumer) ?
                    clipboardConsumer.GetClipboardAbilities() : ElClipboardAbilities.None;
                this.activeDockContent = value;
            }
        }

        #endregion =============================================================================================================






        public ElClipboardAbilities ClipboardAbilities
        {
            get => this.clipboardAbilities;
            set
            {
                if (this.clipboardAbilities != value)
                {
                    this.clipboardAbilities = value;

                    // Update ribbon command button states
                    this.BtnClipboardCut.Enabled = value.HasFlag(ElClipboardAbilities.CanCut);
                    this.BtnClipboardCopy.Enabled = value.HasFlag(ElClipboardAbilities.CanCopy);
                }
            }
        }

        public Shell32.FOLDERVIEWMODE ShellFolderViewMode
        {
            get => this.shellFolderViewMode;
            set
            {
                //                    AppContext.TraceDebug($"Ribbon: ShellFolderViewMode = {value}");

                if (this.shellFolderViewMode == value)
                    return;

                this.ApplicationWindow.BeginInvoke(new MethodInvoker(delegate ()
                //this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    // https://docs.microsoft.com/en-us/windows/win32/windowsribbon/windowsribbon-reference-properties-uipkey-booleanvalue
                    //this.CmdBtnHomeViewExtraLargeIcons.UpdateProperty(ref "ApplicationDefaults.IsChecked", true, true);

                    this.shellFolderViewMode = value;
                    this.BtnHomeViewExtraLargeIcons.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_THUMBNAIL);
                    this.BtnHomeViewLargeIcons.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_ICON);
                    this.BtnHomeViewMediumIcons.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_THUMBSTRIP);
                    this.BtnHomeViewSmallIcons.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_SMALLICON);
                    this.BtnHomeViewList.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_LIST);
                    this.BtnHomeViewDetails.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_DETAILS);
                    this.BtnHomeViewTiles.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_TILE);
                    this.BtnHomeViewContent.BooleanValue = (value == Shell32.FOLDERVIEWMODE.FVM_CONTENT);
                }));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elApplicationWindow"></param>
        /// <param name="ribbon"></param>
        public RibbonItems(ElApplicationWindow elApplicationWindow, Ribbon ribbon)
            : this(ribbon)
        {
            this.ApplicationWindow = elApplicationWindow ?? throw new ArgumentNullException(nameof(elApplicationWindow));

            ////this.SetColors(Color.Wheat, Color.IndianRed, Color.BlueViolet);
            ///


            #region Event Handlers ============================================================================================

            //
            // Quick Access Toolbar Commands ==================================================================================
            //
            this.BtnQAT_OpenNewShellBrowserPanel.ExecuteEvent += this.ApplicationWindow.CmdAppOpenNewShellBrowserPane_ExecuteEvent;

            //
            // Application Menu Items =========================================================================================
            //
            this.BtnApp_OpenCommandPrompt.Enabled = false;
            this.BtnApp_OpenNewShellBrowserPanel.ExecuteEvent += this.ApplicationWindow.CmdAppOpenNewShellBrowserPane_ExecuteEvent;
            this.BtnApp_OpenWindowsPowerShell.Enabled = false;
            this.BtnApp_ChangeElectrifierOptions.Enabled = false;
            this.BtnApp_ChangeFolderAndSearchOptions.Enabled = false;
            this.BtnApp_Help_AboutElectrifier.ExecuteEvent += this.ApplicationWindow.CmdAppHelpAboutElectrifier_ExecuteEvent;
            this.BtnApp_Help_AboutWindows.ExecuteEvent += this.ApplicationWindow.CmdAppHelpAboutWindows_ExecuteEvent;
            this.BtnApp_Close.ExecuteEvent += this.ApplicationWindow.CmdAppClose_ExecuteEvent;

            //
            // Command Group: Home -> Clipboard ===============================================================================
            //
            this.BtnClipboardCut.ExecuteEvent += this.ApplicationWindow.CmdClipboardCut_ExecuteEvent;
            this.BtnClipboardCopy.ExecuteEvent += this.ApplicationWindow.CmdClipboardCopy_ExecuteEvent;
            this.BtnClipboardPaste.ExecuteEvent += this.ApplicationWindow.CmdClipboardPaste_ExecuteEvent;
            this.BtnClipboardHistory.Enabled = false;

            //
            // Command Group: Home -> Organise ================================================================================
            //
            this.BtnOrganiseMoveTo.Enabled = false;
            this.BtnOrganiseCopyTo.Enabled = false;
            this.BtnOrganiseDelete.Enabled = false;
            this.BtnOrganiseRename.Enabled = false;

            //
            // Command Group: Home -> Select ==================================================================================
            //
            this.BtnSelectConditional.Enabled = false;
            this.BtnSelectSelectAll.ExecuteEvent += this.ApplicationWindow.CmdSelectAll_ExecuteEvent;
            this.BtnSelectSelectNone.ExecuteEvent += this.ApplicationWindow.CmdSelectNone_ExecuteEvent;
            this.BtnSelectInvertSelection.ExecuteEvent += this.ApplicationWindow.CmdInvertSelection_ExecuteEvent;

            //
            // Command Group: Home -> Layout ==================================================================================
            //
            this.BtnHomeViewExtraLargeIcons.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewLargeIcons.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewMediumIcons.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewSmallIcons.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewList.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewDetails.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewTiles.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;
            this.BtnHomeViewContent.ExecuteEvent += this.CmdBtnHomeView_ExecuteEvent;

            //
            // Command Group: Desktop -> Icon Layout ==========================================================================
            //
            this.DesktopIconSettingsSaveLayoutButton.ExecuteEvent += this.ApplicationWindow.CmdBtnDesktopIconLayoutSave_ExecuteEvent;
            this.DesktopIconSettingsRestoreLayoutButton.ExecuteEvent += this.ApplicationWindow.CmdBtnDesktopIconLayoutRestore_ExecuteEvent;


            #endregion Event Handlers =========================================================================================

            //// TODO: Iterate through and disable all child elements that have no ExecuteEvent-Handler to get rid of those "Enabled=false"-Statements


            // For test purposes, enable all available Contexts
            this.DesktopToolsTabGroup.ContextAvailable = ContextAvailability.Active;

        }


        private void CmdBtnHomeView_ExecuteEvent(object sender, ExecuteEventArgs e)
        {
            Debug.Assert(sender is RibbonToggleButton);

            this.ApplicationWindow.BeginInvoke(new MethodInvoker(delegate ()
            {
                uint cmdID = (sender as BaseRibbonControl).CommandID;
                Shell32.FOLDERVIEWMODE newShellFolderViewMode;

                switch (cmdID)
                {
                    case Cmd.CmdBtnHomeViewExtraLargeIcons:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_THUMBNAIL;
                        break;
                    case Cmd.CmdBtnHomeViewLargeIcons:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_ICON;
                        break;
                    case Cmd.CmdBtnHomeViewMediumIcons:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_THUMBSTRIP;
                        break;
                    case Cmd.CmdBtnHomeViewSmallIcons:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_SMALLICON;
                        break;
                    case Cmd.CmdBtnHomeViewList:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_LIST;
                        break;
                    case Cmd.CmdBtnHomeViewDetails:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_DETAILS;
                        break;
                    case Cmd.CmdBtnHomeViewTiles:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_TILE;
                        break;
                    case Cmd.CmdBtnHomeViewContent:
                        newShellFolderViewMode = Shell32.FOLDERVIEWMODE.FVM_CONTENT;
                        break;
                    default:
                        throw new IndexOutOfRangeException(nameof(cmdID));
                }

                // Finally, apply new ViewMode if ActiveDockContent is of type ElNavigableDockContent
                if (this.ApplicationWindow.ActiveDockContent is ElNavigableDockContent navigableDockContent)
                    navigableDockContent.ShellFolderViewMode = newShellFolderViewMode;
            }));
        }

        /// <summary>
        /// Process <see cref="IElClipboardConsumer.ClipboardAbilitiesChanged"/> event.
        /// 
        /// In case sender is the active DockContent, update the clipboard buttons accordingly.
        /// </summary>
        /// <param name="sender">The <see cref="IElClipboardConsumer"/> that has changed its <see cref="clipboardAbilities"/>.</param>
        /// <param name="e">The <see cref="ClipboardAbilitiesChangedEventArgs"/>.</param>
        public void ClipboardConsumer_ClipboardAbilitiesChanged(object sender, ClipboardAbilitiesChangedEventArgs e)
        {
            Debug.Assert(sender is IElClipboardConsumer, "sender is not of type IElClipboardConsumer");

            if (sender.Equals(this.ApplicationWindow.ActiveDockContent))
                this.ClipboardAbilities = e.NewClipboardAbilities;
        }
    }
}
