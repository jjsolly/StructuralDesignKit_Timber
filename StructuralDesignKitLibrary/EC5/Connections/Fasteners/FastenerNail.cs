using Dlubal.WS.Rfem6.Model;
using StructuralDesignKitLibrary.Connections.Interface;
using StructuralDesignKitLibrary.Connections.SteelTimberShear;
using StructuralDesignKitLibrary.Connections.TimberTimberShear;
using StructuralDesignKitLibrary.EC5;
using StructuralDesignKitLibrary.EC5.Connections.Interface;
using StructuralDesignKitLibrary.Materials;
using StructuralDesignKitLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StructuralDesignKitLibrary.EC5.EC5_Utilities;

namespace StructuralDesignKitLibrary.Connections.Fasteners
{
	/// <summary>
	/// Nail fastener according to EN 1995-1-1 §8.3
	/// </summary>
	public class FastenerNail : IFastener
	{

		#region Properties
		public EC5_Utilities.FastenerType Type { get; }
		public NailTypes NailType { get; }
		public double Diameter { get; set; }
		public double DiamHead { get; set; }
		public double Fuk { get; set; }
		public double MyRk { get; set; }
		public double Fhk { get; set; }
		public double FaxRk { get; set; }
		public double MaxJohansenPart { get; set; }
		public double a1min { get; set; }
		public double a2min { get; set; }
		public double a3tmin { get; set; }
		public double a3cmin { get; set; }
		public double a4tmin { get; set; }
		public double a4cmin { get; set; }
		public double K90 { get; set; }
		public bool Predrilled { get; set; }
		public int Length { get; set; }
		public double FHeadK { get; set; }
		public double Fax { get; set; }
		public double ThreadLength { get; set; }
		public double NailTensileCapacity { get; set; }


		#endregion


		#region constructor
		public FastenerNail(NailTypes nailType, double diameter, int length, double fuk, bool predrilled, double myrk = 0, double dHead = 0, double fax = 0, double fHead = 0, double threadLength = 0, double steelCapacity = 0)
		{
			Type = EC5_Utilities.FastenerType.Nail;
			if (diameter > 6 && !predrilled) throw new Exception("Nail with a diameter > 6mm needs to be predrilled");
			if (fuk < 600) throw new Exception("Fuk must ve above 600N/mm for nail wires");
			else Fuk = fuk;

			if (dHead > 0) DiamHead = dHead;
			//if not provided, the head diameter is considered to be 1.5x the shaft diameter
			else DiamHead = diameter * 1.5;

			Diameter = diameter;
			Predrilled = predrilled;
			Length = length;


			FHeadK = fHead;
			Fax = fax;
			ThreadLength = threadLength;
			NailTensileCapacity = steelCapacity;

			NailType = nailType;


			switch (NailType)
			{
				case NailTypes.Round:
					MaxJohansenPart = 0; //Conservatively, we set this value to 0, even if the EC5 allows 15%
					if (myrk == 0) MyRk = 0.3 * Fuk * Math.Pow(Diameter, 2.6); //EN 1995-1-1 Eq (8.14)
					else MyRk = myrk;

					break;
				case NailTypes.Squarred_Grooved:
					MaxJohansenPart = 0; //Conservatively, we set this value to 0, even if the EC5 allows 25%
					if (myrk == 0) MyRk = 0.45 * Fuk * Math.Pow(Diameter, 2.6); //EN 1995-1-1 Eq (8.14)
					else MyRk = myrk;

					break;
				case NailTypes.AnnularRing:
					MaxJohansenPart = 0.5; //Other nails according to EN 1995-1-1 §8.2.2 (2)
					if (myrk == 0) MyRk = 0.3 * Fuk * Math.Pow(Diameter, 2.6); //EN 1995-1-1 Eq (8.14)
					else MyRk = myrk;

					break;
				case NailTypes.Twisted:
					MaxJohansenPart = 0.5; //Other nails according to EN 1995-1-1 §8.2.2 (2)
					if (myrk == 0) MyRk = 0.3 * Fuk * Math.Pow(Diameter, 2.6); //EN 1995-1-1 Eq (8.14)
					else MyRk = myrk;

					break;

				default:
					break;
			}

		}
		#endregion

