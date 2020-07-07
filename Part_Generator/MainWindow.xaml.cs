using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IO = System.IO;
using SolidEdgeCommunity.Extensions;

namespace Part_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TItem> Master { get; set; } = new List<TItem>();
        public MainWindow()
        {
            InitializeComponent();

            Master = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TItem>>(IO.File.ReadAllText(@"\\rotork.co.uk\files\US-HOUSTON\ENGINEERING\Gears\Aplications\ABPartsGen\PartsConfigurator.json"));
            
         
            GetCBTypes();

        }

        public void GetCBTypes()
        {
            List<string> options = new List<string>();
            options.Add("-");
            foreach(TItem t  in Master)
            {
                if (!options.Contains(t.Type))
                {
                    options.Add(t.Type);
                }
            }
            options.Sort();
            cbType.ItemsSource = options;
            cbType.SelectedItem = "-";
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> options = new List<string>();
            options.Add("-");

            foreach (TItem t in Master)
            {
                if (t.Type == cbType.SelectedItem.ToString())
                {
                    if (!options.Contains(t.Size))
                    {
                        options.Add(t.Size);
                    }
                }

            }
            //options.Sort();

            cbSize.ItemsSource = options;
            cbSize.SelectedItem = "-";
            cbHand.ItemsSource= null;
            cbDuty.ItemsSource =  null;
            cbMachType.ItemsSource = null;
            }

        private void cbSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> options = new List<string>();
            options.Add("-");

            if(cbType.SelectedItem == null || cbSize.SelectedItem == null)
            {
                return;
            }
            foreach (TItem t in Master)
            {
                if (t.Type == cbType.SelectedItem.ToString() && t.Size == cbSize.SelectedItem.ToString())
                {
                    if (!options.Contains(t.Hand))
                    {
                        options.Add(t.Hand);
                    }
                }

            }
            options.Sort();
            cbHand.ItemsSource = options;
            cbHand.SelectedItem = "-";
            cbDuty.ItemsSource =  null;
            cbMachType.ItemsSource =  null;
            
        }

        private void cbHand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> options = new List<string>();
            options.Add("-");

            foreach (TItem t in Master)
            {
                if (t.Type == cbType.SelectedItem.ToString() &&
                    t.Size == cbSize.SelectedItem.ToString() && 
                    t.Hand == cbHand.SelectedItem.ToString())
                {
                    if (!options.Contains(t.Duty))
                    {
                        options.Add(t.Duty);
                    }
                }

            }
            options.Sort();
            cbDuty.ItemsSource = options;
            cbDuty.SelectedItem = "-";
            cbMachType.ItemsSource = null;
        }

        private void cbDuty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> options = new List<string>();
            options.Add("-");

            foreach (TItem t in Master)
            {
                if (t.Type == cbType.SelectedItem.ToString() &&
                    t.Size == cbSize.SelectedItem.ToString() &&
                    t.Hand == cbHand.SelectedItem.ToString() && 
                    t.Duty == cbDuty.SelectedItem.ToString())
                {
                    if (!options.Contains(t.Mach_Type))
                    {
                        options.Add(t.Mach_Type);
                    }
                }

            }
            options.Sort();
            cbMachType.ItemsSource = options;
            cbMachType.SelectedItem = "-";

        }

        private void cbMachType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<TItem> options = new List<TItem>();
            foreach (TItem t in Master)
            {
                if (t.Type == cbType.SelectedItem.ToString() &&
                    t.Size == cbSize.SelectedItem.ToString() &&
                    t.Hand == cbHand.SelectedItem.ToString() &&
                    t.Duty == cbDuty.SelectedItem.ToString() &&
                    t.Mach_Type == cbMachType.SelectedItem.ToString())
                {
                    options.Add(t);
                }
            }
            try
            {
            options.Sort();
            }
            catch { }
   cbGroup.ItemsSource = options;
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
           TextBox tb = (TextBox)sender;
            defualtAtts da = (defualtAtts)tb.Tag;
            da.Default_Value = tb.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TItem TagItem = ((TItem)cbGroup.SelectedItem);
            //MessageBox.Show(TagItem.Default_attributes[0].Default_Value);
            mTMSLibrary.Screens m = new mTMSLibrary.Screens("72");
            tbResults.Text += TagItem.AddToMTMS(m, tbPartNumber.Text);
            tbResults.Text += TagItem.AddToConfigurator(m, tbPartNumber.Text)+ "\n";
            foreach (defualtAtts da in TagItem.Default_attributes)
            {
               tbResults.Text +=  da.MCO61(m, TagItem, tbPartNumber.Text) + "\n";
            }
            m.Close();
            m = new mTMSLibrary.Screens("13");
            tbResults.Text += TagItem.AddToMTMS(m, tbPartNumber.Text);
            tbResults.Text += TagItem.AddToConfigurator(m, tbPartNumber.Text) + "\n";
            foreach (defualtAtts da in TagItem.Default_attributes)
            {
                tbResults.Text += da.MCO61(m, TagItem, tbPartNumber.Text) + "\n";
            }
            m.Close();

        }        

        private void cbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TItem T = (TItem)cbGroup.SelectedItem;
            try { btnSolidEdge.IsEnabled = (!string.IsNullOrWhiteSpace(T.SolidEdgePart.FilePath)); } catch { btnSolidEdge.IsEnabled = false; }
            spAtts.Children.Clear();
            try
            {
                foreach (defualtAtts da in T.Default_attributes)
                {
                    TextBlock tbl = new TextBlock();
                    tbl.Text = da.Att_Name;
                    tbl.MinWidth = 75;
                    TextBox tb = new TextBox();
                    tb.Text = da.Default_Value;
                    tb.Tag = da;
                    tb.TextChanged += Tb_TextChanged;


                    DockPanel sp = new DockPanel();
                    sp.Margin = new Thickness(5);
                    sp.Children.Add(tbl);
                    sp.Children.Add(tb);
                    spAtts.Children.Add(sp);
                }
            }
            catch
            {

            }
        }

        public string RandString()
        {
            DateTime dt = DateTime.Now;
            return dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + dt.Millisecond.ToString();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


            TItem TagItem = ((TItem)cbGroup.SelectedItem);
            Console.WriteLine(TagItem.SolidEdgePart.FilePath);
            if (!string.IsNullOrEmpty(TagItem.SolidEdgePart.FilePath))
            {
                Console.WriteLine("Yes");
                switch (TagItem.Mach_Type)
                {
                    case "DD":
                        Console.WriteLine("Yes");
                        SolidEdgeDD(TagItem);
                        break;
                    default:
                        MessageBox.Show("This type has not been implemented yet");

                        break;
                }
            }
        }
        public string inputValidator(TItem TagItem)
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
        public string DDinputValidator(TItem TagItem)
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
            if (x > Dic[DDROUND]/3)
            {
                Console.WriteLine(Dic[DDROUND] + " " + TagItem.SolidEdgePart.PilotHoleDia);

                bHold += "The Pilot bore in the quadrant causes a loss of more than 1/3 of the driving wall\n";
            }
            if(TagItem.SolidEdgePart.PilotHoleDia> Dic[DDROUND])
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
        public void SolidEdgeDD(TItem TagItem)
        {
            string bHold = inputValidator(TagItem);
            if (!string.IsNullOrWhiteSpace(bHold))
            {
                MessageBox.Show(bHold);
                return;
            }
            bHold = DDinputValidator(TagItem);
            if (!string.IsNullOrWhiteSpace(bHold))
            {
                MessageBox.Show(bHold);
                return;
            }

            //random string for saving the new files         
            string tempname = RandString();
            //Across
            double AF = double.Parse(TagItem.GetAttByName("DDAFLATS").Default_Value) ;
            double drillDia = Math.Round(AF * 0.375, 0) ;
            double depth = AF * 1.5;
            double DDround = double.Parse(TagItem.GetAttByName("DDROUND").Default_Value) ;
            double CboreDia = Math.Round( Math.Sqrt( Math.Pow( DDround,2) + Math.Pow(AF,2)) +  .2,0,MidpointRounding.AwayFromZero);
            double hold = Math.Sqrt(Math.Pow(DDround, 2.0) - Math.Pow(AF, 2.0)) + drillDia;
            double AFMin = double.Parse(TagItem.GetAttByName("DDACMIN").Default_Value);
            double AFMax = double.Parse(TagItem.GetAttByName("DDACMAX").Default_Value);


            if (hold > DDround) { DDround = hold; }

            SolidEdgeCommunity.OleMessageFilter.Register();
            SolidEdgeFramework.Application application = SolidEdgeCommunity.SolidEdgeUtils.Connect(true,true);
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
            partDocument.SaveAs( @"C:\myloadpoint\" + tempname + ".par");
            //Open Draft file
            SolidEdgeDraft.DraftDocument draftDocument = application.Documents.Open<SolidEdgeDraft.DraftDocument>(TagItem.SolidEdgePart.FilePath + ".dft");
            //Set Link to saved part file
            var m = draftDocument.ModelLinks;
            m.Item(1).ChangeSource(@"C:\myloadpoint\" + tempname + ".par");
            //update draft's views to show new linked part file
            VariablesHelper.UpdateDrawingViews(draftDocument);
            var d = VariablesHelper.GetDimensions((SolidEdgeFramework.SolidEdgeDocument)draftDocument, "DDAFlats");
            VariablesHelper.UpdateTolerance(d,AF,AFMin,AFMax);
            //save draft file
            draftDocument.SaveAs(@"C:\myloadpoint\" + tempname + ".dft");
        }

        private void tbPartNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnmTMS.IsEnabled = !string.IsNullOrEmpty(tbPartNumber.Text);
        }
    }

    public class TItem
    {
        public string GroupName { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Hand { get; set; }
        public string Duty { get; set; }
        public string Mach_Type { get; set; }
        public string MTMS_Item { get; set; }
        public string MTMS_Stage { get; set; }
        public string Model { get; set; }
        public SolidEdgeItem SolidEdgePart { get; set; } = new SolidEdgeItem();

        public List<defualtAtts> Default_attributes { get; set; } = new List<defualtAtts>();

        public string AddToConfigurator(mTMSLibrary.Screens m,  string Part_Number)
        {
            return (string)m.MCO52(Part_Number,Model,GroupName,1,MTMS_Stage,MTMS_Item);

        }
        public string AddToMTMS(mTMSLibrary.Screens m, string Part_Number)
        {
            string desc = Type + ", " + Model + Size + " ";
            switch (Mach_Type)
            {
                case "DD":
                    desc += "DD " + GetAttByName("DDAFLATS").Default_Value + " X" + GetAttByName("DDROUND").Default_Value;
                    break;
                default:
                    break;
            }
            return (string)m.MPD11(Part_Number,"A",desc,desc,"GB","150","","","M","","","","",Model,Size) + "/n" +
                (string)m.MPD40(Part_Number, 1, SolidEdgePart.PartNumber,1,"","");

        }
        public override string ToString()
        {
            return GroupName;
        }
        public defualtAtts GetAttByName (string s)
        {
            foreach(defualtAtts d in Default_attributes)
            {
                if (s == d.Att_Name)
                {
                    return d;
                }
            }
            return null;
        }
    }
    public class defualtAtts
    {
        public string Att_Name { get; set; }
        public int intType { get; set; }
        public int Att_Type { get; set; }
        public string Default_Value { get; set; }

        public string MCO61(mTMSLibrary.Screens m, TItem T, string Part_Number)
        {
            return (string)m.MCO61(Part_Number, T.Model, T.GroupName, T.MTMS_Stage, T.MTMS_Item, Att_Name, intType, Default_Value, Att_Type);
             
        }
    }
    public class SolidEdgeItem
    {
        public string FilePath { get; set; } = "";
        public string GearLine { get; set; } = "";
        public string GearSize { get; set; } = "";
        public string GearHand { get; set; } = "";
        public string PartNumber { get; set; } = "";
        public double PilotHoleDia { get; set; } = 0.0;
        public double MaxDia { get; set; } = 0.0;
        public double Height { get; set; } = 0.0;
    }


}
