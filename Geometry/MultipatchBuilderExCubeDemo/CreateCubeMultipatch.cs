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
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace MultipatchBuilderExCubeDemo
{
  /// <summary>
  /// Creates a cube multipatch using MultipatchBuilderEx and add it to the scene.
  /// </summary>
  internal class CreateCubeMultipatch : Button
  {
    protected override void OnClick()
    {
      if (MapView.Active?.Map == null)
        return;

      QueuedTask.Run(AddCubeToLayer);
    }

    private void AddCubeToLayer()
    {
      //get the first layer from the map
      //layer type must be a multipatch layer type
      // find layer
      var localSceneLayer = MapView.Active.Map.GetLayersAsFlattenedList().FirstOrDefault(l => l.Name == "Cube") as FeatureLayer;
      if (localSceneLayer == null)
        return;

      var cubeMultipatch = MultipatchFactory.CreateCubeMultipatch();

      //add multipatch to selected multipatch layer
      // track the newly created objectID
      long newObjectID = -1;

      var op = new EditOperation();
      op.Name = "Create multipatch feature";
      op.SelectNewFeatures = false;
      op.Create(localSceneLayer, cubeMultipatch, oid => newObjectID = oid);
      if (op.Execute() && op.IsSucceeded == true)
      {
        // save the oid in the module for other commands to use
        Module1.CubeMultipatchOID = newObjectID;
      }
      else
      {
        throw new Exception($@"Multipatch creation failed: {op.ErrorMessage}");
      }

      MapView.Active.ZoomTo(localSceneLayer);
    }
  }
}
