using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;
using System.Windows.Forms;
using System.Threading;

namespace CopySelectionCoordsEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://www.getpaint.net/redirect/plugins.html");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Copy Selection Coords")]
    public class CopySelectionCoordsEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "Copy Selection Coords";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return null;
            }
        }

        public static string SubmenuName
        {
            get
            {
                return "Tools";
            }
        }

        public CopySelectionCoordsEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.None)
        {
        }

        Rectangle selection;

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();

            Thread t = new Thread(new ThreadStart(CopySelectionCoords));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        private void CopySelectionCoords()
        {
            string selectionCoords = selection.Left.ToString() + ", " + selection.Top.ToString() + ", " + selection.Width.ToString() + ", " + selection.Height.ToString();
            Clipboard.SetText(selectionCoords);
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            return new PropertyCollection(props);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
        }

    }
}