		//Spacings according to EN 1995-1-1 Table 8.4
		#region define spacings

		/// <summary>
		/// Define the minimum spacing to alongside the grain in mm
		/// </summary>
		/// <param name="angle">angle to grain in Degree</param>
		/// <returns></returns>
		[Description("Define the minimum spacing to alongside the grain in mm")]
		private double DefineA1Min(double angle)
		{
			double AngleRad = angle * Math.PI / 180;
			return (4 + Math.Abs(Math.Cos(AngleRad))) * Diameter;
		}

		/// <summary>
		/// Define the minimum spacing perpendicular to grain in mm
		/// </summary>
		/// <param name="angle">angle to grain in Degree</param>
		/// <returns></returns>
		[Description("Define the minimum spacing perpendicular to grain in mm")]
		private double DefineA2Min(double angle)
		{
			return 4 * Diameter;
		}


		/// <summary>
		/// Define the Minimum spacing to loaded end in mm
		/// </summary>
		/// <param name="angle">angle to grain in Degree</param>
		/// <returns></returns>
		[Description("Define the Minimum spacing to loaded end in mm")]
		private double DefineA3tMin(double angle)
		{
			return Math.Max(7 * Diameter, 80);
		}

		/// <summary>
		/// Define the Minimum spacing to unloaded end in mm
		/// </summary>
		/// <param name="angle">angle to grain in Degree</param>
		/// <returns></returns>
		[Description("Define the Minimum spacing to unloaded end in mm")]
		private double DefineA3cMin(double angle)
		{
			double AngleRad = angle * Math.PI / 180;
			if (angle <= 150 && angle < 210) return 4 * Diameter;
			else return (1 + 6 * Math.Sin(AngleRad)) * Diameter;
		}

		/// <summary>
		/// Define the Minimum spacing to loaded edge in mm
		/// </summary>
		/// <param name="angle">angle to grain in Degree</param>
		/// <returns></returns>
		[Description("Define the Minimum spacing to loaded edge in mm")]
		private double DefineA4tMin(double angle)
		{
			double AngleRad = angle * Math.PI / 180;
			return Math.Max((2 + 2 * Math.Sin(AngleRad)) * Diameter, 3 * Diameter);

		}

		/// <summary>
		/// Define the minimum spacing to unloaded edge in mm
		/// </summary>
		/// <returns></returns>
		[Description("Define the minimum spacing to unloaded edge in mm")]
		private double DefineA4cMin()
		{
			return 3 * Diameter;
		}

		#endregion


		#region Define minimum penetration length

		#endregion


		public void ComputeEmbedmentStrength(IMaterialTimber timber, double angle, double thickness = 0)
		{
			//LVL and Baubuche need special consideration with nails which are not yet implemented (03/01/2025)
			List<string> CoveredTimber = new List<string>() { "Softwood", "Hardwood", "Glulam", "OSB" };
			if (CoveredTimber.Contains(timber.Type.ToString()))
			{


				if (timber.Type == TimberType.Softwood || timber.Type == TimberType.Hardwood || timber.Type == TimberType.Glulam)
				{
					if (Diameter < 8)
					{
						if (Predrilled)
						{
							Fhk = 0.082 * (1 - 0.01 * Diameter) * timber.RhoK;
						}
						else
						{
							if (timber.RhoK > 500) throw new Exception("Nailed connection in wood with a characteristic density > 500kg/m³ needs to be pre-drilled");
							Fhk = 0.082 * timber.RhoK * Math.Pow(Diameter, -0.3);
						}
					}
					else
					{
						FastenerBolt EquivalentBolt = new FastenerBolt(Diameter, Fuk);
						EquivalentBolt.ComputeEmbedmentStrength(timber, angle);
						Fhk = EquivalentBolt.Fhk;

					}


				}
				else if (timber.Type == TimberType.OSB)
				{
					Fhk = 65 * Math.Pow(Diameter, -0.7) * Math.Pow(thickness, 0.1);
				}


			}
			else throw new Exception("Timber type not yet covered in the SDK");

		}

