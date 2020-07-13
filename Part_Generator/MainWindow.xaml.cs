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
            IO.File.WriteAllText(@"C:\Users\sean.halton\Desktop\New folder\hold.json", Newtonsoft.Json.JsonConvert.SerializeObject(Master));
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
        private IOFileInterface LogFile()
        {           
            string FilePath = @"\\rotork.co.uk\files\US-HOUSTON\ENGINEERING\Gears\Aplications\ABPartsGen\LogFiles\" +
               System.Environment.UserName.ToUpper() +
               Machining.RandString() + ".log";
            IOFileInterface output = new IOFileInterface(FilePath);
            output.sw.WriteLine("Date:{0}", DateTime.Now);
            output.sw.WriteLine("User:{0}", System.Environment.UserName.ToUpper());
            output.sw.WriteLine("*".PadRight(30, '*'));
            output.sw.WriteLine("User Inputs");
            output.sw.WriteLine("*".PadRight(30, '*'));
            output.sw.WriteLine("Input Type: {0}", cbType.Text);
            output.sw.WriteLine("Gear Size: {0}", cbSize.Text);
            output.sw.WriteLine("Hand: {0}", cbHand.Text);
            output.sw.WriteLine("Machining Type: {0}", cbMachType.Text);
            output.sw.WriteLine("Duty: {0}", cbDuty.Text);
            output.sw.WriteLine("MTMS Group: {0}", cbGroup.Text);
            return output;

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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            string hold = "";
            var writing = LogFile();
            TItem TagItem = ((TItem)cbGroup.SelectedItem);
            List<string> Co = new List<string>();
            hold = "Companies selected: ";
            if ((bool)ChBCo72.IsChecked)
            {
                hold += "Co72;";
                Co.Add("72");
            }
            if ((bool)ChBCo13.IsChecked)
            {
                hold += "Co13;";
                Co.Add("13");
            }
            if ((bool)ChBCo81.IsChecked)
            {
                hold += "Co81;";
                Co.Add("81");
            }
            writing.sw.WriteLine(hold);
            writing.sw.WriteLine("*".PadRight(30, '*'));
            foreach (string s in Co)
            {
                mTMSLibrary.Screens m = new mTMSLibrary.Screens(s);
                writing.sw.WriteLine("Co" + s + " results:");
                writing.sw.WriteLine("*".PadRight(30, '*'));
                hold = TagItem.AddToMTMS(m, tbPartNumber.Text, s);
                writing.sw.WriteLine(hold);
                tbResults.Text += hold + "\n";
                hold = TagItem.AddToConfigurator(m, tbPartNumber.Text);
                writing.sw.WriteLine(hold);
                tbResults.Text += hold + "\n";

                if (TagItem.PartInConfig(tbPartNumber.Text, s))
                {

                    MessageBoxResult h = MessageBox.Show(tbPartNumber.Text + " exists in " + TagItem.GroupName + " on Co " + s + "\nContinue?", "", MessageBoxButton.YesNo);
                    if (h == MessageBoxResult.No)
                    {
                        Mouse.OverrideCursor = null;
                        return;
                    }
                } 

                foreach (defualtAtts da in TagItem.Default_attributes)
                {
                    hold = "attribute: " + da.Att_Name + "; Value: " + da.Default_Value + "; Result: " + da.MCO61(m, TagItem, tbPartNumber.Text);
                    writing.sw.WriteLine(hold);
                    tbResults.Text += hold + "\n";
                }
                m.Close();
            }
            writing.Close();
            Mouse.OverrideCursor = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            TItem TagItem = ((TItem)cbGroup.SelectedItem);
            Console.WriteLine(TagItem.SolidEdgePart.FilePath);
            if (!string.IsNullOrEmpty(TagItem.SolidEdgePart.FilePath))
            {
                Console.WriteLine("Yes");
                switch (TagItem.Mach_Type)
                {
                    case "DD":
                        Parts_Interface.Machining.SolidEdgeDD(TagItem);
                        break;
                    default:
                        MessageBox.Show("This type has not been implemented yet");
                        break;
                }
            }
            Mouse.OverrideCursor = null;
        }

        private void tbPartNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnmTMS.IsEnabled = !string.IsNullOrEmpty(tbPartNumber.Text);
        }

        private void btnISOTOL_Click(object sender, RoutedEventArgs e)
        {
            ISOTolerator iso = new ISOTolerator();
            iso.Show();
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
        
        public bool PartExist(string PartNumber, string Co)
        {
            mTMS.PART_DATADataTable PART = new mTMS.PART_DATADataTable();
            mTMSTableAdapters.PART_DATATableAdapter dtaPART = new mTMSTableAdapters.PART_DATATableAdapter();
            dtaPART.Fill(PART, Co, PartNumber.PadRight(20));
            //MessageBox.Show(Co + "\n" + PartNumber.PadRight(20) + "*\n" + PART.Rows.Count); 
            return PART.Rows.Count > 0;
        }
        public bool PartInConfig(string PartNumber, string Co)
        {
            mTMS.MODX_DATADataTable MODX = new mTMS.MODX_DATADataTable();
            mTMSTableAdapters.MODX_DATATableAdapter dtaMODX = new mTMSTableAdapters.MODX_DATATableAdapter();
            dtaMODX.Fill(MODX,Co,Model.PadRight(12),GroupName.PadRight(12), PartNumber.PadRight(20));
            //MessageBox.Show( Co + "\n" + Model + "\n" +GroupName + "\n" + PartNumber + "\n" + MODX.Rows.Count.ToString());
            return MODX.Rows.Count > 0;
        }

        public string AddToMTMS(mTMSLibrary.Screens m, string Part_Number , string site)
        {
            string desc = Type + ", " + Model + Size + " ";
            
            switch (Mach_Type)
            {
                case "DD":
                    desc += "DD " + GetAttByName("DDAFLATS").Default_Value + " X" + GetAttByName("DDROUND").Default_Value;
                    break;
                case "BODY":
                    desc +=  GetAttByName("BPPCD").Default_Value + " " + GetAttByName("BPNUMH").Default_Value + GetAttByName("BPHOLE").Default_Value + " " + GetAttByName("BPCENTRE").Default_Value;
                    break;
                default:
                    break;
            }
            if (!PartExist(Part_Number, site))
            {
                switch (site)
                {
                    case "72":
                        return (string)m.MPD11(Part_Number, "A", desc, desc, "GB", "150", "", "", "M", "", "", "", "", Model, Size, "Y") + "\n" +
                            (string)m.MPD40(Part_Number, 1, SolidEdgePart.PartNumber, 1, "", "");
                        break;
                    default:
                        return (string)m.MPD11(Part_Number, "A", desc, desc, "GB", "150", "", "", "M", "", "", "", "", Model, Size, "N") + "\n" +
                            (string)m.MPD40(Part_Number, 1, SolidEdgePart.PartNumber, 1, "", "");
                        break;
                }
            }
            else { return "Part Already Exists"; }
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
            string output = "";
            if (Default_Value.Contains('-') && Default_Value.Contains(',')){ output = Att_Name + ": Value not written; Range-Lists are not yet supported"; } //TODO: Add support for Range-List
            else if (Default_Value.Contains('-'))
            {
                output = (string)m.MCO61Range(Part_Number, T.Model, T.GroupName, T.MTMS_Stage, T.MTMS_Item, Att_Name, intType, Default_Value.Split('-')[0], Default_Value.Split('-')[1], Att_Type);
            } //TODO: Add support for Range 
            else if (Default_Value.Contains(',')) { output = Att_Name + ": Value not written; Lists are not yet supported"; } //TODO: Add support for List
            else
            {
                output = (string)m.MCO61(Part_Number, T.Model, T.GroupName, T.MTMS_Stage, T.MTMS_Item, Att_Name, intType, Default_Value, Att_Type);
            }
            return output;             
        }
    }
    public class SolidEdgeItem
    {
        public string FilePath { get; set; } = "";
        public string PartNumber { get; set; } = "";
        public double PilotHoleDia { get; set; } = 0.0;
        public double MaxDia { get; set; } = 0.0;
        public double Height { get; set; } = 0.0;
    }

    public class IOFileInterface
    {
        public IO.FileStream fs { get; set; }
        public IO.StreamWriter sw { get; set; }

       public IOFileInterface(string fPath)
        {
            fs = new IO.FileStream(fPath, IO.FileMode.OpenOrCreate);
            sw = new IO.StreamWriter(fs);
        }
       public void Close()
       {
            sw.Close();
            fs.Close();
       }
    }
}
