using System;
using System.Runtime.InteropServices;

namespace WkHtmlToPdfDotNet
{
    internal static class ModuleFactory
    {
        private static IWkHtmlModule GetWindowsModule(bool isDotNetStandard)
        {
            if (isDotNetStandard)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    return new WkHtmlModuleWin64();
                }
            }
            else if (IntPtr.Size == 8) // Is64 bit Arch
            {
                return new WkHtmlModuleWin64();
            }

            return new WkHtmlModuleWin86();
        }

        public static IWkHtmlModule GetModule()
        {
            var isDotNetStandard = false;
#if NETSTANDARD2_0
            isDotNetStandard = true;
#endif
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return GetWindowsModule(isDotNetStandard);
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Return X64 library regardless, since ARM64 needs to run X64 using Rosetta 2
                    return new WkHtmlModuleOsx64();
                }
                else
                {
                    if (!isDotNetStandard)
                    {
                        return new WkHtmlModule();
                    }

                    switch (RuntimeInformation.ProcessArchitecture)
                    {
                        case Architecture.X64:
                            return new WkHtmlModuleLinux64();
                        case Architecture.X86:
                            return new WkHtmlModuleLinux86();
                        case Architecture.Arm:
                        case Architecture.Arm64:
                            return new WkHtmlModuleLinuxArm64();
                        default: // Unreachable
                            return new WkHtmlModule();
                    }
                }
            }
            catch
            {
                throw new NotSupportedException(
                    "Unable to load native library. The platform may be missing native dependencies (libjpeg62, etc). Or the current platform is not supported.");
            }
        }
    }
}