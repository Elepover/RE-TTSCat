using System.Threading;
using System.Windows.Forms;

namespace Re_TTSCat.Windows
{
    public static class AsyncDialog
    {
        public static void Open(string content, string title = "Re: TTSCat", MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            var MsgBoxThread = new Thread(() =>
            {
                MessageBox.Show(content, title, buttons, icon);
            });
            MsgBoxThread.SetApartmentState(ApartmentState.STA);
            MsgBoxThread.Start();
        }
    }
}
