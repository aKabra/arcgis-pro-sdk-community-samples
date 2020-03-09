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

using System;
using System.Linq;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Windows.Media;
using ArcGIS.Desktop.Editing.Attributes;

namespace MultipatchBuilderExCubeDemo
{
  /// <summary>
  /// Apply materials to the cube.
  /// </summary>
  internal class ApplyMaterials : Button
  {
    protected override void OnClick()
    {
      // make sure there's an OID from a created feature
      if (Module1.CubeMultipatchOID == -1)
        return;

      if (MapView.Active?.Map == null)
        return;

      QueuedTask.Run(() => ApplyMaterialsToCube());
    }

    private void ApplyMaterialsToCube()
    {
      // find layer
      var localSceneLayer = MapView.Active.Map.GetLayersAsFlattenedList().FirstOrDefault(l => l.Name == "Cube") as FeatureLayer;
      if (localSceneLayer == null)
        return;

      //create material for top face patch
      BasicMaterial topFaceMaterial = new BasicMaterial();
      topFaceMaterial.Color = Color.FromRgb(203, 65, 84);
      topFaceMaterial.Shininess = 150;
      topFaceMaterial.TransparencyPercent = 50;
      topFaceMaterial.EdgeWidth = 20;

      //create material for bottom face patch
      BasicMaterial bottomFaceMaterial = new BasicMaterial();
      bottomFaceMaterial.Color = Color.FromRgb(203, 65, 84);
      bottomFaceMaterial.EdgeWidth = 20;

      //create material for sides face
      BasicMaterial sidesFaceMaterial = new BasicMaterial();
      sidesFaceMaterial.Color = Color.FromRgb(133, 94, 66);
      sidesFaceMaterial.Shininess = 0;
      sidesFaceMaterial.EdgeWidth = 20;

      // get the multipatch shape using the Inspector
      var insp = new Inspector();
      insp.Load(localSceneLayer, Module1.CubeMultipatchOID);
      var multipatchFromScene = insp.Shape as Multipatch;

      // create a builder
      var cubeMultipatchBuilderEx = new MultipatchBuilderEx(multipatchFromScene);

      //set material to top face patch
      var topFacePatch = cubeMultipatchBuilderEx.Patches[0];
      topFacePatch.Material = topFaceMaterial;

      //set material to bottom face patch
      var bottomFacePatch = cubeMultipatchBuilderEx.Patches[1];
      bottomFacePatch.Material = bottomFaceMaterial;

      //set material to sides face patch
      var sidesFacePatch = cubeMultipatchBuilderEx.Patches[2];
      sidesFacePatch.Material = sidesFaceMaterial;
      
      //create immutable multipatch instance
      Multipatch cubeMultipatchWithMaterials = cubeMultipatchBuilderEx.ToGeometry() as Multipatch;

      // modify operation
      var modifyOp = new EditOperation();
      modifyOp.Name = "Apply materials to multipatch";
      modifyOp.Modify(localSceneLayer, Module1.CubeMultipatchOID, cubeMultipatchWithMaterials);

      if (!modifyOp.Execute() || modifyOp.IsSucceeded != true)
      {
        throw new Exception($@"Multipatch update failed: {modifyOp.ErrorMessage}");
      }
    }
  }
}
