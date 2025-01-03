using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using StructuralDesignKitLibrary.EC5;
using static StructuralDesignKitLibrary.EC5.EC5_Utilities;
using Dlubal.WS.Rfem6.Model;

namespace StructuralDesignKitLibrary.Materials
{
	[Description("OSB classes for OSB/3 and OSB/4 according to EN 12369-1 March 2001")]
	public class MaterialPanelOSB : IMaterialPanel
	{

		#region Object properties

		//Global properties

		public string Grade { get; set; }

		public string Grade_Thickness { get; set; }

		public double Fmk { get; set; }

		public double Density { get; set; }

		public double E { get; set; }

		public double G { get; set; }


		//Specific timber properties

		public TimberType Type { get { return type; } }

		public double Fmyk { get; set; }

		public double Fmzk { get; set; }

		public double Ft0k { get; set; }

		public double Ft90k { get; set; }

		public double Fc0k { get; set; }

		public double Fc90k { get; set; }

		public double Fvk { get; set; }

		public double Frk { get; set; }

		public double E0mean { get; set; }

		public double E90mean { get; set; }

		public double G0mean { get; set; }

		public double E0_005 { get; set; }

		public double G0_005 { get; set; }

		public double RhoMean { get; set; }

		public double RhoK { get; set; }

		public double B0 { get; set; }

		public double Bn { get; set; }


		//Specific panel properties

		public double Fc90k_Perp { get; set; }

		public double G0meanInPlane { get; set; }

		public double G90mean { get; set; }

		public double E90_005 { get; set; }

		public double G0_005InPlane { get; set; }

		public double G90_005 { get; set; }

		public double E0mean_Axial { get; set; }

		public double E90mean_Axial { get; set; }

		public double E0_005_Axial { get; set; }

		public double E90_005_Axial { get; set; }




		#endregion

		#region constructor

		public MaterialPanelOSB(string name, double thickness)
		{


			//define grade_thickness based on name and thickness

			//verify the provided grade exist
			if (Enum.GetNames(typeof(MaterialPanelOSB.Grades)).Contains(name))
			{
				Grade = name;
			}
			else throw new ArgumentException(String.Format("The grade {0} is not present in the database, please look at the documentation", name));

			//Verify the thickness is in range
			if (thickness < 6 || thickness > 25) throw new ArgumentException("The OSB thickness must range from 6 to 25mm");

			string thck;

			if (thickness <= 10) thck = "6_10";
			else if (thickness <= 18) thck = "11_18";
			else thck = "19_25";

			Grade_Thickness = string.Concat(Grade, "_", thck);


			Fmk = MaterialPanelOSB.fmyk[Grade_Thickness];

			Fmyk = MaterialPanelOSB.fmyk[Grade_Thickness];
			Fmzk = MaterialPanelOSB.fmzk[Grade_Thickness];
			Ft0k = MaterialPanelOSB.ft0k[Grade_Thickness];
			Ft90k = MaterialPanelOSB.ft90k[Grade_Thickness];
			Fc0k = MaterialPanelOSB.fc0k[Grade_Thickness];
			Fc90k = MaterialPanelOSB.fc90k[Grade_Thickness];
			Fc90k_Perp = MaterialPanelOSB.fc90k_Perp[Grade_Thickness];
			Fvk = MaterialPanelOSB.fvk[Grade_Thickness];
			Frk = MaterialPanelOSB.frk[Grade_Thickness];

			E0mean = MaterialPanelOSB.e0mean[Grade_Thickness];
			E90mean = MaterialPanelOSB.e90mean[Grade_Thickness];
			E0_005 = MaterialPanelOSB.e0_005[Grade_Thickness];
			E90_005 = MaterialPanelOSB.e90_005[Grade_Thickness];
			E0mean_Axial = MaterialPanelOSB.e0mean_Axial[Grade_Thickness];
			E90mean_Axial = MaterialPanelOSB.e90mean_Axial[Grade_Thickness];
			E0_005_Axial = MaterialPanelOSB.e0_005_Axial[Grade_Thickness];
			E90_005_Axial = MaterialPanelOSB.e90_005_Axial[Grade_Thickness];


			G0mean = MaterialPanelOSB.gmean[Grade_Thickness];
			G0meanInPlane = MaterialPanelOSB.g0meanInPlane[Grade_Thickness];
			G90mean = MaterialPanelOSB.gmean[Grade_Thickness];
			G0_005 = MaterialPanelOSB.g0_005[Grade_Thickness];
			G0_005InPlane = MaterialPanelOSB.g0_005InPlane[Grade_Thickness];
			G90_005 = MaterialPanelOSB.g90_005[Grade_Thickness];


			RhoMean = MaterialPanelOSB.rhoMean[Grade_Thickness];
			RhoK = MaterialPanelOSB.rhoK[Grade_Thickness];
			B0 = ComputeB0(thickness);
			Bn = ComputeBn();



			Density = RhoMean;
			E = E0mean;
			G = G0mean;

		}

