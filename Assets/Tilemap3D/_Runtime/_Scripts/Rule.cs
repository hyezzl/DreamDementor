using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tilemap3D
{
    [Serializable]
    public class Rule
    {
        public Tile tile;
        public EMatchType[] matchGrid = new EMatchType[27];
        public ETransformation transformation = ETransformation.Fixed;

        public enum ERuleRotationAxis { X, Y, Z }
        public enum ERuleMirrorAxis { X, Y, Z, XZ, XY, YZ }

        public bool Matches(RulesetTile rulesetTile, ref Tile[] neighbors)
        {
            return Matches(matchGrid, rulesetTile, ref neighbors);
        }

        public bool Matches(EMatchType[] matchGrid, RulesetTile rulesetTile, ref Tile[] neighbors)
        {
            for (int i = 0; i < 27; i++)
            {
                Tile neighbor = neighbors[i];
                EMatchType matchType = matchGrid[i];

                bool match = matchType switch
                {
                    EMatchType.Anything => true,
                    EMatchType.Empty => !NeighborMatches(rulesetTile, neighbor),
                    EMatchType.Occupied => NeighborMatches(rulesetTile, neighbor),
                    _ => false
                };

                if (!match)
                    return false;
            }

            return true;
        }

        public bool MatchesRotation(RulesetTile rulesetTile, ref Tile[] neighbors, ref Quaternion matchedRotation, ERuleRotationAxis rotationAxis)
        {
            for (int i = 1; i <= 4; i++)
            {
                if (Matches(RotateMatchGrid90Degrees(matchGrid, i, rotationAxis), rulesetTile, ref neighbors))
                {
                    matchedRotation = Quaternion.Euler(0, i * 90, 0);
                    return true;
                }
            }

            return false;
        }

        private EMatchType[] RotateMatchGrid90Degrees(EMatchType[] matchGrid, int times, ERuleRotationAxis rotationAxis)
        {
            EMatchType[] result = new EMatchType[27];
            EMatchType[] rotatedMatchGrid = matchGrid;

            if (rotationAxis == ERuleRotationAxis.Y)
            {
                for (int t = 0; t < times; t++)
                {
                    result[0] = rotatedMatchGrid[6]; result[9] = rotatedMatchGrid[15];  result[18] = rotatedMatchGrid[24];
                    result[1] = rotatedMatchGrid[3]; result[10] = rotatedMatchGrid[12]; result[19] = rotatedMatchGrid[21];
                    result[2] = rotatedMatchGrid[0]; result[11] = rotatedMatchGrid[9];  result[20] = rotatedMatchGrid[18];
                    result[3] = rotatedMatchGrid[7]; result[12] = rotatedMatchGrid[16]; result[21] = rotatedMatchGrid[25];
                    result[4] = rotatedMatchGrid[4]; result[13] = rotatedMatchGrid[13]; result[22] = rotatedMatchGrid[22];
                    result[5] = rotatedMatchGrid[1]; result[14] = rotatedMatchGrid[10]; result[23] = rotatedMatchGrid[19];
                    result[6] = rotatedMatchGrid[8]; result[15] = rotatedMatchGrid[17]; result[24] = rotatedMatchGrid[26];
                    result[7] = rotatedMatchGrid[5]; result[16] = rotatedMatchGrid[14]; result[25] = rotatedMatchGrid[23];
                    result[8] = rotatedMatchGrid[2]; result[17] = rotatedMatchGrid[11]; result[26] = rotatedMatchGrid[20];

                    rotatedMatchGrid = (EMatchType[])result.Clone();
                }
            }
            else if (rotationAxis == ERuleRotationAxis.X)
            {
                for (int t = 0; t < times; t++)
                {
                    result[0] = rotatedMatchGrid[18]; result[9] = rotatedMatchGrid[21];  result[18] = rotatedMatchGrid[24];
                    result[1] = rotatedMatchGrid[19]; result[10] = rotatedMatchGrid[22]; result[19] = rotatedMatchGrid[25];
                    result[2] = rotatedMatchGrid[20]; result[11] = rotatedMatchGrid[23]; result[20] = rotatedMatchGrid[26];
                    result[3] = rotatedMatchGrid[9];  result[12] = rotatedMatchGrid[12]; result[21] = rotatedMatchGrid[15];
                    result[4] = rotatedMatchGrid[10]; result[13] = rotatedMatchGrid[13]; result[22] = rotatedMatchGrid[16];
                    result[5] = rotatedMatchGrid[11]; result[14] = rotatedMatchGrid[14]; result[23] = rotatedMatchGrid[17];
                    result[6] = rotatedMatchGrid[0];  result[15] = rotatedMatchGrid[3];  result[24] = rotatedMatchGrid[6];
                    result[7] = rotatedMatchGrid[1];  result[16] = rotatedMatchGrid[4];  result[25] = rotatedMatchGrid[7];
                    result[8] = rotatedMatchGrid[2];  result[17] = rotatedMatchGrid[5];  result[26] = rotatedMatchGrid[8];

                    rotatedMatchGrid = (EMatchType[])result.Clone();
                }
            }
            else if (rotationAxis == ERuleRotationAxis.Z)
            {
                result[0] = rotatedMatchGrid[18]; result[9] = rotatedMatchGrid[19];  result[18] = rotatedMatchGrid[20];
                result[1] = rotatedMatchGrid[9]; result[10] = rotatedMatchGrid[10]; result[19] = rotatedMatchGrid[11];
                result[2] = rotatedMatchGrid[0];  result[11] = rotatedMatchGrid[1];  result[20] = rotatedMatchGrid[2];
                result[3] = rotatedMatchGrid[21];  result[12] = rotatedMatchGrid[22]; result[21] = rotatedMatchGrid[23];
                result[4] = rotatedMatchGrid[12]; result[13] = rotatedMatchGrid[13]; result[22] = rotatedMatchGrid[14];
                result[5] = rotatedMatchGrid[3];  result[14] = rotatedMatchGrid[4];  result[23] = rotatedMatchGrid[5];
                result[6] = rotatedMatchGrid[24];  result[15] = rotatedMatchGrid[25];  result[24] = rotatedMatchGrid[26];
                result[7] = rotatedMatchGrid[15];  result[16] = rotatedMatchGrid[16]; result[25] = rotatedMatchGrid[17];
                result[8] = rotatedMatchGrid[6];  result[17] = rotatedMatchGrid[7];  result[26] = rotatedMatchGrid[8];

                rotatedMatchGrid = (EMatchType[])result.Clone();
            }

            return rotatedMatchGrid;
        }

        public bool MatchesMirrored(RulesetTile rulesetTile, ref Tile[] neighbors, ref Vector3 matchedScale, ERuleMirrorAxis mirrorAxis)
        {
            if (Matches(matchGrid, rulesetTile, ref neighbors))
            {
                matchedScale = Vector3.one;
                return true;
            }

            if (Matches(MirrorMatchGrid(matchGrid, mirrorAxis), rulesetTile, ref neighbors))
            {
                float xScale = mirrorAxis == ERuleMirrorAxis.X || mirrorAxis == ERuleMirrorAxis.XY || mirrorAxis == ERuleMirrorAxis.XZ ? -1 : 1;
                float yScale = mirrorAxis == ERuleMirrorAxis.Y || mirrorAxis == ERuleMirrorAxis.XY || mirrorAxis == ERuleMirrorAxis.YZ ? -1 : 1;
                float zScale = mirrorAxis == ERuleMirrorAxis.Z || mirrorAxis == ERuleMirrorAxis.XZ || mirrorAxis == ERuleMirrorAxis.YZ ? -1 : 1;
                matchedScale = new Vector3(xScale, yScale, zScale);
                return true;
            }

            return false;
        }

        private EMatchType[] MirrorMatchGrid(EMatchType[] matchGrid, ERuleMirrorAxis mirrorAxis)
        {
            EMatchType[] result = new EMatchType[27];
            EMatchType[] mirroredMatchGrid = matchGrid;

            if (mirrorAxis == ERuleMirrorAxis.X)
            {
                result[0] = mirroredMatchGrid[2]; result[9] = mirroredMatchGrid[11];  result[18] = mirroredMatchGrid[20];
                result[1] = mirroredMatchGrid[1]; result[10] = mirroredMatchGrid[10]; result[19] = mirroredMatchGrid[19];
                result[2] = mirroredMatchGrid[0]; result[11] = mirroredMatchGrid[9];  result[20] = mirroredMatchGrid[18];
                result[3] = mirroredMatchGrid[5]; result[12] = mirroredMatchGrid[14]; result[21] = mirroredMatchGrid[23];
                result[4] = mirroredMatchGrid[4]; result[13] = mirroredMatchGrid[13]; result[22] = mirroredMatchGrid[22];
                result[5] = mirroredMatchGrid[3]; result[14] = mirroredMatchGrid[12]; result[23] = mirroredMatchGrid[21];
                result[6] = mirroredMatchGrid[8]; result[15] = mirroredMatchGrid[17]; result[24] = mirroredMatchGrid[26];
                result[7] = mirroredMatchGrid[7]; result[16] = mirroredMatchGrid[16]; result[25] = mirroredMatchGrid[25];
                result[8] = mirroredMatchGrid[6]; result[17] = mirroredMatchGrid[15]; result[26] = mirroredMatchGrid[24];

                mirroredMatchGrid = result;
            }
            else if (mirrorAxis == ERuleMirrorAxis.Y)
            {
                result[0] = mirroredMatchGrid[18]; result[9] = mirroredMatchGrid[9];   result[18] = mirroredMatchGrid[0];
                result[1] = mirroredMatchGrid[19]; result[10] = mirroredMatchGrid[10]; result[19] = mirroredMatchGrid[1];
                result[2] = mirroredMatchGrid[20]; result[11] = mirroredMatchGrid[11]; result[20] = mirroredMatchGrid[2];
                result[3] = mirroredMatchGrid[21]; result[12] = mirroredMatchGrid[12]; result[21] = mirroredMatchGrid[3];
                result[4] = mirroredMatchGrid[22]; result[13] = mirroredMatchGrid[13]; result[22] = mirroredMatchGrid[4];
                result[5] = mirroredMatchGrid[23]; result[14] = mirroredMatchGrid[14]; result[23] = mirroredMatchGrid[5];
                result[6] = mirroredMatchGrid[24]; result[15] = mirroredMatchGrid[15]; result[24] = mirroredMatchGrid[6];
                result[7] = mirroredMatchGrid[25]; result[16] = mirroredMatchGrid[16]; result[25] = mirroredMatchGrid[7];
                result[8] = mirroredMatchGrid[26]; result[17] = mirroredMatchGrid[17]; result[26] = mirroredMatchGrid[8];

                mirroredMatchGrid = result;
            }
            else if (mirrorAxis == ERuleMirrorAxis.Z)
            {
                result[0] = mirroredMatchGrid[6]; result[9] = mirroredMatchGrid[15];  result[18] = mirroredMatchGrid[24];
                result[1] = mirroredMatchGrid[7]; result[10] = mirroredMatchGrid[16]; result[19] = mirroredMatchGrid[25];
                result[2] = mirroredMatchGrid[8]; result[11] = mirroredMatchGrid[17]; result[20] = mirroredMatchGrid[26];
                result[3] = mirroredMatchGrid[3]; result[12] = mirroredMatchGrid[12]; result[21] = mirroredMatchGrid[21];
                result[4] = mirroredMatchGrid[4]; result[13] = mirroredMatchGrid[13]; result[22] = mirroredMatchGrid[22];
                result[5] = mirroredMatchGrid[5]; result[14] = mirroredMatchGrid[14]; result[23] = mirroredMatchGrid[23];
                result[6] = mirroredMatchGrid[0]; result[15] = mirroredMatchGrid[9];  result[24] = mirroredMatchGrid[18];
                result[7] = mirroredMatchGrid[1]; result[16] = mirroredMatchGrid[10]; result[25] = mirroredMatchGrid[19];
                result[8] = mirroredMatchGrid[2]; result[17] = mirroredMatchGrid[11]; result[26] = mirroredMatchGrid[20];

                mirroredMatchGrid = result;
            }
            else if (mirrorAxis == ERuleMirrorAxis.XZ)
            {
                result = MirrorMatchGrid(matchGrid, ERuleMirrorAxis.X);
                result = MirrorMatchGrid(result, ERuleMirrorAxis.Z);

                mirroredMatchGrid = result;
            }
            else if (mirrorAxis == ERuleMirrorAxis.XY)
            {
                result = MirrorMatchGrid(matchGrid, ERuleMirrorAxis.X);
                result = MirrorMatchGrid(result, ERuleMirrorAxis.Y);

                mirroredMatchGrid = result;
            }
            else if (mirrorAxis == ERuleMirrorAxis.YZ)
            {
                result = MirrorMatchGrid(matchGrid, ERuleMirrorAxis.Y);
                result = MirrorMatchGrid(result, ERuleMirrorAxis.Z);

                mirroredMatchGrid = result;
            }

            return mirroredMatchGrid;
        }

        private bool NeighborMatches(RulesetTile rulesetTile, Tile neighbor)
        {
            if (neighbor == null)
                return false;
            else
                return true;
        }

        [Serializable]
        public enum EMatchType
        {
            Anything = 0,
            Empty = 1,
            Occupied = 2
        }

        [Serializable]
        public enum ETransformation 
        {
            Fixed,
            RotateX,
            RotateY,
            RotateZ,
            MirrorX,
            MirrorY,
            MirrorZ,
            MirrorXZ,
            MirrorXY,
            MirrorYZ
        }
    }
}
