using DesktopWPFAppLowLevelKeyboardHook;
using GlobalLowLevelHooks;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
namespace TypeTool
{
    public partial class MainWindow : MetroWindow
    {
        public LowLevelKeyboardListener _listener;
        public MouseHook _listenmouse;

        int ViTriBoNhoDem;
        int TrangThaiSetting = 0; //0 đóng, 1 mở
        int TagInfo = 1;
        int TrangThaiCopy = 0; //0 là chỉ copy, 1 là cả copy và paste
        int TrangThaiToHopPhim = 0; //0 là không dùng tổ hợp, 1 là có dùng tổ hợp
        int TrangThaiPhimGiu = 0; //0 là đã thả tay ra, 1 là đang giữ

        string HotKeyMod = "";
        string HotKey = "F4";
        string ChuoiChuan = "";

        List<string> ListText = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.OnKeyUp += _listener_OnKeyUp;

            _listenmouse = new MouseHook();
            _listenmouse.LeftButtonDown += _listenmouse_LeftButtonDown;
            _listenmouse.LeftButtonUp += _listenmouse_LeftButtonUp;
            _listenmouse.RightButtonDown += _listenmouse_RightButtonDown;
            _listenmouse.RightButtonUp += _listenmouse_RightButtonUp;
            _listenmouse.MiddleButtonDown += _listenmouse_MiddleButtonDown;
            _listenmouse.MiddleButtonUp += _listenmouse_MiddleButtonUp;
            //_listenmouse.MouseWheel += _listenmouse_MouseWheel;

            txtHotKey.Text = "Hot key: " + HotKey;
            btnStop.Width = 0;
            Topmost = true;

            //đọc file setting
            if (File.Exists("./Setting.txt"))
            {
                StreamReader objReader;

                string filename = "./Setting.txt";

                objReader = new StreamReader(filename);

                do
                {
                    HotKey = objReader.ReadLine();
                    txtHotKey.Text = "Hot key: " + HotKey;

                    HotKeyMod = objReader.ReadLine();
                    txtHotKeyMod.Text = "Hot key: " + HotKeyMod;

                    TrangThaiCopy = int.Parse(objReader.ReadLine());
                    if (TrangThaiCopy == 0)
                    {
                        rbtnCopy.IsChecked = true;
                    }
                    else
                    {
                        rbtnCopyPaste.IsChecked = true;
                    }

                    TrangThaiToHopPhim = int.Parse(objReader.ReadLine());
                    if (TrangThaiToHopPhim == 1)
                    {
                        cbMutilKey.IsChecked = true;
                        RowMod.Height = new GridLength(1, GridUnitType.Star);
                    }

                    if(objReader.ReadLine() == "1") { cbKhoangTrang.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cbKiTuDau.IsChecked = true; GridListChar.Visibility = Visibility.Visible; }
                    if (objReader.ReadLine() == "1") { cbChonTatCa.IsChecked = true; cbChonTatCa.Content = "Bỏ chọn tất cả"; }
                    if (objReader.ReadLine() == "1") { cb1.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb2.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb3.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb4.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb5.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb6.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb7.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb8.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb9.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb10.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb11.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb12.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb13.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb14.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb15.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb16.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb17.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb18.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb19.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb20.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb21.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb22.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb23.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb24.IsChecked = true; }
                    if (objReader.ReadLine() == "1") { cb25.IsChecked = true; }
                }
                while (objReader.Peek() != -1);

                objReader.Close();
            }
        }

        void DoiBoNhoDem()
        {
            System.Windows.Clipboard.Clear();

            ViTriBoNhoDem = ViTriBoNhoDem + int.Parse(txtChangeLine.Text);

            if (ViTriBoNhoDem >= lbText.Items.Count - 1)
            {
                lbText.SelectedIndex = lbText.Items.Count - 1;
                ViTriBoNhoDem = lbText.Items.Count - 1;

                XiLiChuoi(ListText[lbText.Items.Count - 1].ToString());
                System.Windows.Clipboard.SetDataObject(ChuoiChuan);

                //System.Windows.Clipboard.SetDataObject(DataFormats.UnicodeText, ListText[lbText.Items.Count - 1].ToString());
            }
            else
            {
                lbText.SelectedIndex = ViTriBoNhoDem;

                XiLiChuoi(ListText[ViTriBoNhoDem].ToString());
                System.Windows.Clipboard.SetDataObject(ChuoiChuan);
            }

            txtTitle.Text = (ViTriBoNhoDem + 1).ToString() + "/" + lbText.Items.Count.ToString();

            //Đo vị trí để căn giữa
            if (Main.Height >= (22 + 20 * int.Parse(txtChangeLine.Text)))
            {
                if (ViTriBoNhoDem + int.Parse(txtChangeLine.Text) <= lbText.Items.Count - 1)
                {
                    lbText.ScrollIntoView(lbText.Items[ViTriBoNhoDem + int.Parse(txtChangeLine.Text)]);
                }
                else
                {

                    lbText.ScrollIntoView(lbText.Items[lbText.Items.Count - 1]);
                }
            }
            else
            {
                lbText.ScrollIntoView(lbText.Items[ViTriBoNhoDem]);
            }
        }

