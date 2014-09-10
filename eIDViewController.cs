using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FreedelitySmartCard;
using System.Threading;


namespace eID
{
	public partial class eIDViewController : UIViewController
    {
        FIDSmartCardReader eID;
        UIAlertView msg;
		FIDStatusCode status;
		FIDStatusCode statusSign;
		NSMutableData Identity;
		NSMutableData Adresse,digest;
		NSData nsTexte;
		Thread thr;

        public eIDViewController () : base ("eIDViewController",null)
        {
        }
        
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
            
            // Release any cached data, images, etc that aren't in use.
        }
        
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

			try
{
			eID = new FIDSmartCardReader();
			}
			catch
			{

			}



        }


        partial void btLecture (MonoTouch.UIKit.UIButton sender)
		{
			LectureEID();
		}

        partial void btSignature (MonoTouch.UIKit.UIButton sender)
        {
			thr = new Thread( SignatureEID);

			thr.Start();

            //SignatureEID();
        }

		void LectureEID()
        {
          
            FIDSmartCard card;
			FIDBelgianEidCard beid;


			string strNom = "";
			string strAdresse = "";
			Identity = new NSMutableData();
			Adresse = new NSMutableData();


      

            if (eID != null)
            {
				status = eID.Open();

				if (status == FIDStatusCode.OK)
				{

					card = new FIDSmartCard();

					card = eID.GetCard(status,true);

					if (status == FIDStatusCode.OK)
					{
						beid = new FIDBelgianEidCard(card);

						beid.ReadIdentityFile(Identity);

						beid.ReadAddressFile(Adresse);

						strNom = FIDBelgianEidCard.GetLastNameFromID((NSData)Identity);

						strAdresse = FIDBelgianEidCard.GetStreetAndNumberFromAddress((NSData)Adresse) + " " +  FIDBelgianEidCard.GetMunicipalityFromAddress((NSData)Adresse);

						msg = new UIAlertView() { Title = "MEDINECT2020", Message = strNom + " " + strAdresse };
	                    msg.AddButton("OK");
	                        
	                           
	                  	 msg.Show();
					}

				}

				eID.Close();
        
            }

        }

        void SignatureEID()
        {
          
            FIDSmartCard card;
			FIDBelgianEidCard beid;
			FIDPinDialogDescription pinDialog;

			string strNom = "";
			string strAdresse = "";
			Identity = new NSMutableData();
			Adresse = new NSMutableData();
			digest = new NSMutableData();
			int intEssai = 0;
			NSData strSignature;


            if (eID != null)
            {
				status = eID.Open();

				if (status == FIDStatusCode.OK)
				{
					statusSign = FIDStatusCode.WRONG_PIN;

					card = new FIDSmartCard();

					card = eID.GetCard(status,true);

					if (status == FIDStatusCode.OK)
					{
						beid = new FIDBelgianEidCard(card);

						nsTexte = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
						pinDialog = new FIDPinDialogDescription();

						pinDialog.Lang = FIDPinDialogLang.French;
						pinDialog.PinMaxLength = 4;
						pinDialog.PinMinLength = 4;
						//pinDialog.Controller = this;

						strSignature = beid.SignData(nsTexte,FIDHashMethod.SHA1, ref statusSign, ref intEssai,digest,pinDialog);

						InvokeOnMainThread (delegate {

							if (statusSign == FIDStatusCode.OK)
							{
								msg = new UIAlertView() { Title = "MEDINECT", Message = strSignature.ToString() };
	                    		msg.AddButton("OK");
	                        
	                           
	                  			 msg.Show();
							}
							else
							{
								msg = new UIAlertView() { Title = "MEDINECT", Message = "Erreur : " + intEssai };
	                    		msg.AddButton("OK");
	                        
	                           
	                  			 msg.Show();
							}
						});
					}

				}

				eID.Close();
        
            }

        }

     

//		public class eIDEvent : eidEventsDelegate
//        {
//            UIAlertView msg;
//		
//
//            public override void EIDeVent (string type, string Message)
//            {
//                base.EIDeVent (type, Message);
//
//                if (type == "SUCCESS")
//                {
//                    //msg = new UIAlertView() { Title = "MEDINECT", Message = eID.GetSignature().ToString() };
//                    msg.AddButton("OK");
//                        
//                            
//                    msg.Show();
//                }
//
//                if (type == "FAILURE")
//                {
//                    msg = new UIAlertView() { Title = "MEDINECT", Message = "Problème de signature" };
//                    msg.AddButton("OK");
//                        
//                            
//                    msg.Show();
//                }
//
//                if (type == "BAD_PIN")
//                {
//                    msg = new UIAlertView() { Title = "MEDINECT", Message = "Mauvais code PIN" };
//                    msg.AddButton("OK");
//                        
//                            
//                    msg.Show();
//                }
//            }
//
//		
//        }
//
//		public class eIDPIN : eidPinDelegate
//        {
//           	
//            public override bool UseDefaultDialog {
//                get {
//                    return true;
//                }
//            }
//
//            public override string DefaultDialogLang {
//                get {
//                    return "fr";
//                }
//            }
//
//            public override string AskPinCode {
//                get {
//                    return "";
//                }
//            }
//
//        }
         
    }

}

