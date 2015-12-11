using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kareoclientapp.KareoAPI;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;


namespace Kareoclientapp
{
    class Program
    {
        // below is the username/pass for TVN
         const string CUSTOMERKEY = "k83wm27de95x";

        const string USER = "tvneuro@accelsg.com";
        const string PASSWORD = "Accel$1115#";
       
        // below is the username/pass for IPI
       // const string CUSTOMERKEY = "s98tg42ja37o";

       // const string USER = "ipi@accelsg.com";
       // const string PASSWORD = "Accel$0715#";
        
        // below is the username/pass for sierra
      // const string CUSTOMERKEY = "c75ox84yn96j";
       
       //const string USER = "sierra@accelsg.com";
        //const string PASSWORD = "Accel$0515#";
        

        // Below is the customer key, username/pass for FWBSI
      // const string CUSTOMERKEY = "f65px74in98o";
       
        //const string USER = "kumarc@accelsg.com";
        
       // const string PASSWORD = "bobby50";

        //below is the customer key, username/pass for NASA
       // const string CUSTOMERKEY = "g42dn87qx53t";

       // const string USER = "nasa@accelsg.com";

       // const string PASSWORD = "Accel$0615#";

        public void GetAppointments()
        {
            KareoServicesClient consume = new KareoServicesClient();

            RequestHeader requestHeader = new RequestHeader();
            requestHeader.CustomerKey = CUSTOMERKEY;
            requestHeader.User = USER;
            requestHeader.Password = PASSWORD;
           


            // use filter to limit results 
            AppointmentFilter filt = new AppointmentFilter();
            PatientFilter filter = new PatientFilter();
           
            
        filt.StartDate = DateTime.Today.ToString();
         filt.EndDate = DateTime.Today.AddDays(1).ToString();
        // filt.StartDate = DateTime.Today.AddDays(-4).ToString();
         // filt.EndDate = DateTime.Today.AddDays(-4).ToString();
         // filt.PatientFullName = "BILL CRANFORD";
         // filt.ResourceName = "Provider Test";
            
           
            
            //filt.StartDate = DateTime.Now.ToString();
           // filt.PracticeName = "Fort Worth Brain & Spine Institute";
           // filt.ServiceLocationName = "Fort Worth Brain & Spine Institu";
           
           // filt.ServiceLocationName = "Keller North Fort Worth Clinic";
           // filt.ServiceLocationName = "Granbury";
         // filt.PatientFullName = "Swager William";
           // filt.ConfirmationStatus = "Rescheduled";
            
            
            
            
            // create request object
            try
            {
               GetAppointmentsReq requestapp = new GetAppointmentsReq();
                requestapp.RequestHeader = requestHeader;
                requestapp.Filter = filt;

                
                
                

              //create response object 
            GetAppointmentsResp responseapp = consume.GetAppointments(requestapp);
            //    GetPatientsResp responseapp = consume.GetPatients(requestapp);
            // This actually calls the Kareo Service passing in the parameters for the request 
            //GetPatientsResp response = consume.GetPatients(request);

            // Check the response for an error 
            if (responseapp.ErrorResponse.IsError)
            {
                System.Diagnostics.Trace.WriteLine(responseapp.ErrorResponse.ErrorMessage);
                using (StreamWriter w = File.AppendText("myListener.log"))
                {
                    Log(responseapp.ErrorResponse.ErrorMessage, w);
                    }

                using (StreamReader r = File.OpenText("myListener.log"))
                {
                   // DumpLog(r);
                }
            }

            else if (!responseapp.SecurityResponse.SecurityResultSuccess){
                System.Diagnostics.Trace.WriteLine(responseapp.SecurityResponse.SecurityResult);

                using (StreamWriter w = File.AppendText("myListener.log"))
        {
            Log(responseapp.SecurityResponse.SecurityResult, w);
            }

        using (StreamReader r = File.OpenText("myListener.log"))
        {
            //DumpLog(r);
        }
        }
            else
            {

                // There was no error print out the data to a folder
                foreach (AppointmentData p in responseapp.Appointments)
               // foreach(PatientData p in responseapp.Patients)
                {
                    
               
                    // check patientreq 
                    if (p.PatientFullName != "")
                    {

                        filter.FullName = p.PatientFullName;
                        GetPatientsReq requestapppatient = new GetPatientsReq();
                        requestapppatient.RequestHeader = requestHeader;
                        requestapppatient.Filter = filter;

                        GetPatientsResp responseapppatient = consume.GetPatients(requestapppatient);

                        if (responseapppatient.ErrorResponse.IsError)
                        {
                            System.Diagnostics.Trace.WriteLine(responseapppatient.ErrorResponse.ErrorMessage);
                            using (StreamWriter w = File.AppendText("myListener.log"))
                            {
                                Log(responseapppatient.ErrorResponse.ErrorMessage, w);
                            }

                            using (StreamReader r = File.OpenText("myListener.log"))
                            {
                                // DumpLog(r);
                            }
                        }

                        else if (!responseapppatient.SecurityResponse.SecurityResultSuccess)
                        {
                            System.Diagnostics.Trace.WriteLine(responseapppatient.SecurityResponse.SecurityResult);

                            using (StreamWriter w = File.AppendText("myListener.log"))
                            {
                                Log(responseapppatient.SecurityResponse.SecurityResult, w);
                            }

                            using (StreamReader r = File.OpenText("myListener.log"))
                            {
                                //DumpLog(r);
                            }
                        }
                        else
                        {
                            foreach (PatientData pa in responseapppatient.Patients)
                            {
                                XmlSerializer serializer1 = new XmlSerializer(pa.GetType());
                                string test = pa.MedicalRecordNumber.ToString();
                                Random rng1 = new Random();
                                using (TextWriter writer1 = new StreamWriter(@"E:\kareo\TVN\" + GetUniqueKey(25) + ".xml"))
                                {
                                    serializer1.Serialize(writer1, pa);
                                }
                                // System.Console.WriteLine(p.PatientFullName);
                                XmlSerializer serializer = new XmlSerializer(p.GetType());
                                Random rng = new Random();
                                using (TextWriter writer = new StreamWriter(@"E:\kareo\TVN\" + GetUniqueKey(26) + ".xml"))
                                {
                                    p.PatientCaseID = pa.MedicalRecordNumber.ToString();
                                    serializer.Serialize(writer, p);
                                    // serializer1.Serialize(writer, pa);

                                }
                            }
                        }
                    }
                    
                }
            }   
                   
            }
            catch (Exception e)
            {
                using (StreamWriter w = File.AppendText("myListener.log"))
                {
                    Log(e.Message, w);
                }
            }
           // System.Console.ReadLine();
        }
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }

        static void Main(string[] args)
        {
         //   System.Net.ServicePointManager.ServerCertificateValidationCallback =
          //      ((sender, certificate, chain, sslPolicyErrors) => true);

            Program bind = new Program();
            bind.GetAppointments();
           //System.Console.ReadLine();
        }
    }
}