        void XiLiChuoi(string s)
        {
            if (cb1.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('*');
            }

            if (cb2.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('/');
            }

            if (cb3.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('`');
            }

            if (cb4.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('~');
            }

            if (cb5.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('@');
            }

            if (cb6.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('#');
            }

            if (cb7.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('%');
            }

            if (cb8.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('^');
            }

            if (cb9.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('&');
            }

            if (cb10.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('(');
            }

            if (cb11.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart(')');
            }

            if (cb12.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('-');
            }

            if (cb13.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('_');
            }

            if (cb14.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('=');
            }

            if (cb15.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('+');
            }

            if (cb16.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('[');
            }

            if (cb17.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart(']');
            }

            if (cb18.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('{');
            }

            if (cb19.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('}');
            }

            if (cb20.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('\\');
            }

            if (cb21.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('|');
            }

            if (cb22.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart(';');
            }

            if (cb23.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart(':');
            }

            if (cb24.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('<');
            }

            if (cb25.IsChecked == true && cbKiTuDau.IsChecked == true)
            {
                s = s.TrimStart('>');
            }

            if (cbKhoangTrang.IsChecked == true)
            {
                s = ChuanHoaDauCau(s).Trim();
                while (s.IndexOf("  ") > 0)
                {
                    s = s.Replace("  ", " ");
                }
            }

            ChuoiChuan = s;
        }

