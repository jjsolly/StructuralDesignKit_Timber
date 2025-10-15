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
    public static class EC5_MemberCheck
    {

        /// <summary>
        /// Tension parallel to the grain EN 1995-1 §6.1.2 - Eq(6.1)
        /// </summary>
        /// <param name="Sig0_t_d">Design tensile stress</param>
        /// <param name="material">Material Object</param>
        /// <param name="Kmod">modification factor</param>
        /// <param name="Ym">Material Safety factor</param>
        /// <param name="Kh">Size Factor for Cross section</param>
        /// <param name="Kl_LVL">Mofification factor for LVL member length</param>
        /// <returns>Design ratio for Tension parallel to the grain according to EN 1995-1 §6.1.2 - Eq(6.1)</returns>
        [Description("Rectangular Section Check to EN 1995-1")]
        public static double RectangularMemberCheck(CrossSectionRectangular crossSection, double kmod, double gamma_M, double leff_y, double leff_z, double Fx, double Fy, double Fz, double Mx, double My, double Mz, out List<string> report) 
        {
            //Initially without ability to do fire checks

            //Initialize variables
            report = new List<string>();
            List<double> utilisations = new List<double>();

            //Grab key data
            
            //Material - get from cross-section
            MaterialTimberGeneric crossSectionMaterial = crossSection.Material as MaterialTimberGeneric;
            if (crossSectionMaterial == null) throw new ArgumentException("The material of the cross section must be a Timber Material");

            //Get cross-section factors
            double khy = EC5.EC5_Factors.Kh_Bending(crossSectionMaterial.Type, crossSection.H);
            double khz = EC5.EC5_Factors.Kh_Bending(crossSectionMaterial.Type, crossSection.B);
            double kht = EC5.EC5_Factors.Kh_Tension(crossSectionMaterial.Type, crossSection.B, crossSection.H);
            double kl_lvl = 1.0;
            if(crossSectionMaterial.Type==TimberType.LVL | crossSectionMaterial.Type == TimberType.Baubuche)
            {
                kl_lvl = EC5.EC5_Factors.Kl_LVL(crossSectionMaterial.Type,Math.Max(leff_y,leff_z));
            }

            //Add some lines to the report
            report.Add("Rectangular Section Check to EN 1995-1");
            report.Add("Cross Section: " + crossSection.Name + " (b=" + crossSection.B + "mm, h=" + crossSection.H + "mm)");
            report.Add("Material: " + crossSectionMaterial.Type + "Grade+"+crossSectionMaterial.Grade);
            report.Add("kmod=" + kmod + ", gamma_M=" + gamma_M + ", khy=" + Math.Round(khy,2) + ", khz=" + Math.Round(khz,2) + ", kht=" + Math.Round(kht,2) + ", kl_lvl=" + Math.Round(kl_lvl,2));

            //Setup variables to be used in checking
            double sig_myd = 0.0;
            double sig_mzd = 0.0;
            double tau_yd = 0.0;
            double tau_zd = 0.0;
            double tau_tord = 0.0;
            double sig_t0d = 0.0;
            double sig_c0d = 0.0;

            //Assign correct variables for tension stresses
            if (Fx != 0.0)
            {
                double sig_ax = crossSection.ComputeNormalStress(Fx); //returns absolute value
                if (Fx > 0) sig_t0d = sig_ax;
                if (Fx < 0) sig_c0d = sig_ax;
            }

            //Test for Tension Stress - Clause 6.1.2
            if (sig_t0d > 0.0)
            {
                double check612 = EC5_CrossSectionCheck.TensionParallelToGrain(sig_t0d, crossSectionMaterial, kmod, gamma_M, kht, kl_lvl, false);

                report.Add("Section under tension - clause 6.1.2");
                report.Add("Util=" + Math.Round(check612, 2));
                utilisations.Add(check612);
            }

            //Test for Compressive Stress - Clause 6.1.4
            if (sig_c0d > 0.0)
            {

                double check614 = EC5.EC5_CrossSectionCheck.CompressionParallelToGrain(sig_c0d, crossSectionMaterial, kmod, gamma_M, false);

                report.Add("Section under compression - clause 6.1.4");
                report.Add("Utilisation=" + Math.Round(check614, 2));
                utilisations.Add(check614);
            }

            //Test for Bending Stresses - clause 6.1.6
            if (My != 0.0 || Mz != 0.0)
            {
                sig_myd = crossSection.ComputeStressBendingY(My);
                sig_mzd = crossSection.ComputeStressBendingZ(Mz);

                double check616 = EC5.EC5_CrossSectionCheck.Bending(sig_myd, sig_mzd, crossSection, crossSectionMaterial, kmod, gamma_M, khy, khz, false);

                report.Add("Section under bending - clause 6.1.6");
                report.Add("Utilisation=" + Math.Round(check616, 2));
                utilisations.Add(check616);
            }

            //Test for Shear = clause 6.1.7
            if (Fy != 0.0 || Fz != 0.0)
            {
                tau_yd = crossSection.ComputeShearY(Fy);
                tau_zd = crossSection.ComputeShearZ(Fz);

                double check617 = EC5.EC5_CrossSectionCheck.Shear(tau_yd, tau_zd, crossSectionMaterial, kmod, gamma_M, false);

                report.Add("Section under shear - clause 6.1.7");
                report.Add("Utilisation=" + Math.Round(check617, 2));
                utilisations.Add(check617);
            }

            //Test for Torsion = clause 6.1.8
            if (Mx != 0.0)
            {
                tau_tord = crossSection.ComputeTorsion(Mx);

                double check618 = EC5.EC5_CrossSectionCheck.Torsion(tau_tord, tau_yd, tau_zd, crossSection, crossSectionMaterial, kmod, gamma_M, false);

                report.Add("Section under torsion - clause 6.1.8");
                report.Add("Utilisation=" + Math.Round(check618, 2));
                utilisations.Add(check618);
            }

            //Test for Combined Stresses
            //Combined Bending and Axial Tension - Clause 6.2.3
            if (sig_t0d > 0.0 && (sig_myd > 0 | sig_mzd > 0))
            {
                double check623 = EC5.EC5_CrossSectionCheck.BendingAndTension(sig_myd, sig_mzd, sig_t0d, crossSection, crossSectionMaterial, kmod, gamma_M,khy,khz,kht,kl_lvl,false);

                report.Add("Section under Bending+Tension - clause 6.2.3");
                report.Add("Utilisation=" + Math.Round(check623, 2));
                utilisations.Add(check623);
            }

            //Combined Bending and Compression - Clause 6.2.4
            if (sig_c0d > 0.0 && (sig_myd > 0 | sig_mzd > 0))
            {
                double check624 = EC5.EC5_CrossSectionCheck.BendingAndCompression(sig_myd, sig_mzd, sig_c0d, crossSection, crossSectionMaterial, kmod, gamma_M,khy,khz,false);

                report.Add("Section under Bending+Compression - clause 6.2.4");
                report.Add("Utilisation=" + Math.Round(check624, 2));
                utilisations.Add(check624);
            }

            //Stability of Members

            // Clause 6.3.2 - column buckling - equations 6.23 & 6.24
            if (sig_c0d > 0.0)
            {
                double check632 = EC5.EC5_CrossSectionCheck.BendingAndBuckling(sig_myd, sig_mzd, sig_c0d, leff_y, leff_z, crossSection, crossSectionMaterial, kmod, gamma_M,khy,khz,false);

                report.Add("Section under Compression - Check Buckling - clause 6.3.2");
                report.Add("Utilisation=" + Math.Round(check632, 2));
                utilisations.Add(check632);
            }

            //Clause 6.3.3 - LTB - check it in all cases I think unless ONLY TENSION
            if (sig_c0d > 0 | sig_myd > 0 | sig_mzd > 0)
            {
                double leff_ltb = Math.Max(leff_y, leff_z);
                double check633 = EC5.EC5_CrossSectionCheck.LateralTorsionalBuckling(sig_myd, sig_mzd, sig_c0d, leff_y, leff_z, leff_ltb, crossSection, crossSectionMaterial, kmod, gamma_M,khy,khz,false);
                report.Add("Section check LTB - clause 6.3.3");
                report.Add("Utilisation=" + Math.Round(check633, 2));
                utilisations.Add(check633);
            }

            double max_utilisation = 0.0;
            if (utilisations.Count > 0) max_utilisation = utilisations.OrderByDescending(p => p).ToList()[0];
            return max_utilisation;
        }
        /// <summary>
        /// Rectagular Section Check to EN 1995-1 without returning the report
        /// </summary>
        /// <param name="crossSection"></param>
        /// <param name="kmod"></param>
        /// <param name="gamma_M"></param>
        /// <param name="leff_y"></param>
        /// <param name="leff_z"></param>
        /// <param name="Fx"></param>
        /// <param name="Fy"></param>
        /// <param name="Fz"></param>
        /// <param name="Mx"></param>
        /// <param name="My"></param>
        /// <param name="Mz"></param>
        /// <returns>max utilisation of all relevant section checks to EN1995-1</returns>
        public static double RectangularMemberCheck(CrossSectionRectangular crossSection, double kmod, double gamma_M, double leff_y, double leff_z, double Fx, double Fy, double Fz, double Mx, double My, double Mz)
        {
            List<string> report = new List<string>();
            double max_utilisation = RectangularMemberCheck(crossSection,kmod,gamma_M,leff_y,leff_z,Fx,Fy,Fz,Mx,My,Mz,out report);
            return max_utilisation;
        }

    }
}