		/// <summary>
		/// Nail withdrawal capacity according to EN 1995-1-1 §8.3.3
		/// </summary>
		/// <param name="ConnectionType">Type of connection to consider</param>
		/// <param name="fax"></param>
		/// <param name="fHead"></param>
		/// <exception cref="Exception"></exception>
		public void ComputeWithdrawalStrength(IShearCapacity ConnectionType)
		{
			List<double> WithdrawalCapacity = new List<double>();

			if (NailTensileCapacity == 0) WithdrawalCapacity.Add(ComputeNailSteelTensileStrength());
			else WithdrawalCapacity.Add(NailTensileCapacity);

		

		
			double faxRk2 = FHeadK * Math.Pow(DiamHead, 2);

			double tPen;


	
			if (ConnectionType is TimberTimberSingleShear)
			{
				TimberTimberSingleShear connection = ConnectionType as TimberTimberSingleShear;
				if (connection.T2 > ThreadLength) tPen = ThreadLength;
				else tPen = connection.T2;
				WithdrawalCapacity.Add(Fax * Diameter * tPen);
				WithdrawalCapacity.Add(faxRk2);
			}

			else if (ConnectionType is TimberTimberDoubleShear)
			{
				TimberTimberDoubleShear connection = ConnectionType as TimberTimberDoubleShear;
				if (connection.T1 > ThreadLength) tPen = ThreadLength;
				else tPen = connection.T1;
				WithdrawalCapacity.Add(Fax * Diameter * tPen);
				WithdrawalCapacity.Add(faxRk2);
			}

			else if (ConnectionType is SingleOuterSteelPlate)
			{
				SingleOuterSteelPlate connection = ConnectionType as SingleOuterSteelPlate;
				if (connection.TimberThickness > ThreadLength) tPen = ThreadLength;
				else tPen = connection.TimberThickness;
				WithdrawalCapacity.Add(Fax * Diameter * tPen);
			}

			else throw new Exception("Bolt Withdrawal capacity not yet implemented");

			FaxRk = WithdrawalCapacity.Min();

		}


		/// <summary>
		/// Compute the tensile strength of the nail
		/// </summary>
		/// <returns></returns>
		private double ComputeNailSteelTensileStrength()
		{
			//Simple addition to create a upper limit for steel
			double aeff = Math.PI * Math.Pow(Diameter, 2) / 4;
			return 0.9 * aeff * Fuk / 1.25; //consider k2 = 0.9 and Ym2 = 1.25
		}


		public double ComputeEffectiveNumberOfFastener(int n, double a1, double angle)
		{
			if (n == 1) return 1;
			else
			{
				double nef_0 = Math.Min(n, Math.Pow(n, 0.9) * Math.Pow(a1 / (13 * Diameter), 0.25));

				int angleFirstQuadrant = SDKUtilities.ComputeAngleToFirstQuadrant(angle);

				return SDKUtilities.LinearInterpolation(Convert.ToDouble(angleFirstQuadrant), 0, nef_0, 90, n);
			}
		}

		public void ComputeSpacings(double angle)
		{
			a1min = DefineA1Min(angle);
			a2min = DefineA2Min(angle);
			a3tmin = DefineA3tMin(angle);
			a3cmin = DefineA3cMin(angle);
			a4tmin = DefineA4tMin(angle);
			a4cmin = DefineA4cMin();
		}


		public enum NailTypes
		{
			Round,
			Squarred_Grooved,
			AnnularRing,
			Twisted,
		}





		///
		///
		///Brand
		///Name
		///Type
		///OuterDiam
		///ShankDiam
		///Fuk
		///Myrk
		///HeadDiam
		///Table fHead
		///TimberType -> Fhead  / min thick max thick / min Rhok, max RhoK
		///Table Fax
		///length
		///length thread
		///ETA
		///
		///
		///
		///
		///
		///
		///
		///
		///
		///
		///
		///
		///





	}
}
