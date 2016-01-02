using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using NotificationsExtensions.Tiles;

namespace TimeApprox.PRC
{
    public sealed class TileCreator
    {
        public static XmlDocument GenerateTile(string language, DateTimeOffset dateTime, Tier tier)
        {
            ApproxTime at = new ApproxTime(language);
            string time = at.GetCurrentApproxTime(dateTime, tier);
            
            return GenerateTextTile(time);
        }

        public static XmlDocument GenerateTextTile(string text)
        {
            TileContent tile = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.None,
                    TileMedium = new TileBinding() { Content = CreateMediumAdaptiveTextContent(text) },
                    TileWide = new TileBinding() { Content = CreateLargeAdaptiveTextContent(text) },
                    TileLarge = new TileBinding() { Content = CreateLargeAdaptiveTextContent(text) },
                }
            };

            var xdoc = new XmlDocument();
            xdoc.LoadXml(tile.GetContent());

            return xdoc;
        }

        private static TileBindingContentAdaptive CreateMediumAdaptiveTextContent(string text)
        {
            TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new TileText()
                    {
                        Text = text,
                        Style = TileTextStyle.Base,
                        Wrap = true,
                        Align = TileTextAlign.Center,
                    },
                },
                TextStacking = TileTextStacking.Center,
            };

            return bindingContent;
        }

        private static TileBindingContentAdaptive CreateLargeAdaptiveTextContent(string text)
        {
            TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new TileText()
                    {
                        Text = text,
                        Style = TileTextStyle.Subheader,
                        Wrap = true,
                        Align = TileTextAlign.Center,
                    },
                },
                TextStacking = TileTextStacking.Center,
            };

            return bindingContent;
        }
    }
}
