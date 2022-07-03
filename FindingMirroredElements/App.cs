using Autodesk.Revit.UI;
using FindingMirroredElements.IExternalCommands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FindingMirroredElements
{
    public class App : IExternalApplication
    {
        private const string TabName = "V2 Tools";
        private const string PanelName = "Элементы АР";
        private const string ButtonName = "Поиск\nотзеркаленных";
        private readonly string _buttonTooltip = $"Поиск отзеркаленных окон, дверей, обобщенных моделей, колонн и витражей.\n{typeof(App).Assembly.GetName().Version}";
        public const string Title = "Поиск отзеркаленных элементов";
        private static readonly List<string> RequiredRevitVersions = new() { "2019", "2020", "2021", "2022", "2023" };

        public Result OnStartup(UIControlledApplication application)
        {
            if (RunningWrongRevitVersion(application.ControlledApplication.VersionNumber))
            {
                return Result.Cancelled;
            }

            CreateRibbonTab(application);
            var panel = CreateRibbonPanel(application);
            CreateButton(panel);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private static bool RunningWrongRevitVersion(string currentRevitVersion)
        {
            return !RequiredRevitVersions.Contains(currentRevitVersion);
        }

        private void CreateRibbonTab(UIControlledApplication application)
        {
            try
            {
                application.CreateRibbonTab(TabName);
            }
            catch
            {
                // ignored
            }
        }

        private RibbonPanel CreateRibbonPanel(UIControlledApplication application)
        {
            foreach (RibbonPanel panel in application.GetRibbonPanels(TabName))
            {
                if (panel.Name == PanelName)
                {
                    return panel;
                }
            }

            return application.CreateRibbonPanel(TabName, PanelName);
        }

        private void CreateButton(RibbonPanel panel)
        {
            var buttonData = new PushButtonData(
                nameof(FindingMirroredElements),
                ButtonName,
                typeof(Command).Assembly.Location,
                typeof(Command).FullName
            );

            var pushButton = panel.AddItem(buttonData) as PushButton;
            pushButton!.LargeImage = GetImageSourceByBitMapFromResource(Properties.Resources.flip_FILL0_wght400_GRAD0_opsz48, 32);
            pushButton.Image = GetImageSourceByBitMapFromResource(Properties.Resources.flip_FILL0_wght400_GRAD0_opsz48, 16);
            pushButton.ToolTip = _buttonTooltip;
        }

        private ImageSource GetImageSourceByBitMapFromResource(Bitmap source, int size)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size, size)
            );
        }
    }
}
