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
using Part_Generator.Parts_Interface;
using System.IO;

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
        private object[] LogFile()
        {
            object[] output = new object[1];
            string FilePath = @"\\rotork.co.uk\files\US-HOUSTON\ENGINEERING\Gears\Aplications\ABPartsGen\LogFiles\MTMSEntry" +
               System.Environment.UserName.ToUpper() +
               Machining.RandString() + ".log";
            IO.FileStream fs = new IO.FileStream(FilePath, IO.FileMode.OpenOrCreate);
            IO.StreamWriter sw = new IO.StreamWriter(fs);
            sw.WriteLine("Date:{0}", DateTime.Now);
            sw.WriteLine("User:{0}", System.Environment.UserName.ToUpper());
            sw.WriteLine("*".PadRight(30, '*'));
            sw.WriteLine("User Inputs");
            sw.WriteLine("*".PadRight(30, '*'));
            sw.WriteLine("Input Type: {0}", cbType.Text);
            sw.WriteLine("Gear Size: {0}", cbSize.Text);
            sw.WriteLine("Hand: {0}", cbHand.Text);
            sw.WriteLine("Machining Type: {0}", cbMachType.Text);
            sw.WriteLine("Duty: {0}", cbDuty.Text);
            sw.WriteLine("MTMS Group: {0}", cbGroup.Text);
            sw.WriteLine("*".PadRight(30, '*'));
            output[0] = fs;
            output[1] = sw;
            return output;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string hold = "";
            var writing = LogFile();
            var sw = (IO.StreamWriter)writing[1];
            TItem TagItem = ((TItem)cbGroup.SelectedItem);
            List<string> Co = new List<string>();
            //TODO: add user choice to company selection
            Co.Add("72");
            Co.Add("13");
            foreach ( string s in Co)
            {
                mTMSLibrary.Screens m = new mTMSLibrary.Screens(s);
                sw.WriteLine("Co" + s + " results:");
                sw.WriteLine("*".PadRight(30, '*'));
                hold = TagItem.AddToMTMS(m, tbPartNumber.Text);
                sw.WriteLine(hold);
                tbResults.Text += hold + "\n";
                hold = TagItem.AddToConfigurator(m, tbPartNumber.Text);
                sw.WriteLine(hold);
                tbResults.Text += hold + "\n";
                foreach (defualtAtts da in TagItem.Default_attributes)
                {
                    hold = "attribute: " + da.Att_Name + "; Value: " + da.Default_Value + "; Result: " + da.MCO61(m, TagItem, tbPartNumber.Text);
                    sw.WriteLine(hold);
                    tbResults.Text += hold + "\n";
                }
                m.Close();
            }
            sw.Close();
            IO.FileStream fs = (IO.FileStream)writing[0];
            fs.Close();
            
            

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
                       Parts_Interface.Machining.SolidEdgeDD(TagItem);
                        break;
                    default:
                        MessageBox.Show("This type has not been implemented yet");

                        break;
                }
            }
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
