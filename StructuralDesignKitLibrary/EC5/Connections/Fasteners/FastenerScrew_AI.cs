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
using System.Net.Http.Headers;
using static StructuralDesignKitLibrary.EC5.EC5_Utilities;

namespace StructuralDesignKitLibrary.Connections.Fasteners
{
    /// <summary>
    /// Screw fastener according to EN 1995-1-1 §8.5
    /// </summary>
    public class FastenerScrew : IFastener
    {
        #region Properties

        public FastenerType Type { get; }
        public double Diameter { get; set; }
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

        // Screw-specific
        public double Length {get;set;} // Total length of the screw
        public double ThreadLength {get;set;} // Length of the threaded part of the screw
        public double TensileCapacity {get;set;}
        public bool PreDrilled { get; set; } // Indicates if the screw is pre-drilled
        public double ShankDiameter { get; set; } // Diameter of the shank
        public double ThreadOuterDiameter { get; set; }
        public double ThreadInnerDiameter { get; set; }
        public double HeadDepth { get; set; } // Depth of the screw head
        public double HeadDiameter { get; set; } // Diameter of the screw head
        public IFastener ShearFasternerEquivalent { get; set; } // Equivalent fastener for shear calculations


        #endregion

        #region Constructor

        public FastenerScrew(double threadOuterDiameter, double threadInnerDiameter, double shankDiameter, double length, double threadLength, double headDiameter, double headDepth, double fuk, bool predrilled)
        {
            Type = FastenerType.Screw;
            ShankDiameter = shankDiameter;
            ThreadOuterDiameter = threadOuterDiameter;
            ThreadInnerDiameter = threadInnerDiameter;
            Length = length;
            ThreadLength = threadLength;
            HeadDiameter = headDiameter;
            HeadDepth = headDepth;  
            PreDrilled = predrilled;
            Fuk = fuk;

            //calculate effective diameter to EN 1995-1-1 §8.7.1 (3)
            Diameter = 1.1 * threadInnerDiameter;
            //Note that clause EN 1995-1-1 §8.7.1 (2) potentially allows for larger d_ef but requires knowledge of the timber thickness.

            //for all shear components of the screw - we can calculate them in accordance with the equivalent nail or screw as per EN 1995-1-1 §8.7.1 (4) & (5)
            if (Diameter > 6) ShearFasternerEquivalent = new FastenerDowel(Diameter, Fuk);
            else ShearFasternerEquivalent = new FastenerNail(FastenerNail.NailTypes.Round, Diameter, Length, Fuk, PreDrilled);

            MaxJohansenPart = 1.0; // EN 1995-1-1 §8.2.2 (2)
            MyRk = ShearFasternerEquivalent.MyRk;
        }

        //todo: add a function to construct a screw from rothoblaas data or similar rather than manually setting properties

        #endregion

        #region Embedment Strength

        public void ComputeEmbedmentStrength(IMaterialTimber timber, double angle, double thickness = 0)
        {
            ShearFasternerEquivalent.ComputeEmbedmentStrength(timber, angle, thickness);
            Fhk = ShearFasternerEquivalent.Fhk;
        }

        #endregion

        #region Tensile Strength Checks

        public void ComputeScrewTensileStrength()
        {
            //To EN195-1-1 §8.7.2

            //Tensile strength is the minimum of:

            //withdrawal strength which includes threaded parts + head-pull through + tensile failure of screw shaft + tensile failure of screw head
            //OR
            //buckling failure of screw in compression which we should check separately!
        }
        public void ComputeWithdrawalStrength(IShearCapacity connection)
        {
            // JJS TO REVIEW
            //CANNOT BE THE SAME AS FOR DOWELS OR NAILS EVEN IF THAT'S WHAT THEY THINK!!

            //double tPen = 0;
            //double faxRkHead = 0;
            //List<double> capacities = new List<double>();

            //if (connection is ITimberTimberShear timberConnection)
            //{
            //    tPen = Math.Min(PenetrationDepth, timberConnection.T2);
            //    if (tPen < 6 * Diameter) throw new Exception("Screw penetration depth must be at least 6d");

            //    capacities.Add(Fax * Diameter * tPen);
            //}
            //else if (connection is SingleOuterSteelPlate steelConnection)
            //{
            //    tPen = Math.Min(PenetrationDepth, steelConnection.TimberThickness);
            //    if (tPen < 6 * Diameter) throw new Exception("Screw penetration depth must be at least 6d");

            //    capacities.Add(Fax * Diameter * tPen);
            //}
            //else
            //{
            //    throw new Exception("Withdrawal capacity for this connection type is not implemented.");
            //}

            //if (TensileCapacity > 0)
            //    capacities.Add(TensileCapacity);
            //else
            //    capacities.Add(ComputeScrewSteelTensileStrength());

            //FaxRk = capacities.Min();
        }

        private double ComputeScrewSteelTensileStrength()
        {
            double aeff = Math.PI * Math.Pow(Diameter, 2) / 4;
            return 0.9 * aeff * Fuk / 1.25;
        }

        #endregion

        #region Spacings

        public void ComputeSpacings(double? angle, IShearCapacity connection)
        {
            //equations need to be properly worked out
            // Spacing adjustments for OSB or steel plates can be added here if needed
        }

        #endregion

        #region Effective Fastener Count

        public double ComputeEffectiveNumberOfFastener(int n, double a1, double angle)
        {
            return 0.0;
        }

        #endregion

        #region Slip Modulus

        public double ComputeSlipModulus(double timberDensity)
        {
            return 0.0;
        }

        #endregion
    }
}
