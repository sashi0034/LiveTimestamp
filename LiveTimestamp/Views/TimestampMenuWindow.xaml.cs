using LiveTimestamp.Utils;
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
using System.Xml.Linq;

namespace LiveTimestamp.Views
{
    public record TimeStampElementProp(
        string Format
        ) { }

    /// <summary>
    /// TimestampMenuWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TimestampMenuWindow : Window
    {
        private readonly App appRef;

        private bool _isActivated = false;
        public bool IsActivated => _isActivated;

        int _selectedIndex = 0;
        public int SelectedIndex => _selectedIndex;

        private readonly KeyDownChecker keyDownChecker;

        private readonly List<TimestampElementLine> elementList = new List<TimestampElementLine>();

        public string SelectedFormat => elementList[_selectedIndex].TextContent;

        public TimestampMenuWindow(App app)
        {
            InitializeComponent();

            appRef = app;
            keyDownChecker = new KeyDownChecker(onKeyDown);

            clearElements();

            foreach (var element in getInitialTimeStampElements())
            {
                addElement(new TimestampElementLine().ApplyProp(element));
            }

            changeSelectedIndex(0);
        }

        private void clearElements()
        {
            elementList.Clear();
            panelElements.Children.Clear();
        }
        private void addElement(TimestampElementLine element)
        {
            elementList.Add(element);
            panelElements.Children.Add(element);
        }
        private void removeElement(TimestampElementLine element)
        {
            elementList.Remove(element);
            panelElements.Children.Remove(element);
        }
        private void changeSelectedIndex(int newIndex)
        {
            elementList[_selectedIndex].IsSelected = false;
            _selectedIndex= newIndex;
            elementList[_selectedIndex].IsSelected = true;
        }


        private List<TimeStampElementProp> getInitialTimeStampElements()
        {
            return new List<TimeStampElementProp>()
            {
                new TimeStampElementProp("yyyy-MM-dd-HH-mm-ss"),
                new TimeStampElementProp("_yyyy_MM_dd_HH_mm_ss"),
                new TimeStampElementProp("yyyyMMddHHmmss"),
            };
        }

        public void StartPopup()
        {
            _isActivated = true;

            Util.ShowWindowAboveCursor(this);

            keyDownChecker.StartCheck(appRef.KeyConfigWindow.GetSelectedKey());

            this.Dispatcher.Invoke(async () =>
            {
                await Util.WaitUntil(() => isFinishedConfirm());
                _isActivated = false;
                keyDownChecker.StopCheck();
                Hide();
            });
        }

        private static bool isFinishedConfirm()
        {
            return 
                !Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) &&
                !Keyboard.IsKeyDown(Key.LeftAlt) &&
                !Keyboard.IsKeyDown(Key.LWin);
        }

        private void onKeyDown()
        {
            changeSelectedIndex((_selectedIndex + 1) % elementList.Count);
        }
    }
}
