﻿using System.Linq;
using System.Text;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class BootstrapSceneLoader : MonoBehaviour {

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    await DataLoader.LoadTask.Task;
    var scenes = from scene in Registry.Get<SceneData>()
                 where scene.IsSelectable && scene.IsVisible
                 orderby scene.Type, scene.LoadPriority
                 select scene;
    var builder = new StringBuilder();
    SceneData sceneToLoad = null;
    foreach (var scene in scenes) {
      builder.AppendLine($"    {scene.Name} ({scene.Type}) Priority: {scene.LoadPriority}");
      if (sceneToLoad == null) {
        sceneToLoad = scene;
      }
    }
    Debug.Log($"Bootstrap Scene Load Ordering: \n{builder}");
    if (sceneToLoad != null) {
      Debug.Log($"Loading {sceneToLoad}...");
      // FIXME
      // await sceneToLoad.GameScene.LoadAsync();
    } else {
      // TODO(james7132): Handle the error
    }
  }

}

}