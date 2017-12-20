using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.NavMesh;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public static class StringPullingPathSmoothing
    {
        /// <summary>
        /// Method used to smooth a received path, using a string pulling technique
        /// it returns a new path, where the path positions are selected in order to provide a smoother path
        /// </summary>
        /// <param name="data"></param>
        /// <param name="globalPath"></param>
        /// <returns></returns>
        public static GlobalPath SmoothPath(KinematicData data, GlobalPath globalPath)
        {
            NavMeshEdge edge;
            var lookAhead = 3;
            Vector3 lookAheadTarget;

            var smoothedPath = new GlobalPath
            {
                IsPartial = globalPath.IsPartial
            };

            //we will string pull from the begginning to the end
            var endPosition = globalPath.PathPositions.Last();

            var currentPosition = data.position;

            for (int i = 0; i < globalPath.PathNodes.Count; i++)
            {
                edge = globalPath.PathNodes[i] as NavMeshEdge;
                if (edge != null)
                {
                    
                    if ((i + lookAhead) < globalPath.PathNodes.Count)
                    {
                        lookAheadTarget = globalPath.PathNodes[i + lookAhead].LocalPosition;
                    }
                    else
                    {
                        lookAheadTarget = endPosition;
                    }

                    if (!lookAheadTarget.Equals(currentPosition))
                    {
                        var closestPointInEdge = MathHelper.ClosestPointInLineSegment2ToLineSegment1(currentPosition, lookAheadTarget, edge.PointOne, edge.PointTwo, lookAheadTarget);
                        smoothedPath.PathPositions.Add(closestPointInEdge);
                        currentPosition = closestPointInEdge;
                    }
                }
            }

            smoothedPath.PathPositions.Add(endPosition);

            return smoothedPath;
        }


    }
}