		public MaterialPanelOSB(Grades_Thickness name, double thickness)
		{

			string grade = name.ToString();

			//verify the provided grade exist
			if (Enum.GetNames(typeof(MaterialPanelOSB.Grades_Thickness)).Contains(grade))
			{
				Grade_Thickness = grade;
			}
			else throw new ArgumentException(String.Format("The grade {0} is not present in the database, please look at the documentation", name));

			//Verify the thickness is in range
			if (thickness < 6 || thickness > 25) throw new ArgumentException("The OSB thickness must range from 6 to 25mm");


			Fmk = MaterialPanelOSB.fmyk[Grade_Thickness];

			Fmyk = MaterialPanelOSB.fmyk[Grade_Thickness];
			Fmzk = MaterialPanelOSB.fmzk[Grade_Thickness];
			Ft0k = MaterialPanelOSB.ft0k[Grade_Thickness];
			Ft90k = MaterialPanelOSB.ft90k[Grade_Thickness];
			Fc0k = MaterialPanelOSB.fc0k[Grade_Thickness];
			Fc90k = MaterialPanelOSB.fc90k[Grade_Thickness];
			Fc90k_Perp = MaterialPanelOSB.fc90k_Perp[Grade_Thickness];
			Fvk = MaterialPanelOSB.fvk[Grade_Thickness];
			Frk = MaterialPanelOSB.frk[Grade_Thickness];

			E0mean = MaterialPanelOSB.e0mean[Grade_Thickness];
			E90mean = MaterialPanelOSB.e90mean[Grade_Thickness];
			E0_005 = MaterialPanelOSB.e0_005[Grade_Thickness];
			E90_005 = MaterialPanelOSB.e90_005[Grade_Thickness];
			E0mean_Axial = MaterialPanelOSB.e0mean_Axial[Grade_Thickness];
			E90mean_Axial = MaterialPanelOSB.e90mean_Axial[Grade_Thickness];
			E0_005_Axial = MaterialPanelOSB.e0_005_Axial[Grade_Thickness];
			E90_005_Axial = MaterialPanelOSB.e90_005_Axial[Grade_Thickness];


			G0mean = MaterialPanelOSB.gmean[Grade_Thickness];
			G0meanInPlane = MaterialPanelOSB.g0meanInPlane[Grade_Thickness];
			G90mean = MaterialPanelOSB.gmean[Grade_Thickness];
			G0_005 = MaterialPanelOSB.g0_005[Grade_Thickness];
			G0_005InPlane = MaterialPanelOSB.g0_005InPlane[Grade_Thickness];
			G90_005 = MaterialPanelOSB.g90_005[Grade_Thickness];


			RhoMean = MaterialPanelOSB.rhoMean[Grade_Thickness];
			RhoK = MaterialPanelOSB.rhoK[Grade_Thickness];
			B0 = ComputeB0(thickness);
			Bn = ComputeBn();


			Density = RhoMean;
			E = E0mean;
			G = G0mean;

		}
		#endregion

		#region Material properties

		public static TimberType type = TimberType.OSB;

		//Material properties of OSB according to EN 12369-1 March 2001
		public enum Grades
		{
			OSB3,
			OSB4,
		};

		//Material properties of OSB according to EN 12369-1 March 2001 - depending on the thickness

		public enum Grades_Thickness
		{
			OSB3_6_10,
			OSB3_11_18,
			OSB3_19_25,
			OSB4_6_10,
			OSB4_11_18,
			OSB4_19_25,
		};

		public static readonly Dictionary<string, double> fmyk = new Dictionary<string, double>

		{
			//OSB 3
			{"OSB3_6_10", 18},
			{"OSB3_11_18", 16.4},
			{"OSB3_19_25", 14.8},
			
			//OSB 4
			{"OSB4_6_10", 24.5},
			{"OSB4_11_18", 23.0},
			{"OSB4_19_25", 21.0},

		};

		public static readonly Dictionary<string, double> fmzk = new Dictionary<string, double>

		{
			//OSB 3
			{"OSB3_6_10", 9.0},
			{"OSB3_11_18", 8.2},
			{"OSB3_19_25", 7.4},
			
			//OSB 4
			{"OSB4_6_10", 13.0},
			{"OSB4_11_18", 12.2},
			{"OSB4_19_25", 11.4},

		};

