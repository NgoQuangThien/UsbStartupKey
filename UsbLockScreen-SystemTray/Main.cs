using Microsoft.Win32;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace UsbStartupKey_SystemTray
{
    public partial class Main : Form
    {
        internal static class UsbNotification
        {
            public const int DbtDevicearrival = 0x8000; // system detected a new device        
            public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
            public const int WmDevicechange = 0x0219; // device change event      
            private const int DbtDevtypDeviceinterface = 5;
            private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices

            /// <summary>
            /// Registers a window to receive notifications when USB devices are plugged or unplugged.
            /// </summary>
            /// <param name="windowHandle">Handle to the window receiving notifications.</param>
            public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
            {
                DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
                {
                    DeviceType = DbtDevtypDeviceinterface,
                    Reserved = 0,
                    ClassGuid = GuidDevinterfaceUSBDevice,
                    Name = 0
                };

                dbi.Size = Marshal.SizeOf(dbi);
                IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
                Marshal.StructureToPtr(dbi, buffer, true);
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct DevBroadcastDeviceinterface
            {
                internal int Size;
                internal int DeviceType;
                internal int Reserved;
                internal Guid ClassGuid;
                internal short Name;
            }
        }

        public class User
        {
            public String Pin = @"";
            public String Serial = @"";
        }

        public User TokenUser = new User();
        public bool UsbValid = false;

        [DllImport("user32")]
        public static extern void LockWorkStation();

        // Specify the path to unmanaged PKCS#11 library provided by the cryptographic device vendor
        public string pkcs11LibraryPath = @"C:\Windows\System32\BkavCAv2S.dll";
        // Create factories used by Pkcs11Interop library
        public Pkcs11InteropFactories factories = new Pkcs11InteropFactories();

        public IPkcs11Library pkcs11Library;

        public Main()
        {
            InitializeComponent();

            LoadUnmanagedPKCS11();

            this.notifyIcon_main.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.notifyIcon_main.ContextMenuStrip.Items.Add("Open", null, this.menuItem_Open_Click);
            this.notifyIcon_main.ContextMenuStrip.Items.Add("Minimize to Tray", null, this.menuItem_Minimize2Tray_Click);
            this.notifyIcon_main.ContextMenuStrip.Items.Add("Quit", null, this.menuItem_Quit_Click);

            //UsbNotification.RegisterUsbDeviceNotification(this.Handle);

            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        private void LoadUnmanagedPKCS11()
        {
            // Load unmanaged PKCS#11 library
            try
            {
                pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, pkcs11LibraryPath, AppType.MultiThreaded);
            }
            catch
            {
                MessageBox.Show("Cannot load PKCS#11 library", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(1);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                TokenUser.Pin = appSettings["Pin"];
                TokenUser.Serial = appSettings["Serial"];
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Cannot read AppSettings", "Error");
                Application.Exit();
            }

            if (!String.IsNullOrEmpty(TokenUser.Pin) && !String.IsNullOrEmpty(TokenUser.Serial))
            {
                textBox_pin.Text = TokenUser.Pin;
                textBox_serial.Text = TokenUser.Serial;

                Check_Usb_Device();
                LockIfInvalid();
            }
        }

        private void LockIfInvalid()
        {
            if (!UsbValid)
                LockWorkStation();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {

            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                if (!UsbValid)
                {
                    Thread UnlockThread = new Thread(new ThreadStart(Check_Usb_Device));
                    UnlockThread.IsBackground = true;
                    UnlockThread.Start();

                    LockIfInvalid();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
                // USB Added
                if ((int)m.WParam == UsbNotification.DbtDevicearrival)
                {
                    if (!UsbValid)
                    {
                        Thread.Sleep(500);
                        Thread AddedThread = new Thread(new ThreadStart(Check_Usb_Device));
                        AddedThread.IsBackground = true;
                        AddedThread.Start();
                    }
                }
                // USB Removed
                else if ((int)m.WParam == UsbNotification.DbtDeviceremovecomplete)
                {
                    if (UsbValid)
                    {
                        Thread RemovedThread = new Thread(new ThreadStart(Check_Usb_Device));
                        RemovedThread.IsBackground = true;
                        RemovedThread.Start();
                        RemovedThread.Join();

                        LockIfInvalid();
                    }
                }
            }
        }

        public void Check_Usb_Device()
        {
            bool TempValid = false;
            if (String.IsNullOrEmpty(TokenUser.Pin) && String.IsNullOrEmpty(TokenUser.Serial))
            {
                TempValid = true;
            }
            else
            {
                List<ISlot> slots = pkcs11Library.GetSlotList(SlotsType.WithOrWithoutTokenPresent);
                // Get list of all available slots
                foreach (ISlot slot in slots)
                {
                    // Show basic information about slot
                    ISlotInfo slotInfo = slot.GetSlotInfo();

                    if (slotInfo.SlotFlags.TokenPresent)
                    {
                        List<String> Serials = GetSerials(slot, TokenUser.Pin);
                        foreach (String Serial in Serials)
                        {
                            if (Serial.Equals(TokenUser.Serial))
                            {
                                TempValid = true;
                                notifyIcon_main.ShowBalloonTip(1000, notifyIcon_main.BalloonTipTitle, "USB is valid", ToolTipIcon.Info);
                            }
                        }
                    }
                }
            }
            UsbValid = TempValid;
        }

        public static List<String> GetSerials(ISlot slot, String UserPin)
        {
            List<String> serials = new List<String>();
            using (ISession session = slot.OpenSession(SessionType.ReadOnly))
            {
                try
                {
                    session.Login(CKU.CKU_USER, UserPin);

                    // Prepare attribute template that defines search criteria
                    List<IObjectAttribute> objectAttributes = new List<IObjectAttribute>();
                    //objectAttributes.Add(session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_DATA));
                    objectAttributes.Add(session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true));
                    objectAttributes.Add(session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509));

                    // Find all objects that match provided attributes
                    List<IObjectHandle> foundObjects = session.FindAllObjects(objectAttributes);

                    // Prepare list of empty attributes we want to read
                    List<CKA> attributes = new List<CKA>();
                    attributes.Add(CKA.CKA_SERIAL_NUMBER);

                    foreach (IObjectHandle object_result in foundObjects)
                    {
                        // Get value of specified attributes
                        List<IObjectAttribute> objectAttributes_result = session.GetAttributeValue(object_result, attributes);
                        foreach (IObjectAttribute objectAttribute in objectAttributes_result)
                        {
                            serials.Add(BitConverter.ToString(objectAttribute.GetValueAsByteArray()).Replace("-", ""));
                        }
                    }

                    session.Logout();
                }
                catch
                {
                    return serials;
                }
            }
            return serials;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox_pin.Text) || String.IsNullOrEmpty(textBox_serial.Text))
                return;

            DialogResult dialogResult = MessageBox.Show("Do you want to save changes?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                TokenUser.Pin = textBox_pin.Text;
                TokenUser.Serial = textBox_serial.Text;

                Check_Usb_Device();
                if (UsbValid)
                {
                    try
                    {
                        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        var settings = configFile.AppSettings.Settings;
                        if (settings["Pin"] == null)
                            settings.Add("Pin", TokenUser.Pin);
                        else
                            settings["Pin"].Value = TokenUser.Pin;
                        if (settings["Serial"] == null)
                            settings.Add("Serial", TokenUser.Serial);
                        else
                            settings["Serial"].Value = TokenUser.Serial;

                        configFile.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                        MessageBox.Show("USB is valid. Please restart the application!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        System.Environment.Exit(1);
                    }
                    catch (ConfigurationErrorsException)
                    {
                        MessageBox.Show("Error writing app settings", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("USB is invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox_pin.Clear();
                    textBox_serial.Clear();

                    TokenUser.Pin = textBox_pin.Text;
                    TokenUser.Serial = textBox_serial.Text;
                }

            }
        }

        private void menuItem_Open_Click(object Sender, EventArgs e)
        {
            this.Activate();
            this.Show();
        }
        private void menuItem_Minimize2Tray_Click(object Sender, EventArgs e)
        {
            this.Hide();
        }
        private void menuItem_Quit_Click(object Sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
