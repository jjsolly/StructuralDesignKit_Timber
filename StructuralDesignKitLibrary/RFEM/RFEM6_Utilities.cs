using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlubal.WS.Rfem6.Application;
using Dlubal.WS.Rfem6.Model;
using System.IO;
using System.ServiceModel;
using static System.Net.Mime.MediaTypeNames;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
using Dlubal.RFEM5;
using System.Net;
using System.ServiceModel.Channels;

namespace StructuralDesignKitLibrary.RFEM
{
	public class RFEM6_Utilities : IRFEM_Utilities_Interface<ModelClient>
	{


		/// <summary>
		/// Address to access the RFEM server
		/// 
		/// The EndpointAddress class is part of Windows Communication Foundation (WCF) and is used to specify the address of a web service or network service
		/// It is commonly used in WCF clients to define where a service is hosted.
		/// The default value is http://localhost:8081, which suggests that a local web service or API is expected to be running on port 8081.
		/// </summary>
		public static EndpointAddress Address { get; set; } = new EndpointAddress("http://localhost:8081");



		/// <summary>
		/// In Windows Communication Foundation (WCF), a binding defines how a client and a service communicate
		/// </summary>

		private static BasicHttpBinding Binding
		{
			get
			{
				BasicHttpBinding binding = new BasicHttpBinding
				{
					// Send timeout is set to 180 seconds.
					SendTimeout = new TimeSpan(0, 0, 180),
					UseDefaultWebProxy = true,
					//Limiting the return value to 1 GByte
					MaxReceivedMessageSize = 1000000000,
				};

				return binding;
			}
		}

		//private static RfemApplicationClient application = null;
		private static ApplicationClient application = null;


		public ApplicationClient GetRFEMApplication()
		{

			string CurrentDirectory = Directory.GetCurrentDirectory();

			ModelClient model = null;
			try
			{
				#region Application Settings

				application_information ApplicationInfo;

				try
				{
					// connects to RFEM6 or RSTAB9 application
					application = new ApplicationClient(Binding, Address);

				}
				catch (Exception exception)
				{
					if (application != null)
					{
						if (application.State != CommunicationState.Faulted)
						{
							application.Close();
						}
						else
						{
							application.Abort();
						}

						application = null;
					}
				}
				#endregion

			}

			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}

			return application;
		}




		public void CloseRFEMModel(ModelClient model)
		{
			model.Close();
		}

		public ModelClient GetActiveModel()
		{

			var RFEMApp = GetRFEMApplication();
			var modelUrl = RFEMApp.get_active_model();

			ModelClient model = new ModelClient(Binding, new EndpointAddress(modelUrl));

			return model;
		}

		public List<ModeShape> GetAllStandardizedDisplacement(ModelClient model, int NVC)
		{
			throw new NotImplementedException();
		}

		public List<FeMeshNode> GetFENodes(ModelClient model, int NVC)
		{
			throw new NotImplementedException();
		}

		public List<double> GetModalMasses(ModelClient model, int NVC)
		{
			throw new NotImplementedException();
		}

		public List<double> GetNaturalFrequencies(ModelClient model, int NVC)
		{
			throw new NotImplementedException();
		}


		public ModelClient OpenModel()
		{
			throw new NotImplementedException();
		}


	}


}
