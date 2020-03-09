/*

   Copyright 2020 Esri

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

   See the License for the specific language governing permissions and
   limitations under the License.

*/

using System.Collections.Generic;
using ArcGIS.Core.Geometry;

namespace MultipatchBuilderExCubeDemo
{
  /// <summary>
  /// Creates a cube multipatch using MultipatchBuilderEx.
  /// </summary>
  class MultipatchFactory
  {
    internal static Multipatch CreateCubeMultipatch()
    {
      double side = 5.0;

      MultipatchBuilderEx cubeMultipatchBuilderEx = new MultipatchBuilderEx();

      //create top face patch
      Patch topFacePatch = cubeMultipatchBuilderEx.MakePatch(esriPatchType.FirstRing);
      topFacePatch.Coords = new List<Coordinate3D>
      {
        new Coordinate3D(0, 0, side),
        new Coordinate3D(0, side, side),
        new Coordinate3D(side, side, side),
        new Coordinate3D(side, 0, side),
      };

      //create bottom face patch
      Patch bottomFacePatch = cubeMultipatchBuilderEx.MakePatch(esriPatchType.FirstRing);
      bottomFacePatch.Coords = new List<Coordinate3D>
      {
        new Coordinate3D(0, 0, 0),
        new Coordinate3D(0, side, 0),
        new Coordinate3D(side, side, 0),
        new Coordinate3D(side, 0, 0),
      };

      //create sides face patch
      Patch sidesFacePatch = cubeMultipatchBuilderEx.MakePatch(esriPatchType.TriangleStrip);
      sidesFacePatch.Coords = new List<Coordinate3D>
      {
        new Coordinate3D(0, 0, 0),
        new Coordinate3D(0, 0, side),
        new Coordinate3D(side, 0, 0),
        new Coordinate3D(side, 0, side),
        new Coordinate3D(side, side, 0),
        new Coordinate3D(side, side, side),
        new Coordinate3D(0, side, 0),
        new Coordinate3D(0, side, side),
        new Coordinate3D(0, 0, 0),
        new Coordinate3D(0, 0, side),
      };

      //add patches to multipatch builder
      cubeMultipatchBuilderEx.Patches.Add(topFacePatch);
      cubeMultipatchBuilderEx.Patches.Add(bottomFacePatch);
      cubeMultipatchBuilderEx.Patches.Add(sidesFacePatch);

      return cubeMultipatchBuilderEx.ToGeometry() as Multipatch;
    }
  }
}
