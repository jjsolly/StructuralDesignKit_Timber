using StructuralDesignKitLibrary.EC5;
using StructuralDesignKitLibrary.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using System.Security.Permissions;
using Dlubal.WS.Rfem6.Model;

namespace StructuralDesignKitLibrary.Utilities
{
    public static class ConnectionUtilities
    {

    }

    public class RectangularMemberFastenerGrid
    {
        public Plane ConnectionPlane; //plane located at centre of applied load
        public List<Point3d> FastenerLocations;

    }

    public class RectangularTimberMember
    {
        public int B; //width
        public int H; //height
        public CrossSections.CrossSectionRectangular CrossSection;
        public double Length;
        public double Ley;
        public double Lez;

        public EC5.EC5_Utilities.ServiceClass ServiceClass;

        Line memberCentroid;
        public Plane MemberPlane; //aligned with primary axes - IS THIS REQUIRED?
        public MemberLoads MemberLoads;

        public RectangularTimberMember(int width, int height, double length, string material, string serviceClass, Plane memberPlane)
        {
            //Basic properties
            B = width;
            H = height;
            IMaterialTimber mat = EC5_Utilities.GetTimberFromGrade(material);
            CrossSection = new CrossSections.CrossSectionRectangular(B, H, mat);
            ServiceClass = EC5.EC5_Utilities.GetServiceClass(serviceClass);
            MemberLoads = new MemberLoads();

            //default the buckling lengths to 1 x length
            Length = length;
            Ley = Length;
            Lez = Length;

            //setup the geometry bits - helpful for later
            MemberPlane = memberPlane;
            memberCentroid = new Line(memberPlane.Origin, memberPlane.XAxis * Length);
        }
    }
    public class MemberLoads
    {
        public List<string> LoadNames;
        public List<string> LoadDurations;
        public List<Vector3d> Forces;
        public List<Vector3d> Moments;
        public MemberLoads()
        {
            LoadNames = new List<string>();
            LoadDurations = new List<string>();
            Forces = new List<Vector3d>();
            Moments = new List<Vector3d>();
        }
        public MemberLoads(List<string> loadNames, List<string> loadDurations, List<Vector3d> forces, List<Vector3d> moments)
        {
            LoadNames = loadNames;
            LoadDurations = loadDurations;
            Forces = forces;
            Moments = moments;
        }
        public void AddMemberLoad(string loadName, string loadDuration, Vector3d forces, Vector3d moments)
        {
            LoadNames.Add(loadName);
            LoadDurations.Add(loadDuration);   
            Forces.Add(forces);
            Moments.Add(moments);
        }
    }


}
