using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace BranchingTreeBasic
{
    public class BranchingTreeBasicComponent : GH_Component
    {
        //Constructor

        public BranchingTreeBasicComponent() : base("BranchingTreeBasic", "BranchingTreeBasic", "For prototype", "User", "Test")
        {
        }

        //Input
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Length", "L", "The length of trunk", GH_ParamAccess.item);
            pManager.AddNumberParameter("Branch Angle", "BA", "The Angle(degree) of branches", GH_ParamAccess.item);
            pManager.AddNumberParameter("Branch Scale", "BS", "The Scale of branches", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Branch Number", "N", "The number of branches", GH_ParamAccess.item);
        }

        //Output
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Check", "C", "Check line list", GH_ParamAccess.item);
            pManager.AddLineParameter("Branches", "B", "Generated branching", GH_ParamAccess.list);
        }

        //Method(Entry point)
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //1.Declare placeholder variables for the input data.
            double length = double.NaN;
            double branchAngle = double.NaN;
            double branchScale = double.NaN;
            int num = 0;

            //2.Retrieve input data.
            if (!DA.GetData(0, ref length)) { return; }
            if (!DA.GetData(1, ref branchAngle)) { return; }
            if (!DA.GetData(2, ref branchScale)) { return; }
            if (!DA.GetData(3, ref num)) { return; }

            //Set the branch angle by converting it from degs to radians
            double bAngRad = ((Math.PI / 100) * branchAngle);

            //Create a new line in the yDirection from the origin and add it to the list
            List<Line> lines = new List<Line>();
            Point3d stPt = new Point3d(0, 0, 0);
            Vector3d unitYvect = new Vector3d(0, 1, 0);
            Line ln0 = new Line(stPt, unitYvect, length);
            lines.Add(ln0);

            //Create two temporary lists and set the first equal to the lines list
            List<Line> tempList = new List<Line>();
            List<Line> tempList2 = new List<Line>();
            tempList = lines;

            int i = 0;
            while (i < num)
            {
                tempList2 = CreateBranch(tempList, branchScale, bAngRad);
                tempList = tempList2;
                lines.AddRange(tempList2);
                i++;
            }


            int count = lines.Count;

            DA.SetData(0, count);
            DA.SetDataList(1, lines);
        }

        //Method
        public List<Line> CreateBranch(List<Line> lines, double bScale, double bAng)
        {
            var newLines = new List<Line>();

            foreach (Line ln in lines)
            {
                //Get the length of the current trunk and create a new scaled branch.
                double newLength = ln.Length * bScale;

                //Get the endPt of the trunk and its tangent vector.
                Point3d endPt = ln.To;
                Vector3d unitTan = ln.UnitTangent;

                //Create two new lines and make the second line have a negative rotation anngle
                Line ln1 = new Line(endPt, unitTan, newLength);
                ln1.Transform(Rhino.Geometry.Transform.Rotation(bAng, endPt));
                Line ln2 = new Line(endPt, unitTan, newLength);
                ln2.Transform(Rhino.Geometry.Transform.Rotation(bAng * -1, endPt));

                //Add these new branches to the list
                newLines.Add(ln1);
                newLines.Add(ln2);
            }

            return newLines;

        }

        
        //>>Icon made by Freepik from www.flaticon.com 
        protected override System.Drawing.Bitmap Icon => BranchingTreeBasic.Properties.Resources.treeIcon;

        public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

        public override Guid ComponentGuid => new Guid("{1A6C1AE2-E074-407D-B896-7F7510B5EA40}");
    }
}
