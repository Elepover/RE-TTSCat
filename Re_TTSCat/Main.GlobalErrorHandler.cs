using System;

namespace Re_TTSCat
{
    public partial class Main
    {
        public void GlobalErrorHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var obj = (Exception) e.ExceptionObject;
            System.Windows.MessageBox.Show($"未知错误发生{(e.IsTerminating ? "，弹幕姬即将退出" : "，如您后续遇到问题，请尝试重启弹幕姬")}，反馈详细信息: {obj.ToString()}", "Re: TTSCat - 意外错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
