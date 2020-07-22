using System;
using System.IO;

namespace PaulZero.RoutineEnforcer
{
    public static class PathUtilities
    {
        public static string GetProgramDataDirectory()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            return Path.Combine(programData, "PaulZero", "WindowsRoutine");
        }
    }
}
