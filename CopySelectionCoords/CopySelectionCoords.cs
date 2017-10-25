using System;
using System.Drawing;
using System.Reflection;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;
using System.Windows.Forms;
using System.Threading;

namespace CopySelectionCoordsEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author => ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
        public string Copyright => ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
        public string DisplayName => ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("http://www.getpaint.net/redirect/plugins.html");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Copy Selection Coords")]
    public class CopySelectionCoordsEffectPlugin : PropertyBasedEffect
    {
        private const string StaticName = "Copy Selection Coords";
        private static readonly Image StaticIcon = null;
        private const string SubmenuName = "Tools";

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
            string selectionCoords = $"{selection.Left}, {selection.Top}, {selection.Width}, {selection.Height}";
            Clipboard.SetText(selectionCoords);
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return PropertyCollection.CreateEmpty();
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (length == 0) return;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                DstArgs.Surface.CopySurface(SrcArgs.Surface, renderRects[i].Location, renderRects[i]);
            }
        }
    }
}
