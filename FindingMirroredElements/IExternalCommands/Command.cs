using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FindingMirroredElements.Services;
using FindingMirroredElements.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FindingMirroredElements.IExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Command : IExternalCommand
    {
        private static Document _doc;
        private int _flippedElementCounter;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitManager.CommandData = commandData;
            _doc = RevitManager.Document;

            try
            {
                DoJob();
                ShowReport();
            }
            catch (Exception e)
            {
                var report = $"Ошибка: {e.Message}";
                var extra = $"Ошибка: {e}";
                var reportWindow = new ReportWindow(report, extra);
                reportWindow.ShowDialog();
            }

            return Result.Succeeded;
        }        

        private void ShowReport()
        {
            var report = $"Отзеркалено {_flippedElementCounter}.";
            var reportWindow = new ReportWindow(report);
            reportWindow.ShowDialog();
        }
        private void DoJob()
        {
            List<Element> elements = new List<Element>();

            var windows = GetWindows();
            var stainedglasses = GetStainedGlass();
            var doors = GetDoors();
            var columns = GetColumns();
            var genericModels = GetGenericModels();

            elements.AddRange(windows);
            elements.AddRange(stainedglasses);
            elements.AddRange(doors);
            elements.AddRange(columns);
            elements.AddRange(genericModels);
                
            using var t = new Transaction(_doc);

            t.Start(App.Title);

            foreach (FamilyInstance fi in elements)
            {
                fi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("");

                if (fi.Mirrored && fi.SuperComponent == null)
                {
                    // отсеиваем витражи
                    if (fi.Host != null && ((fi.Host) as Wall).CurtainGrid != null)
                    {
                        fi.Host.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Элемент отзеркален");
                    }
                    else
                    {
                        fi.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Элемент отзеркален");
                        _flippedElementCounter++;
                    }
                }
            }

            t.Commit();
        }

        private static IEnumerable<Element> GetWindows()
        {
            return new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Windows);
        }

        private static IEnumerable<Element> GetStainedGlass()
        {
            return new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_CurtainWallPanels);
        }

        private static IEnumerable<Element> GetDoors()
        {
            return new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Doors);
        }

        private static IEnumerable<Element> GetColumns()
        {
            return new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Columns);
        }

        private static IEnumerable<Element> GetWalls()
        {
            return new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Walls);
        }

        private static IEnumerable<Element> GetGenericModels()
        {
            return new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_GenericModel).ToElements();
        }
    }
}