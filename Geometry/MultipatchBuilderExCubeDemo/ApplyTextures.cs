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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System.Windows.Resources;
using System.Windows;

namespace MultipatchBuilderExCubeDemo
{
  /// <summary>
  /// Apply textures to the cube.
  /// </summary>
  internal class ApplyTextures : Button
  {
    protected override void OnClick()
    {
      // make sure there's an OID from a created feature
      if (Module1.CubeMultipatchOID == -1)
        return;

      if (MapView.Active?.Map == null)
        return;

      QueuedTask.Run(() => ApplyTexturesToCube());
    }

    private void ApplyTexturesToCube()
    {
      // find layer
      var localSceneLayer = MapView.Active.Map.GetLayersAsFlattenedList().FirstOrDefault(l => l.Name == "Cube") as FeatureLayer;
      if (localSceneLayer == null)
        return;

      //create material
      BasicMaterial textureMaterial = new BasicMaterial();

      //read jpeg texture buffer
      byte[] imageBuffer = GetBufferImage("pack://application:,,,/MultipatchBuilderExCubeDemo;component/Textures/Rubik_cube.jpg", esriTextureCompressionType.CompressionJPEG);
      JPEGTexture rubiksCubeTexture = new JPEGTexture(imageBuffer);

      //create a texture resource to hold the texture
      textureMaterial.TextureResource = new TextureResource(rubiksCubeTexture);

      // get the multipatch shape using the Inspector
      var insp = new Inspector();
      insp.Load(localSceneLayer, Module1.CubeMultipatchOID);
      var multipatchFromScene = insp.Shape as Multipatch;

      //create Multipatch builder from Multipatch
      var cubeMultipatchBuilderExWithTextures = new MultipatchBuilderEx(multipatchFromScene);

      //assign texture coordinate to patches

      //assign texture coordinates to top face patch
      Patch topFacePatch = cubeMultipatchBuilderExWithTextures.Patches[0];
      topFacePatch.TextureCoords2D = new List<Coordinate2D>
      {
        new Coordinate2D(0.25, 0.33),
        new Coordinate2D(0.25, 0),
        new Coordinate2D(0.5, 0),
        new Coordinate2D(0.5, 0.33),
        new Coordinate2D(0.25, 0.33)
      };

      //assign texture coordinates to bottom face patch
      Patch bottomFacePatch = cubeMultipatchBuilderExWithTextures.Patches[1];
      bottomFacePatch.TextureCoords2D = new List<Coordinate2D>
      {
        new Coordinate2D(0.25, 1),
        new Coordinate2D(0.25, 0.66),
        new Coordinate2D(0.5, 0.66),
        new Coordinate2D(0.5, 1),
        new Coordinate2D(0.25, 1)
      };

      //assign texture coordinates to sides face patch
      Patch sidesFacePatch = cubeMultipatchBuilderExWithTextures.Patches[2];
      sidesFacePatch.TextureCoords2D = new List<Coordinate2D>
      {
        new Coordinate2D(0, 0.66),
        new Coordinate2D(0, 0.33),
        new Coordinate2D(0.25, 0.66),
        new Coordinate2D(0.25, 0.33),
        new Coordinate2D(0.5, 0.66),
        new Coordinate2D(0.5, 0.33),
        new Coordinate2D(0.75, 0.66),
        new Coordinate2D(0.75, 0.33),
        new Coordinate2D(1.0, 0.66),
        new Coordinate2D(1.0, 0.33)
      };

      //assign texture material to all the patches
      foreach (Patch p in cubeMultipatchBuilderExWithTextures.Patches)
      {
        p.Material = textureMaterial;
      }

      //create immutable multipatch instance
      Multipatch cubeMultipatchWithTextures = cubeMultipatchBuilderExWithTextures.ToGeometry() as Multipatch;

      // modify operation
      var modifyOp = new EditOperation();
      modifyOp.Name = "Apply materials with texture to multipatch";
      modifyOp.Modify(localSceneLayer, Module1.CubeMultipatchOID, cubeMultipatchWithTextures);

      if (!modifyOp.Execute() || modifyOp.IsSucceeded != true)
      {
        throw new Exception($@"Multipatch update failed: {modifyOp.ErrorMessage}");
      }
    }

    // sUri of the form  "pack://application:,,,/myPack;component/Images/image.jpg"
    private byte[] GetBufferImage(string sUri, esriTextureCompressionType compressionType)
    {
      System.Drawing.Imaging.ImageFormat format = (compressionType == esriTextureCompressionType.CompressionJPEG) ? System.Drawing.Imaging.ImageFormat.Jpeg : System.Drawing.Imaging.ImageFormat.Bmp;

      Uri uri = new Uri(sUri, System.UriKind.RelativeOrAbsolute);
      StreamResourceInfo info = Application.GetResourceStream(uri);
      System.Drawing.Image image = System.Drawing.Image.FromStream(info.Stream);

      MemoryStream memoryStream = new MemoryStream();

      image.Save(memoryStream, format);
      byte[] imageBuffer = memoryStream.ToArray();

      return imageBuffer;
    }

    // fileName of the form  "d:\Temp\Image.jpg"
    private byte[] GetBufferFromImageFile(string fileName, esriTextureCompressionType compressionType)
    {
      System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
      MemoryStream memoryStream = new MemoryStream();

      System.Drawing.Imaging.ImageFormat format = compressionType == esriTextureCompressionType.CompressionJPEG ? System.Drawing.Imaging.ImageFormat.Jpeg : System.Drawing.Imaging.ImageFormat.Bmp;
      image.Save(memoryStream, format);
      byte[] imageBuffer = memoryStream.ToArray();

      return imageBuffer;
    }
  }
}