		public static readonly Dictionary<string, double> ft0k = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 9.9},
			{"OSB3_11_18", 9.4},
			{"OSB3_19_25", 9.0},

			//OSB 4
			{"OSB4_6_10", 11.9},
			{"OSB4_11_18", 11.4},
			{"OSB4_19_25", 10.9},
		};

		public static readonly Dictionary<string, double> ft90k = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 7.2},
			{"OSB3_11_18", 7.0},
			{"OSB3_19_25", 6.8},

			//OSB 4
			{"OSB4_6_10", 8.5},
			{"OSB4_11_18", 8.2},
			{"OSB4_19_25", 8.0},
		};

		public static readonly Dictionary<string, double> fc0k = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 15.9},
			{"OSB3_11_18", 15.4},
			{"OSB3_19_25", 14.8 },

			//OSB 4
			{"OSB4_6_10", 18.1},
			{"OSB4_11_18", 17.6},
			{"OSB4_19_25", 17.0},

		};

		public static readonly Dictionary<string, double> fc90k = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 12.9},
			{"OSB3_11_18", 12.7},
			{"OSB3_19_25", 12.4 },

			//OSB 4
			{"OSB4_6_10", 14.3},
			{"OSB4_11_18", 14.0},
			{"OSB4_19_25", 13.7},

		};

		public static readonly Dictionary<string, double> fc90k_Perp = new Dictionary<string, double>
		{
			///
			///Conservatively, the compressive strength perpendicular to the board has been set to 2.5N/mm² (equivalent to C24)
			///Specific values need to be retrived from manufacturer
			///

			//OSB 3
			{"OSB3_6_10", 2.5},
			{"OSB3_11_18", 2.5},
			{"OSB3_19_25", 2.5 },

			//OSB 4
			{"OSB4_6_10", 2.5},
			{"OSB4_11_18", 2.5},
			{"OSB4_19_25", 2.5},

		};


		//In plane shear
		public static readonly Dictionary<string, double> fvk = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 6.8},
			{"OSB3_11_18", 6.8},
			{"OSB3_19_25", 6.8},

			//OSB 4
			{"OSB4_6_10", 6.9},
			{"OSB4_11_18", 6.9},
			{"OSB4_19_25", 6.9},


		};

		//Rolling shear
		public static readonly Dictionary<string, double> frk = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 1.0},
			{"OSB3_11_18", 1.0},
			{"OSB3_19_25", 1.0},

			//OSB 4
			{"OSB4_6_10", 1.1},
			{"OSB4_11_18", 1.1},
			{"OSB4_19_25", 1.1},


		};

		public static readonly Dictionary<string, double> e0mean = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 4930},
			{"OSB3_11_18", 4930},
			{"OSB3_19_25", 4930},

			//OSB 4
			{"OSB4_6_10", 6780},
			{"OSB4_11_18", 6780},
			{"OSB4_19_25", 6780},


		};

		public static readonly Dictionary<string, double> e90mean = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 1980},
			{"OSB3_11_18", 1980},
			{"OSB3_19_25", 1980},

			//OSB 4
			{"OSB4_6_10", 2680},
			{"OSB4_11_18", 2680},
			{"OSB4_19_25", 2680},


		};

		public static readonly Dictionary<string, double> e0mean_Axial = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 3800},
			{"OSB3_11_18", 3800},
			{"OSB3_19_25", 3800},

			//OSB 4
			{"OSB4_6_10", 4300},
			{"OSB4_11_18", 4300},
			{"OSB4_19_25", 4300},


		};

		public static readonly Dictionary<string, double> e90mean_Axial = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 3000},
			{"OSB3_11_18", 3000},
			{"OSB3_19_25", 3000},

			//OSB 4
			{"OSB4_6_10", 3200},
			{"OSB4_11_18", 3200},
			{"OSB4_19_25", 3200},


		};


		public static readonly Dictionary<string, double> e0_005 = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 4930 * 0.85},
			{"OSB3_11_18", 4930 * 0.85},
			{"OSB3_19_25", 4930 * 0.85},

			//OSB 4
			{"OSB4_6_10", 6780 * 0.85},
			{"OSB4_11_18", 6780 * 0.85},
			{"OSB4_19_25", 6780 * 0.85},

		};

		public static readonly Dictionary<string, double> e90_005 = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 1980 * 0.85},
			{"OSB3_11_18", 1980 * 0.85},
			{"OSB3_19_25", 1980 * 0.85},

			//OSB 4
			{"OSB4_6_10", 2680 * 0.85},
			{"OSB4_11_18", 2680 * 0.85},
			{"OSB4_19_25", 2680 * 0.85},

		};

		public static readonly Dictionary<string, double> e0_005_Axial = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 3800 * 0.85},
			{"OSB3_11_18", 3800 * 0.85},
			{"OSB3_19_25", 3800 * 0.85},

			//OSB 4
			{"OSB4_6_10", 4300 * 0.85},
			{"OSB4_11_18", 4300 * 0.85},
			{"OSB4_19_25", 4300 * 0.85},


		};

		public static readonly Dictionary<string, double> e90_005_Axial = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 3000 * 0.85},
			{"OSB3_11_18", 3000 * 0.85},
			{"OSB3_19_25", 3000 * 0.85},

			//OSB 4
			{"OSB4_6_10", 3200 * 0.85},
			{"OSB4_11_18", 3200 * 0.85},
			{"OSB4_19_25", 3200 * 0.85},


		};


		//Rolling shear for OSB
		public static readonly Dictionary<string, double> gmean = new Dictionary<string, double>
		{

			//OSB 3
			{"OSB3_6_10", 50},
			{"OSB3_11_18", 50},
			{"OSB3_19_25", 50},

			//OSB 4
			{"OSB4_6_10", 60},
			{"OSB4_11_18", 60},
			{"OSB4_19_25", 60},
		};


		public static readonly Dictionary<string, double> g0meanInPlane = new Dictionary<string, double>
		{

			//OSB 3
			{"OSB3_6_10", 1080},
			{"OSB3_11_18", 1080},
			{"OSB3_19_25", 1080},

			//OSB 4
			{"OSB4_6_10", 1090},
			{"OSB4_11_18", 1090},
			{"OSB4_19_25", 1090},
		};

		//Rolling shear for OSB
		public static readonly Dictionary<string, double> g90mean = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 50},
			{"OSB3_11_18", 50},
			{"OSB3_19_25", 50},

			//OSB 4
			{"OSB4_6_10", 60},
			{"OSB4_11_18", 60},
			{"OSB4_19_25", 60},

		};


		public static readonly Dictionary<string, double> g0_005 = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 50 * 0.85},
			{"OSB3_11_18", 50 * 0.85},
			{"OSB3_19_25", 50 * 0.85},

			//OSB 4
			{"OSB4_6_10", 60 * 0.85},
			{"OSB4_11_18", 60 * 0.85},
			{"OSB4_19_25", 60 * 0.85},

		};


		public static readonly Dictionary<string, double> g0_005InPlane = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 1080 * 0.85},
			{"OSB3_11_18", 1080 * 0.85},
			{"OSB3_19_25", 1080 * 0.85},

			//OSB 4
			{"OSB4_6_10", 1090 * 0.85},
			{"OSB4_11_18", 1090 * 0.85},
			{"OSB4_19_25", 1090 * 0.85},

		};


		public static readonly Dictionary<string, double> g90_005 = new Dictionary<string, double>
		{
			//Characteristic values are equal to mean values x 0.85

			//OSB 3
			{"OSB3_6_10", 50 * 0.85},
			{"OSB3_11_18", 50 * 0.85},
			{"OSB3_19_25", 50 * 0.85},

			//OSB 4
			{"OSB4_6_10", 60 * 0.85},
			{"OSB4_11_18", 60 * 0.85},
			{"OSB4_19_25", 60 * 0.85},

		};



		public static readonly Dictionary<string, double> rhoMean = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 650},
			{"OSB3_11_18", 650},
			{"OSB3_19_25", 650},

			//OSB 4
			{"OSB4_6_10", 650},
			{"OSB4_11_18", 650},
			{"OSB4_19_25", 650},


		};

		public static readonly Dictionary<string, double> rhoK = new Dictionary<string, double>
		{
			//OSB 3
			{"OSB3_6_10", 550},
			{"OSB3_11_18", 550},
			{"OSB3_19_25", 550},

			//OSB 4
			{"OSB4_6_10", 550},
			{"OSB4_11_18", 550},
			{"OSB4_19_25", 550},


		};


		/// <summary>
		/// Compute the according to EN 1995-1-2:2010-12 §3.4.2 (9) Surfaces unprotected throughout the time of fire exposure  
		/// </summary>
		/// <param name="thickness"></param>
		/// <returns></returns>
		private double ComputeB0(double thickness)
		{
			double b0_base = 0.9; //according to DIN EN 1995 - 1 - 2 table 3.1
			double kp = Math.Sqrt(450 / this.RhoK);
			double kh = Math.Sqrt(20 / thickness);

			double b0 = b0_base * kp * kh;

			return b0;
		}

		private double ComputeBn()
		{
			//Not to be used (member charing rate) - a value is given for compatibility
			return this.B0 + 0.2;
		}


		#endregion
	}
}
