using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SeaFight.Enums;

namespace SeaFight.Helpers
{
    public static class ErrorSignalizationHelper
    {
        public static List<string> Log { get; private set; } = new List<string> { "Error log:" };

        public static void ErrorDetected(string info = "", ReasonType reasonType = ReasonType.OtherError)
        {
            switch (reasonType)
            {
                case ReasonType.ResourceDictionaryError:
                    PrintAndSaveToLog($"ResourceDictionary doesn't contain required {info} resource.");
                    break;

                case ReasonType.ICollectionError:
                    PrintAndSaveToLog($"ICollection error: {info}.");
                    break;

                case ReasonType.Exception:
                    PrintAndSaveToLog($"Exception catched: {info}");
                    break;

                case ReasonType.NullError:
                    PrintAndSaveToLog($"{info} is null.");
                    break;

                case ReasonType.OtherError:
                    PrintAndSaveToLog($"Undefined error: {info}.");
                    break;

                default:
                    PrintAndSaveToLog("Bad day...");
                    break;
            }

            void PrintAndSaveToLog(string data)
            {
                Console.WriteLine(data);
                Log.Add(data);
            }
        }

        public static void PrintLog()
        {
            Task.Run(() => Log.ForEach(Console.WriteLine));
        }
    }
}
