using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDRScreenshot
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            this.Shown += (s, e) =>
            {
                Visible = false;
            };
            Win32.RegisterHotKey(this.Handle, 0, Win32.MOD_SHIFT | Win32.MOD_WIN, Win32.KEY_PRINTSCREEN);
            var notifyIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Visible = true,
                Text = "HDR Screenshot"
            };
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
            {
                new MenuItem("Exit", (s, e) => {
                    this.Close();
                })
            });
            notifyIcon.ShowBalloonTip(5000, "HDR Screenshot Running", "Press Shift + Win + PrintScreen to take a screenshot.", ToolTipIcon.Info);
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_HOTKEY)
            {
                Screenshot.Capture();
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Win32.UnregisterHotKey(this.Handle, 0);
            base.OnFormClosing(e);
        }
    }
}
