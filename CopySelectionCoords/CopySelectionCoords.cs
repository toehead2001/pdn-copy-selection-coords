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
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("https://www.getpaint.net/redirect/plugins.html");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Copy Selection Coords")]
    public class CopySelectionCoordsEffectPlugin : PropertyBasedEffect
    {
        public CopySelectionCoordsEffectPlugin()
            : base("Copy Selection Coords", null, "Tools", new EffectOptions { Flags = EffectFlags.None })
        {
        }

        private Rectangle selection;

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
