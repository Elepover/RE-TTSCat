using System;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Re_TTSCat.Data
{
    public static partial class Extensions
    {
        public static Task BeginAsync(this Storyboard storyboard)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (storyboard == null)
                tcs.SetException(new ArgumentNullException());
            else
            {
                EventHandler onComplete = null;
                onComplete = (s, e) => {
                    storyboard.Completed -= onComplete;
                    tcs.SetResult(true);
                };
                storyboard.Completed += onComplete;
                storyboard.Begin();
            }
            return tcs.Task;
        }
    }
}
