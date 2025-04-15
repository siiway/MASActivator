using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace MASActivator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            requestAdmin();
        }

        /// <summary>
        /// Request administrator privileges if the application is not already running with elevated permissions.
        /// </summary>
        private void requestAdmin()
        {
            if (!IsRunAsAdministrator())
            {
                // 如果没有管理员权限，则请求提升权限
                RunAsAdministrator();
            }
        }

        /// <summary>
        /// Check if the application is running with administrator privileges.
        /// </summary>
        /// <returns>
        /// A boolean indicating whether the application is running as administrator.
        /// </returns>
        private static bool IsRunAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Run the application as administrator.
        /// </summary>
        private static void RunAsAdministrator()
        {
            // 重新启动应用程序以管理员身份
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                UseShellExecute = true,
                Verb = "runas"  // 请求管理员权限
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                // 处理异常，例如用户拒绝提升权限
                MessageBox.Show("无法以管理员身份运行应用程序：" + ex.Message, "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Environment.Exit(0); // 退出当前实例
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            setupOtherPage();
        }

        private void OpenWebPage(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening web page: \n" + ex.Message, "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Mainform Design

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenWebPage("https://massgrave.dev");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenWebPage("https://github.com/massgravel/Microsoft-Activation-Scripts");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenWebPage("https://github.com/Siiway/MASActivator");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        #endregion

        private void setupOtherPage()
        {
            setupWindowsEditionsComboBox();
        }

        #region Change Windows Edition

        [DllImport("DismApi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int DismInitialize(DismLogLevel logLevel, IntPtr logFilePath, IntPtr scratchDirectory);

        [DllImport("DismApi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int DismOpenSession(string imagePath, IntPtr windowsDirectory, IntPtr systemDrive, out uint session);

        [DllImport("DismApi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int DismSetEdition(uint session, string edition, string productKey, IntPtr cancelEvent, IntPtr progress, IntPtr userData);

        [DllImport("DismApi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int DismShutdown();

        private enum DismLogLevel
        {
            Errors = 0,
            Warnings = 1,
            Information = 2
        }

        /// <summary>
        /// Change Windows edition using DISM API.
        /// </summary>
        /// <param name="targetEdition">Terget Edition</param>
        /// <param name="productKey">Product Key</param>
        private void ChangeWindowsEditionUsingDismApi(string targetEdition, string productKey)
        {
            uint session = 0;
            bool hasError = false;

            try
            {
                // 初始化 DISM API
                int initResult = DismInitialize(DismLogLevel.Information, IntPtr.Zero, IntPtr.Zero);
                if (initResult != 0)
                {
                    hasError = true;
                    MessageBox.Show($"DismInitialize failed with error code: \n{initResult}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // 打开 DISM 会话
                int openSessionResult = DismOpenSession("DISM_{53BFAE52-B167-4E2F-A258-0A37B57FF845}", IntPtr.Zero, IntPtr.Zero, out session);
                if (openSessionResult != 0)
                {
                    hasError = true;
                    MessageBox.Show($"DismOpenSession failed with error code: \n{openSessionResult}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // 设置版本
                int setEditionResult = DismSetEdition(session, targetEdition, productKey, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if (setEditionResult != 0)
                {
                    hasError = true;
                    MessageBox.Show($"DismSetEdition failed with error code: \n{setEditionResult}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (hasError)
                {
                    MessageBox.Show("Failed to change Windows edition.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Windows edition changed successfully. Restart System?", "MASA", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.OK)
                    {
                        Process.Start("shutdown", "/r /t 0");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 关闭 DISM API
                if (session != 0)
                {
                    DismShutdown();
                }
            }
        }

        private void btnChangeWindowsEdition_Click(object sender, EventArgs e)
        {
            // 获取选中的 Windows 版本
            string selectedEdition = comboBoxWindowsEditions.SelectedItem.ToString();
            if (selectedEdition != null && selectedEdition != "Select Windows Edition")
            {
                // 获取产品密钥
                string productKey = getProductKey(selectedEdition);

                if (!string.IsNullOrEmpty(productKey))
                {
                    // 调用 DISM API 更改版本
                    ChangeWindowsEditionUsingDismApi(selectedEdition, productKey);
                }
                else
                {
                    MessageBox.Show("No valid product key found for the selected edition.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid Windows edition.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        List<string> targetEditions = GetTargetWindowsEditions();

        private void setupWindowsEditionsComboBox()
        {
            comboBoxWindowsEditions.Items.Clear();
            targetEditions.Sort();
            targetEditions.Insert(0, "Select Windows Edition");
            comboBoxWindowsEditions.Items.AddRange(targetEditions.ToArray());
            comboBoxWindowsEditions.SelectedIndex = 0;
        }

        /// <summary>
        /// Get the target Windows editions based on the current OS version.
        /// </summary>
        /// <returns>
        /// A list of upgradeable Windows editions.
        /// </returns>
        static List<string> GetTargetWindowsEditions()
        {
            List<string> targetEditions = new List<string>();
            int winBuild = GetWindowsBuild();

            if (winBuild >= 10240)
            {
                targetEditions.AddRange(GetEditionsFromDism());
            }
            else
            {
                targetEditions.AddRange(GetEditionsFromPowerShell());
            }

            return targetEditions.Distinct().ToList();
        }

        /// <summary>
        /// Get the Windows build number from the OS version.
        /// </summary>
        /// <returns>
        /// An integer representing the Windows build number.
        /// </returns>
        static int GetWindowsBuild()
        {
            return Environment.OSVersion.Version.Build;
        }

        /// <summary>
        /// Get the target Windows editions using DISM command.
        /// </summary>
        /// <returns>
        /// A list of upgradeable Windows editions from DISM.
        /// </returns>
        static List<string> GetEditionsFromDism()
        {
            List<string> editions = new List<string>();
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c dism /online /english /Get-TargetEditions",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    if (output != null)
                    {
                        process.WaitForExit();
                        MatchCollection matches = Regex.Matches(output, @"Target Edition : (\S+)");
                        foreach (Match match in matches)
                        {
                            editions.Add(match.Groups[1].Value);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: No output from DISM command.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return editions;
        }

        /// <summary>
        /// Get the target Windows editions using PowerShell command.
        /// </summary>
        /// <returns>
        /// A list of upgradeable Windows editions from PowerShell.
        /// </returns>
        static List<string> GetEditionsFromPowerShell()
        {
            List<string> editions = new List<string>();
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"& { ($f=[io.file]::ReadAllText($env:SystemRoot+'\\servicing\\Packages\\Microsoft-Windows-Edition~*.mum') -split ':cbsxml:\\.*')[1] -match 'Target Edition : (\\S+)' | ForEach-Object { $_.Groups[1].Value } }\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    editions.AddRange(output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    MessageBox.Show("Error: Process is null.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return editions;
        }

        /// <summary>
        /// Change Windows edition using DISM command.
        /// Now using DISM API instead of this; keep this in the code.
        /// </summary>
        /// <param name="edition">Target Edition</param>
        private void changeWindowsEdition(string edition)
        {
            if (edition == "Select Windows Edition")
            {
                MessageBox.Show("Please select a valid Windows edition.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 获取产品密钥
            string productKey = getProductKey(edition);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c dism /online /Set-Edition:{edition} /ProductKey:{productKey} /AcceptEula",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    MessageBox.Show(output, "MASA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error: Process is null.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Get the product key for the specified edition.
        /// </summary>
        /// <param name="edition">Windows Edition</param>
        /// <returns>
        /// The product key for the specified edition.
        /// </returns>
        private string getProductKey(string edition)
        {
            // 定义产品密钥数据
            var productKeyData = new List<(string Key, string KeyType, string EditionID, string VersionName)>
            {
                ("XGVPP-NMH47-7TTHJ-W3FW7-8HV2C", "OEM:NONSLP", "Enterprise", ""),
                ("D6RD9-D4N8T-RT9QX-YW6YT-FCWWJ", "Retail", "Starter", ""),
                ("3V6Q6-NQXCX-V8YXR-9QCYV-QPFCT", "Volume:MAK", "EnterpriseN", ""),
                ("3NFXW-2T27M-2BDW6-4GHRV-68XRX", "Retail", "StarterN", ""),
                ("VK7JG-NPHTM-C97JM-9MPGT-3V66T", "Retail", "Professional", ""),
                ("2B87N-8KFHP-DKV6R-Y2C8J-PKCKT", "Retail", "ProfessionalN", ""),
                ("4CPRK-NM3K3-X6XXQ-RXX86-WXCHW", "Retail", "CoreN", ""),
                ("N2434-X9D7W-8PF6X-8DV9T-8TYMD", "Retail", "CoreCountrySpecific", ""),
                ("BT79Q-G7N6G-PGBYW-4YWX6-6F4BT", "Retail", "CoreSingleLanguage", ""),
                ("YTMG3-N6DKC-DKB77-7M9GH-8HVX7", "Retail", "Core", ""),
                ("XKCNC-J26Q9-KFHD2-FKTHY-KD72Y", "OEM:NONSLP", "PPIPro", ""),
                ("YNMGQ-8RYV3-4PGQ3-C8XTP-7CFBY", "Retail", "Education", ""),
                ("84NGF-MHBT6-FXBX8-QWJK7-DRR8H", "Retail", "EducationN", ""),
                ("KCNVH-YKWX8-GJJB9-H9FDT-6F7W2", "Volume:MAK", "EnterpriseS_VB", ""),
                ("43TBQ-NH92J-XKTM7-KT3KK-P39PB", "OEM:NONSLP", "EnterpriseS_RS5", ""),
                ("NK96Y-D9CD8-W44CQ-R8YTK-DYJWX", "OEM:NONSLP", "EnterpriseS_RS1", ""),
                ("FWN7H-PF93Q-4GGP8-M8RF3-MDWWW", "OEM:NONSLP", "EnterpriseS_TH", ""),
                ("RQFNW-9TPM3-JQ73T-QV4VQ-DV9PT", "Volume:MAK", "EnterpriseSN_VB", ""),
                ("M33WV-NHY3C-R7FPM-BQGPT-239PG", "Volume:MAK", "EnterpriseSN_RS5", ""),
                ("2DBW3-N2PJG-MVHW3-G7TDK-9HKR4", "Volume:MAK", "EnterpriseSN_RS1", ""),
                ("NTX6B-BRYC2-K6786-F6MVQ-M7V2X", "Volume:MAK", "EnterpriseSN_TH", ""),
                ("G3KNM-CHG6T-R36X3-9QDG6-8M8K9", "Retail", "ProfessionalSingleLanguage", ""),
                ("HNGCC-Y38KG-QVK8D-WMWRK-X86VK", "Retail", "ProfessionalCountrySpecific", ""),
                ("DXG7C-N36C4-C4HTG-X4T3X-2YV77", "Retail", "ProfessionalWorkstation", ""),
                ("WYPNQ-8C467-V2W6J-TX4WX-WT2RQ", "Retail", "ProfessionalWorkstationN", ""),
                ("8PTT6-RNW4C-6V7J2-C2D3X-MHBPB", "Retail", "ProfessionalEducation", ""),
                ("GJTYN-HDMQY-FRR76-HVGC7-QPF8P", "Retail", "ProfessionalEducationN", ""),
                ("C4NTJ-CX6Q2-VXDMR-XVKGM-F9DJC", "Volume:MAK", "EnterpriseG", ""),
                ("46PN6-R9BK9-CVHKB-HWQ9V-MBJY8", "Volume:MAK", "EnterpriseGN", ""),
                ("NJCF7-PW8QT-3324D-688JX-2YV66", "Retail", "ServerRdsh", ""),
                ("XQQYW-NFFMW-XJPBH-K8732-CKFFD", "OEM:DM", "IoTEnterprise", ""),
                ("QPM6N-7J2WJ-P88HH-P3YRH-YY74H", "OEM:NONSLP", "IoTEnterpriseS", ""),
                ("K9VKN-3BGWV-Y624W-MCRMQ-BHDCD", "Retail", "CloudEditionN", ""),
                ("KY7PN-VR6RX-83W6Y-6DDYQ-T6R4W", "Retail", "CloudEdition", ""),
                ("V3WVW-N2PV2-CGWC3-34QGF-VMJ2C", "Retail", "Cloud", ""),
                ("NH9J3-68WK7-6FB93-4K3DF-DJ4F6", "Retail", "CloudN", ""),
                ("2HN6V-HGTM8-6C97C-RK67V-JQPFD", "Retail", "CloudE", ""),
                ("WC2BQ-8NRM3-FDDYY-2BFGV-KHKQY", "Volume:GVLK", "ServerStandard", "RS1"),
                ("CB7KF-BWN84-R7R2Y-793K2-8XDDG", "Volume:GVLK", "ServerDatacenter", "RS1"),
                ("JCKRF-N37P4-C2D82-9YXRT-4M63B", "Volume:GVLK", "ServerSolution", "RS1"),
                ("QN4C6-GBJD2-FB422-GHWJK-GJG2R", "Volume:GVLK", "ServerCloudStorage", "RS1"),
                ("VP34G-4NPPG-79JTQ-864T4-R3MQX", "Volume:GVLK", "ServerAzureCor", "RS1"),
                ("9JQNQ-V8HQ6-PKB8H-GGHRY-R62H6", "Retail", "ServerAzureNano", "RS1"),
                ("VN8D3-PR82H-DB6BJ-J9P4M-92F6J", "Retail", "ServerStorageStandard", "RS1"),
                ("48TQX-NVK3R-D8QR3-GTHHM-8FHXC", "Retail", "ServerStorageWorkgroup", "RS1"),
                ("2HXDN-KRXHB-GPYC7-YCKFJ-7FVDG", "Volume:GVLK", "ServerDatacenterACor", "RS3"),
                ("PTXN8-JFHJM-4WC78-MPCBR-9W4KR", "Volume:GVLK", "ServerStandardACor", "RS3"),
            };
            // 遍历数据以匹配目标 Edition
            foreach (var (key, keyType, editionID, versionName) in productKeyData)
            {
                if (string.Equals(edition, editionID, StringComparison.OrdinalIgnoreCase))
                {
                    return key; // 返回匹配的密钥
                }
            }
            // 如果未找到匹配项，返回空字符串或抛出异常
            MessageBox.Show($"No product key found for edition: {edition}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return string.Empty;
        }

        #endregion
    }
}