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
using System.Windows.Shapes;

namespace Part_Generator
{
    /// <summary>
    /// Interaction logic for ISOTolerator.xaml
    /// </summary>
    public partial class ISOTolerator : Window
    {
        public ISOTolerator()
        {
            InitializeComponent();
        }

        private void tbDimension_TextChanged(object sender, TextChangedEventArgs e)
        {

            //get the textbox that fired the event
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var text = textBox.Text;
            var output = new StringBuilder();
            //use this boolean to determine if the dot already exists
            //in the text so far.
            var dotEncountered = false;
            //loop through all of the text
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (char.IsDigit(c))
                {
                    //append any digit.
                    output.Append(c);
                }
                else if (!dotEncountered && c == '.')
                {
                    //append the first dot encountered
                    output.Append(c);
                    dotEncountered = true;
                }
            }
            var newText = output.ToString();
            textBox.Text = newText;
            //set the caret to the end of text
            textBox.CaretIndex = newText.Length;

            if (cbTol.SelectedItem != null )
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    tblockMax.Text = "";
                    tblockMin.Text = "";
                }
                else
                {
                    Console.WriteLine(cbTol.Text);
                    var hold = VariablesHelper.IsoTolerance(double.Parse(textBox.Text), cbTol.Text);
                    tblockMax.Text = hold[1].ToString();
                    tblockMin.Text = hold[0].ToString();

                }
            }
        }

        private void cbTol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbDimension.Text))
            {
                Console.WriteLine(cbTol.Text);
                var hold = VariablesHelper.IsoTolerance(double.Parse(tbDimension.Text), ((ComboBoxItem)e.AddedItems[0]).Content.ToString());
                tblockMax.Text = hold[1].ToString();
                tblockMin.Text = hold[0].ToString();
            }
        }
    }
}

