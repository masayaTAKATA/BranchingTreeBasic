using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace BranchingTreeBasic
{
    /// <summary>
    /// Branching Tree
    /// multi-threading component inherited from GH_TaskCapableComponent
    /// </summary>
    public class BranchingTreeBasicTaskComponent : GH_TaskCapableComponent<BranchingTreeBasicTaskComponent.SolveResults>
    {
        //Constructor
        public BranchingTreeBasicTaskComponent() : base("BranchingTreeBasic_Task", "BTB_t", "Multi-threading compute Recursive treeline", "Meenaxy", "Test")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Length", "L", "The length of trunk", GH_ParamAccess.item);
            pManager.AddNumberParameter("Branch Angle", "BA", "The angle(degree) of branches", GH_ParamAccess.item);
            pManager.AddNumberParameter("Branch Scale", "BS", "The scale of branches", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Branch number", "N", "The number of branches", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Branches", "B", "Generated branch lines", GH_ParamAccess.list);
        }

        ///<summary>
        ///separate method
        ///1. Collect input data
        ///2. Compute results on given data
        ///3. Set output data
        /// </summary>
        
        public class SolveResults
        {
            public List<Line> Value { get; set; }
        }


        /// <summary>
        /// static Method create lines for recursive process
        /// Create a compute function the takes the input retrieve from IGH_DataAccess
        /// and returns an instance of SolveResults.
        /// </summary>
        
        private static SolveResults ComputeRecursiveLines(double length, double branchAngle, double branchScale, int num)
        {
            var result = new SolveResults();

            //Set the branch angle by converting it from degrees to radians
            double bAngleRad = (Math.PI / 100) * branchAngle;

            //Create a new line in the yDirection from the origin and add it to the list.
            var lines = new List<Line>();
            var stPt = new Point3d(0, 0, 0);
            var unitYvect = new Vector3d(0, 1, 0);
            var ln0 = new Line(stPt, unitYvect, length);
            lines.Add(ln0);

            //Create two temporary lists and set the first equal to the lines list.
            var tempList01 = new List<Line>();
            var tempList02 = new List<Line>();
            tempList01 = lines;

            int i = 0;
            while (i < num)
            {
                tempList02 = CreateBranch(tempList01, branchScale, bAngleRad);
                tempList01 = tempList02;
                lines.AddRange(tempList02);
                i++;
            }
            result.Value = lines;

            return result;
        }


        public static List<Line> CreateBranch(List<Line> lines, double bScale, double bAng)
        {
            var newLines = new List<Line>();

            foreach(var ln in lines)
            {
                //Get the length of the current trunk and create a new scaled branch.
                double newLength = ln.Length * bScale;

                //Get the endPt of the trunk and its tangent vector.
                var endPt = ln.To;
                var unitTan = ln.UnitTangent;

                //Create two new lines and make the second line have a negative rotation angle.
                var ln1 = new Line(endPt, unitTan, newLength);
                ln1.Transform(Transform.Rotation(bAng, endPt));
                var ln2 = new Line(endPt, unitTan, newLength);
                ln2.Transform(Transform.Rotation(bAng * -1, endPt));

                //Add these new branches to the list
                newLines.Add(ln1);
                newLines.Add(ln2);
            }

            return newLines;
        }
        

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Declare placeholder variables for the input data
            double length = double.NaN;
            double branchAngle = double.NaN;
            double branchScale = double.NaN;
            int num = 0;

            //Retrieve input data
            if (!DA.GetData(0, ref length)) { return; }
            if (!DA.GetData(1, ref branchAngle)) { return; }
            if (!DA.GetData(2, ref branchScale)) { return; }
            if (!DA.GetData(3, ref num)) { return; }

            if (InPreSolve)
            {
                //Queue up the task
                Task<SolveResults> task = Task.Run(() => ComputeRecursiveLines(length, branchAngle, branchScale, num), CancelToken);
                TaskList.Add(task);

                return;
            }

            if(!GetSolveResults(DA, out SolveResults result))
            {
                //Compute result on a given data
                result = ComputeRecursiveLines(length, branchAngle, branchScale, num);
            }
            
            //Set output data
            if(result != null)
            {
                DA.SetDataList(0, result.Value);
            }
            
        }

        /// <summary>
        /// https://www.flaticon.com/
        ///Icon made by Freepik from www.flaticon.com 
        /// </summary>
        protected override System.Drawing.Bitmap Icon => BranchingTreeBasic.Properties.Resources.treeIcon;

        public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

        public override Guid ComponentGuid => new Guid("{5FF9717F-9227-4D67-AA01-FEA31081DB6D}");

    }
}
