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

using RibbonLib.Controls;
using RibbonLib.Controls.Events;
using RibbonLib.Controls.Properties;
using System;

namespace electrifier.Core.Components
{
    // INotifyPropertyChanged
    // https://docs.microsoft.com/de-de/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?view=net-5.0


    public interface IRibbonConsumer
    {
        RibbonItems RibbonItems { get; }

        //        RibbonConsumerStateMap RibbonStateMap { get; }


        IBaseRibbonControlBinding[] InitializeRibbonBinding(RibbonItems ribbonItems);

        void ActivateRibbonState();

        // TODO 27.04.21, 17:17: IsActivelyBoundToRibbon() Property


        /// <summary>
        /// Eigentlich Unsinn, weil wir könnten auch alle RibbonControls abklappern und
        /// diejenigen, die im neuen DockPanel nicht gesetzt sind einfach ausknippsen...
        /// </summary>
        void DeactivateRibbonState();

    }

    public interface IBaseRibbonControlBinding
    {
        //        BaseRibbonControl BaseRibbonControl { get; }

        void UpdateBaseControlState();
    }


    /// <summary>
    /// The underlying <see cref="RibbonButton"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IEnabledPropertiesProvider</term><description>IEnabledPropertiesProvider</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ILabelPropertiesProvider</term><description>ILabelPropertiesProvider</description></item>
    /// <item><term>ILabelDescriptionPropertiesProvider</term><description>ILabelDescriptionPropertiesProvider</description></item>
    /// <item><term>IImagePropertiesProvider</term><description>IImagePropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// <item><term>IExecuteEventsProvider</term><description>IExecuteEventsProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonButtonBinding :
        IBaseRibbonControlBinding,
        IEnabledPropertiesProvider,
        IExecuteEventsProvider
    {
        public RibbonButton BaseRibbonControl { get; }

        private bool enabled;
        public bool Enabled
        {
            get => this.enabled;
            set
            {
                this.enabled = value;
                // TODO: Enable base button! But only if this RibbonConsumer is the active DockPanelDocument
                // => Wir brauchen eine Referenz zum IRibbonConsumer,
                //    der Eigenschaft IsActiveRibbonConsumer hat...
                // => ABER: Wir führen eigene Liste mit allen RibbonConsumern, und wenn einer aktiviert wird der alte deaktiviert, mit Debug-Code ob wirklich nur einer aktiv!
                this.BaseRibbonControl.Enabled = value;
            }
        }

        public event EventHandler<ExecuteEventArgs> ExecuteEvent;

        public RibbonButtonBinding(RibbonButton ribbonButton, EventHandler<ExecuteEventArgs> executeEvent, bool enabled = false)
        {
            this.BaseRibbonControl = ribbonButton;

            this.Enabled = enabled;
            this.ExecuteEvent = executeEvent;
        }

        public void UpdateBaseControlState()
        {
            this.BaseRibbonControl.Enabled = this.Enabled;
        }
    }

    /// <summary>
    /// The underlying <see cref="RibbonDropDownButton"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IEnabledPropertiesProvider</term><description>IEnabledPropertiesProvider</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ILabelPropertiesProvider</term><description>ILabelPropertiesProvider</description></item>
    /// <item><term>ILabelDescriptionPropertiesProvider</term><description>ILabelDescriptionPropertiesProvider</description></item>
    /// <item><term>IImagePropertiesProvider</term><description>IImagePropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonDropDownButtonBinding :
        IBaseRibbonControlBinding
    {
        public RibbonDropDownButton BaseRibbonControl { get; }

        private bool enabled;
        public bool Enabled
        {
            get => this.enabled;
            set
            {
                this.enabled = value;
                // TODO: Enable base button! But only if this RibbonConsumer is the active DockPanelDocument
            }
        }

        public RibbonDropDownButtonBinding(RibbonDropDownButton ribbonDropDownButton)
        {
            this.BaseRibbonControl = ribbonDropDownButton;

        }

        public void UpdateBaseControlState()
        {
            this.BaseRibbonControl.Enabled = this.Enabled;
        }
    }

