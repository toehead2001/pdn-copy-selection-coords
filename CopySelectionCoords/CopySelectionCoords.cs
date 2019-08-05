using PaintDotNet;
using PaintDotNet.AppModel;
using PaintDotNet.Clipboard;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;
using System;
using System.Drawing;
using System.Reflection;

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

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();
            string selectionCoords = $"{selection.Left}, {selection.Top}, {selection.Width}, {selection.Height}";

            Services.GetService<IClipboardService>().SetText(selectionCoords);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
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
