using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class PathCalculator
    {
        private PositionData oldPos;
        private PositionData newPos;
        private float timeDifference;

        public PathCalculator() { }

        public void setTimeDiff(float timeDifference)
        {
            this.timeDifference = timeDifference;
        }

        public void setInitialPos(PositionData newPos)
        {
            this.newPos = newPos;
        }
        public void setAfterPos(PositionData newPos, float timeDifference)
        {
            this.oldPos = this.newPos;
            this.newPos = newPos;
            this.timeDifference = timeDifference;
        }

        public List<Coordinate> calcPath()
        {
            oldPos.calcAbsolutePos();
            newPos.calcAbsolutePos();

            if (newPos.getAbsoluteTargetCartesianPos().item3 <= 0)
            {
                return null;
            }

            Coordinate targetSpeed = Coordinate.realNumMul(Coordinate.vectorSum(newPos.getAbsoluteTargetCartesianPos(), Coordinate.realNumMul(oldPos.getAbsoluteTargetCartesianPos(), -1)), 1/timeDifference);
            float eta = newPos.getAbsoluteTargetCartesianPos().item3 / ((-1)*targetSpeed.item3);

            List<Coordinate> path = new List<Coordinate>();
            path.Add(newPos.getAbsoluteTargetCartesianPos());
            for(float i=1; i<eta; i = i + 1)
            {
                path.Add(Coordinate.vectorSum(path[path.Count - 1], targetSpeed));
            }

            return path;
        }
        public List<Coordinate> calcPath(Coordinate oldCrd, Coordinate recnCrd)
        {
            Coordinate targetSpeed = Coordinate.realNumMul(Coordinate.vectorSum(recnCrd, Coordinate.realNumMul(oldCrd, -1)), 1/timeDifference);
            float eta = recnCrd.item3 / ((-1)*targetSpeed.item3);

            List<Coordinate> path = new List<Coordinate>();
            path.Add(recnCrd);
            for(float i=1; i<eta; i = i + 1)
            {
                path.Add(Coordinate.vectorSum(path[path.Count - 1], targetSpeed));
            }

            return path;
        }
    }