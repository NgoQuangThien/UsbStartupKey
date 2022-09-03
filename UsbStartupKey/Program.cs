using Microsoft.Win32;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;

namespace UsbStartupKey
{
    internal class Program
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
            public static void RegisterUsbDeviceNotification()
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
            public String Pin = @"18032000";
            public String Serial = @"021054033C77529C283D793624F0B8AF78E7";
        }

        public static bool UsbValid = false;

        [DllImport("user32")]
        public static extern void LockWorkStation();

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

        public static void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {

            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {

                if (!UsbValid)
                    Console.WriteLine("Locking");
                else
                    Console.WriteLine("Unlocking");
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
                if ((int)m.WParam == UsbNotification.DbtDevicearrival)
                {
                    if (!UsbValid)
                    {
                        Thread ValidateThread = new Thread(new ThreadStart(Check_Usb_Device));
                        ValidateThread.IsBackground = true;
                        ValidateThread.Start();
                    }
                }
                else if ((int)m.WParam == UsbNotification.DbtDeviceremovecomplete)
                {
                    if (UsbValid)
                    {
                        Thread ValidateThread = new Thread(new ThreadStart(LockIfInvalid));
                        ValidateThread.IsBackground = true;
                        ValidateThread.Start();
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            User ThienNQ = new User();

            UsbNotification.RegisterUsbDeviceNotification();

            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            if (args.Length != 0)
            {
                foreach (string arg in args)
                {
                    try
                    {
                        String[] variable = arg.Split('=');
                        switch (variable[0])
                        {
                            case "--pin":
                                ThienNQ.Pin = variable[1];
                                break;
                            case "--serial":
                                ThienNQ.Serial = variable[1];
                                break;
                            default:
                                break;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }

            // Specify the path to unmanaged PKCS#11 library provided by the cryptographic device vendor
            string pkcs11LibraryPath = @"C:\Windows\System32\BkavCAv2S.dll";

            // Create factories used by Pkcs11Interop library
            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            // Load unmanaged PKCS#11 library
            IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, pkcs11LibraryPath, AppType.MultiThreaded);

            while (true)
            {
                List<ISlot> slots = pkcs11Library.GetSlotList(SlotsType.WithOrWithoutTokenPresent);
                // Get list of all available slots
                foreach (ISlot slot in slots)
                {
                    // Show basic information about slot
                    ISlotInfo slotInfo = slot.GetSlotInfo();

                    if (slotInfo.SlotFlags.TokenPresent)
                    {
                        List<String> Serials = GetSerials(slot, "18032000");
                        foreach (String Serial in Serials)
                        {
                            Console.WriteLine(Serial);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
