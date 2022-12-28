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

namespace LiveTimestamp.Views
{
    /// <summary>
    /// TimestampElement.xaml の相互作用ロジック
    /// </summary>
    public partial class TimestampElementLine : UserControl
    {
        private readonly Brush selectedBorder;

        public TimestampElementLine()
        {
            InitializeComponent();

            selectedBorder = border.BorderBrush;
            
            IsSelected = false;
        }

        public TimestampElementLine ApplyProp(TimeStampElementProp prop)
        {
            TextContent = prop.Format;
            return this;
        }

        public bool IsSelected
        {
            get
            {
                return border.BorderBrush == selectedBorder;
            }
            set
            {
                border.BorderBrush = value ? selectedBorder : Brushes.Transparent;
            }
        }

        public string TextContent
        {
            get
            {
                return text.Text;
            }
            set
            {
                text.Text = value;
            }
        }
    }
}
