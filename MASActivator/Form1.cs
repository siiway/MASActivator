using System;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace MASActivator
{
    public partial class MainForm : Form
    {

        #region Application Global ENVs
        bool debug = false;
        #endregion

        #region Application Startup Init
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
                // ���û�й���ԱȨ�ޣ�����������Ȩ��
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
            // ��������Ӧ�ó����Թ���Ա���
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                UseShellExecute = true,
                Verb = "runas"  // �������ԱȨ��
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                // �����쳣�������û��ܾ�����Ȩ��
                MessageBox.Show("Unable to restart as Admin: \n" + ex.Message, "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Environment.Exit(0); // �˳���ǰʵ��
        }

        #endregion

        #region Mainform Design
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

        #region Pages Setup
        private void setupOtherPage()
        {
            setupWindowsEditionsComboBox();
        }

        #endregion

        #region Get Windows Target Editions

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

        #endregion

        #region Change Windows Editions
        private bool ConfirmEditionChange(string targetEdition)
        {
            DialogResult result = MessageBox.Show(
                $"Changing the current edition to [{targetEdition}].\n\nImportant: Save your work before continuing. The system will auto-restart.",
                "MASA - Change Edition",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            return result == DialogResult.OK;
        }


        private void PrepareForEditionChange()
        {
            try
            {
                // ֹͣ TrustedInstaller ����
                ServiceController service = new ServiceController("TrustedInstaller");
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }

                // ���ݲ�������־
                string cbsLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "logs", "cbs", "cbs.log");
                string dismLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "logs", "DISM", "dism.log");

                string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ChangeEdition_Logs");
                Directory.CreateDirectory(backupFolder);

                File.Copy(cbsLogPath, Path.Combine(backupFolder, $"cbs_backup_{DateTime.Now:yyyyMMdd_HHmmss}.log"), true);
                File.Copy(dismLogPath, Path.Combine(backupFolder, $"dism_backup_{DateTime.Now:yyyyMMdd_HHmmss}.log"), true);

                File.Delete(cbsLogPath);
                File.Delete(dismLogPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during preparation: {ex.Message}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        private void RestoreTrustedInstallerService()
        {
            try
            {
                // ���� TrustedInstaller ����
                ServiceController service = new ServiceController("TrustedInstaller");
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring TrustedInstaller service: {ex.Message}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void changeWindowsEdition(string edition)
        {
            if (edition == "Select Windows Edition")
            {
                MessageBox.Show("Please select a valid Windows edition.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (debug)
            {
                // �������ҵ���������汾��ʹ�� CBS XML ����
                PrepareForEditionChange();
                changeWindowsEditionUsingCBS(edition);
                RestoreTrustedInstallerService();
                return;
            }
            else
            {
                // DISM
                changeWindowsEditionUsingDISM(edition, getProductKey(edition));
            }
        }

        private string getProductKey(string edition)
        {
            // �����Ʒ��Կ����
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
            // ����������ƥ��Ŀ�� Edition
            foreach (var (key, keyType, editionID, versionName) in productKeyData)
            {
                if (string.Equals(edition, editionID, StringComparison.OrdinalIgnoreCase))
                {
                    return key; // ����ƥ�����Կ
                }
            }
            // ���δ�ҵ�ƥ������ؿ��ַ������׳��쳣
            MessageBox.Show($"No product key found for edition: {edition}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return string.Empty;
        }

        private void btnChangeWindowsEdition_Click(object sender, EventArgs e)
        {
            // ��ȡѡ�е� Windows �汾
            string selectedEdition = comboBoxWindowsEditions.SelectedItem.ToString();
            if (selectedEdition != null)
            {
                // ���÷������� Windows �汾
                changeWindowsEdition(selectedEdition);
            }
            else
            {
                MessageBox.Show("Selected Windows Edition is NULL.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void changeWindowsEditionUsingDISM(string targetEdition, string productKey)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c dism /online /Set-Edition:{targetEdition} /ProductKey:{productKey} /AcceptEula",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            MessageBox.Show("Windows edition changed successfully. The system will now restart.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start("shutdown", "/r /t 0");
                        }
                        else
                        {
                            MessageBox.Show($"DISM command failed with error: {error}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to start DISM process.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void changeWindowsEditionUsingCBS(string targetEdition)
        {
            try
            {
                // ��ʾ�û����湤����ȷ�ϼ���
                DialogResult result = MessageBox.Show(
                    $"Changing the current edition to [{targetEdition}].\n\nImportant: Save your work before continuing. The system will auto-restart.",
                    "MASA - Change Edition",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                {
                    MessageBox.Show("Operation cancelled by the user.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ������ʱ CBS XML �ļ�
                string tempCbsScriptPath = CreateTemporaryCbsXml(targetEdition);

                if (string.IsNullOrEmpty(tempCbsScriptPath))
                {
                    MessageBox.Show("Failed to create CBS XML file.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ִ�� CBS �ű�
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "DISM.exe",
                    Arguments = $"/online /Apply-CustomDataFile:{tempCbsScriptPath}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        int exitCode = process.ExitCode;
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            MessageBox.Show("Windows edition changed successfully. The system will now restart.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start("shutdown", "/r /t 0");
                        }
                        else
                        {
                            MessageBox.Show($"CBS script execution failed with exit code: {exitCode}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to start CBS script process.", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CreateTemporaryCbsXml(string targetEdition)
        {
            try
            {
                // ���� CBS XML ����
                string cbsXmlContent = $@"<?xml version=""1.0"" encoding=""utf-8""?>
        <Deployment xmlns=""http://schemas.microsoft.com/embedded/2004/10/ImageUpdate"" Name=""SetEdition"" Description=""Set Windows Edition"">
          <Run>
            <SetEdition Name=""{targetEdition}"" />
          </Run>
        </Deployment>";

                // ������ʱ�ļ�·��
                string tempCbsScriptPath = Path.Combine(Path.GetTempPath(), "SetEdition.cbs.xml");

                // д���ļ�
                File.WriteAllText(tempCbsScriptPath, cbsXmlContent);

                return tempCbsScriptPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating CBS XML file: {ex.Message}", "MASA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion
    }
}