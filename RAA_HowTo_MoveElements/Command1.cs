#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAA_HowTo_MoveElements
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // prompt the user to select elements to move
            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements to move");

            int counter = 0;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Move Elements");
                foreach (Element elem in pickList)
                {
                    XYZ oldPoint = null;
                    // filter out anything that isn't a family instance
                    if (elem is FamilyInstance)
                    {
                        // get the element's location point
                        LocationPoint oldLocation = elem.Location as LocationPoint;
                        oldPoint = oldLocation.Point as XYZ;
                    }
                    else if(elem is Wall)
                    {
                        // get the element's end point
                        LocationCurve oldLocation = elem.Location as LocationCurve;
                        oldPoint = oldLocation.Curve.GetEndPoint(0);
                    }

                    if(oldPoint != null)
                    {
                        // move the element
                        XYZ newPoint = new XYZ(0 - oldPoint.X, 0 - oldPoint.Y, 0 - oldPoint.Z);
                        elem.Location.Move(newPoint);
                        counter++;
                    }
                }
                t.Commit();

                if(counter > 0)
                {
                    TaskDialog.Show("Complete", $"Moved {counter} elements.");
                }
                else
                {
                    TaskDialog.Show("Complete", "There were no elements to move");
                }
            }


            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
