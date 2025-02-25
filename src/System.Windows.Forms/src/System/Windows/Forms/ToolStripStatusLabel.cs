﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A non selectable ToolStrip item
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public partial class ToolStripStatusLabel : ToolStripLabel, IAutomationLiveRegion
    {
        private static readonly Padding defaultMargin = new Padding(0, 3, 0, 2);
        private Padding scaledDefaultMargin = defaultMargin;

        private Border3DStyle borderStyle = Border3DStyle.Flat;
        private ToolStripStatusLabelBorderSides borderSides = ToolStripStatusLabelBorderSides.None;
        private bool spring;
        private AutomationLiveSetting liveSetting;

        /// <summary>
        ///  A non selectable ToolStrip item
        /// </summary>
        public ToolStripStatusLabel()
        {
            Initialize();
        }

        public ToolStripStatusLabel(string text) : base(text, null, false, null)
        {
            Initialize();
        }

        public ToolStripStatusLabel(Image image) : base(null, image, false, null)
        {
            Initialize();
        }

        public ToolStripStatusLabel(string text, Image image) : base(text, image, false, null)
        {
            Initialize();
        }

        public ToolStripStatusLabel(string text, Image image, EventHandler onClick) : base(text, image,/*isLink=*/false, onClick, null)
        {
            Initialize();
        }

        public ToolStripStatusLabel(string text, Image image, EventHandler onClick, string name) : base(text, image,/*isLink=*/false, onClick, name)
        {
            Initialize();
        }

        /// <summary>
        ///  Creates a new AccessibleObject for this ToolStripStatusLabel instance.
        ///  The AccessibleObject instance returned by this method supports UIA Live Region feature.
        /// </summary>
        /// <returns>
        ///  AccessibleObject for this ToolStripStatusLabel instance.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripStatusLabelAccessibleObject(this);
        }

        /// <summary>
        ///  Creates an instance of the object that defines how image and text
        ///  gets laid out in the ToolStripItem
        /// </summary>
        private protected override ToolStripItemInternalLayout CreateInternalLayout()
        {
            return new ToolStripStatusLabelLayout(this);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new ToolStripItemAlignment Alignment
        {
            get => base.Alignment;
            set => base.Alignment = value;
        }

        [DefaultValue(Border3DStyle.Flat)]
        [SRDescription(nameof(SR.ToolStripStatusLabelBorderStyleDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public Border3DStyle BorderStyle
        {
            get
            {
                return borderStyle;
            }
            set
            {
                SourceGenerated.EnumValidator.Validate(value);

                if (borderStyle != value)
                {
                    borderStyle = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(ToolStripStatusLabelBorderSides.None)]
        [SRDescription(nameof(SR.ToolStripStatusLabelBorderSidesDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public ToolStripStatusLabelBorderSides BorderSides
        {
            get
            {
                return borderSides;
            }
            set
            {
                // no Enum.IsDefined as this is a flags enum.
                if (borderSides != value)
                {
                    borderSides = value;
                    LayoutTransaction.DoLayout(Owner, this, PropertyNames.BorderStyle);
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Called by all constructors of ToolStripButton.
        /// </summary>
        private void Initialize()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(defaultMargin);
            }
        }

        protected internal override Padding DefaultMargin
        {
            get
            {
                return scaledDefaultMargin;
            }
        }

        [DefaultValue(false)]
        [SRDescription(nameof(SR.ToolStripStatusLabelSpringDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public bool Spring
        {
            get { return spring; }
            set
            {
                if (spring != value)
                {
                    spring = value;
                    if (ParentInternal is not null)
                    {
                        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Spring);
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates the "politeness" level that a client should use
        ///  to notify the user of changes to the live region.
        /// </summary>
        [SRCategory(nameof(SR.CatAccessibility))]
        [DefaultValue(AutomationLiveSetting.Off)]
        [SRDescription(nameof(SR.LiveRegionAutomationLiveSettingDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutomationLiveSetting LiveSetting
        {
            get
            {
                return liveSetting;
            }
            set
            {
                SourceGenerated.EnumValidator.Validate(value);
                liveSetting = value;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (IsParentAccessibilityObjectCreated && LiveSetting != AutomationLiveSetting.Off)
            {
                AccessibilityObject.RaiseLiveRegionChanged();
            }
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (BorderSides != ToolStripStatusLabelBorderSides.None)
            {
                return base.GetPreferredSize(constrainingSize) + new Size(4, 4);
            }
            else
            {
                return base.GetPreferredSize(constrainingSize);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Owner is not null)
            {
                ToolStripRenderer renderer = Renderer;

                renderer.DrawToolStripStatusLabelBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

                if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                {
                    renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, InternalLayout.ImageRectangle));
                }

                PaintText(e.Graphics);
            }
        }
    }
}
