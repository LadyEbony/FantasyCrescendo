﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.Characters.UI {

public class CharacterPortrait : UIBehaviour, IInitializable<PlayerConfig>, IStateView<PlayerConfig> {

  public Graphic Image;
  public AspectRatioFitter Fitter;
  public Vector2 RectBias;
  public bool Cropped;
  public Color DisabledTint = Color.grey;

  public Rect CropRect;
  Color DefaultColor;
  RectTransform RectTransform;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected override void Awake() {
    RectTransform = transform as RectTransform;
    if (Image != null) {
      DefaultColor = Image.color;
      Image.enabled = false;
    }
  }

  public async Task Initialize(PlayerConfig config) {
    if (Image == null) return;
    var selection = config.Selection;
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    if (character == null) return;
    var portrait = await selection.GetCharacterPallete().Portrait.LoadAsset<Sprite>().ToTask();
    SetImage(portrait);
    Image.color = (character.IsSelectable && character.IsVisible) ? DefaultColor : DisabledTint;
    Image.enabled = true;
    if (Fitter != null) {
      Fitter.aspectRatio = GetAspectRatio(GetPixelRect(portrait.texture));
    }
    var rawImage = Image as RawImage;
    if (rawImage == null) return;
    var texture = portrait.texture;
    CropRect = Cropped ? UVToPixelRect(character.PortraitCropRect, texture) : GetPixelRect(texture);
    CropRect.position += Vector2.Scale(RectBias, GetTextureSize(texture));
    rawImage.texture = portrait.texture;
    SetRect();
  }

  void SetImage(Sprite image) {
    var uiImage = Image as Image;
    var rawImage = Image as RawImage;
    if (uiImage != null) {
      uiImage.sprite = image;
    } else if (rawImage != null) {
      rawImage.texture = image.texture;
    }
  }

  public void ApplyState(ref PlayerConfig config) => Initialize(config);

  protected override void OnRectTransformDimensionsChange() => SetRect();

  void SetRect() {
    var rawImage = Image as RawImage;
    if (RectTransform == null || rawImage == null || rawImage.texture == null) return;
    Vector2 size = RectTransform.rect.size;
    float aspect = size.x / size.y;
    Texture texture = rawImage.texture;
    Rect imageRect = EnforceAspect(CropRect, aspect);
    if (imageRect.width > texture.width || imageRect.height > texture.height) {
      imageRect = RestrictRect(imageRect, texture.width, texture.height, aspect);
      imageRect.center = GetTextureCenter(texture);
    }
    rawImage.uvRect = PixelToUVRect(imageRect, texture);
  }

  Rect PixelToUVRect(Rect pixelRect, Texture texture) {
    var scale = Vector2.zero;
    scale.x = texture.width != 0 ? 1f / texture.width : 0f;
    scale.y = texture.height != 0 ? 1f / texture.height: 0f;
    return new Rect(Vector2.Scale(scale, pixelRect.position), Vector2.Scale(scale, pixelRect.size));
  }

  Rect UVToPixelRect(Rect uvRect, Texture texture) {
    var size = GetTextureSize(texture);
    return new Rect(Vector2.Scale(size, uvRect.position), Vector2.Scale(size, uvRect.size));
  }

  Rect GetPixelRect(Texture texture) => new Rect(0, 0, texture.width, texture.height);
  Vector2 GetTextureSize(Texture texture) => new Vector2(texture.width, texture.height);
  Vector2 GetTextureCenter(Texture texture) => GetTextureSize(texture) / 2;

  Rect EnforceAspect(Rect rect, float aspect) {
    if (GetAspectRatio(rect) < aspect) {
      // Image is wider than cropRect, extend it sideways
      float width = aspect * rect.height;
      float widthDiff = rect.width - width;
      rect.width = width;
      rect.x -= widthDiff / 2;
      return rect;
    }
    // Image is wider than cropRect, extend it vertically
    float height = aspect * rect.width;
    float heightDiff = rect.height - height;
    rect.height = height;
    rect.x -= heightDiff / 2;
    return rect;
  }

  float GetAspectRatio(Rect rect) {
    if (Mathf.Approximately(rect.height, 0f))
      return float.NaN;
    return rect.width / rect.height;
  }

  Rect RestrictRect(Rect rect, float width, float height, float aspect) {
    Vector2 center = rect.center;
    if (Mathf.Approximately(height, 0))
      throw new ArgumentException();
    float enclosingAspectRatio = width / height;
    if (aspect < enclosingAspectRatio) {
      rect.width = width;
      rect.height = width / aspect;
    } else {
      rect.height = height;
      rect.width = height * aspect;
    }
    rect.center = center;
    return rect;
  }

}

}