
//在AssetBundle.SetAssetBundleDecryptKey() 执行完毕之后运行下面的代码   使用Dnspy缝合  或者反射注入等

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

private void Extract()
{
	string outDir = Path.Combine(Application.streamingAssetsPath, "Extract");
	if (!Directory.Exists(outDir))
	{
		Directory.CreateDirectory(outDir);
	}
	DirectoryInfo[] directories = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "AssetBundle\\Prefabs\\Texture")).GetDirectories();
	for (int i = 0; i < directories.Length; i++)
	{
		FileInfo[] files = directories[i].GetFiles("*.ab");
		for (int j = 0; j < files.Length; j++)
		{
			AssetBundle ab = AssetBundle.LoadFromFile(files[j].FullName);
			foreach (string assetName in ab.GetAllAssetNames())
			{
				Debug.LogError(assetName);
				string fileName = Path.GetFileName(assetName).ToLower();
				if (Path.GetExtension(fileName) == ".png")
				{
					Texture2D source = ab.LoadAsset<Texture2D>(assetName);
					if (source != null)
					{
						RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
						Graphics.Blit(source, renderTex);
						RenderTexture previous = RenderTexture.active;
						RenderTexture.active = renderTex;
						Texture2D texture2D = new Texture2D(source.width, source.height);
						texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTex.width, (float)renderTex.height), 0, 0);
						texture2D.Apply();
						RenderTexture.active = previous;
						RenderTexture.ReleaseTemporary(renderTex);
						byte[] data = texture2D.EncodeToPNG();
						File.WriteAllBytes(Path.Combine(outDir, fileName), data);
					}
				}
			}
		}
	}
}