        static string ChuanHoaDauCau(string s)
        {
            string ChuoiXuLiXong = "";
            int i = 0;
            List<string> ListChuoi = new List<string>();

            ListChuoi = new List<string>(s.Split(','));                
            while (i <= ListChuoi.Count - 1)
            {
                ListChuoi[i] = ListChuoi[i].Trim();

                if (i == ListChuoi.Count - 1)
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i];
                    break;
                }
                else
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i] + ", ";
                }

                i++;
            }

            i = 0; ListChuoi.Clear();
            ListChuoi = new List<string>(ChuoiXuLiXong.Split('.'));
            ChuoiXuLiXong = "";
            while (i <= ListChuoi.Count - 1)
            {
                ListChuoi[i] = ListChuoi[i].Trim();

                if (i == ListChuoi.Count - 1)
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i];
                    break;
                }
                else
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i] + ". ";
                }

                i++;
            }

            i = 0; ListChuoi.Clear();
            ListChuoi = new List<string>(ChuoiXuLiXong.Split(';'));
            ChuoiXuLiXong = "";
            while (i <= ListChuoi.Count - 1)
            {
                ListChuoi[i] = ListChuoi[i].Trim();

                if (i == ListChuoi.Count - 1)
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i];
                    break;
                }
                else
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i] + "; ";
                }

                i++;
            }

            i = 0; ListChuoi.Clear();
            ListChuoi = new List<string>(ChuoiXuLiXong.Split('?'));
            ChuoiXuLiXong = "";
            while (i <= ListChuoi.Count - 1)
            {
                ListChuoi[i] = ListChuoi[i].Trim();

                if (i == ListChuoi.Count - 1)
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i];
                    break;
                }
                else
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i] + "? ";
                }

                i++;
            }

            i = 0; ListChuoi.Clear();
            ListChuoi = new List<string>(ChuoiXuLiXong.Split('!'));
            ChuoiXuLiXong = "";
            while (i <= ListChuoi.Count - 1)
            {
                ListChuoi[i] = ListChuoi[i].Trim();

                if (i == ListChuoi.Count - 1)
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i];
                    break;
                }
                else
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i] + "! ";
                }

                i++;
            }   

            i = 0; ListChuoi.Clear();
            ListChuoi = new List<string>(ChuoiXuLiXong.Split(':'));
            ChuoiXuLiXong = "";
            while (i <= ListChuoi.Count - 1)
            {
                ListChuoi[i] = ListChuoi[i].Trim();

                if (i == ListChuoi.Count - 1)
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i];
                    break;
                }
                else
                {
                    ChuoiXuLiXong = ChuoiXuLiXong + ListChuoi[i] + ": ";
                }

                i++;
            }

            ListChuoi.Clear();
            
            while (ChuoiXuLiXong.IndexOf("! !") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("! !", "!!");
            }

            while (ChuoiXuLiXong.IndexOf("? ?") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("? ?", "??");
            }

            while (ChuoiXuLiXong.IndexOf(". ?") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace(". ?", ".?");
            }

            while (ChuoiXuLiXong.IndexOf(". !") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace(". !", ".!");
            }

            while (ChuoiXuLiXong.IndexOf("? !") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("? !", "?!");
            }

            while (ChuoiXuLiXong.IndexOf("! ?") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("! ?", "!?");
            }

            while (ChuoiXuLiXong.IndexOf("... ") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("... ", "...");
            }

            while (ChuoiXuLiXong.IndexOf("( ") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("( ", "(");
            }

            while (ChuoiXuLiXong.IndexOf(" )") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace(" )", ")");
            }

            while (ChuoiXuLiXong.IndexOf("[ ") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("[ ", "[");
            }

            while (ChuoiXuLiXong.IndexOf(" ]") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace(" ]", "]");
            }

            while (ChuoiXuLiXong.IndexOf("{ ") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("{ ", "{");
            }

            while (ChuoiXuLiXong.IndexOf(" }") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace(" }", "}");
            }

            while (ChuoiXuLiXong.IndexOf(". . .") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace(". . .", "...");
            }

            while (ChuoiXuLiXong.IndexOf("…") > -1)
            {
                ChuoiXuLiXong = ChuoiXuLiXong.Replace("…", "...");
            }

            return ChuoiXuLiXong;
        }

        #region Mouse

        /*
        private void _listenmouse_MouseWheel(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (GridSetting.Visibility == Visibility.Visible)
            {
                if(IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline)
                {
                    txtHotKeyMod.Text = "Hot key: MouseWheel";
                }
                
                if(IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline)
                {
                    txtHotKey.Text = "Hot key: MouseWheel";
                }
                
            }       
        }
        */

        private void _listenmouse_MiddleButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            //Kiểm tra xem có nhấn giữ phím không
            if ("MiddleButton" == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 0;
            }

            //Nhận tín hiệu khi dùng tổ hợp
            if ("MiddleButton" == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true && TrangThaiPhimGiu == 1)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            //Nhận tín hiệu khi không dùng tổ hợp
            if ("MiddleButton" == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked != true)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            if (GridSetting.Visibility == Visibility.Visible)
            {
                if (IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + "MiddleButton" != txtHotKey.Text)
                {
                    txtHotKeyMod.Text = "Hot key: MiddleButton";
                }

                if (IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + "MiddleButton" != txtHotKeyMod.Text)
                {
                    txtHotKey.Text = "Hot key: MiddleButton";
                }
            }
            
        }
        private void _listenmouse_MiddleButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            //Đánh dấu là phím đang được nhấn
            if ("MiddleButton" == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 1;
            }

            //Điều kiện  GridSetting.Visibility là để tránh trường hợp đang cài đặt mà nhảy lệnh CTRL V
            if ("MiddleButton" == HotKey && GridSetting.Visibility != Visibility.Visible)
            {
                //Trường hợp có dùng paste và có dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && TrangThaiPhimGiu == 1 && cbMutilKey.IsChecked == true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }

                //Trường hợp có dùng paste và không dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && cbMutilKey.IsChecked != true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }
            }
        }

        private void _listenmouse_RightButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {

            //Kiểm tra xem có nhấn giữ phím không
            if ("RightButton" == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 0;
            }

            //Nhận tín hiệu khi dùng tổ hợp
            if ("RightButton" == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true && TrangThaiPhimGiu == 1)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            //Nhận tín hiệu khi không dùng tổ hợp
            if ("RightButton" == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked != true)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            if (GridSetting.Visibility == Visibility.Visible)
            {
                if (IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + "RightButton" != txtHotKey.Text)
                {
                    txtHotKeyMod.Text = "Hot key: RightButton";
                }

                if (IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + "RightButton" != txtHotKeyMod.Text)
                {
                    txtHotKey.Text = "Hot key: RightButton";
                }
            }

            System.GC.Collect();
        }
        private void _listenmouse_RightButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            //Đánh dấu là phím đang được nhấn
            if ("RightButton" == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 1;
            }

            //Điều kiện  GridSetting.Visibility là để tránh trường hợp đang cài đặt mà nhảy lệnh CTRL V
            if ("RightButton" == HotKey && GridSetting.Visibility != Visibility.Visible)
            {
                //Trường hợp có dùng paste và có dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && TrangThaiPhimGiu == 1 && cbMutilKey.IsChecked == true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }

                //Trường hợp có dùng paste và không dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && cbMutilKey.IsChecked != true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }
            }
        }

        private void _listenmouse_LeftButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            //Kiểm tra xem có nhấn giữ phím không
            if ("LeftButton" == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 0;
            }

            //Nhận tín hiệu khi dùng tổ hợp
            if ("LeftButton" == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true && TrangThaiPhimGiu == 1)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            //Nhận tín hiệu khi không dùng tổ hợp
            if ("LeftButton" == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked != true)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            //Nhận tín hiệu phím trong lúc cài đặt
            if (GridSetting.Visibility == Visibility.Visible)
            {
                if (IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + "LeftButton" != txtHotKey.Text)
                {
                    txtHotKeyMod.Text = "Hot key: " + "LeftButton";
                }

                if (IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + "LeftButton" != txtHotKeyMod.Text)
                {
                    txtHotKey.Text = "Hot key: " + "LeftButton";
                }
            }

            System.GC.Collect();
        }
        private void _listenmouse_LeftButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            //Đánh dấu là phím đang được nhấn
            if ("LeftButton" == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 1;
            }

            //Điều kiện  GridSetting.Visibility là để tránh trường hợp đang cài đặt mà nhảy lệnh CTRL V
            if ("LeftButton" == HotKey && GridSetting.Visibility != Visibility.Visible)
            {
                //Trường hợp có dùng paste và có dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && TrangThaiPhimGiu == 1 && cbMutilKey.IsChecked == true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }

                //Trường hợp có dùng paste và không dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && cbMutilKey.IsChecked != true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }
            }

            System.GC.Collect();
        }

        #endregion

        #region Keyboard

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            //Đánh dấu là phím đang được nhấn
            if(e.KeyPressed.ToString() == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 1;
            }

            //Điều kiện  GridSetting.Visibility là để tránh trường hợp đang cài đặt mà nhảy lệnh CTRL V
            if (e.KeyPressed.ToString() == HotKey && GridSetting.Visibility != Visibility.Visible)
            {
                //Trường hợp có dùng paste và có dùng tổ hợp
                if(rbtnCopyPaste.IsChecked == true && TrangThaiPhimGiu == 1 && cbMutilKey.IsChecked == true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }

                //Trường hợp có dùng paste và không dùng tổ hợp
                if (rbtnCopyPaste.IsChecked == true && cbMutilKey.IsChecked != true)
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");               
                }                
            }

            System.GC.Collect();
        }

        private void _listener_OnKeyUp(object sender, KeyUpArgs e)
        {
            //Kiểm tra xem có nhấn giữ phím không
            if(e.KeyUp.ToString() == HotKeyMod && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true)
            {
                TrangThaiPhimGiu = 0;
            }

            //Nhận tín hiệu khi dùng tổ hợp
            if (e.KeyUp.ToString() == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked == true && TrangThaiPhimGiu == 1 )
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }
            
            //Nhận tín hiệu khi không dùng tổ hợp
            if (e.KeyUp.ToString() == HotKey && GridSetting.Visibility != Visibility.Visible && cbMutilKey.IsChecked != true)
            {
                if (ViTriBoNhoDem < lbText.Items.Count - 1)
                {
                    DoiBoNhoDem();                    
                }
                else
                {
                    Menu.Width = 90;
                    btnStop.Width = 0;
                    txtTitle.Text = "Tool type";

                    _listenmouse.Uninstall();
                    _listener.UnHookKeyboard();
                }

                System.GC.Collect();
            }

            //Nhận tín hiệu phím trong lúc cài đặt
            if(GridSetting.Visibility == Visibility.Visible)
            {
                if(rbtnCopyPaste.IsChecked == true)
                {
                    if (IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline
                    && e.KeyUp.ToString() != "V"
                    && e.KeyUp.ToString() != "LeftCtrl"
                    && e.KeyUp.ToString() != "RightCtrl"
                    && "Hot key: " + e.KeyUp.ToString() != txtHotKey.Text)
                    {
                        txtHotKeyMod.Text = "Hot key: " + e.KeyUp.ToString();
                    }

                    if (IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline
                    && e.KeyUp.ToString() != "V"
                    && e.KeyUp.ToString() != "LeftCtrl"
                    && e.KeyUp.ToString() != "RightCtrl"
                    && "Hot key: " + e.KeyUp.ToString() != txtHotKeyMod.Text)
                    {
                        txtHotKey.Text = "Hot key: " + e.KeyUp.ToString();
                    }
                }
                else
                {
                    if (IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + e.KeyUp.ToString() != txtHotKey.Text)
                    {
                        txtHotKeyMod.Text = "Hot key: " + e.KeyUp.ToString();
                    }

                    if (IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline && "Hot key: " + e.KeyUp.ToString() != txtHotKeyMod.Text)
                    {
                        txtHotKey.Text = "Hot key: " + e.KeyUp.ToString();
                    }
                }                                                    
            }

            System.GC.Collect();
        }

        #endregion

        #region Event

        private void txtTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            //txtTitle.Text = "cao= " + Main.ActualHeight.ToString() + "  rộng" + Main.ActualWidth.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            btnClose.Focusable = false;

            this.Close();
        }

        private void btnAddText_Click(object sender, RoutedEventArgs e)
        {
            btnAddText.Focusable = false;

            if (TrangThaiSetting == 1)
            {
                btnSetting_Click(sender, e);
            }

            if (scrollHelp.Visibility == Visibility.Visible)
            {
                btnHelp_Click(sender, e);
            }
          
            string FileName = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;

                lbText.Items.Clear();
                ViTriBoNhoDem = 0;
                scrollText.ScrollToTop();
                ListText.Clear();
                txtTitle.Text = "Tool type";
            }
        
            if (FileName != "")
            {
                StreamReader objReader;

                objReader = new StreamReader(FileName);

                TagInfo = 1;

                //đọc từng dòng trong file txt rồi đưa lên listbox
                do
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    lbItem.Content = objReader.ReadLine() + "";
                    lbText.Items.Add(lbItem);
                    lbItem.Tag = TagInfo.ToString();
                    lbItem.Focusable = false;
                    //lbItem.ToolTip = TagInfo.ToString();
                    lbItem.PreviewMouseLeftButtonDown += LbItem_PreviewMouseLeftButtonDown;
                    ListText.Add(lbItem.Content.ToString()); 
                    TagInfo++;
                }
                while (objReader.Peek() != -1);

                TagInfo = 1;

                objReader.Close();
                System.GC.Collect();
            }
        }

        private void LbItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;

            lbText.SelectedIndex = int.Parse(item.Tag.ToString()) -1;
            ViTriBoNhoDem = int.Parse(item.Tag.ToString()) - 1;

            XiLiChuoi(ListText[int.Parse(item.Tag.ToString()) - 1].ToString());
            System.Windows.Clipboard.SetDataObject(ChuoiChuan);

            txtTitle.Text = (ViTriBoNhoDem +1).ToString() + "/" + lbText.Items.Count.ToString();

        }    

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Focusable = false;

            if (GridText.Visibility == Visibility.Visible)
            {
                BtnText_Click(sender, e);
            }

            if (TrangThaiSetting == 1)
            {
                btnSetting_Click(sender, e);
            }

            if(scrollHelp.Visibility == Visibility.Visible)
            {
                btnHelp_Click(sender, e);
            }

            if (lbText.Items.Count > 0)
            {
               
                Menu.Width = 0;
                btnStop.Width = 50;

                _listenmouse.Install();
                _listener.HookKeyboard();

                if(ViTriBoNhoDem == 0 || ViTriBoNhoDem == lbText.Items.Count - 1)
                {
                    ViTriBoNhoDem = 0;

                    XiLiChuoi(ListText[ViTriBoNhoDem].ToString());
                    System.Windows.Clipboard.SetDataObject(ChuoiChuan);

                    lbText.SelectedIndex = ViTriBoNhoDem;
                    scrollText.ScrollToTop();
                }
                
                txtTitle.Text = (ViTriBoNhoDem + 1).ToString() + "/" + lbText.Items.Count.ToString();
            }
            else
            {
                MessageBox.Show("Bạn chưa nạp File Text", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            btnMinus.Focusable = false;

            if (int.Parse(txtChangeLine.Text) > 1)
            {
                txtChangeLine.Text = (int.Parse(txtChangeLine.Text) - 1).ToString();
            }
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            btnPlus.Focusable = false;
            txtChangeLine.Text = (int.Parse(txtChangeLine.Text) + 1).ToString();
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            btnSetting.Focusable = false;

            if (TrangThaiSetting == 0)
            {
                btnSetting.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
                
                scrollText.Visibility = Visibility.Collapsed;
                GridSetting.Visibility = Visibility.Visible;

                TrangThaiSetting = 1;
            }
            else
            {
                btnSetting.Background = Brushes.Transparent;              

                scrollText.Visibility = Visibility.Visible;
                GridSetting.Visibility = Visibility.Collapsed;

                TrangThaiSetting = 0;
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            btnHelp.Focusable = false;

            if (scrollText.Visibility == Visibility.Visible)
            {
                scrollText.Visibility = Visibility.Collapsed;
                scrollHelp.Visibility = Visibility.Visible;
            }
            else
            {
                scrollText.Visibility = Visibility.Visible;
                scrollHelp.Visibility = Visibility.Collapsed;
            }
        }
      
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnStop.Focusable = false;
            Menu.Width = 90;
            btnStop.Width = 0;

            _listenmouse.Uninstall();
            _listener.UnHookKeyboard();

            txtTitle.Text = "Tool type";
        }
      
        private void btnMinisize_Click(object sender, RoutedEventArgs e)
        {
            btnMinisize.Focusable = false;
            //txtTitle.Text = Main.ActualWidth + "/" + Main.ActualHeight;
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void btnTopMost_Click(object sender, RoutedEventArgs e)
        {
            btnTopMost.Focusable = false;

            if (iconTopMostModel.Kind == MaterialDesignThemes.Wpf.PackIconKind.VectorArrangeAbove)
            {
                iconTopMostModel.Kind = MaterialDesignThemes.Wpf.PackIconKind.VectorArrangeBelow;
                Topmost = false;
            }
            else
            {
                iconTopMostModel.Kind = MaterialDesignThemes.Wpf.PackIconKind.VectorArrangeAbove;
                Topmost = true;
            }
        }

        private void lbText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {                         
            System.GC.Collect();          
        }

        private void btnChangeHotKey_Click(object sender, RoutedEventArgs e)
        {
            btnChangeHotKey.Focusable = false;

            btnChangeHotKey.BorderThickness = new Thickness(1, 1, 1, 1);
            btnChangeHotKey.BorderBrush = Brushes.Red;

            if (IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline)
            {
                IconHotKey.Kind = MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline;
                IconHotKeyMod.Kind = MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline;

                ttHotKey.Visibility = Visibility.Visible;
                ttHotKey.IsOpen = true;

                _listenmouse.Install();
                _listener.HookKeyboard();
            }
        }
                 
        private void BtnChangeHotKeyMod_Click(object sender, RoutedEventArgs e)
        {
            btnChangeHotKeyMod.Focusable = false;
            
            btnChangeHotKeyMod.BorderThickness = new Thickness(1, 1, 1, 1);
            btnChangeHotKeyMod.BorderBrush = Brushes.Red;

            if (IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline)
            {
                IconHotKeyMod.Kind = MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline;
                IconHotKey.Kind = MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline;

                ttHotKeyMod.Visibility = Visibility.Visible;
                ttHotKeyMod.IsOpen = true;

                _listenmouse.Install();
                _listener.HookKeyboard();
            }
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listenmouse.Uninstall();
            _listener.UnHookKeyboard();

            if (rbtnCopy.IsChecked == true)
            {
                TrangThaiCopy = 0;
            }
            else
            {
                TrangThaiCopy = 1;
            }

            //tạo file txt và ghi lần lượt từng dòng vào
            if (File.Exists("./Setting.txt"))
            {
                File.Delete("./Setting.txt");
            }        
            using (StreamWriter sw = File.CreateText("Setting.txt"))
            {
                sw.WriteLine(HotKey.ToString());
                sw.WriteLine(HotKeyMod.ToString());
                sw.WriteLine(TrangThaiCopy.ToString());
                sw.WriteLine(TrangThaiToHopPhim.ToString());

                if(cbKhoangTrang.IsChecked == true) {sw.WriteLine("1");}
                else { sw.WriteLine("0"); }

                if (cbKiTuDau.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cbChonTatCa.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb1.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb2.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb3.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb4.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb5.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb6.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb7.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb8.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb9.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb10.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb11.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb12.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb13.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb14.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb15.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb16.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb17.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb18.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb19.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb20.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb21.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb22.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb23.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb24.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }

                if (cb25.IsChecked == true) { sw.WriteLine("1"); }
                else { sw.WriteLine("0"); }
            }
        }

        private void CbMutilKey_Click(object sender, RoutedEventArgs e)
        {
            cbMutilKey.Focusable = false;
            //IconHotKey.Kind = MaterialDesignThemes.Wpf.PackIconKind.StopCircle;
            if (cbMutilKey.IsChecked == true)
            {
                TrangThaiToHopPhim = 1;
                RowMod.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                TrangThaiToHopPhim = 0;
                RowMod.Height = new GridLength(0, GridUnitType.Star);
            }

        }

        private void RbtnCopy_Click(object sender, RoutedEventArgs e)
        {
            HotKey = " ";
            HotKeyMod = " ";

            txtHotKey.Text = "Hot key: ";
            txtHotKeyMod.Text = "Hot key: ";
        }

        private void RbtnCopyPaste_Click(object sender, RoutedEventArgs e)
        {
            HotKey = " ";
            HotKeyMod = " ";

            txtHotKey.Text = "Hot key: ";
            txtHotKeyMod.Text = "Hot key: ";
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            btnClear.Focusable = false;

            HotKey = " ";
            HotKeyMod = " ";

            txtHotKey.Text = "Hot key: ";
            txtHotKeyMod.Text = "Hot key: ";
        }

        private void BtnText_Click(object sender, RoutedEventArgs e)
        {

            btnText.Focusable = false;

            if(GridText.Visibility == Visibility.Visible)
            {
                btnText.Background = Brushes.Transparent;

                GridText.Visibility = Visibility.Collapsed;

            }
            else
            {
                GridText.Visibility = Visibility.Visible;

                btnText.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
            }

            //MessageBox.Show(cb9.Content.ToString());
            
        }

        private void CbKiTuDau_Click(object sender, RoutedEventArgs e)
        {
            if (cbKiTuDau.IsChecked == true)
            {
                GridListChar.Visibility = Visibility.Visible;
            }
            else
            {
                GridListChar.Visibility = Visibility.Collapsed;
            }
        }

        private void CbChonTatCa_Click(object sender, RoutedEventArgs e)
        {
            if(cbChonTatCa.IsChecked == true)
            {
                cbChonTatCa.Content = "Bỏ chọn tất cả";

                cb1.IsChecked = true;
                cb2.IsChecked = true;
                cb3.IsChecked = true;
                cb4.IsChecked = true;
                cb5.IsChecked = true;
                cb6.IsChecked = true;
                cb7.IsChecked = true;
                cb8.IsChecked = true;
                cb9.IsChecked = true;
                cb10.IsChecked = true;
                cb11.IsChecked = true;
                cb12.IsChecked = true;
                cb13.IsChecked = true;
                cb14.IsChecked = true;
                cb15.IsChecked = true;
                cb16.IsChecked = true;
                cb17.IsChecked = true;
                cb18.IsChecked = true;
                cb19.IsChecked = true;
                cb20.IsChecked = true;
                cb21.IsChecked = true;
                cb22.IsChecked = true;
                cb23.IsChecked = true;
                cb24.IsChecked = true;
                cb25.IsChecked = true;
            }
            else
            {
                cbChonTatCa.Content = "Chọn tất cả";

                cb1.IsChecked = false;
                cb2.IsChecked = false;
                cb3.IsChecked = false;
                cb4.IsChecked = false;
                cb5.IsChecked = false;
                cb6.IsChecked = false;
                cb7.IsChecked = false;
                cb8.IsChecked = false;
                cb9.IsChecked = false;
                cb10.IsChecked = false;
                cb11.IsChecked = false;
                cb12.IsChecked = false;
                cb13.IsChecked = false;
                cb14.IsChecked = false;
                cb15.IsChecked = false;
                cb16.IsChecked = false;
                cb17.IsChecked = false;
                cb18.IsChecked = false;
                cb19.IsChecked = false;
                cb20.IsChecked = false;
                cb21.IsChecked = false;
                cb22.IsChecked = false;
                cb23.IsChecked = false;
                cb24.IsChecked = false;
                cb25.IsChecked = false;
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            rbtnCopy.IsChecked = true;
            cbMutilKey.IsChecked = false;
            txtHotKey.Text = "Hot key: F4";
            txtHotKeyMod.Text = "";
            HotKey = "F4";
            HotKeyMod = "";
            TrangThaiToHopPhim = 0;
            RowMod.Height = new GridLength(0, GridUnitType.Star);
        }

        #endregion

        #region Giao diện

        private void btnChangeHotKey_MouseEnter(object sender, MouseEventArgs e)
        {
            btnChangeHotKey.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnChangeHotKey_MouseLeave(object sender, MouseEventArgs e)
        {
            if(IconHotKey.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline)
            {
                HotKey = txtHotKey.Text.Split(' ')[2];

                btnChangeHotKey.BorderThickness = new Thickness(0, 0, 0, 0);
                IconHotKey.Kind = MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline;

                ttHotKey.IsOpen = false;
                ttHotKey.Visibility = Visibility.Collapsed;

                _listenmouse.Uninstall();
                _listener.UnHookKeyboard();
            }
            
            btnChangeHotKey.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF535353"));
        }

        private void BtnChangeHotKeyMod_MouseEnter(object sender, MouseEventArgs e)
        {         
            btnChangeHotKeyMod.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void BtnChangeHotKeyMod_MouseLeave(object sender, MouseEventArgs e)
        {
            if(IconHotKeyMod.Kind == MaterialDesignThemes.Wpf.PackIconKind.PauseCircleOutline)
            {
                HotKeyMod = txtHotKeyMod.Text.Split(' ')[2];

                btnChangeHotKeyMod.BorderThickness = new Thickness(0, 0, 0, 0);
                IconHotKeyMod.Kind = MaterialDesignThemes.Wpf.PackIconKind.PlayCircleOutline;

                ttHotKeyMod.IsOpen = false;
                ttHotKeyMod.Visibility = Visibility.Collapsed;

                _listenmouse.Uninstall();
                _listener.UnHookKeyboard();
            }
            
            btnChangeHotKeyMod.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF535353"));
        }

        private void btnStop_MouseEnter(object sender, MouseEventArgs e)
        {
            btnStop.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnStop_MouseLeave(object sender, MouseEventArgs e)
        {
            btnStop.Background = Brushes.Transparent;
        }

        private void btnHelp_MouseEnter(object sender, MouseEventArgs e)
        {
            btnHelp.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnHelp_MouseLeave(object sender, MouseEventArgs e)
        {
            btnHelp.Background = Brushes.Transparent;
        }

        private void btnSetting_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TrangThaiSetting == 0)
            {
                btnSetting.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
            }
        }

        private void btnSetting_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TrangThaiSetting == 0)
            {
                btnSetting.Background = Brushes.Transparent;
            }
        }

        private void btnMinus_MouseEnter(object sender, MouseEventArgs e)
        {
            btnMinus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnMinus_MouseLeave(object sender, MouseEventArgs e)
        {
            btnMinus.Background = Brushes.Transparent;
        }

        private void btnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            btnStart.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            btnStart.Background = Brushes.Transparent;
        }
       
        private void btnPlus_MouseEnter(object sender, MouseEventArgs e)
        {
            btnPlus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnPlus_MouseLeave(object sender, MouseEventArgs e)
        {
            btnPlus.Background = Brushes.Transparent;
        }

        private void btnAddText_MouseEnter(object sender, MouseEventArgs e)
        {
            btnAddText.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void btnAddText_MouseLeave(object sender, MouseEventArgs e)
        {
            btnAddText.Background = Brushes.Transparent;
        }

        private void BtnClear_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void BtnClear_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClear.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF535353"));
        }

        private void BtnText_MouseEnter(object sender, MouseEventArgs e)
        {
            btnText.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void BtnText_MouseLeave(object sender, MouseEventArgs e)
        {
            if (GridText.Visibility == Visibility.Collapsed)
            {
                btnText.Background = Brushes.Transparent;
            }
        }

        private void BtnReset_MouseEnter(object sender, MouseEventArgs e)
        {
            btnReset.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF707070"));
        }

        private void BtnReset_MouseLeave(object sender, MouseEventArgs e)
        {
            btnReset.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF535353"));
        }

        #endregion






    }
}
