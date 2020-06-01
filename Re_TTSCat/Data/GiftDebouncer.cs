using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Re_TTSCat.Data
{
    public sealed class GiftDebouncer
    {
        private const int tickInterval = 1;
        private List<UserGift> giftCache = new List<UserGift>();
        private Timer timer;

        private void timerTickCallback(object state)
        {
            lock (giftCache)
            {
                var currentTime = DateTimeOffset.Now;
                var giftsToDebounce = giftCache.Where(x => x.TargetTime < currentTime).ToList();
                giftsToDebounce.ForEach(x => giftCache.Remove(x));

                foreach (var item in giftsToDebounce)
                    GiftDebouncedEvent?.Invoke(this, item);
            }
        }

        public GiftDebouncer() => timer = new Timer(new TimerCallback(timerTickCallback), null, 0, 1000);

        public void Add(UserGift gift)
        {
            lock (giftCache)
            {
                var item = giftCache.FirstOrDefault(x => x.IsAddable(gift));
                if (item == null)
                    giftCache.Add(gift);
                else
                    item.Qty += gift.Qty;
            }
        }

        public event EventHandler<UserGift> GiftDebouncedEvent;
    }
}
