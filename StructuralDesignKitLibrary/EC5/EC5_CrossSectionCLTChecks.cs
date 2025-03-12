using StructuralDesignKitLibrary.CrossSections.Interfaces;
using StructuralDesignKitLibrary.Materials;
using static StructuralDesignKitLibrary.EC5.EC5_Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StructuralDesignKitLibrary.CrossSections;
using System.Security.Policy;

namespace StructuralDesignKitLibrary.EC5
{
	public static class EC5_CrossSectionCLTCheck
	{

		
		[Description("Tension parallel to the grain §6.1.2")]
		public static double TensionParallelToGrain(double Sig0_t_d, IMaterial material, double Kmod, double Ym, double Kh = 1, double Kl_LVL = 1, bool FireCheck = false)
		{
		  throw new NotImplementedException();
		}


	   
		[Description("Compression parallel to the grain EN 1995-1 §6.1.4 - Eq(6.2)")]
		public static double CompressionParallelToGrain(double Sig0_c_d, IMaterial material, double Kmod, double Ym, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}



		[Description("Compression stresses at an angle to the grain EN 1995-1 §6.2.2 - Eq(6.16)")]
		public static double CompressionAtAnAngleToGrain(double SigAlpha_c_d, double angleToGrain, IMaterial material, double Kmod, double Ym, double kc90 = 1, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}


		[Description("Bending EN 1995-1 §6.1.6 - Eq(6.11) + Eq(6.12)")]
		public static double Bending(double SigMyd, double SigMzd, ICrossSection crossSection, IMaterial material, double Kmod, double Ym, double khy = 1, double khz = 1, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}


		
		[Description("Shear DIN EN 1995-1 +NA §6.1.7 - Eq(6.13) + Eq(6.13a) + Eq(NA.54)")]
		public static double Shear(double TauYd, double TauZd, IMaterial material, double Kmod, double Ym, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}



		[Description("Computes the shear checks but returns a list of doubles for the 3 equations. Can be used both for shear check and torsion check")]
		private static List<double> ComputeShearCheck(double TauYd, double TauZd, IMaterial material, double Kmod, double Ym, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}


		
		[Description("Torsion DIN EN 1995-1 +NA §6.1.8 - Eq(6.15) + Eq(NA.55)")]
		public static double Torsion(double TauTorsion, double TauYd, double TauZd, ICrossSection crossSection, IMaterial material, double Kmod, double Ym, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}


		
		[Description("Bending and tension EN 1995-1 +NA §6.2.3 Eq(6.17) + Eq(6.18)")]
		public static double BendingAndTension(double SigMyd, double SigMzd, double Sig0_t_d, ICrossSection crossSection, IMaterial material, double Kmod, double Ym, double khy = 1, double khz = 1, double Kh_Tension = 1, double Kl_LVL = 1, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}



		[Description("Combined Bending and Compression EN 1995-1 §6.2.4 - Eq(6.19) + Eq(6.20)")]
		public static double BendingAndCompression(double SigMyd, double SigMzd, double Sig0_c_d, ICrossSection crossSection, IMaterial material, double Kmod, double Ym, double khy = 1, double khz = 1, bool FireCheck = false)
		{
			throw new NotImplementedException();

		}


		
		[Description("Bending and Buckling EN 1995-1 §6.3.2 - Eq(6.23) + Eq(6.24)")]
		public static double BendingAndBuckling(double SigMyd, double SigMzd, double Sig0_c_d, double Leff_Y, double Leff_Z, ICrossSection crossSection, IMaterial material, double Kmod, double Ym, double khy = 1, double khz = 1, bool FireCheck = false)
		{

			throw new NotImplementedException();
		}


	
		[Description("Lateral Torsional buckling according to DIN EN 1995-1 §6.3.3 Eq(6.33) + Eq(6.35) + Eq(NA.60) + Eq(NA.61)")]
		public static double LateralTorsionalBuckling(double SigMyd, double SigMzd, double Sig0_c_d, double Leff_Y, double Leff_Z, double Leff_LTB, ICrossSection crossSection, IMaterial material, double Kmod, double Ym, double khy = 1, double khz = 1, bool FireCheck = false)
		{

			throw new NotImplementedException();

		}



	}
}
