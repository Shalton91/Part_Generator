using SolidEdgeCommunity.Extensions; // https://github.com/SolidEdgeCommunity/SolidEdge.Community/wiki/Using-Extension-Methods
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class VariablesHelper
{
    public static void ReportVariables(SolidEdgeAssembly.AssemblyDocument document)
    {
        ReportVariables((SolidEdgeFramework.SolidEdgeDocument)document);
    }

    public static void ReportVariables(SolidEdgeDraft.DraftDocument document)
    {
        ReportVariables((SolidEdgeFramework.SolidEdgeDocument)document);
    }

    public static void ReportVariables(SolidEdgePart.PartDocument document)
    {
        ReportVariables((SolidEdgeFramework.SolidEdgeDocument)document);
    }

    public static void ReportVariables(SolidEdgePart.SheetMetalDocument document)
    {
        ReportVariables((SolidEdgeFramework.SolidEdgeDocument)document);
    }

    public static void ReportVariables(SolidEdgePart.WeldmentDocument document)
    {
        ReportVariables((SolidEdgeFramework.SolidEdgeDocument)document);
    }

    public static void ReportVariables(SolidEdgeFramework.SolidEdgeDocument document)
    {
        SolidEdgeFramework.Variables variables = null;
        SolidEdgeFramework.VariableList variableList = null;
        SolidEdgeFramework.variable variable = null;
        SolidEdgeFrameworkSupport.Dimension dimension = null;

        if (document == null) throw new ArgumentNullException("document");

        // Get a reference to the Variables collection.
        variables = (SolidEdgeFramework.Variables)document.Variables;

        // Get a reference to the variablelist.
        variableList = (SolidEdgeFramework.VariableList)variables.Query(
            pFindCriterium: "*",
            NamedBy: SolidEdgeConstants.VariableNameBy.seVariableNameByBoth,
            VarType: SolidEdgeConstants.VariableVarType.SeVariableVarTypeBoth);

        // Process variables.
        foreach (var variableListItem in variableList.OfType<object>())
        {
            // Not used in this sample but a good example of how to get the runtime type.
            var variableListItemType = SolidEdgeCommunity.Runtime.InteropServices.ComObject.GetType(variableListItem);

            // Use helper class to get the object type.
            var objectType = SolidEdgeCommunity.Runtime.InteropServices.ComObject.GetPropertyValue<SolidEdgeFramework.ObjectType>(variableListItem, "Type", (SolidEdgeFramework.ObjectType)0);

            // Process the specific variable item type.
            switch (objectType)
            {
                case SolidEdgeFramework.ObjectType.igDimension:
                    // Get a reference to the dimension.
                    dimension = (SolidEdgeFrameworkSupport.Dimension)variableListItem;
                    Console.WriteLine("Dimension: '{0}' = '{1}' ({2})", dimension.DisplayName, dimension.Value, objectType);
                    break;
                case SolidEdgeFramework.ObjectType.igVariable:
                    variable = (SolidEdgeFramework.variable)variableListItem;
                    Console.WriteLine("Variable: '{0}' = '{1}' ({2})", variable.DisplayName, variable.Value, objectType);
                    break;
                default:
                    // Other SolidEdgeConstants.ObjectType's may exist.
                    break;
            }
        }
    }

    public static void UpdateDimensions(SolidEdgeFramework.SolidEdgeDocument document, Dictionary<string,double> DimName)
    {
        SolidEdgeFramework.Variables variables = null;
        SolidEdgeFramework.VariableList variableList = null;
        SolidEdgeFrameworkSupport.Dimension dimension = null;

        if (document == null) throw new ArgumentNullException("document");

        // Get a reference to the Variables collection.
        variables = (SolidEdgeFramework.Variables)document.Variables;

        // Get a reference to the variablelist.
        variableList = (SolidEdgeFramework.VariableList)variables.Query(
            pFindCriterium: "*",
            NamedBy: SolidEdgeConstants.VariableNameBy.seVariableNameByBoth,
            VarType: SolidEdgeConstants.VariableVarType.SeVariableVarTypeBoth);

        // Process variables.
        foreach (var variableListItem in variableList.OfType<object>())
        {
            // Not used in this sample but a good example of how to get the runtime type.
            var variableListItemType = SolidEdgeCommunity.Runtime.InteropServices.ComObject.GetType(variableListItem);

            // Use helper class to get the object type.
            var objectType = SolidEdgeCommunity.Runtime.InteropServices.ComObject.GetPropertyValue<SolidEdgeFramework.ObjectType>(variableListItem, "Type", (SolidEdgeFramework.ObjectType)0);

            //Process the specific variable item type.

            if (objectType == SolidEdgeFramework.ObjectType.igDimension)
            {
                dimension = (SolidEdgeFrameworkSupport.Dimension)variableListItem;
                if (DimName.Keys.Contains(dimension.DisplayName))
                {
                    dimension.Value = DimName[dimension.DisplayName];
                    
                }

            }
        }
    }
    public static void UpdatePrperties(SolidEdgeFramework.SolidEdgeDocument document, Dictionary<string, string> PropName)
    {
        var propertySets = (SolidEdgeFramework.PropertySets)document.Properties;
        foreach (var properties in propertySets.OfType<SolidEdgeFramework.Properties>())
        {
            Console.WriteLine(properties.Name);
            foreach (var property in properties.OfType<SolidEdgeFramework.Property>())
            {
                Console.WriteLine("     " + property.Name);
                if (PropName.Keys.Contains(property.Name))
                {
                    property.set_Value(PropName[property.Name]);
                }
            }
        }

    }
    public static void UpdateDrawingViews(SolidEdgeDraft.DraftDocument draftDocument)
    {
        SolidEdgeDraft.Sections sections = null;
        SolidEdgeDraft.Section section = null;
        SolidEdgeDraft.SectionSheets sectionSheets = null;
        SolidEdgeDraft.DrawingViews drawingViews = null;
        try
        {
            if (draftDocument != null)
            {
                // Get a reference to the Sections collection.
                sections = draftDocument.Sections;

                // Get a reference to the WorkingSection.
                section = sections.WorkingSection;

                // Get a reference to the Sheets collection.
                sectionSheets = section.Sheets;

                foreach (var sheet in sectionSheets.OfType<SolidEdgeDraft.Sheet>())
                {
                    Console.WriteLine("Processing sheet '{0}'.", sheet.Name);

                    // Get a reference to the DrawingViews collection.
                    drawingViews = sheet.DrawingViews;

                    foreach (var drawingView in drawingViews.OfType<SolidEdgeDraft.DrawingView>())
                    {
                        // Updates an out-of-date drawing view.
                        drawingView.Update();

                        // Note: You can use ForceUpdate() even if it is not out-of-date.

                        Console.WriteLine("Updated drawing view '{0}'.", drawingView.Name);
                    }
                }
            }
            else
            {
                throw new System.Exception("No active document.");
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            SolidEdgeCommunity.OleMessageFilter.Unregister();
        }
    }
    public double[] IsoTolerance(double Nom, string IsoTol)
    {
        //Some Tolerances use the same tolerance for different bands
        //Use the below template and delete the overlap to reduce computations
        //    if(Nom>0.0 && Nom <= 3.0){max  += ?.??}
        //    else if (Nom>3.0 && Nom <= 6.0){max += ?.??;}
        //    else if (Nom>6.0 && Nom <= 10.0){max += ?.??;}
        //    else if (Nom>10.0 && Nom <= 18.0){max += ?.??;}
        //    else if (Nom>18.0 && Nom <= 30.0){max += ?.??;}
        //    else if (Nom>30.0 && Nom <= 40.0){max += ?.??;}
        //    else if (Nom>40.0 && Nom <= 50.0){max += ?.??;}
        //    else if (Nom>50.0 && Nom <= 65.0){max += ?.??;}
        //    else if (Nom>65.0 && Nom <= 80.0){max += ?.??;}
        //    else if (Nom>80.0 && Nom <= 100.0){max += ?.??;}
        //    else if (Nom>100.0 && Nom <= 120.0){max += ?.??;}
        //    else if (Nom>120.0 && Nom <= 140.0){max += ?.??;}
        //    else if (Nom>140.0 && Nom <= 160.0){max += ?.??;}
        //    else if (Nom>160.0 && Nom <= 180.0){max += ?.??;}
        //    else if (Nom>180.0 && Nom <= 200.0){max += ?.??;}
        //    else if (Nom>200.0 && Nom <= 225.0){max += ?.??;}
        //    else if (Nom>225.0 && Nom <= 250.0){max += ?.??;}
        //    else if (Nom>250.0 && Nom <= 280.0){max += ?.??;}
        //    else if (Nom>280.0 && Nom <= 315.0){max += ?.??;}
        //    else if (Nom>315.0 && Nom <= 355.0){max += ?.??;}
        //    else if (Nom>355.0 && Nom <= 400.0){max += ?.??;}

        double min = Nom;
        double max = Nom;
       
        switch(IsoTol)
        {
            case "H11":
            if(Nom>0.0 && Nom <= 3.0){max  += 0.06;}
            else if (Nom>3.0 && Nom <= 6.0){max += 0.075;}
            else if (Nom>6.0 && Nom <= 10.0){max += 0.09;}
            else if (Nom>10.0 && Nom <= 18.0){max += 0.11;}
            else if (Nom>18.0 && Nom <= 30.0){max += 0.13;}
            else if (Nom>30.0 && Nom <= 50.0){max += 0.16;}
            else if (Nom>50.0 && Nom <= 80.0){max += 0.19;}
            else if (Nom>80.0 && Nom <= 120.0){max += 0.22;}
            else if (Nom>120.0 && Nom <= 180.0){max += 0.25;}
            else if (Nom>180.0 && Nom <= 250.0){max += 0.29;}
            else if (Nom>250.0 && Nom <= 315.0){max += 0.32;}
            else if (Nom>315.0 && Nom <= 400.0){max += 0.36;}
            break;
            case "H12":
            if(Nom>0.0 && Nom <= 3.0){max  += 0.1;}
            else if (Nom>3.0 && Nom <= 6.0){max += 0.12;}
            else if (Nom>6.0 && Nom <= 10.0){max += 0.15;}
            else if (Nom>10.0 && Nom <= 18.0){max += 0.18;}
            else if (Nom>18.0 && Nom <= 30.0){max += 0.21;}
            else if (Nom>30.0 && Nom <= 50.0){max += 0.25;}
            else if (Nom>50.0 && Nom <= 80.0){max += 0.30;}
            else if (Nom>80.0 && Nom <= 120.0){max += 0.35;}
            else if (Nom>120.0 && Nom <= 180.0){max += 0.40;}
            break;
        }
        double[] OutPut = new double[min,max];



        return OutPut;
    }
}