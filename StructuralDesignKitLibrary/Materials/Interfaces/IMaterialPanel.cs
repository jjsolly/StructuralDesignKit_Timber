using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralDesignKitLibrary.Materials
{
	public interface IMaterialPanel : IMaterialTimber
	{

		/// <summary>
		/// Characteristic compression strength perpendicular to the panel
		/// </summary>
		[Description("Characteristic compression strength perpendicular to the panel")]
		double Fc90k_Perp { get; set; }


		/// <summary>
		/// Mean shear Modulus parallel to the grain
		/// </summary>
		[Description("Mean shear Modulus parallel to the grain | In plane of the panel ")]
		double G0meanInPlane { get; set; }

		/// <summary>
		/// Mean shear Modulus parallel to the grain
		/// </summary>
		[Description("Mean rolling shear Modulus parallel to the grain")]
		double G90mean { get; set; }


		/// <summary>
		/// Mean Modulus of Elasticity parallel to grain - Tension and compression
		/// </summary>
		[Description("Mean Modulus of Elasticity parallel to grain - Tension and compression")]
		double E0mean_Axial { get; set; }



		/// <summary>
		/// Mean Modulus of Elasticity perpendicular to grain - Tension and compression
		/// </summary>
		[Description("Mean Modulus of Elasticity perpendicular to grain - Tension and compression")]
		double E90mean_Axial { get; set; }



		/// <summary>
		/// Characteristic Modulus of Elasticity perpendicular to grain
		/// </summary>
		[Description("Characteristic Modulus of Elasticity perpendicular to grain")]
		double E90_005 { get; set; }


		/// <summary>
		/// Characteristic Modulus of Elasticity parallel to grain - Tension and compression
		/// </summary>
		[Description("Characteristic Modulus of Elasticity parallel to grain - Tension and compression")]
		double E0_005_Axial { get; set; }


		/// <summary>
		/// Characteristic Modulus of Elasticity perpendicular to grain - Tension and compression
		/// </summary>
		[Description("Characteristic Modulus of Elasticity perpendicular to grain - Tension and compression")]
		double E90_005_Axial { get; set; }


		/// <summary>
		/// Characteristic shear Modulus parallel to the grain | In plane of the panel
		/// </summary>
		[Description("Characteristic shear Modulus parallel to the grain | In plane of the panel ")]
		double G0_005InPlane { get; set; }

		/// <summary>
		/// Characteristic rolling shear Modulus parallel to the grain
		/// </summary>
		[Description("Characteristic rolling shear Modulus parallel to the grain")]
		double G90_005 { get; set; }

	}
}
