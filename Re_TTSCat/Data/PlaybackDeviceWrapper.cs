using NAudio.Wave;
using System;

namespace Re_TTSCat.Data
{
    public class PlaybackDeviceWrapper
    {
        public Guid DeviceGuid;

        public override string ToString() => ToString(false);

        public string ToString(bool showGuid)
        {
            foreach (var dev in DirectSoundOut.Devices)
            {
                if (dev.Guid == DeviceGuid)
                    return $"{(showGuid ? dev.Guid.ToString() + " - " : string.Empty)}{dev.Description}";
            }
            return "(设备不存在)";
        }
    }
}
