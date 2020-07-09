using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidEdgeCommunity.Extensions;

namespace Part_Generator.Parts_Interface
{
    static class Machining
    {
        //creates a name for the new files created. 
        public static string RandString()
        {
            DateTime dt = DateTime.Now;
            return dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + dt.Millisecond.ToString();

        }

        static public void SolidEdgeDD(TItem TagItem)
        {
            string bHold = inputValidator(TagItem);
            if (!string.IsNullOrWhiteSpace(bHold))
            {
              System.Windows.MessageBox.Show(bHold);
                return;
            }
            bHold = DDinputValidator(TagItem);
            if (!string.IsNullOrWhiteSpace(bHold))
            {
                System.Windows.MessageBox.Show(bHold);
                return;
            }

            //random string for saving the new files         
            string tempname = RandString();
            //Across
            double AF = double.Parse(TagItem.GetAttByName("DDAFLATS").Default_Value);
            double drillDia = Math.Round(AF * 0.375, 0);
            double depth = AF * 1.5;
            double DDround = double.Parse(TagItem.GetAttByName("DDROUND").Default_Value);
            double CboreDia = Math.Round(Math.Sqrt(Math.Pow(DDround, 2) + Math.Pow(AF, 2)) + .2, 0, MidpointRounding.AwayFromZero);
            double hold = Math.Sqrt(Math.Pow(DDround, 2.0) - Math.Pow(AF, 2.0)) + drillDia;
            double AFMin = double.Parse(TagItem.GetAttByName("DDACMIN").Default_Value);
            double AFMax = double.Parse(TagItem.GetAttByName("DDACMAX").Default_Value);


            if (hold > DDround) { DDround = hold; }

            SolidEdgeCommunity.OleMessageFilter.Register();
            SolidEdgeFramework.Application application = SolidEdgeCommunity.SolidEdgeUtils.Connect(true, true);
            SolidEdgePart.PartDocument partDocument = application.Documents.Open<SolidEdgePart.PartDocument>(TagItem.SolidEdgePart.FilePath + ".par");

            //update properties
            Dictionary<string, string> Props = new Dictionary<string, string>();
            string IDesc = TagItem.Type;
            IDesc += ", " + TagItem.Model + TagItem.Size;
            IDesc += " " + TagItem.Duty;
            IDesc += " DD" + TagItem.GetAttByName("DDAFLATS").Default_Value;
            IDesc += " Ø" + TagItem.GetAttByName("DDROUND").Default_Value;
            Props.Add("Title", IDesc);
            VariablesHelper.UpdatePrperties((SolidEdgeFramework.SolidEdgeDocument)partDocument, Props);

            //Update machining dimensions
            Dictionary<string, double> dimensions = new Dictionary<string, double>();
            dimensions.Add("DDAFlats", AF);
            dimensions.Add("DDRFlats", DDround);
            dimensions.Add("DDCornerRadious", drillDia / 2.0);
            dimensions.Add("DD_FiniteDepth", depth);
            dimensions.Add("CBoreDia", CboreDia);
            VariablesHelper.UpdateDimensions((SolidEdgeFramework.SolidEdgeDocument)partDocument, dimensions);

            //Save Part File
            partDocument.SaveAs(@"C:\myloadpoint\" + tempname + ".par");
            //Open Draft file
            SolidEdgeDraft.DraftDocument draftDocument = application.Documents.Open<SolidEdgeDraft.DraftDocument>(TagItem.SolidEdgePart.FilePath + ".dft");
            //Set Link to saved part file
            var m = draftDocument.ModelLinks;
            m.Item(1).ChangeSource(@"C:\myloadpoint\" + tempname + ".par");
            //update draft's views to show new linked part file
            VariablesHelper.UpdateDrawingViews(draftDocument);
            var d = VariablesHelper.GetDimensions((SolidEdgeFramework.SolidEdgeDocument)draftDocument, "DDAFlats");
            VariablesHelper.UpdateTolerance(d, AF, AFMin, AFMax);
            //save draft file
            draftDocument.SaveAs(@"C:\myloadpoint\" + tempname + ".dft");
        }

        static string inputValidator(TItem TagItem)
        {
            string bHold = "";
            foreach (var t in TagItem.Default_attributes)
            {
                if (string.IsNullOrWhiteSpace(t.Default_Value))
                {
                    bHold += "Please add a value for " + t.Att_Name + "\n";
                }
            }
            return bHold;
        }
        static string DDinputValidator(TItem TagItem)
        {
            string bHold = "";
            var AFNOM = TagItem.GetAttByName("DDAFLATS");
            var AFMIN = TagItem.GetAttByName("DDACMIN");
            var AFMAX = TagItem.GetAttByName("DDACMAX");
            var DDROUND = TagItem.GetAttByName("DDROUND");



            Dictionary<defualtAtts, double> Dic = new Dictionary<defualtAtts, double>();
            Dic.Add(AFNOM, new double());
            Dic.Add(AFMIN, new double());
            Dic.Add(AFMAX, new double());
            Dic.Add(DDROUND, new double());

            //Check for all inputs being Doubles
            foreach (defualtAtts d in Dic.Keys)
            {
                double dout = Dic[d];
                if (!double.TryParse(d.Default_Value, out dout))
                {
                    bHold += d.Att_Name + " needs to be a number\n";
                    return bHold;
                }

            }
            Dic.Clear();
            Dic.Add(AFNOM, double.Parse(AFNOM.Default_Value));
            Dic.Add(AFMIN, double.Parse(AFMIN.Default_Value));
            Dic.Add(AFMAX, double.Parse(AFMAX.Default_Value));
            Dic.Add(DDROUND, double.Parse(DDROUND.Default_Value));

            //Check that Min is smaller than Max
            if (Dic[AFMAX] <= Dic[AFMIN])
            {
                bHold += "DD Max is must be greater than DD Min\n";
            }
            //Check that AF is smaller than Round
            if (Dic[DDROUND] <= Dic[AFNOM])
            {
                bHold += "DD Round must be greater than DD across flats\n";
            }
            //Check Rounds and Flat are greater than pilot
            double x = Math.Sqrt((Math.Pow(TagItem.SolidEdgePart.PilotHoleDia, 2) - Math.Pow(Dic[AFNOM], 2)));
            if (x > Dic[DDROUND] / 3)
            {
                Console.WriteLine(Dic[DDROUND] + " " + TagItem.SolidEdgePart.PilotHoleDia);

                bHold += "The Pilot bore in the quadrant causes a loss of more than 1/3 of the driving wall\n";
            }
            if (TagItem.SolidEdgePart.PilotHoleDia > Dic[DDROUND])
            {
                Console.WriteLine(Dic[DDROUND] + " " + TagItem.SolidEdgePart.PilotHoleDia);
                bHold += "The Pilot Bore (" + TagItem.SolidEdgePart.PilotHoleDia + ") is greater than the DD Round (" + Dic[DDROUND] + ")\n";

            }
            //Check that CBore will not remove indicator holes
            double CboreDia = Math.Round(Math.Sqrt(Math.Pow(Dic[DDROUND], 2) + Math.Pow(Dic[AFNOM], 2)) + .2, 0, MidpointRounding.AwayFromZero);
            if (CboreDia > TagItem.SolidEdgePart.MaxDia)
            {

                bHold += "Counterbore will interfear with indicator threaded holes\n";
            }

            return bHold;
        }

    }
}
