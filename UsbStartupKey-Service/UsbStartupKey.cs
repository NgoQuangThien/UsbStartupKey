using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;

namespace UsbStartupKey_Service
{
    public partial class UsbStartupKey : ServiceBase
    {
        public class User
        {
            public String Pin = @"18032000";
            public String Serial = @"021054033C77529C283D793624F0B8AF78E7";
        }

        [DllImport("user32")]
        public static extern void LockWorkStation();

        public User ThienNQ = new User();
        public bool IsLock = true;

        // Specify the path to unmanaged PKCS#11 library provided by the cryptographic device vendor
        public string pkcs11LibraryPath = @"C:\Windows\System32\BkavCAv2S.dll";
        // Create factories used by Pkcs11Interop library
        public Pkcs11InteropFactories factories = new Pkcs11InteropFactories();

        public UsbStartupKey(string[] args)
        {
            InitializeComponent();

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
        }

        protected override void OnStart(string[] args)
        {
            // Set up a timer that triggers every minute.
            Timer timer = new Timer();
            timer.Interval = 3000; // 3 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
        }

        public static List<String> GetSerials(ISlot slot, String UserPin)
        {
            List<String> serials = new List<String>();
            using (ISession session = slot.OpenSession(SessionType.ReadOnly))
            {
                try
                {
                    session.Login(CKU.CKU_USER, UserPin);
                }
                catch
                {
                    return serials;
                }

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
            return serials;
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // Load unmanaged PKCS#11 library
            using (IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, pkcs11LibraryPath, AppType.MultiThreaded))
            {
                List<ISlot> slots = pkcs11Library.GetSlotList(SlotsType.WithOrWithoutTokenPresent);

                // Get list of all available slots
                foreach (ISlot slot in pkcs11Library.GetSlotList(SlotsType.WithOrWithoutTokenPresent))
                {
                    // Show basic information about slot
                    ISlotInfo slotInfo = slot.GetSlotInfo();

                    if (slotInfo.SlotFlags.TokenPresent)
                    {
                        List<String> Serials = GetSerials(slot, ThienNQ.Pin);
                        foreach (String Serial in Serials)
                        {
                            if (Serial.Equals(ThienNQ.Serial))
                                IsLock = false;
                        }
                    }
                }
            }
            if (IsLock)
                LockWorkStation();
            else
                IsLock = true;
        }
    }
}
