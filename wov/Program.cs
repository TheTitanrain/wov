using CSCore.CoreAudioAPI;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace VolumeLevel
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    IDictionary<string, float> procs = new Dictionary<string, float>();
                    foreach (var session in sessionEnumerator)
                    {
                        using (var audioMeterInformation = session.QueryInterface<AudioMeterInformation>())
                        using (var session2 = session.QueryInterface<AudioSessionControl2>())
                        {
                            if (session2.ProcessID > 0 && audioMeterInformation != null) procs[session2.Process.MainWindowTitle] = audioMeterInformation.GetPeakValue();
                        }
                    }
                    if (procs.Count > 0)
                    {
                        Console.WriteLine(new JavaScriptSerializer().Serialize(procs));
                    }
                    else
                    {
                        Console.WriteLine("{}");
                    }
                    Environment.Exit(0);
                }
            }
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }
    }
}