    /// <summary>
    /// The underlying <see cref="RibbonGroup"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ILabelPropertiesProvider</term><description>ILabelPropertiesProvider</description></item>
    /// <item><term>IImagePropertiesProvider</term><description>IImagePropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonGroupBinding :
        IBaseRibbonControlBinding
    {
        public RibbonGroup BaseRibbonControl { get; }

        public RibbonGroupBinding(RibbonGroup ribbonGroup)
        {
            this.BaseRibbonControl = ribbonGroup;
        }
        public void UpdateBaseControlState()
        {
        }
    }

    /// <summary>
    /// The underlying <see cref="RibbonMenuGroup"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IEnabledPropertiesProvider</term><description>IEnabledPropertiesProvider</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ILabelPropertiesProvider</term><description>ILabelPropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonMenuGroupBinding :
        IBaseRibbonControlBinding
    {
        public RibbonMenuGroup BaseRibbonControl { get; }

        private bool enabled;
        public bool Enabled
        {
            get => this.enabled;
            set
            {
                this.enabled = value;
                // TODO: Enable base button! But only if this RibbonConsumer is the active DockPanelDocument
            }
        }

        public RibbonMenuGroupBinding(RibbonMenuGroup ribbonMenuGroup)
        {
            this.BaseRibbonControl = ribbonMenuGroup;

        }

        public void UpdateBaseControlState()
        {
            this.BaseRibbonControl.Enabled = this.Enabled;
        }
    }

    /// <summary>
    /// The underlying <see cref="RibbonSplitButton"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IEnabledPropertiesProvider</term><description>IEnabledPropertiesProvider</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonSplitButtonBinding :
        IBaseRibbonControlBinding
    {
        public RibbonSplitButton BaseRibbonControl { get; }

        private bool enabled;
        public bool Enabled
        {
            get => this.enabled;
            set
            {
                this.enabled = value;
                // TODO: Enable base button! But only if this RibbonConsumer is the active DockPanelDocument
            }
        }

        public RibbonSplitButtonBinding(RibbonSplitButton ribbonSplitButton, bool enabled = true)
        {
            this.BaseRibbonControl = ribbonSplitButton;
            this.Enabled = enabled;
        }


        public void UpdateBaseControlState()
        {
            this.BaseRibbonControl.Enabled = this.Enabled;
        }
    }

    /// <summary>
    /// The underlying <see cref="RibbonToggleButton"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ILabelPropertiesProvider</term><description>ILabelPropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonToggleButtonBinding :
        IBaseRibbonControlBinding
    {
        public RibbonToggleButton BaseRibbonControl { get; }
        public RibbonToggleButtonBinding(RibbonToggleButton ribbonToggleButton)
        {
            this.BaseRibbonControl = ribbonToggleButton;
        }
        public void UpdateBaseControlState()
        {
        }
    }



    /// <summary>
    /// The underlying <see cref="RibbonTab"/> derives from:
    /// <list type="bullet">
    /// <item><term>BaseRibbonControl</term><description>BaseRibbonControl</description></item>
    /// <item><term>IKeytipPropertiesProvider</term><description>IKeytipPropertiesProvider</description></item>
    /// <item><term>ILabelPropertiesProvider</term><description>ILabelPropertiesProvider</description></item>
    /// <item><term>ITooltipPropertiesProvider</term><description>ITooltipPropertiesProvider</description></item>
    /// </list>
    /// </summary>
    public class RibbonTabBinding :
        IBaseRibbonControlBinding
    {
        public BaseRibbonControl BaseRibbonControl { get; }
        public RibbonTabBinding(RibbonTab ribbonTab)
        {
            this.BaseRibbonControl = ribbonTab;
        }

        // TODO: => Where is Visible property!?!
        public void UpdateBaseControlState()
        {
        }
    }
}
